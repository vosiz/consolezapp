namespace ConsoleZapp
{
    public readonly struct DialogOption
    {
        public readonly string Label;
        public readonly string[] Answers;

        // Lowercases every accepted answer, so Dialog.Match never needs to care about the casing they were declared with
        private static string[] ToLowerAll(string[] answers)
        {
            var lowered = new string[answers.Length];

            for (var i = 0; i < answers.Length; i++)
                lowered[i] = answers[i].ToLowerInvariant();

            return lowered;
        }

        // Constructor with a label and one or more accepted typed answers, matched case-insensitively - the first is the canonical answer, used both for display (e.g. the "[y]" hint) and as the value Dialog.Match returns
        public DialogOption(string label, params string[] answers)
        {
            Label = label;
            Answers = ToLowerAll(answers);
        }
    }
}
