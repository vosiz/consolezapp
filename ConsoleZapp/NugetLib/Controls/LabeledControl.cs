namespace ConsoleZapp
{
    public class LabeledControl : Control
    {
        private readonly string Label;
        private readonly string Unit;
        private readonly Text ValueText = new Text();

        // Constructor with label
        public LabeledControl(string label, string unit = null)
        {
            Label = label;
            Unit = unit;
        }

        // Sets value using format string and arguments
        public void SetValue(string fmt, params object[] args)
        {
            ValueText.SetText(fmt, args);
        }

        // Renders control content
        public override string Render()
        {
            var rendered = $"{Label}: {ValueText.Render()}";

            if (!string.IsNullOrEmpty(Unit))
                rendered += $" {Unit}";

            return rendered;
        }
    }
}
