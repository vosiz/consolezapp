using ConsoleZapp;

namespace CzappTuiTester
{
    internal class Program
    {
        // Typing this command exits the loop
        private const string ExitCommand = "exit";

        // Typing this command writes a line longer than the window width, to check overflow truncation
        private const string LongLineCommand = "long";

        // Typing this command adds units to the usage counter, to test RichText per-part recoloring
        private const string TokensCommand = "add10";

        // Typing this command writes a line with non-ASCII glyphs, to check they survive a scroll
        // without corrupting into replacement chars (the retained-buffer redraw fix)
        private const string SpecialCharsCommand = "special";

        // Typing this command writes a batch of lorem-ipsum filler lines at once, to check body scrolling/redraw with a large volume of lines in one go
        private const string LoremCommand = "lorem";

        // Typing this command toggles Body's ScrollMode, to compare Manual (position pinned while reviewing) vs AutoScroll (always follows new content) - test with PageUp then "lorem"
        private const string ScrollModeCommand = "scrollmode";

        // Highlighted magenta wherever they occur in typed input (registered keyword-color test)
        private const string InfoCommand = "info";
        private const string WarnCommand = "warn";
        private const string ErrorCommand = "error";

        // Typing this command lists every command the sandbox currently has
        private const string HelpCommand = "help";

        // Total tokens for the RichText usage test, used/total turn red past 50% of this, green otherwise
        private const int TotalTokens = 50;

        // App-level "accepted" rule for the RecolorLastInput test: input longer than this is a meaningful message
        private const int AcceptedLengthThreshold = 10;

        // Number of filler lines written on startup, useful for jumping straight into a scrolled state
        private const int FillerLines = 0;

        // Number of lines the "lorem" command writes at once
        private const int LoremLineCount = 30;

        // Classic lorem-ipsum filler words, cycled to build lines of varying length
        private static readonly string[] LOREM_WORDS = "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua".Split(' ');

        // Updates the RichText usage control's parts, recoloring only the used part red past 50% usage, green otherwise
        static void UpdateTokensParts(RichText control, int used, int total)
        {
            var over_threshold = used > total * 0.5f;
            var color = over_threshold ? Cli.Conclr.Red : Cli.Conclr.Green;

            control.AddText("label", "Used: ");
            control.AddText("used", color, Cli.Conclr.DefBg, "{0}", used);
            control.AddText("sep", "/");
            control.AddText("total", "{0}", total);
            control.AddText("unit", " units");
        }

        // Builds a single numbered lorem-ipsum line of varying word count, for the "lorem" command's scrolling test
        static string BuildLoremLine(int line_number)
        {
            var word_count = 6 + (line_number % 10);
            var words = new string[word_count];

            for (var i = 0; i < word_count; i++)
                words[i] = LOREM_WORDS[(line_number + i) % LOREM_WORDS.Length];

            return string.Format("{0,2}: {1}", line_number, string.Join(" ", words));
        }

        // Prints every command the sandbox currently has
        static void PrintHelp(Tui tui)
        {
            var commands = new[] {
                HelpCommand, ExitCommand, LongLineCommand, TokensCommand, SpecialCharsCommand,
                LoremCommand, ScrollModeCommand,
            };

            tui.WriteLine("Commands: {0}", string.Join(", ", commands));
        }

        static void Main(string[] args)
        {
            var header = new Header();

            header.AddControl("title", new Text()).SetText("CzappTuiTester");
            header.AddControl("hint", new Text()).SetText("Type 'help' to list all commands");
            var cmds = header.AddControl("cmds", new Text());
            cmds.SetText("Cmds: {0}", 0);

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

            // No width passed: Tui reads the console's live width at Print() time (resize the
            // window before starting to see it reflected in the header border)
            var body = new Body();
            var tui = new Tui(header, body);

            // Border color test: header box border (corners, edges) drawn in red
            tui.SetBorderColor(Cli.Conclr.Red, Cli.Conclr.DefBg);

            // Colored prompt test: prompt itself is orange, typed-in text stays the console's normal color
            tui.SetPromptColor(Cli.Conclr.Yellowd, Cli.Conclr.DefBg);

            // Registered keyword-color test: these words get highlighted magenta wherever they occur in input
            tui.AddKeywordColor(InfoCommand, Cli.Conclr.Magenta, Cli.Conclr.DefBg);
            tui.AddKeywordColor(WarnCommand, Cli.Conclr.Magenta, Cli.Conclr.DefBg);
            tui.AddKeywordColor(ErrorCommand, Cli.Conclr.Magenta, Cli.Conclr.DefBg);

            tui.Print();

            for (var i = 1; i <= FillerLines; i++)
                tui.WriteLine("Filler line {0}", i);

            string command;

            var cmd_count = 0;
            var scroll_mode = ScrollMode.Manual;

            do
            {
                command = tui.ReadCommand();

                // RecolorLastInput test: app-level "accepted" rule - long enough to be a meaningful message
                if (command != null && command.Length > AcceptedLengthThreshold)
                    tui.RecolorLastInput(Cli.Conclr.Green, Cli.Conclr.DefBg);

                if (command == LongLineCommand)
                    tui.WriteLine("Overflow test: {0}", new string('X', 200));
                else if (command == TokensCommand)
                    tui.WriteLine("Units used: {0}/{1}", tokens_used, TotalTokens);
                else if (command == SpecialCharsCommand)
                    tui.WriteLine("Special chars: ■■■ ⣿⣿⣿ (type a few more commands to scroll this line and check it stays intact)");
                else if (command == LoremCommand)
                {
                    for (var i = 1; i <= LoremLineCount; i++)
                        tui.WriteLine(BuildLoremLine(i));
                }
                else if (command == ScrollModeCommand)
                {
                    scroll_mode = scroll_mode == ScrollMode.Manual ? ScrollMode.AutoScroll : ScrollMode.Manual;
                    tui.SetScrollMode(scroll_mode);
                    tui.WriteLine("Scroll mode: {0}", scroll_mode);
                }
                else if (command == HelpCommand)
                    PrintHelp(tui);
                else if (command != ExitCommand)
                    tui.WriteLine(Cli.Conclr.Green, Cli.Conclr.DefBg, "You said: {0}", command);

                // RichText live update test: bump token usage, recoloring used/total past 80%
                if (command == TokensCommand)
                {
                    tokens_used += 10;
                    UpdateTokensParts(tokens, tokens_used, TotalTokens);
                    tui.UpdateControl("tokens");
                }

                // Live header update test: bump the command count in place, without reprinting the header
                if (command != ExitCommand)
                {
                    cmd_count++;
                    cmds.SetText("Cmds: {0}", cmd_count);
                    tui.UpdateControl("cmds");
                }

            } while (command != ExitCommand);
        }
    }
}
