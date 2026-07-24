using System;
using System.Collections.Generic;
using System.Text;
using ConsoleZapp.Interop;

namespace ConsoleZapp
{
    public class Body
    {
        // Set by Tui to its own (private) CheckResize, invoked from inside ReadLineFromKeys's
        // native read loop the instant a WINDOW_BUFFER_SIZE_EVENT arrives
        internal Action ResizeCheck;

        private string Prompt = "> ";
        private Cli.Conclr? PromptForeground;
        private Cli.Conclr? PromptBackground;

        private readonly List<Part> KeywordColors = new List<Part>();

        private string LastInputLine;
        private int LastInputRow = -1;

        private int TopRow;
        private int CurrentRow;

        // Retained scrollback: one entry per row currently visible in the scroll region, oldest
        // first. Redraws are sourced from here instead of reading back the live console screen
        // content (Console.MoveBufferArea), which is what corrupts non-ASCII glyphs on scroll.
        private readonly List<List<Part>> Rows = new List<List<Part>>();

        // Constructor
        public Body()
        {
        }

        // Sets the prompt shown before reading a command
        public void SetPrompt(string prompt)
        {
            Prompt = prompt;
        }

        // Sets the color the prompt is written in; the typed-in command itself keeps the console's normal color
        public void SetPromptColor(Cli.Conclr fg, Cli.Conclr bg)
        {
            PromptForeground = fg;
            PromptBackground = bg;
        }

        // Registers an exact keyword that gets highlighted in the given colors wherever it occurs in typed input
        public void AddKeywordColor(string keyword, Cli.Conclr fg, Cli.Conclr bg)
        {
            KeywordColors.Add(new Part { Text = keyword, Foreground = fg, Background = bg });
        }

        // Recolors the whole last input line (prompt included) in place, e.g. to indicate it was accepted.
        // Caller decides when this applies - must be called before any further write scrolls the area, since
        // the targeted row is remembered by absolute position, not tracked through later scrolling.
        public void RecolorLastInput(Cli.Conclr fg, Cli.Conclr bg)
        {
            if (LastInputRow < 0)
                return;

            Console.SetCursorPosition(0, LastInputRow);

            Console.ForegroundColor = (ConsoleColor)fg;
            Console.BackgroundColor = (ConsoleColor)bg;
            Console.Write(LastInputLine);
            Console.ResetColor();

            if (Rows.Count > 0)
                Rows[Rows.Count - 1] = new List<Part> { new Part { Text = LastInputLine, Foreground = fg, Background = bg } };
        }

        // Sets the row where the scrolling area begins, called by Tui after the header is printed
        public void Init(int top_row)
        {
            TopRow = top_row;
            CurrentRow = top_row;
            Rows.Clear();
        }

        // Re-attaches the scroll region to a (possibly unchanged) top row and redraws every retained
        // row, without discarding scrollback history - called by Tui when it detects a console
        // resize, as opposed to Init() which starts a fresh session
        public void Redraw(int top_row)
        {
            TopRow = top_row;

            TrimToCapacity();
            RedrawRows();

            CurrentRow = TopRow + Rows.Count;
        }

        // Writes a formatted line into the scrolling area, scrolling the area up if needed
        public void WriteLine(string fmt, params object[] args)
        {
            var text = ClampToWindowWidth(string.Format(fmt, args));

            WriteRow(new List<Part> { new Part { Text = text } });
        }

        // Writes a formatted line in the given colors into the scrolling area, scrolling the area up if needed
        public void WriteLine(Cli.Conclr fg, Cli.Conclr bg, string fmt, params object[] args)
        {
            var text = ClampToWindowWidth(string.Format(fmt, args));

            WriteRow(new List<Part> { new Part { Text = text, Foreground = fg, Background = bg } });
        }

        // Writes a line built from independently colored parts into the scrolling area, scrolling the area up if needed
        public void WriteLine(IEnumerable<Part> parts)
        {
            WriteRow(new List<Part>(parts));
        }

        // Prints the prompt and reads a command line from the console, scrolling the area up if needed
        public string ReadCommand()
        {
            PrepareRow();

            var command = ReadLineFromKeys();

            var row = CurrentRow;
            CurrentRow++;

            LastInputRow = row;
            LastInputLine = Prompt + command;

            HighlightKeywords(command, row);

            Rows.Add(BuildCommandRowParts(command));

            return command;
        }

        // Reads a line character-by-character via raw ReadConsoleInputW (see Interop/ConsoleInput.cs)
        // instead of Console.ReadLine/Console.ReadKey. Keeps the cursor pinned to this one row - long
        // input horizontally scrolls the visible window instead of letting conhost wrap/scroll
        // natively (which used to drag the fixed header along, see "Real window width + dynamic
        // redraw" in .goals.md) - and reads the raw UnicodeChar straight from the key event, sidestepping
        // Console.ReadKey's lossy codepage translation of multibyte typed input. The same native read
        // also picks up window-resize events, so ResizeCheck runs immediately on resize instead of only
        // being noticed the next time some other Tui method is called. Renders at the live CurrentRow
        // field (not a snapshotted row number) so a resize-triggered Body.Redraw() mid-loop - which can
        // shift CurrentRow if the window got shorter - is picked up correctly on the very next render.
        private string ReadLineFromKeys()
        {
            var text = new StringBuilder();
            var cursor = 0;

            while (true)
            {
                RenderInputRow(CurrentRow, text, cursor);

                var key_event = ConsoleInput.ReadKeyOrResize(ResizeCheck);

                if (key_event == null)
                    continue;

                var virtual_key = (ConsoleKey)key_event.Value.VirtualKeyCode;
                var character = key_event.Value.Char;

                // wRepeatCount can coalesce a held key into a single event - replay it that many
                // times so holding a key still behaves like the equivalent number of keystrokes
                var repeat_count = Math.Max(1, (int)key_event.Value.RepeatCount);

                for (var i = 0; i < repeat_count; i++)
                {
                    if (virtual_key == ConsoleKey.Enter)
                        return text.ToString();

                    ApplyKey(virtual_key, character, text, ref cursor);
                }
            }
        }

        // Applies a single decoded key press to the in-progress input line
        private static void ApplyKey(ConsoleKey virtual_key, char character, StringBuilder text, ref int cursor)
        {
            switch (virtual_key)
            {
                case ConsoleKey.Backspace:
                    if (cursor > 0)
                    {
                        text.Remove(cursor - 1, 1);
                        cursor--;
                    }
                    break;

                case ConsoleKey.Delete:
                    if (cursor < text.Length)
                        text.Remove(cursor, 1);
                    break;

                case ConsoleKey.LeftArrow:
                    if (cursor > 0)
                        cursor--;
                    break;

                case ConsoleKey.RightArrow:
                    if (cursor < text.Length)
                        cursor++;
                    break;

                case ConsoleKey.Home:
                    cursor = 0;
                    break;

                case ConsoleKey.End:
                    cursor = text.Length;
                    break;

                default:
                    if (!char.IsControl(character))
                    {
                        text.Insert(cursor, character);
                        cursor++;
                    }
                    break;
            }
        }

        // Redraws the prompt + currently typed text on the given row, horizontal-scrolling the visible
        // window so the cursor position always stays on-screen instead of wrapping to another row
        private void RenderInputRow(int row, StringBuilder text, int cursor)
        {
            var available_width = Math.Max(0, Console.WindowWidth - 1 - Prompt.Length);

            var window_start = Math.Max(0, cursor - available_width);
            var window_length = Math.Min(available_width, text.Length - window_start);
            var visible = text.ToString(window_start, window_length);

            ClearRow(row);

            var has_color = PromptForeground.HasValue;

            if (has_color)
            {
                Console.ForegroundColor = (ConsoleColor)PromptForeground.Value;
                Console.BackgroundColor = (ConsoleColor)PromptBackground.Value;
            }

            Console.Write(Prompt);

            if (has_color)
                Console.ResetColor();

            Console.Write(visible);

            Console.SetCursorPosition(Prompt.Length + (cursor - window_start), row);
        }

        // Appends a row's parts to the retained buffer and writes it to the console, scrolling first if needed
        private void WriteRow(List<Part> parts)
        {
            PrepareRow();

            Rows.Add(parts);

            RewriteRow(CurrentRow, parts);

            CurrentRow++;
        }

        // Recolors any registered keyword occurrences found in the just-typed command, in place
        private void HighlightKeywords(string command, int row)
        {
            if (string.IsNullOrEmpty(command))
                return;

            foreach (var keyword in KeywordColors)
            {
                var start = 0;

                while ((start = command.IndexOf(keyword.Text, start, StringComparison.Ordinal)) >= 0)
                {
                    Console.SetCursorPosition(Prompt.Length + start, row);

                    Console.ForegroundColor = (ConsoleColor)keyword.Foreground.Value;
                    Console.BackgroundColor = (ConsoleColor)keyword.Background.Value;
                    Console.Write(keyword.Text);
                    Console.ResetColor();

                    start += keyword.Text.Length;
                }
            }
        }

        // Builds the retained-buffer parts for a typed command row (prompt + command, keyword matches
        // colored), mirroring what ReadCommand/HighlightKeywords already put on screen - used so a later
        // redraw from the buffer (on scroll or resize) reproduces the same highlighting
        private List<Part> BuildCommandRowParts(string command)
        {
            var parts = new List<Part> { new Part { Text = Prompt, Foreground = PromptForeground, Background = PromptBackground } };

            if (string.IsNullOrEmpty(command))
                return parts;

            parts.AddRange(SplitByKeywords(command));

            return parts;
        }

        // Splits command text into parts around registered keyword matches, earliest match wins on overlap
        private List<Part> SplitByKeywords(string command)
        {
            var matches = new List<(int Start, int End, Part Keyword)>();

            foreach (var keyword in KeywordColors)
            {
                var start = 0;

                while ((start = command.IndexOf(keyword.Text, start, StringComparison.Ordinal)) >= 0)
                {
                    matches.Add((start, start + keyword.Text.Length, keyword));
                    start += keyword.Text.Length;
                }
            }

            matches.Sort((a, b) => a.Start.CompareTo(b.Start));

            var parts = new List<Part>();
            var cursor = 0;

            foreach (var match in matches)
            {
                if (match.Start < cursor)
                    continue;

                if (match.Start > cursor)
                    parts.Add(new Part { Text = command.Substring(cursor, match.Start - cursor) });

                parts.Add(new Part { Text = command.Substring(match.Start, match.End - match.Start), Foreground = match.Keyword.Foreground, Background = match.Keyword.Background });
                cursor = match.End;
            }

            if (cursor < command.Length)
                parts.Add(new Part { Text = command.Substring(cursor) });

            return parts;
        }

        // Truncates text that would overflow the window width, avoiding a native wrap/scroll on write
        private static string ClampToWindowWidth(string text)
        {
            const string ellipsis = "...";

            var max_length = Console.WindowWidth - 1;

            if (text.Length <= max_length)
                return text;

            var content_length = Math.Max(0, max_length - ellipsis.Length);

            return text.Substring(0, content_length) + ellipsis;
        }

        // Scrolls the scrolling area up by one row if the cursor has reached the bottom of the window
        private void PrepareRow()
        {
            var bottom_row = Console.WindowHeight - 1;

            // The very last row is kept blank as a safety margin against conhost's own native scroll
            // (which would drag the fixed header along with it, since it knows nothing about our
            // retained-buffer scrolling). Originally needed because Console.ReadLine() always echoed
            // a newline past the window bottom on Enter; ReadCommand no longer uses it (see
            // ReadLineFromKeys) but the reservation is cheap insurance, not worth relitigating for
            // one row of vertical space.
            var safe_row = bottom_row - 1;

            if (CurrentRow <= safe_row)
                return;

            if (Rows.Count > 0)
                Rows.RemoveAt(0);

            RedrawRows();

            CurrentRow = safe_row;
        }

        // Drops the oldest retained rows past whatever the current window height can show - used when
        // the window shrinks (via Redraw()); normal per-line scrolling (PrepareRow) already drops rows
        // one at a time as it goes, so it doesn't need this
        private void TrimToCapacity()
        {
            var capacity = Math.Max(0, Console.WindowHeight - 1 - TopRow);

            while (Rows.Count > capacity)
                Rows.RemoveAt(0);
        }

        // Redraws the whole scroll region from the retained buffer, blanking any rows past the buffer's
        // current content - used instead of Console.MoveBufferArea, whose ReadConsoleOutput/WriteConsoleOutput
        // CHAR_INFO copy path corrupts non-ASCII glyphs (e.g. block/braille characters) under codepage 65001
        // in the legacy Windows console host
        private void RedrawRows()
        {
            var bottom_row = Console.WindowHeight - 1;
            var safe_row = bottom_row - 1;
            var row = TopRow;

            foreach (var parts in Rows)
            {
                RewriteRow(row, parts);
                row++;
            }

            while (row <= safe_row)
            {
                ClearRow(row);
                row++;
            }
        }

        // Clears a row, then writes the given parts into it from column 0, coloring each part per its own setting
        private static void RewriteRow(int row, List<Part> parts)
        {
            ClearRow(row);
            RenderParts(parts);
        }

        // Blanks a row without disturbing the cursor row itself, leaving the cursor at column 0
        private static void ClearRow(int row)
        {
            Console.SetCursorPosition(0, row);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, row);
        }

        // Writes parts at the current cursor position, coloring each part per its own setting
        private static void RenderParts(IEnumerable<Part> parts)
        {
            foreach (var part in parts)
            {
                var has_color = part.Foreground.HasValue;

                if (has_color)
                {
                    Console.ForegroundColor = (ConsoleColor)part.Foreground.Value;
                    Console.BackgroundColor = (ConsoleColor)part.Background.Value;
                }

                Console.Write(part.Text);

                if (has_color)
                    Console.ResetColor();
            }
        }
    }
}
