using System;
using System.Text;
using Commons = Vosiz.Commons;

namespace ConsoleZapp
{
    public class ProgressBar : Control
    {
        private static readonly Commons.Unit PERCENT_UNIT =
            new Commons.Unit("%", Commons.UnitSymbolPlacement.AfterWithSpace, true);

        public float Progress { get; private set; }

        public BarColor BracketColor { get; private set; } = BarColor.White;
        public BarColor LabelColor { get; private set; } = BarColor.White;
        public BarColor PercentColor { get; private set; } = BarColor.White;
        public BarColor EmptyColor { get; private set; } = BarColor.White;
        public BarColor FullColor { get; private set; } = BarColor.White;

        private readonly string Label;

        private int Margin = 1;
        private int Decimals = 0;
        private char EmptyChar = ' ';
        private char FullChar = '■';

        // Writes text to the console in the given color
        private static void WriteColored(string text, BarColor color)
        {
            Console.ForegroundColor = color.ToConsoleColor();
            Console.Write(text);
            Console.ResetColor();
        }

        // Constructor with label and total width
        public ProgressBar(string label, int width = 80)
        {
            Label = label;
            SetWidth(width);
        }

        // Renders control content
        public override string Render()
        {
            var layout = ComputeLayout();

            var bar = new string(FullChar, layout.fill_length) + new string(EmptyChar, layout.empty_length);
            var margin = new string(' ', Margin);

            return $"{margin}{Label} [{bar}] {layout.percent_text}{margin}";
        }

        // Renders and writes the bar directly to the console, applying per-segment colors
        public override void Print()
        {
            // matches Tui.Print()'s UTF-8 setup - ProgressBar can be printed standalone, outside a Tui, where nothing else would set this for FullChar's default box-drawing glyph
            Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

            var layout = ComputeLayout();
            var margin = new string(' ', Margin);

            Console.Write(margin);
            WriteColored(Label, LabelColor);
            Console.Write(" ");
            WriteColored("[", BracketColor);
            WriteColored(new string(FullChar, layout.fill_length), FullColor);
            WriteColored(new string(EmptyChar, layout.empty_length), EmptyColor);
            WriteColored("]", BracketColor);
            Console.Write(" ");
            WriteColored(layout.percent_text, PercentColor);
            Console.Write(margin);
        }

        // Sets progress using a 0f-1f fraction
        public void SetProgress(float fraction)
        {
            Progress = Math.Max(0f, Math.Min(1f, fraction));
        }
        // Using a 0-100 percent value
        public void SetProgress(int percent)
        {
            SetProgress(percent / 100f);
        }
        // From a value within a min-max range
        public void SetProgress(double value, double min, double max)
        {
            SetProgress((float)((value - min) / (max - min)));
        }

        // Sets the margin on both sides of the bar, clamped to non-negative
        public void SetMargin(int margin)
        {
            Margin = Math.Max(0, margin);
        }

        // Sets the number of decimal places shown in the percent text, clamped to non-negative
        public void SetDecimals(int decimals)
        {
            Decimals = Math.Max(0, decimals);
        }

        // Sets the character used for the empty portion of the bar
        public void SetEmptyChar(char empty_char)
        {
            EmptyChar = empty_char;
        }

        // Sets the character used for the filled portion of the bar
        public void SetFullChar(char full_char)
        {
            FullChar = full_char;
        }

        // Sets the color of the surrounding brackets
        public void SetBracketColor(BarColor color)
        {
            BracketColor = color;
        }

        // Sets the color of the label
        public void SetLabelColor(BarColor color)
        {
            LabelColor = color;
        }

        // Sets the color of the percent text
        public void SetPercentColor(BarColor color)
        {
            PercentColor = color;
        }

        // Sets the color of the empty portion of the bar
        public void SetEmptyColor(BarColor color)
        {
            EmptyColor = color;
        }

        // Sets the color of the filled portion of the bar
        public void SetFullColor(BarColor color)
        {
            FullColor = color;
        }

        // Computes the percent text and bar-fill split shared by Render() and Print()
        private (string percent_text, int fill_length, int empty_length) ComputeLayout()
        {
            var percent = Progress * 100f;
            var percent_text = new Commons.Quantity(Label, PERCENT_UNIT, percent).ToString(Decimals);

            var fixed_length = (Margin * 2) + Label.Length + 4 + percent_text.Length;
            var bar_width = Math.Max(0, Width - fixed_length);

            var fill_length = (int)Math.Round(bar_width * Progress);
            var empty_length = bar_width - fill_length;

            return (percent_text, fill_length, empty_length);
        }
    }
}
