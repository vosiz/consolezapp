namespace ConsoleZapp
{
    public class Percentage : IControl
    {
        private readonly string Label;

        public int Value { get; private set; }

        // Constructor with label
        public Percentage(string label)
        {
            Label = label;
        }

        // Sets percent value
        public void SetValue(int percent)
        {
            Value = percent;
        }

        // Renders control content
        public string Render()
        {
            return $"{Label}: {Value}%";
        }
    }
}
