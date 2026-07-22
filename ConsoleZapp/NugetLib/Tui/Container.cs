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
                PrintRow(control, width);
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
            WriteRow(control, Width);
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

        // Prints a single control's row padded to the container width, then moves to the next line
        private void PrintRow(Control control, int width)
        {
            WriteRow(control, width);
            Console.WriteLine();
        }

        // Writes a single control's row at the current cursor position, coloring each part per its own setting
        private void WriteRow(Control control, int width)
        {
            Console.Write(BorderVertical);
            Console.Write(' ');

            var written = 0;

            foreach (var part in control.GetParts())
            {
                var has_part_color = part.Foreground.HasValue;

                if (has_part_color)
                {
                    Console.ForegroundColor = (ConsoleColor)part.Foreground.Value;
                    Console.BackgroundColor = (ConsoleColor)part.Background.Value;
                }

                Console.Write(part.Text);
                written += part.Text.Length;

                if (has_part_color)
                    Console.ResetColor();
            }

            var pad_length = Math.Max(0, width - 4 - written);
            var has_fill_color = control.FillRowBackground && control.Foreground.HasValue;

            if (has_fill_color)
            {
                Console.ForegroundColor = (ConsoleColor)control.Foreground.Value;
                Console.BackgroundColor = (ConsoleColor)control.Background.Value;
            }

            Console.Write(new string(' ', pad_length));

            if (has_fill_color)
                Console.ResetColor();

            Console.Write(' ');
            Console.Write(BorderVertical);
        }
    }
}
