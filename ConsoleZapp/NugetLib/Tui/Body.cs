using System;
using System.Collections.Generic;
using System.Text;
using ConsoleZapp.Interop;

namespace ConsoleZapp
{
    public class Body
    {
        // Set by Tui to its own CheckResize
        // - invoked from ReadLineFromKeys's native loop, on resize
        public Action ResizeCheck;

        private string Prompt = "> ";
        private Cli.Conclr? PromptForeground;
        private Cli.Conclr? PromptBackground;

        private readonly List<Part> KeywordColors = new List<Part>();

        // Submitted commands, oldest first
        // - recalled via Up/DownArrow in ReadLineFromKeys
        // - never trimmed, same as Rows
        private readonly List<string> History = new List<string>();

        // Index into History currently shown while recalling
        // - -1 means not recalling, the live line is shown instead
        private int HistoryIndex = -1;

        // In-progress line as it stood before recall started
        // - restored once the user arrows past the newest entry
        private string HistoryPendingLine;

        private string LastInputLine;
        private int LastInputRow = -1;

        private int TopRow;
        private int CurrentRow;

        /* Retained scrollback, one entry per line, oldest first, never
           trimmed - the whole session stays in memory (see .goals.md).
           Redraws source from here rather than the live screen, which
           is what corrupts non-ASCII glyphs on scroll (MoveBufferArea) */
        private readonly List<List<Part>> Rows = new List<List<Part>>();

        private ScrollMode Mode = ScrollMode.Manual;

        /* Absolute index into Rows of the topmost row shown while
           reviewing; -1 means following the live tail (the normal
           state). Absolute, not relative, so a mid-review position
           doesn't drift when new content is appended (see .goals.md) */
        private int ScrollOffset = -1;

        // True while ReadLineFromKeys is reading a line
        // - tells RedrawRows/ContentCapacity to reserve the bottom row
        private bool PromptActive;

        // Replaces the line's content, cursor to end
        private static void SetLineText(StringBuilder text, ref int cursor, string value)
        {
            text.Clear();
            text.Append(value);
            cursor = text.Length;
        }

        // Applies a decoded key press to the input line
        private static void ApplyKey(ConsoleKey virtual_key, char character, StringBuilder text, ref int cursor)
        {
            switch (virtual_key)
            {
                case ConsoleKey.Backspace:
                    if (cursor > 0)
                    {
                        var length = SurrogatePairWidthBefore(text, cursor);
                        text.Remove(cursor - length, length);
                        cursor -= length;
                    }
                    break;

                case ConsoleKey.Delete:
                    if (cursor < text.Length)
                        text.Remove(cursor, SurrogatePairWidthAfter(text, cursor));
                    break;

                case ConsoleKey.LeftArrow:
                    if (cursor > 0)
                        cursor -= SurrogatePairWidthBefore(text, cursor);
                    break;

                case ConsoleKey.RightArrow:
                    if (cursor < text.Length)
                        cursor += SurrogatePairWidthAfter(text, cursor);
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

        // Chars to remove before cursor for Backspace/Left
        // - returns 2 if they form a surrogate pair, else 1
        private static int SurrogatePairWidthBefore(StringBuilder text, int cursor)
        {
            if (cursor >= 2 && char.IsSurrogatePair(text[cursor - 2], text[cursor - 1]))
                return 2;

            return 1;
        }

        // Chars to remove after cursor for Delete/Right
        // - returns 2 if they form a surrogate pair, else 1
        private static int SurrogatePairWidthAfter(StringBuilder text, int cursor)
        {
            if (cursor + 1 < text.Length && char.IsSurrogatePair(text[cursor], text[cursor + 1]))
                return 2;

            return 1;
        }

        // Truncates text overflowing the window width
        // - avoids a native wrap/scroll on write
        private static string ClampToWindowWidth(string text)
        {
            const string ELLIPSIS = "...";

            var max_length = Console.WindowWidth - 1;

            if (text.Length <= max_length)
                return text;

            var content_length = Math.Max(0, max_length - ELLIPSIS.Length);

            return text.Substring(0, content_length) + ELLIPSIS;
        }

        // Clears a row, writes parts from column 0
        // - each part in its own color
        private static void RewriteRow(int row, List<Part> parts)
        {
            ClearRow(row);
            RenderParts(parts);
        }

        // Blanks a row, cursor left at column 0
        private static void ClearRow(int row)
        {
            Console.SetCursorPosition(0, row);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, row);
        }

        // Writes parts at the cursor, each its own color
        private static void RenderParts(IEnumerable<Part> parts)
        {
            foreach (var part in parts)
                ColorWriter.Write(part.Foreground, part.Background, part.Text);
        }

        // Constructor
        public Body() { }

        // Sets the prompt shown before reading a command
        public void SetPrompt(string prompt)
        {
            Prompt = prompt;
        }

        // Sets the prompt's color
        // - typed text itself stays the console's normal color
        public void SetPromptColor(Cli.Conclr fg, Cli.Conclr bg)
        {
            PromptForeground = fg;
            PromptBackground = bg;
        }
        // As a single bundled pair
        public void SetPromptColor(ColorPair pair)
        {
            SetPromptColor(pair.Foreground, pair.Background);
        }

        // Sets what happens to scroll on new content
        // - Manual keeps position, AutoScroll snaps to tail
        public void SetScrollMode(ScrollMode mode)
        {
            Mode = mode;
        }

        // Registers a keyword highlighted wherever typed
        public void AddKeywordColor(string keyword, Cli.Conclr fg, Cli.Conclr bg)
        {
            KeywordColors.Add(new Part { Text = keyword, Foreground = fg, Background = bg });
        }

        // Recolors the last input line in place
        // - call before any further write scrolls the area
        public void RecolorLastInput(Cli.Conclr fg, Cli.Conclr bg)
        {
            if (LastInputRow < 0)
                return;

            Console.SetCursorPosition(0, LastInputRow);
            ColorWriter.Write(fg, bg, LastInputLine);

            if (Rows.Count > 0)
                Rows[Rows.Count - 1] = new List<Part> { new Part { Text = LastInputLine, Foreground = fg, Background = bg } };
        }

        // Sets where the scroll area begins
        // - called by Tui after the header prints
        public void Init(int top_row)
        {
            TopRow = top_row;
            CurrentRow = top_row;
            Rows.Clear();
            ScrollOffset = -1;
        }

        // Re-attaches the scroll region, redraws all rows
        // - keeps scrollback, unlike Init()'s fresh start
        public void Redraw(int top_row)
        {
            TopRow = top_row;

            RedrawRows();

            CurrentRow = NextContentRow();
        }

        // Writes a formatted line, scrolling up if needed
        public void WriteLine(string fmt, params object[] args)
        {
            var text = ClampToWindowWidth(string.Format(fmt, args));

            WriteRow(new List<Part> { new Part { Text = text } });
        }
        // In the given colors
        public void WriteLine(Cli.Conclr fg, Cli.Conclr bg, string fmt, params object[] args)
        {
            var text = ClampToWindowWidth(string.Format(fmt, args));

            WriteRow(new List<Part> { new Part { Text = text, Foreground = fg, Background = bg } });
        }
        // Built from independently colored parts
        public void WriteLine(IEnumerable<Part> parts)
        {
            WriteRow(new List<Part>(parts));
        }

        // Prints the prompt, reads a command line
        public string ReadCommand()
        {
            PreparePromptRow();

            var command = ReadLineFromKeys();

            PromptActive = false;

            var row = CurrentRow;

            LastInputRow = row;
            LastInputLine = Prompt + command;

            HighlightKeywords(command, row);

            Rows.Add(BuildCommandRowParts(command));

            if (!string.IsNullOrEmpty(command))
                History.Add(command);

            return command;
        }

        // Prints a dialog, reads a matching answer
        // - unrecognized input reprints the question, loops
        public DialogOption ReadDialog(Dialog dialog)
        {
            while (true)
            {
                foreach (var line in dialog.BuildLines())
                    WriteLine("{0}", line);

                var input = ReadCommand();
                var matched = dialog.Match(input);

                if (matched.HasValue)
                    return matched.Value;

                WriteLine("Not recognized - please choose one of the options above.");
            }
        }

        // Reads a line character-by-character
        // - via raw ReadConsoleInputW, not Console.ReadLine
        // - keeps cursor pinned, no native wrap/scroll
        // - PageUp/PageDown review history (see ScrollOffset)
        // - UpArrow/DownArrow recall History instead
        private string ReadLineFromKeys()
        {
            var text = new StringBuilder();
            var cursor = 0;

            HistoryIndex = -1;

            while (true)
            {
                if (ScrollOffset >= 0)
                    RedrawRows();
                else
                    RenderInputRow(CurrentRow, text, cursor);

                var key_event = ConsoleInput.ReadKeyOrResize(ResizeCheck);

                if (key_event == null)
                    continue;

                var virtual_key = (ConsoleKey)key_event.Value.VirtualKeyCode;
                var character = key_event.Value.Char;

                // wRepeatCount can coalesce a held key into a single event - replay it that many times so holding a key still behaves like the equivalent number of keystrokes
                var repeat_count = Math.Max(1, (int)key_event.Value.RepeatCount);

                for (var i = 0; i < repeat_count; i++)
                {
                    if (virtual_key == ConsoleKey.PageUp)
                    {
                        ScrollPageUp();
                        continue;
                    }

                    if (virtual_key == ConsoleKey.PageDown)
                    {
                        ScrollPageDown();

                        // ScrollPageDown can itself land back on the live tail (ScrollOffset reset to -1) - that transition needs the same re-render SnapToLive does below, or the content area is left showing the last review page instead of the true tail
                        if (ScrollOffset < 0)
                            SnapToLive(text, cursor);

                        continue;
                    }

                    if (ScrollOffset >= 0)
                        SnapToLive(text, cursor);

                    if (virtual_key == ConsoleKey.Enter)
                        return text.ToString();

                    // UpArrow/DownArrow browse History instead of moving within the line
                    if (virtual_key == ConsoleKey.UpArrow)
                    {
                        RecallHistory(-1, text, ref cursor);
                        continue;
                    }

                    if (virtual_key == ConsoleKey.DownArrow)
                    {
                        RecallHistory(1, text, ref cursor);
                        continue;
                    }

                    ApplyKey(virtual_key, character, text, ref cursor);
                }
            }
        }

        // Leaves review mode, redraws as the live tail
        // - also re-renders the prompt row
        private void SnapToLive(StringBuilder text, int cursor)
        {
            ScrollOffset = -1;
            CurrentRow = NextContentRow();
            RedrawRows();

            // make the prompt + in-progress text visible again immediately - otherwise a key that returns straight away (Enter) would submit before the row ever shows it
            RenderInputRow(CurrentRow, text, cursor);
        }

        // Moves review position one screenful back
        // - clamped to the start of retained history
        private void ScrollPageUp()
        {
            var capacity = VisibleCapacity();
            var live_top = Math.Max(0, Rows.Count - capacity);
            var current_top = ScrollOffset >= 0 ? ScrollOffset : live_top;

            ScrollOffset = Math.Max(0, current_top - capacity);
        }

        // Moves review position one screenful forward
        // - snaps to live tail once caught up, else no-op
        private void ScrollPageDown()
        {
            if (ScrollOffset < 0)
                return;

            var capacity = VisibleCapacity();
            var live_top = Math.Max(0, Rows.Count - capacity);
            var next_top = ScrollOffset + capacity;

            ScrollOffset = next_top >= live_top ? -1 : next_top;
        }

        // Moves through History by the given step
        // - saves/restores the in-progress line at the edges
        private void RecallHistory(int step, StringBuilder text, ref int cursor)
        {
            if (History.Count == 0)
                return;

            if (HistoryIndex < 0)
            {
                if (step > 0)
                    return;

                HistoryPendingLine = text.ToString();
                HistoryIndex = History.Count - 1;
            }
            else
            {
                var next_index = HistoryIndex + step;

                if (next_index >= History.Count)
                {
                    HistoryIndex = -1;
                    SetLineText(text, ref cursor, HistoryPendingLine);
                    return;
                }

                HistoryIndex = Math.Max(0, next_index);
            }

            SetLineText(text, ref cursor, History[HistoryIndex]);
        }

        // Redraws prompt + typed text on the given row
        // - horizontally scrolls so the cursor stays visible
        private void RenderInputRow(int row, StringBuilder text, int cursor)
        {
            var available_width = Math.Max(0, Console.WindowWidth - 1 - Prompt.Length);

            var window_start = Math.Max(0, cursor - available_width);
            var window_length = Math.Min(available_width, text.Length - window_start);
            var visible = text.ToString(window_start, window_length);

            ClearRow(row);

            ColorWriter.Write(PromptForeground, PromptBackground, Prompt);

            Console.Write(visible);

            Console.SetCursorPosition(Prompt.Length + (cursor - window_start), row);
        }

        // Appends parts to the buffer, writes if not reviewing
        // - shifts the window/prompt first if needed
        private void WriteRow(List<Part> parts)
        {
            // a reserved prompt row can never take the single-row fast path below: appending content may move where that reserved row lands, which the fast path doesn't account for
            var needs_full_redraw = PromptActive || Rows.Count >= ContentCapacity();
            var target_row = NextContentRow();

            Rows.Add(parts);

            if (Mode == ScrollMode.AutoScroll)
                ScrollOffset = -1;

            if (ScrollOffset >= 0)
                return;

            if (needs_full_redraw)
            {
                CurrentRow = NextContentRow();
                RedrawRows();
            }
            else
            {
                CurrentRow = target_row;
                RewriteRow(CurrentRow, parts);
            }
        }

        // Recolors registered keywords in the typed command
        private void HighlightKeywords(string command, int row)
        {
            if (string.IsNullOrEmpty(command))
                return;

            var column = Prompt.Length;

            foreach (var part in SplitByKeywords(command))
            {
                if (part.Foreground.HasValue)
                {
                    Console.SetCursorPosition(column, row);
                    ColorWriter.Write(part.Foreground, part.Background, part.Text);
                }

                column += part.Text.Length;
            }
        }

        // Builds retained-buffer parts for a command row
        // - mirrors what ReadCommand put on screen
        private List<Part> BuildCommandRowParts(string command)
        {
            var parts = new List<Part>
            {
                new Part { Text = Prompt, Foreground = PromptForeground, Background = PromptBackground },
            };

            if (string.IsNullOrEmpty(command))
                return parts;

            parts.AddRange(SplitByKeywords(command));

            return parts;
        }

        // Splits text around keyword matches
        // - earliest match wins on overlap
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

                parts.Add(new Part
                {
                    Text = command.Substring(match.Start, match.End - match.Start),
                    Foreground = match.Keyword.Foreground,
                    Background = match.Keyword.Background,
                });
                cursor = match.End;
            }

            if (cursor < command.Length)
                parts.Add(new Part { Text = command.Substring(cursor) });

            return parts;
        }

        // Reserves the prompt row, shifts window if full
        // - called before ReadLineFromKeys starts reading
        private void PreparePromptRow()
        {
            PromptActive = true;

            if (ScrollOffset >= 0)
                return;

            var capacity = ContentCapacity();

            if (Rows.Count >= capacity)
                RedrawRows();

            CurrentRow = NextContentRow();
        }

        // Total rows the scroll region can show
        private int VisibleCapacity()
        {
            var bottom_row = Console.WindowHeight - 1;

            // the very last row is kept blank as a safety margin against conhost's own native scroll, which would drag the fixed header along with it - cheap insurance, not worth removing
            var safe_row = Math.Max(TopRow, bottom_row - 1);

            return safe_row - TopRow + 1;
        }

        // Rows available for retained content
        // - one less while a live prompt reserves its row
        private int ContentCapacity()
        {
            var capacity = VisibleCapacity();

            if (PromptActive && ScrollOffset < 0)
                return Math.Max(0, capacity - 1);

            return capacity;
        }

        // The absolute row the next written line lands on
        private int NextContentRow()
        {
            return TopRow + Math.Min(Rows.Count, Math.Max(0, VisibleCapacity() - 1));
        }

        // Redraws the scroll region from the retained buffer
        // - avoids Console.MoveBufferArea, which corrupts glyphs
        private void RedrawRows()
        {
            var capacity = ContentCapacity();
            var content_end_row = TopRow + Math.Max(0, Math.Min(capacity, VisibleCapacity()) - 1);
            var safe_row = TopRow + Math.Max(0, VisibleCapacity() - 1);

            var start = ScrollOffset >= 0 ? Math.Min(ScrollOffset, Rows.Count) : Math.Max(0, Rows.Count - capacity);
            var row = TopRow;

            for (var i = start; i < Rows.Count && row <= content_end_row; i++)
            {
                RewriteRow(row, Rows[i]);
                row++;
            }

            while (row <= safe_row)
            {
                ClearRow(row);
                row++;
            }
        }
    }
}
