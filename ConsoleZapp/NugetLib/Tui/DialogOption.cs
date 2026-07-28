namespace ConsoleZapp
{
    public readonly struct DialogOption
    {
        public readonly string Label;
        public readonly string[] Answers;

        // Lowercases every accepted answer
        // - so Dialog.Match never cares about casing
        private static string[] ToLowerAll(string[] answers)
        {
            var lowered = new string[answers.Length];

            for (var i = 0; i < answers.Length; i++)
                lowered[i] = answers[i].ToLowerInvariant();

            return lowered;
        }

        // Constructor with a label and its accepted answer(s)
        // - matched case-insensitively
        // - first answer is canonical (display + Match's return)
        public DialogOption(string label, params string[] answers)
        {
            Label = label;
            Answers = ToLowerAll(answers);
        }
    }
}
