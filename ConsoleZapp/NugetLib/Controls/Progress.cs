namespace ConsoleZapp
{
    public class Progress : IControl
    {
        private readonly string PartSymbol;

        public int Current { get; private set; }
        public int Total { get; private set; }

        // Constructor with optional part symbol
        public Progress(string part_symbol = null)
        {
            PartSymbol = part_symbol;
        }

        // Sets current value
        public void SetCurrent(int current)
        {
            Current = current;
        }

        // Sets total value
        public void SetTotal(int total)
        {
            Total = total;
        }

        // Adds a number of finished parts to Current
        public void PartsDone(int count)
        {
            Current += count;
        }

        // Renders control content
        public string Render()
        {
            var rendered = $"{Current}/{Total}";

            if (!string.IsNullOrEmpty(PartSymbol))
                rendered += $" {PartSymbol}";

            return rendered;
        }
    }
}
