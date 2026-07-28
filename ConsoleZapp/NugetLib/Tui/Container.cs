using System;

namespace ConsoleZapp
{
    public class Container
    {
        private char BorderHorizontal   = '─';
        private char BorderVertical     = '│';
        private char BorderTopLeft      = '┌';
        private char BorderTopRight     = '┐';
        private char BorderBottomLeft   = '└';
        private char BorderBottomRight  = '┘';

        private Cli.Conclr? BorderForeground;
        private Cli.Conclr? BorderBackground;

        private readonly OrderedMap<Control> Controls = new OrderedMap<Control>();

        private int TopRow;
        private int Width;

        // Constructor
        public Container() { }

        // Adds a control, returns it back for chaining
        public T AddControl<T>(string name, T control) where T : Control
        {
            Controls.Set(name, control);
            return control;
        }

        // Retrieves a previously added control by name
        public Control GetControl(string name)
        {
            return Controls.TryGetValue(name, out var control) ? control : null;
        }

        // Overrides the Unicode box-drawing border chars
        public void SetBorderChars(
            char horizontal,
            char vertical,
            char top_left,
            char top_right,
            char bottom_left,
            char bottom_right)
        {
            BorderHorizontal = horizontal;
            BorderVertical = vertical;
            BorderTopLeft = top_left;
            BorderTopRight = top_right;
            BorderBottomLeft = bottom_left;
            BorderBottomRight = bottom_right;
        }

        // Sets the border's color
        public void SetBorderColor(Cli.Conclr fg, Cli.Conclr bg)
        {
            BorderForeground = fg;
            BorderBackground = bg;
        }

        // Returns total printed row count
        // - borders + one row per control
        public int GetHeight()
        {
            return 2 + Controls.Count;
        }

        // Prints as a bordered box, one control per row
        public void Print(int width)
        {
            TopRow = Console.CursorTop;
            Width = width;

            var row = TopRow;

            PrintBorder(width, is_top: true, row);
            row++;

            foreach (var control in Controls.Values)
            {
                control.SetWidth(width - 4);
                Console.SetCursorPosition(0, row);
                WriteRow(control, width);
                row++;
            }

            PrintBorder(width, is_top: false, row);
            row++;

            // a row exactly Console.WindowWidth characters wide (border+padding+border, see WriteRow/PrintBorder) makes conhost auto-wrap on its own last character - relying on Console.WriteLine() here would advance a second time on top of that, so every subsequent write is positioned explicitly instead
            Console.SetCursorPosition(0, row);
        }

        // Re-renders one control's row in place
        // - borders and other rows untouched
        public void UpdateControl(string name)
        {
            if (!Controls.TryGetValue(name, out var control))
                return;

            var row_index = GetControlRowIndex(name);

            Console.SetCursorPosition(0, TopRow + 1 + row_index);
            WriteRow(control, Width);
        }

        // Finds a control's row position, by add order
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

        // Prints a horizontal border line at the given row
        // - picks corner chars for top vs. bottom edge
        private void PrintBorder(int width, bool is_top, int row)
        {
            var left_corner = is_top ? BorderTopLeft : BorderBottomLeft;
            var right_corner = is_top ? BorderTopRight : BorderBottomRight;

            Console.SetCursorPosition(0, row);
            WriteBorderText(left_corner + new string(BorderHorizontal, Math.Max(0, width - 2)) + right_corner);
        }

        // Writes a control's row at the cursor
        // - each part in its own color
        private void WriteRow(Control control, int width)
        {
            WriteBorderText(BorderVertical.ToString());
            Console.Write(' ');

            var available = Math.Max(0, width - 4);
            var written = 0;

            foreach (var part in control.GetParts())
            {
                if (written >= available)
                    break;

                var text = part.Text;
                var remaining = available - written;

                if (text.Length > remaining)
                    text = text.Substring(0, remaining);

                ColorWriter.Write(part.Foreground, part.Background, text);
                written += text.Length;
            }

            var pad_length = Math.Max(0, width - 4 - written);
            var fill_fg = control.FillRowBackground ? control.Foreground : null;
            var fill_bg = control.FillRowBackground ? control.Background : null;

            ColorWriter.Write(fill_fg, fill_bg, new string(' ', pad_length));

            Console.Write(' ');
            WriteBorderText(BorderVertical.ToString());
        }

        // Writes text in the border color, if set
        private void WriteBorderText(string text)
        {
            ColorWriter.Write(BorderForeground, BorderBackground, text);
        }
    }
}
