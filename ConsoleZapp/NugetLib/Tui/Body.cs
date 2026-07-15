using System;

namespace ConsoleZapp
{
    public class Body
    {
        private string Prompt = "> ";

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
            Console.Write(string.Format(fmt, args));

            CurrentRow++;
        }

        // Prints the prompt and reads a command line from the console, scrolling the area up if needed
        public string ReadCommand()
        {
            PrepareRow();

            Console.SetCursorPosition(0, CurrentRow);
            Console.Write(Prompt);

            var command = Console.ReadLine();

            CurrentRow++;

            return command;
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
