using ConsoleZapp;

namespace CzappTuiTester
{
    internal class Program
    {
        // Console width passed to Tui, matches the project's min-width-80 convention
        private const int Width = 80;

        // Typing this command exits the loop
        private const string ExitCommand = "exit";

        // Typing this command writes a line longer than the window width, to check overflow truncation
        private const string LongLineCommand = "long";

        // Number of filler lines written on startup, useful for jumping straight into a scrolled state
        private const int FillerLines = 0;

        static void Main(string[] args)
        {
            var header = new Header();

            header.AddControl("title", new Text()).SetText("CzappTuiTester");
            header.AddControl("progress", new Progress("cmds")).SetTotal(0);

            var body = new Body();
            var tui = new Tui(header, body, Width);

            tui.Print();

            for (var i = 1; i <= FillerLines; i++)
                tui.WriteLine("Filler line {0}", i);

            string command;

            var progress = (Progress)header.GetControl("progress");

            do
            {
                command = tui.ReadCommand();

                if (command == LongLineCommand)
                    tui.WriteLine("Overflow test: {0}", new string('X', 200));
                else if (command != ExitCommand)
                    tui.WriteLine(Cli.Conclr.Green, Cli.Conclr.DefBg, "You said: {0}", command);

                // Live header update test: bump the command count in place, without reprinting the header
                if (command != ExitCommand)
                {
                    progress.PartsDone(1);
                    tui.UpdateControl("progress");
                }

            } while (command != ExitCommand);
        }
    }
}
