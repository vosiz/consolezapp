using System.Collections.Generic;
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

        // Typing this command adds tokens to the usage counter, to test RichText per-part recoloring
        private const string TokensCommand = "tok";

        // Typing this command writes a body line with several independently colored parts
        private const string TypesCommand = "types";

        // Typing one of these severities updates the "Last state" header control to match its color
        private const string InfoCommand = "info";
        private const string WarnCommand = "warn";
        private const string ErrorCommand = "error";

        // Total tokens for the RichText usage test, used/total turn red past 50% of this, green otherwise
        private const int TotalTokens = 50;

        // Number of filler lines written on startup, useful for jumping straight into a scrolled state
        private const int FillerLines = 0;

        // Updates the RichText usage control's parts, recoloring only the used part red past 50% usage, green otherwise
        static void UpdateTokensParts(RichText control, int used, int total)
        {
            var over_threshold = used > total * 0.5f;
            var color = over_threshold ? Cli.Conclr.Red : Cli.Conclr.Green;

            control.AddText("label", "Used: ");
            control.AddText("used", color, Cli.Conclr.DefBg, "{0}", used);
            control.AddText("sep", "/");
            control.AddText("total", "{0}", total);
            control.AddText("unit", " tokens");
        }

        // Shared severity -> color mapping, used by both the "types" line and the "Last state" header control
        static void GetSeverityColor(string severity, out Cli.Conclr fg, out Cli.Conclr bg)
        {
            switch (severity)
            {
                case InfoCommand:
                    fg = Cli.Conclr.White;
                    bg = Cli.Conclr.Green;
                    break;
                case WarnCommand:
                    fg = Cli.Conclr.Black;
                    bg = Cli.Conclr.Yellowd;
                    break;
                case ErrorCommand:
                    fg = Cli.Conclr.White;
                    bg = Cli.Conclr.Red;
                    break;
                default:
                    fg = Cli.Conclr.DefFg;
                    bg = Cli.Conclr.DefBg;
                    break;
            }
        }

        // Builds a "Types: info warn error" line, each severity independently colored, to test Body.WriteLine(Part[])
        static List<Part> BuildTypesLine()
        {
            var parts = new List<Part> { new Part { Text = "Types: " } };

            foreach (var severity in new[] { InfoCommand, WarnCommand, ErrorCommand })
            {
                if (parts.Count > 1)
                    parts.Add(new Part { Text = " " });

                GetSeverityColor(severity, out var fg, out var bg);
                parts.Add(new Part { Text = severity, Foreground = fg, Background = bg });
            }

            return parts;
        }

        // Updates the "Last state" header control's value part, coloring it to match the given severity
        static void UpdateLastState(RichText control, string severity)
        {
            control.AddText("label", "Last state: ");

            if (severity == null)
                control.AddText("value", "unknown");
            else
            {
                GetSeverityColor(severity, out var fg, out var bg);
                control.AddText("value", fg, bg, severity);
            }
        }

        static void Main(string[] args)
        {
            var header = new Header();

            header.AddControl("title", new Text()).SetText("CzappTuiTester");
            header.AddControl("progress", new Progress("cmds")).SetTotal(0);

            // Color test: full-row background fill vs. text-only background
            var colored_full = header.AddControl("colored_full", new Text());
            colored_full.SetText("Full-row color (fill background)");
            colored_full.SetColor(Cli.Conclr.Yellow, Cli.Conclr.Blued);
            colored_full.SetFillRowBackground(true);

            var colored_text = header.AddControl("colored_text", new Text());
            colored_text.SetText("Text-only color (no fill)");
            colored_text.SetColor(Cli.Conclr.Black, Cli.Conclr.Yellow);

            // RichText color test: per-part recoloring (used turns red past 50% usage)
            var tokens = header.AddControl("tokens", new RichText());
            var tokens_used = 0;
            UpdateTokensParts(tokens, tokens_used, TotalTokens);

            // RichText live update test: "Last state: unknown" recolors to match the last typed severity
            var last_state = header.AddControl("last_state", new RichText());
            UpdateLastState(last_state, null);

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
                else if (command == TokensCommand)
                    tui.WriteLine("Tokens used: {0}/{1}", tokens_used, TotalTokens);
                else if (command == TypesCommand)
                    tui.WriteLine(BuildTypesLine());
                else if (command != ExitCommand)
                    tui.WriteLine(Cli.Conclr.Green, Cli.Conclr.DefBg, "You said: {0}", command);

                // RichText live update test: bump token usage, recoloring used/total past 80%
                if (command == TokensCommand)
                {
                    tokens_used += 10;
                    UpdateTokensParts(tokens, tokens_used, TotalTokens);
                    tui.UpdateControl("tokens");
                }

                // RichText live update test: recolor "Last state" to match the typed severity
                if (command == InfoCommand || command == WarnCommand || command == ErrorCommand)
                {
                    UpdateLastState(last_state, command);
                    tui.UpdateControl("last_state");
                }

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
