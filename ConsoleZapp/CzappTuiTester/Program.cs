using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        // Typing this command runs a Dialog.YesNoCancel - accepts "y"/"yes", "n"/"no", "c"/"cancel", anything else loops with a re-prompt
        private const string YesNoCommand = "yesno";

        // Typing this command runs a Stacked-layout Dialog built from plain labels (auto-numbered "1".."3"), to check the multi-line rendering
        private const string MenuCommand = "menu";

        // Typing this command runs a Stacked-layout Dialog with a random number of options (2-5), each a random capitalized lorem word
        private const string RandomMenuCommand = "randommenu";

        // Bounds (inclusive) for the "randommenu" command's random option count
        private const int RandomMenuMinOptions = 2;
        private const int RandomMenuMaxOptions = 5;

        // Highlighted magenta wherever they occur in typed input (registered keyword-color test)
        private const string InfoCommand = "info";
        private const string WarnCommand = "warn";
        private const string ErrorCommand = "error";

        // Typing this command lists every command the sandbox currently has
        private const string HelpCommand = "help";

        // Typing this command re-colors the "ColorPreset" header row with a randomly picked ColorPresets entry
        private const string RandomClrCommand = "randomclr";

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

        // Every ColorPresets field, name -> (canonical name, pair), keyed case-insensitively so typed commands match regardless of case
        private static readonly Dictionary<string, (string Name, ColorPair Pair)> PRESETS = BuildPresets();

        // Shared randomizer for the "randomclr" command and the initial pick at startup
        private static readonly Random RNG = new Random();

        // Reflects over ColorPresets' public static fields to build the command lookup
        static Dictionary<string, (string Name, ColorPair Pair)> BuildPresets()
        {
            var presets = new Dictionary<string, (string Name, ColorPair Pair)>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in typeof(ColorPresets).GetFields(BindingFlags.Public | BindingFlags.Static))
                presets[field.Name] = (field.Name, (ColorPair)field.GetValue(null));

            return presets;
        }

        // Updates the "ColorPreset" header row: the name colored by the given pair, then the same name again in the console's default colors, so it stays readable even if the pair itself is a low-contrast combination
        static void UpdatePresetRow(RichText control, string name, ColorPair pair)
        {
            control.AddText("label", "ColorPreset: ");
            control.AddText("value", pair, name);
            control.AddText("readable", " - {0}", name);
        }

        // Picks a random ColorPresets entry and applies it to the "ColorPreset" row - shared by the "randomclr" command and the initial pick at startup
        static void ApplyRandomPreset(RichText control)
        {
            var picked = PRESETS.Values.ElementAt(RNG.Next(PRESETS.Count));
            UpdatePresetRow(control, picked.Name, picked.Pair);
        }

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

        // Builds a random number (RandomMenuMinOptions..RandomMenuMaxOptions) of capitalized lorem-word labels, for the "randommenu" command
        static string[] BuildRandomMenuLabels()
        {
            var count = RNG.Next(RandomMenuMinOptions, RandomMenuMaxOptions + 1);
            var labels = new string[count];

            for (var i = 0; i < count; i++)
            {
                var word = LOREM_WORDS[RNG.Next(LOREM_WORDS.Length)];
                labels[i] = char.ToUpperInvariant(word[0]) + word.Substring(1);
            }

            return labels;
        }

        // Prints every command the sandbox currently has
        static void PrintHelp(Tui tui)
        {
            var commands = new[] {
                HelpCommand, ExitCommand, LongLineCommand, TokensCommand, SpecialCharsCommand,
                LoremCommand, ScrollModeCommand, RandomClrCommand, YesNoCommand, MenuCommand, RandomMenuCommand,
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

            // ColorPresets command test: colored name + readable fallback, set to a random preset at startup and by any typed preset name or "randomclr"
            var preset_row = header.AddControl("preset", new RichText());
            ApplyRandomPreset(preset_row);

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
                else if (command == YesNoCommand)
                {
                    var choice = tui.ReadDialog(Dialog.YesNoCancel("Proceed with the risky operation?")).Value;
                    tui.WriteLine("Dialog result: {0} ({1})", choice.Label, choice.Answers[0]);
                }
                else if (command == MenuCommand)
                {
                    var menu = new Dialog("Pick an action:", DialogLayout.Stacked, "Start", "Pause", "Stop");
                    var choice = tui.ReadDialog(menu).Value;
                    tui.WriteLine("Dialog result: {0} ({1})", choice.Label, choice.Answers[0]);
                }
                else if (command == RandomMenuCommand)
                {
                    var menu = new Dialog("Pick a random option:", DialogLayout.Stacked, BuildRandomMenuLabels());
                    var choice = tui.ReadDialog(menu).Value;
                    tui.WriteLine("Dialog result: {0} ({1})", choice.Label, choice.Answers[0]);
                }
                else if (command == HelpCommand)
                    PrintHelp(tui);
                else if (command == RandomClrCommand)
                {
                    ApplyRandomPreset(preset_row);
                    tui.UpdateControl("preset");
                }
                else if (command != null && PRESETS.TryGetValue(command, out var matched_preset))
                {
                    UpdatePresetRow(preset_row, matched_preset.Name, matched_preset.Pair);
                    tui.UpdateControl("preset");
                }
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
