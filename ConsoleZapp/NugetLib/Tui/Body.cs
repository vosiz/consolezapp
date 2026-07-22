using System;
using System.Collections.Generic;

namespace ConsoleZapp
{
    public class Body
    {
        private string Prompt = "> ";
        private Cli.Conclr? PromptForeground;
        private Cli.Conclr? PromptBackground;

        private readonly List<Part> KeywordColors = new List<Part>();

        private string LastInputLine;
        private int LastInputRow = -1;

        private int TopRow;
        private int CurrentRow;

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
        }

        // Sets the row where the scrolling area begins, called by Tui after the header is printed
        public void Init(int top_row)
        {
            TopRow = top_row;
            CurrentRow = top_row;
        }

        // Writes a formatted line into the scrolling area, scrolling the area up if needed
        public void WriteLine(string fmt, params object[] args)
        {
            PrepareRow();

            Console.SetCursorPosition(0, CurrentRow);
            Console.Write(ClampToWindowWidth(string.Format(fmt, args)));

            CurrentRow++;
        }

        // Writes a formatted line in the given colors into the scrolling area, scrolling the area up if needed
        public void WriteLine(Cli.Conclr fg, Cli.Conclr bg, string fmt, params object[] args)
        {
            PrepareRow();

            Console.SetCursorPosition(0, CurrentRow);

            Console.ForegroundColor = (ConsoleColor)fg;
            Console.BackgroundColor = (ConsoleColor)bg;
            Console.Write(ClampToWindowWidth(string.Format(fmt, args)));
            Console.ResetColor();

            CurrentRow++;
        }

        // Writes a line built from independently colored parts into the scrolling area, scrolling the area up if needed
        public void WriteLine(IEnumerable<Part> parts)
        {
            PrepareRow();

            Console.SetCursorPosition(0, CurrentRow);

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

            CurrentRow++;
        }

        // Prints the prompt and reads a command line from the console, scrolling the area up if needed
        public string ReadCommand()
        {
            PrepareRow();

            var row = CurrentRow;

            Console.SetCursorPosition(0, row);

            var has_color = PromptForeground.HasValue;

            if (has_color)
            {
                Console.ForegroundColor = (ConsoleColor)PromptForeground.Value;
                Console.BackgroundColor = (ConsoleColor)PromptBackground.Value;
            }

            Console.Write(Prompt);

            if (has_color)
                Console.ResetColor();

            var command = Console.ReadLine();

            CurrentRow++;

            LastInputRow = row;
            LastInputLine = Prompt + command;

            HighlightKeywords(command, row);

            return command;
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

            // The very last row is kept blank on purpose: Console.ReadLine() always echoes a newline on
            // Enter regardless of input length, and writing on the literal last row would push that
            // newline past the window bottom, triggering conhost's own native scroll (which drags the
            // fixed header along with it, since it knows nothing about our MoveBufferArea-based scrolling).
            var safe_row = bottom_row - 1;

            if (CurrentRow <= safe_row)
                return;

            Console.MoveBufferArea(0, TopRow + 1, Console.WindowWidth, bottom_row - TopRow, 0, TopRow);

            CurrentRow = safe_row;
        }
    }
}
