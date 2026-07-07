using System;
using Commons = Vosiz.Commons;

namespace ConsoleZapp
{
    public class Percentage : IControl
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

        // Sets percent value (0-100)
        public void SetValue(int percent)
        {
            Value = percent;
        }

        // Sets percent value from a 0.0f-1.0f ratio
        public void SetValue(float ratio)
        {
            Value = ratio * 100f;
        }

        // Renders control content, whole numbers
        public string Render()
        {
            return Render(0);
        }

        // Renders control content with given decimal places
        public string Render(int decimals)
        {
            var quantity = new Commons.Quantity(Label, PercentUnit, Value);
            return $"{Label}: {quantity.ToString(decimals)}";
        }

        // Writes the rendered content to the console
        public void Print()
        {
            Console.Write(Render());
        }
    }
}
