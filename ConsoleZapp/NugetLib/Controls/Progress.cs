namespace ConsoleZapp
{
    public class Progress : IControl
    {
        private readonly string Unit;

        public int Current { get; private set; }
        public int Total { get; private set; }

        // Constructor with optional unit
        public Progress(string unit = null)
        {
            Unit = unit;
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

        // Renders control content
        public string Render()
        {
            var rendered = $"{Current}/{Total}";

            if (!string.IsNullOrEmpty(Unit))
                rendered += $" {Unit}";

            return rendered;
        }
    }
}
