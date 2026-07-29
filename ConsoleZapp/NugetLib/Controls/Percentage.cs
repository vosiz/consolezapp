using System;
using Commons = Vosiz.Commons;

namespace ConsoleZapp
{
    public class Percentage : Control
    {
        private static readonly Commons.Unit PERCENT_UNIT =
            new Commons.Unit("%", Commons.UnitSymbolPlacement.AfterWithSpace, true);

        public float Value { get; private set; }

        private readonly string Label;

        // Constructor with label
        public Percentage(string label)
        {
            Label = label;
        }

        // Renders control content, whole numbers
        public override string Render()
        {
            return Render(0);
        }
        // With given decimal places
        public string Render(int decimals)
        {
            var quantity = new Commons.Quantity(Label, PERCENT_UNIT, Value);
            return $"{Label}: {quantity.ToString(decimals)}";
        }

        // Sets percent value (0-100), clamped to that range
        public void SetValue(int percent)
        {
            Value = Math.Max(0f, Math.Min(100f, percent));
        }
        // From a 0.0f-1.0f ratio
        public void SetValue(float ratio)
        {
            Value = Math.Max(0f, Math.Min(100f, ratio * 100f));
        }
    }
}
