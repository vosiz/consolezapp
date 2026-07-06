using System;

namespace ConsoleZapp
{
    public class ProgressBar : IControl
    {
        private const char FillChar = '=';
        private const char EmptyChar = ' ';
        private const int Margin = 1;

        private readonly string Label;
        private readonly int Width;

        public float Progress { get; private set; }

        // Constructor with label and total width
        public ProgressBar(string label, int width = 80)
        {
            Label = label;
            Width = width;
        }

        // Sets progress using a 0f-1f fraction
        public void SetProgress(float fraction)
        {
            Progress = Math.Max(0f, Math.Min(1f, fraction));
        }

        // Sets progress using a 0-100 percent value
        public void SetProgress(int percent)
        {
            SetProgress(percent / 100f);
        }

        // Renders control content
        public string Render()
        {
            var percent = (int)Math.Round(Progress * 100);
            var percent_text = percent.ToString().PadLeft(3);

            var fixed_length = (Margin * 2) + Label.Length + 8;
            var bar_width = Width - fixed_length;

            var fill_length = (int)Math.Round(bar_width * Progress);
            var empty_length = bar_width - fill_length;

            var bar = new string(FillChar, fill_length) + new string(EmptyChar, empty_length);
            var margin = new string(' ', Margin);

            return $"{margin}{Label} [{bar}] {percent_text}%{margin}";
        }
    }
}
