using System;
using System.Collections.Generic;

namespace ConsoleZapp
{
    public class Container
    {
        private const char BorderHorizontal = '-';
        private const char BorderVertical = '|';
        private const char BorderCorner = '+';

        private readonly Dictionary<string, Control> Controls = new Dictionary<string, Control>();

        // Constructor
        public Container()
        {
        }

        // Adds a control under the given name, returns it back for chaining
        public T AddControl<T>(string name, T control) where T : Control
        {
            Controls[name] = control;
            return control;
        }

        // Retrieves a previously added control by name
        public Control GetControl(string name)
        {
            return Controls.TryGetValue(name, out var control) ? control : null;
        }

        // Returns the total row count this container takes up when printed (borders + one row per control)
        public int GetHeight()
        {
            return 2 + Controls.Count;
        }

        // Prints the container as a bordered box, one control per row
        public void Print(int width)
        {
            PrintBorder(width);

            foreach (var control in Controls.Values)
            {
                control.SetWidth(width - 4);
                PrintRow(control.Render(), width);
            }

            PrintBorder(width);
        }

        // Prints a horizontal border line
        private void PrintBorder(int width)
        {
            Console.WriteLine(BorderCorner + new string(BorderHorizontal, width - 2) + BorderCorner);
        }

        // Prints a single content row padded to the container width
        private void PrintRow(string content, int width)
        {
            var padded = content.PadRight(width - 4);

            Console.WriteLine($"{BorderVertical} {padded} {BorderVertical}");
        }
    }
}
