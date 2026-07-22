using System;
using System.Collections.Generic;

namespace ConsoleZapp
{
    public class Container
    {
        private char BorderHorizontal = '─';
        private char BorderVertical = '│';
        private char BorderTopLeft = '┌';
        private char BorderTopRight = '┐';
        private char BorderBottomLeft = '└';
        private char BorderBottomRight = '┘';

        private Cli.Conclr? BorderForeground;
        private Cli.Conclr? BorderBackground;

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

        // Overrides the border characters used when printing this container, replacing the Unicode box-drawing default
        public void SetBorderChars(char horizontal, char vertical, char top_left, char top_right, char bottom_left, char bottom_right)
        {
            BorderHorizontal = horizontal;
            BorderVertical = vertical;
            BorderTopLeft = top_left;
            BorderTopRight = top_right;
            BorderBottomLeft = bottom_left;
            BorderBottomRight = bottom_right;
        }

        // Sets the color the border (corners, edges) is printed in
        public void SetBorderColor(Cli.Conclr fg, Cli.Conclr bg)
        {
            BorderForeground = fg;
            BorderBackground = bg;
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

            PrintBorder(width, is_top: true);

            foreach (var control in Controls.Values)
            {
                control.SetWidth(width - 4);
                PrintRow(control, width);
            }

            PrintBorder(width, is_top: false);
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

        // Prints a horizontal border line, picking corner chars based on whether it's the top or bottom edge
        private void PrintBorder(int width, bool is_top)
        {
            var left_corner = is_top ? BorderTopLeft : BorderBottomLeft;
            var right_corner = is_top ? BorderTopRight : BorderBottomRight;

            WriteBorderText(left_corner + new string(BorderHorizontal, width - 2) + right_corner);
            Console.WriteLine();
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
            WriteBorderText(BorderVertical.ToString());
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
            WriteBorderText(BorderVertical.ToString());
        }

        // Writes text in the border color, if set, resetting afterwards
        private void WriteBorderText(string text)
        {
            var has_color = BorderForeground.HasValue;

            if (has_color)
            {
                Console.ForegroundColor = (ConsoleColor)BorderForeground.Value;
                Console.BackgroundColor = (ConsoleColor)BorderBackground.Value;
            }

            Console.Write(text);

            if (has_color)
                Console.ResetColor();
        }
    }
}
