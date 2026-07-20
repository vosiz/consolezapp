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

        private int TopRow;
        private int Width;

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
            TopRow = Console.CursorTop;
            Width = width;

            PrintBorder(width);

            foreach (var control in Controls.Values)
            {
                control.SetWidth(width - 4);
                PrintRow(control.Render(), width);
            }

            PrintBorder(width);
        }

        // Re-renders a single control's row in place, leaving borders and every other row untouched
        public void UpdateControl(string name)
        {
            if (!Controls.TryGetValue(name, out var control))
                return;

            var row_index = GetControlRowIndex(name);

            Console.SetCursorPosition(0, TopRow + 1 + row_index);
            Console.Write(FormatRow(control.Render(), Width));
        }

        // Finds a control's row position within this container, based on add order
        private int GetControlRowIndex(string name)
        {
            var index = 0;

            foreach (var key in Controls.Keys)
            {
                if (key == name)
                    return index;

                index++;
            }

            return -1;
        }

        // Prints a horizontal border line
        private void PrintBorder(int width)
        {
            Console.WriteLine(BorderCorner + new string(BorderHorizontal, width - 2) + BorderCorner);
        }

        // Prints a single content row padded to the container width
        private void PrintRow(string content, int width)
        {
            Console.WriteLine(FormatRow(content, width));
        }

        // Formats a single content row, padded and wrapped in border characters
        private string FormatRow(string content, int width)
        {
            var padded = content.PadRight(width - 4);

            return $"{BorderVertical} {padded} {BorderVertical}";
        }
    }
}
