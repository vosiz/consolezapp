using System;
using Commons = Vosiz.Commons;

namespace ConsoleZapp
{
    public class Percentage : Control
    {
        private static readonly Commons.Unit PercentUnit = 
            new Commons.Unit("%", Commons.UnitSymbolPlacement.AfterWithSpace, true);

        public float Value { get; private set; }

        private readonly string Label;

        // Constructor with label
        public Percentage(string label)
        {
            Label = label;
        }

        // Sets percent value (0-100), clamped to that range
        public void SetValue(int percent)
        {
            Value = Math.Max(0f, Math.Min(100f, percent));
        }

        // Sets percent value from a 0.0f-1.0f ratio, clamped to that range
        public void SetValue(float ratio)
        {
            Value = Math.Max(0f, Math.Min(100f, ratio * 100f));
        }

        // Renders control content, whole numbers
        public override string Render()
        {
            return Render(0);
        }

        // Renders control content with given decimal places
        public string Render(int decimals)
        {
            var quantity = new Commons.Quantity(Label, PercentUnit, Value);
            return $"{Label}: {quantity.ToString(decimals)}";
        }
    }
}
