using System;
using System.Collections.Generic;
using System.Text;
using ConsoleZapp.Interop;

namespace ConsoleZapp
{
    public class Body
    {
        // Set by Tui to its own (private) CheckResize, invoked from inside ReadLineFromKeys's native read loop the instant a WINDOW_BUFFER_SIZE_EVENT arrives
        internal Action ResizeCheck;

        private string Prompt = "> ";
        private Cli.Conclr? PromptForeground;
        private Cli.Conclr? PromptBackground;

        private readonly List<Part> KeywordColors = new List<Part>();

        private string LastInputLine;
        private int LastInputRow = -1;

        private int TopRow;
        private int CurrentRow;

        // Retained scrollback: one entry per line ever written, oldest first, never trimmed - the whole session's history stays in memory (see .goals.md, "Real, reviewable scrollback for Body").
        // Redraws are sourced from here instead of reading back the live console screen content (Console.MoveBufferArea), which is what corrupts non-ASCII glyphs on scroll.
        private readonly List<List<Part>> Rows = new List<List<Part>>();

        private ScrollMode Mode = ScrollMode.Manual;

        // Absolute index into Rows of the topmost row currently shown, while reviewing history; -1 means "following the live tail" (the normal state). Deliberately an absolute index, not "N rows back from the tail" - so a mid-review position doesn't drift when new content is appended (see .goals.md).
        private int ScrollOffset = -1;

        // True for the duration of ReadLineFromKeys - lets RedrawRows/ContentCapacity know to leave the bottom row free for the live-typed prompt instead of filling it with history
        private bool PromptActive;

        // Constructor
        public Body() { }

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

        // Sets what happens to a mid-review scroll position when new content is written - Manual (default) keeps it fixed, AutoScroll snaps back to the live tail on every write
        public void SetScrollMode(ScrollMode mode)
        {
            Mode = mode;
        }

        // Registers an exact keyword that gets highlighted in the given colors wherever it occurs in typed input
        public void AddKeywordColor(string keyword, Cli.Conclr fg, Cli.Conclr bg)
        {
            KeywordColors.Add(new Part { Text = keyword, Foreground = fg, Background = bg });
        }

        // Recolors the whole last input line (prompt included) in place, e.g. to indicate it was accepted.
        // Caller decides when this applies - must be called before any further write scrolls the area, since the targeted row is remembered by absolute position, not tracked through later scrolling.
        public void RecolorLastInput(Cli.Conclr fg, Cli.Conclr bg)
        {
            if (LastInputRow < 0)
                return;

            Console.SetCursorPosition(0, LastInputRow);
            ColorWriter.Write(fg, bg, LastInputLine);

            if (Rows.Count > 0)
                Rows[Rows.Count - 1] = new List<Part> { new Part { Text = LastInputLine, Foreground = fg, Background = bg } };
        }

        // Sets the row where the scrolling area begins, called by Tui after the header is printed
        public void Init(int top_row)
        {
            TopRow = top_row;
            CurrentRow = top_row;
            Rows.Clear();
            ScrollOffset = -1;
        }

        // Re-attaches the scroll region to a (possibly unchanged) top row and redraws every retained row, without discarding scrollback history - called by Tui when it detects a console resize, as opposed to Init() which starts a fresh session
        public void Redraw(int top_row)
        {
            TopRow = top_row;

            RedrawRows();

            CurrentRow = NextContentRow();
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
            PreparePromptRow();

            var command = ReadLineFromKeys();

            PromptActive = false;

            var row = CurrentRow;

            LastInputRow = row;
            LastInputLine = Prompt + command;

            HighlightKeywords(command, row);

            Rows.Add(BuildCommandRowParts(command));

            return command;
        }

        // Reads a line character-by-character via raw ReadConsoleInputW (Interop/ConsoleInput.cs) instead of Console.ReadLine/Console.ReadKey - keeps the cursor pinned to this row (no native wrap/scroll dragging the header), sidesteps ReadKey's lossy codepage translation, and picks up resize events immediately.
        // Renders at the live CurrentRow field so a mid-loop Body.Redraw() is picked up correctly on the next render.
        // PageUp/PageDown scroll through retained history instead of editing the line - rendered full-screen in place of the input row. Any other key snaps back to the live tail first (see ScrollOffset), then falls through to normal handling, so e.g. typing a character both exits review and gets typed.
        private string ReadLineFromKeys()
        {
            var text = new StringBuilder();
            var cursor = 0;

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

                    ApplyKey(virtual_key, character, text, ref cursor);
                }
            }
        }

        // Leaves review mode and redraws the content area as the true live tail, then re-renders the prompt row - used both when any regular key exits review, and when ScrollPageDown itself scrolls back down to the live tail
        private void SnapToLive(StringBuilder text, int cursor)
        {
            ScrollOffset = -1;
            CurrentRow = NextContentRow();
            RedrawRows();

            // make the prompt + in-progress text visible again immediately - otherwise a key that returns straight away (Enter) would submit before the row ever shows it
            RenderInputRow(CurrentRow, text, cursor);
        }

        // Moves the reviewed position one screenful back (towards older history), entering review mode if not already in it, clamped to the start of retained history
        private void ScrollPageUp()
        {
            var capacity = VisibleCapacity();
            var live_top = Math.Max(0, Rows.Count - capacity);
            var current_top = ScrollOffset >= 0 ? ScrollOffset : live_top;

            ScrollOffset = Math.Max(0, current_top - capacity);
        }

        // Moves the reviewed position one screenful forward (towards newer history), snapping back to the live tail once it catches up - a no-op if already live
        private void ScrollPageDown()
        {
            if (ScrollOffset < 0)
                return;

            var capacity = VisibleCapacity();
            var live_top = Math.Max(0, Rows.Count - capacity);
            var next_top = ScrollOffset + capacity;

            ScrollOffset = next_top >= live_top ? -1 : next_top;
        }

        // Applies a single decoded key press to the in-progress input line
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

        // Returns 2 if the two chars immediately before the cursor form a surrogate pair, 1 otherwise - so Backspace/Left can't split a pair produced by ReadConsoleInputW
        private static int SurrogatePairWidthBefore(StringBuilder text, int cursor)
        {
            if (cursor >= 2 && char.IsSurrogatePair(text[cursor - 2], text[cursor - 1]))
                return 2;

            return 1;
        }

        // Returns 2 if the two chars immediately after the cursor form a surrogate pair, 1 otherwise - so Delete/Right can't split a pair produced by ReadConsoleInputW
        private static int SurrogatePairWidthAfter(StringBuilder text, int cursor)
        {
            if (cursor + 1 < text.Length && char.IsSurrogatePair(text[cursor], text[cursor + 1]))
                return 2;

            return 1;
        }

        // Redraws the prompt + currently typed text on the given row, horizontal-scrolling the visible window so the cursor position always stays on-screen instead of wrapping to another row
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

        // Appends a row's parts to the retained buffer and, unless the user is mid-review (Manual scroll mode), writes it to the console, shifting the visible window (or repositioning a concurrently-active prompt) first if needed
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

        // Recolors any registered keyword occurrences found in the just-typed command, in place - reuses SplitByKeywords so live and post-redraw overlap resolution always agree
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

        // Builds the retained-buffer parts for a typed command row (prompt + command, keyword matches colored), mirroring what ReadCommand/HighlightKeywords already put on screen - used so a later redraw from the buffer (on scroll or resize) reproduces the same highlighting
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

        // Truncates text that would overflow the window width, avoiding a native wrap/scroll on write
        private static string ClampToWindowWidth(string text)
        {
            const string ELLIPSIS = "...";

            var max_length = Console.WindowWidth - 1;

            if (text.Length <= max_length)
                return text;

            var content_length = Math.Max(0, max_length - ELLIPSIS.Length);

            return text.Substring(0, content_length) + ELLIPSIS;
        }

        // Marks the prompt row as reserved and, if the live tail is already full, shifts the visible window so the bottom row is free for it - called before ReadLineFromKeys starts reading, mirroring what WriteRow does for a regular line
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

        // Total rows the scroll region can physically show, regardless of what's currently reserved for a live prompt
        private int VisibleCapacity()
        {
            var bottom_row = Console.WindowHeight - 1;

            // the very last row is kept blank as a safety margin against conhost's own native scroll, which would drag the fixed header along with it - cheap insurance, not worth removing
            var safe_row = Math.Max(TopRow, bottom_row - 1);

            return safe_row - TopRow + 1;
        }

        // How many of VisibleCapacity's rows are available for retained content - one less than full while a live prompt is being typed (its row is reserved), unless the user has scrolled away to review history, in which case the prompt isn't shown at all and the full capacity is history
        private int ContentCapacity()
        {
            var capacity = VisibleCapacity();

            if (PromptActive && ScrollOffset < 0)
                return Math.Max(0, capacity - 1);

            return capacity;
        }

        // The absolute row the (Rows.Count)-th line (0-indexed) belongs on - i.e. where the next written line goes, or where the live prompt sits once it's the last thing pending. Always keyed off the full VisibleCapacity, independent of whether that row is currently reserved for a prompt: the reservation only affects how much of Rows gets drawn above it (see ContentCapacity), not which row this is.
        private int NextContentRow()
        {
            return TopRow + Math.Min(Rows.Count, Math.Max(0, VisibleCapacity() - 1));
        }

        // Redraws the whole scroll region from the retained buffer - used instead of Console.MoveBufferArea, whose CHAR_INFO copy path corrupts non-ASCII glyphs (e.g. block/braille characters) under codepage 65001 in the legacy console host.
        // Sourced from ScrollOffset when reviewing history, otherwise from the live tail (the last ContentCapacity rows, leaving room for a reserved prompt row if one is active). Always blanks every row through the true bottom of the region, even past what ContentCapacity draws, so a shrinking reservation can't leave stale content behind.
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
                ColorWriter.Write(part.Foreground, part.Background, part.Text);
        }
    }
}
