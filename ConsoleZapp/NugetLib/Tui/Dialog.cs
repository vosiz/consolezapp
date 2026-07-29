using System.Collections.Generic;

namespace ConsoleZapp
{
    public class Dialog
    {
        public readonly string Question;
        public readonly List<DialogOption> Options;
        public readonly DialogLayout Layout;

        // Builds a Yes/No dialog - accepts "y"/"yes" and "n"/"no"
        public static Dialog YesNo(string question)
        {
            return new Dialog(
                question,
                DialogLayout.Inline,
                new DialogOption("Yes", "y", "yes"),
                new DialogOption("No", "n", "no"));
        }

        // Builds a Yes/No/Cancel dialog - adds "c"/"cancel" to YesNo
        public static Dialog YesNoCancel(string question)
        {
            return new Dialog(
                question,
                DialogLayout.Inline,
                new DialogOption("Yes", "y", "yes"),
                new DialogOption("No", "n", "no"),
                new DialogOption("Cancel", "c", "cancel"));
        }

        // Auto-numbers plain labels "1".."9", one accepted answer per option
        private static List<DialogOption> BuildNumberedOptions(string[] labels)
        {
            var options = new List<DialogOption>();

            for (var i = 0; i < labels.Length; i++)
                options.Add(new DialogOption(labels[i], (i + 1).ToString()));

            return options;
        }

        // Constructor with explicit, self-answered options
        public Dialog(string question, DialogLayout layout, params DialogOption[] options)
        {
            Question = question;
            Layout = layout;
            Options = new List<DialogOption>(options);
        }
        // Constructor from plain labels, auto-numbered "1".."9"
        public Dialog(string question, DialogLayout layout, params string[] labels)
        {
            Question = question;
            Layout = layout;
            Options = BuildNumberedOptions(labels);
        }

        // Builds the lines to print for this dialog, per its layout
        public IEnumerable<string> BuildLines()
        {
            if (Layout == DialogLayout.Stacked)
                return BuildStackedLines();

            return new[] { BuildInlineLine() };
        }

        // Matches typed input against options, case-insensitively
        // - returns null if nothing matches
        public DialogOption? Match(string input)
        {
            var normalized = (input ?? string.Empty).Trim().ToLowerInvariant();

            foreach (var option in Options)
                foreach (var answer in option.Answers)
                    if (answer == normalized)
                        return option;

            return null;
        }

        // Builds a single "Question Label1 [answer1], Label2 [answer2]" line
        private string BuildInlineLine()
        {
            var parts = new List<string>();

            foreach (var option in Options)
                parts.Add(string.Format("{0} [{1}]", option.Label, option.Answers[0]));

            return string.Format("{0} {1}", Question, string.Join(", ", parts));
        }

        // Builds the question + one "answer) Label" line per option
        private List<string> BuildStackedLines()
        {
            var lines = new List<string> { Question };

            foreach (var option in Options)
                lines.Add(string.Format("{0}) {1}", option.Answers[0], option.Label));

            return lines;
        }
    }
}
