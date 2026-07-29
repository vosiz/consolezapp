using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleZapp;

namespace CzappTuiTester
{
    public class Program
    {
        // Exits the loop
        private const string EXIT_COMMAND = "exit";

        // Writes an over-width line, checks overflow truncation
        private const string LONG_LINE_COMMAND = "long";

        // Adds units to the usage counter
        // - tests RichText per-part recoloring
        private const string TOKENS_COMMAND = "add10";

        // Writes a line with non-ASCII glyphs
        // - checks they survive a scroll intact (retained-buffer redraw)
        private const string SPECIAL_CHARS_COMMAND = "special";

        // Writes a batch of lorem-ipsum lines at once
        // - checks scrolling/redraw under volume
        private const string LOREM_COMMAND = "lorem";

        // Toggles Body's ScrollMode (Manual vs AutoScroll)
        // - test with PageUp then "lorem"
        private const string SCROLL_MODE_COMMAND = "scrollmode";

        // Runs a Dialog.YesNoCancel
        // - accepts y/yes, n/no, c/cancel, else re-prompts
        private const string YES_NO_COMMAND = "yesno";

        // Runs a Stacked Dialog from plain labels
        // - auto-numbered "1".."3", checks multi-line render
        private const string MENU_COMMAND = "menu";

        // Runs a Stacked Dialog with random options
        // - 2-5 options, random capitalized lorem words
        private const string RANDOM_MENU_COMMAND = "randommenu";

        // Bounds (inclusive) for randommenu's option count
        private const int RANDOM_MENU_MIN_OPTIONS = 2;
        private const int RANDOM_MENU_MAX_OPTIONS = 5;

        // Highlighted magenta wherever typed
        // - keyword-color test
        private const string INFO_COMMAND = "info";
        private const string WARN_COMMAND = "warn";
        private const string ERROR_COMMAND = "error";

        // Lists every available command
        private const string HELP_COMMAND = "help";

        // Re-colors the "ColorPreset" row with a random pick
        private const string RANDOM_CLR_COMMAND = "randomclr";

        // Total tokens for the usage test
        // - used/total turn red past 50%, green otherwise
        private const int TOTAL_TOKENS = 50;

        // RecolorLastInput test threshold
        // - input longer than this counts as a meaningful message
        private const int ACCEPTED_LENGTH_THRESHOLD = 10;

        // Filler lines written on startup
        // - jump straight into a scrolled state
        private const int FILLER_LINES = 0;

        // Number of lines the "lorem" command writes at once
        private const int LOREM_LINE_COUNT = 30;

        // Classic lorem-ipsum filler words, cycled to build lines of varying length
        private static readonly string[] LOREM_WORDS = "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua".Split(' ');

        // Every ColorPresets field, keyed case-insensitively
        // - so typed commands match regardless of case
        private static readonly Dictionary<string, (string Name, ColorPair Pair)> PRESETS = BuildPresets();

        // Shared randomizer
        // - for "randomclr" and the initial pick at startup
        private static readonly Random RNG = new Random();

        // Builds the command lookup from ColorPresets' fields
        private static Dictionary<string, (string Name, ColorPair Pair)> BuildPresets()
        {
            var presets = new Dictionary<string, (string Name, ColorPair Pair)>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in typeof(ColorPresets).GetFields(BindingFlags.Public | BindingFlags.Static))
                presets[field.Name] = (field.Name, (ColorPair)field.GetValue(null));

            return presets;
        }

        // Updates the "ColorPreset" row
        // - name colored by the pair, then again in default colors
        // - stays readable even if the pair is low-contrast
        private static void UpdatePresetRow(RichText control, string name, ColorPair pair)
        {
            control.AddText("label", "ColorPreset: ");
            control.AddText("value", pair, name);
            control.AddText("readable", " - {0}", name);
        }

        // Picks a random ColorPresets entry, applies it to the row
        // - shared by "randomclr" and the initial pick at startup
        private static void ApplyRandomPreset(RichText control)
        {
            var picked = PRESETS.Values.ElementAt(RNG.Next(PRESETS.Count));
            UpdatePresetRow(control, picked.Name, picked.Pair);
        }

        // Updates the usage control's parts
        // - recolors the used part red past 50%, green otherwise
        private static void UpdateTokensParts(RichText control, int used, int total)
        {
            var over_threshold = used > total * 0.5f;
            var color = over_threshold ? Cli.Conclr.Red : Cli.Conclr.Green;

            control.AddText("label", "Used: ");
            control.AddText("used", color, Cli.Conclr.DefBg, "{0}", used);
            control.AddText("sep", "/");
            control.AddText("total", "{0}", total);
            control.AddText("unit", " units");
        }

        // Builds one numbered lorem-ipsum line
        // - varying word count, for the "lorem" scrolling test
        private static string BuildLoremLine(int line_number)
        {
            var word_count = 6 + (line_number % 10);
            var words = new string[word_count];

            for (var i = 0; i < word_count; i++)
                words[i] = LOREM_WORDS[(line_number + i) % LOREM_WORDS.Length];

            return string.Format("{0,2}: {1}", line_number, string.Join(" ", words));
        }

        // Builds random capitalized lorem-word labels
        // - count within RANDOM_MENU_MIN_OPTIONS..RANDOM_MENU_MAX_OPTIONS
        private static string[] BuildRandomMenuLabels()
        {
            var count = RNG.Next(RANDOM_MENU_MIN_OPTIONS, RANDOM_MENU_MAX_OPTIONS + 1);
            var labels = new string[count];

            for (var i = 0; i < count; i++)
            {
                var word = LOREM_WORDS[RNG.Next(LOREM_WORDS.Length)];
                labels[i] = char.ToUpperInvariant(word[0]) + word.Substring(1);
            }

            return labels;
        }

        // Prints every command the sandbox currently has
        private static void PrintHelp(Tui tui)
        {
            var commands = new[] {
                HELP_COMMAND, EXIT_COMMAND, LONG_LINE_COMMAND, TOKENS_COMMAND, SPECIAL_CHARS_COMMAND,
                LOREM_COMMAND, SCROLL_MODE_COMMAND, RANDOM_CLR_COMMAND, YES_NO_COMMAND, MENU_COMMAND, RANDOM_MENU_COMMAND,
            };

            tui.WriteLine("Commands: {0}", string.Join(", ", commands));
        }

        private static void Main(string[] args)
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
            UpdateTokensParts(tokens, tokens_used, TOTAL_TOKENS);

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
            tui.AddKeywordColor(INFO_COMMAND, Cli.Conclr.Magenta, Cli.Conclr.DefBg);
            tui.AddKeywordColor(WARN_COMMAND, Cli.Conclr.Magenta, Cli.Conclr.DefBg);
            tui.AddKeywordColor(ERROR_COMMAND, Cli.Conclr.Magenta, Cli.Conclr.DefBg);

            tui.Print();

            for (var i = 1; i <= FILLER_LINES; i++)
                tui.WriteLine("Filler line {0}", i);

            string command;

            var cmd_count = 0;
            var scroll_mode = ScrollMode.Manual;

            do
            {
                command = tui.ReadCommand();

                // RecolorLastInput test: app-level "accepted" rule - long enough to be a meaningful message
                if (command != null && command.Length > ACCEPTED_LENGTH_THRESHOLD)
                    tui.RecolorLastInput(Cli.Conclr.Green, Cli.Conclr.DefBg);

                if (command == LONG_LINE_COMMAND)
                    tui.WriteLine("Overflow test: {0}", new string('X', 200));
                else if (command == TOKENS_COMMAND)
                    tui.WriteLine("Units used: {0}/{1}", tokens_used, TOTAL_TOKENS);
                else if (command == SPECIAL_CHARS_COMMAND)
                    tui.WriteLine("Special chars: ■■■ ⣿⣿⣿ (type a few more commands to scroll this line and check it stays intact)");
                else if (command == LOREM_COMMAND)
                {
                    for (var i = 1; i <= LOREM_LINE_COUNT; i++)
                        tui.WriteLine(BuildLoremLine(i));
                }
                else if (command == SCROLL_MODE_COMMAND)
                {
                    scroll_mode = scroll_mode == ScrollMode.Manual ? ScrollMode.AutoScroll : ScrollMode.Manual;
                    tui.SetScrollMode(scroll_mode);
                    tui.WriteLine("Scroll mode: {0}", scroll_mode);
                }
                else if (command == YES_NO_COMMAND)
                {
                    var choice = tui.ReadDialog(Dialog.YesNoCancel("Proceed with the risky operation?")).Value;
                    tui.WriteLine("Dialog result: {0} ({1})", choice.Label, choice.Answers[0]);
                }
                else if (command == MENU_COMMAND)
                {
                    var menu = new Dialog("Pick an action:", DialogLayout.Stacked, "Start", "Pause", "Stop");
                    var choice = tui.ReadDialog(menu).Value;
                    tui.WriteLine("Dialog result: {0} ({1})", choice.Label, choice.Answers[0]);
                }
                else if (command == RANDOM_MENU_COMMAND)
                {
                    var menu = new Dialog("Pick a random option:", DialogLayout.Stacked, BuildRandomMenuLabels());
                    var choice = tui.ReadDialog(menu).Value;
                    tui.WriteLine("Dialog result: {0} ({1})", choice.Label, choice.Answers[0]);
                }
                else if (command == HELP_COMMAND)
                    PrintHelp(tui);
                else if (command == RANDOM_CLR_COMMAND)
                {
                    ApplyRandomPreset(preset_row);
                    tui.UpdateControl("preset");
                }
                else if (command != null && PRESETS.TryGetValue(command, out var matched_preset))
                {
                    UpdatePresetRow(preset_row, matched_preset.Name, matched_preset.Pair);
                    tui.UpdateControl("preset");
                }
                else if (command != EXIT_COMMAND)
                    tui.WriteLine(Cli.Conclr.Green, Cli.Conclr.DefBg, "You said: {0}", command);

                // RichText live update test: bump token usage, recoloring used/total past 80%
                if (command == TOKENS_COMMAND)
                {
                    tokens_used += 10;
                    UpdateTokensParts(tokens, tokens_used, TOTAL_TOKENS);
                    tui.UpdateControl("tokens");
                }

                // Live header update test: bump the command count in place, without reprinting the header
                if (command != EXIT_COMMAND)
                {
                    cmd_count++;
                    cmds.SetText("Cmds: {0}", cmd_count);
                    tui.UpdateControl("cmds");
                }

            } while (command != EXIT_COMMAND);
        }
    }
}
