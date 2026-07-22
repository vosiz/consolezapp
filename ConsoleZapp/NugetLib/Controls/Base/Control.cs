using System;
using System.Collections.Generic;

namespace ConsoleZapp
{
    public abstract class Control
    {
        public const int MinWidth = 40;
        public const int MaxWidth = 120;

        public Cli.Conclr? Foreground { get; private set; }
        public Cli.Conclr? Background { get; private set; }
        public bool FillRowBackground { get; private set; }

        protected int Width = 80;

        // Sets the render width available to the control, clamped to a sane range
        public virtual void SetWidth(int width)
        {
            Width = Math.Max(MinWidth, Math.Min(MaxWidth, width));
        }

        // Sets the control's foreground/background color
        public void SetColor(Cli.Conclr fg, Cli.Conclr bg)
        {
            Foreground = fg;
            Background = bg;
        }

        // Sets whether the background color fills the whole row up to the box border, or just the text
        public void SetFillRowBackground(bool fill)
        {
            FillRowBackground = fill;
        }

        // Renders control content
        public abstract string Render();

        // Returns the colored parts making up this control's content, defaults to the whole rendered content as one part
        public virtual IEnumerable<Part> GetParts()
        {
            yield return new Part { Text = Render(), Foreground = Foreground, Background = Background };
        }

        // Writes the rendered content to the console, using each part's own color
        public virtual void Print()
        {
            foreach (var part in GetParts())
            {
                var has_color = part.Foreground.HasValue;

                if (has_color)
                {
                    Console.ForegroundColor = (ConsoleColor)part.Foreground.Value;
                    Console.BackgroundColor = (ConsoleColor)part.Background.Value;
                }

                Console.Write(part.Text);

                if (has_color)
                    Console.ResetColor();
            }
        }
    }
}
