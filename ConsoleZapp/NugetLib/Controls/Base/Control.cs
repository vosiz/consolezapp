using System;

namespace ConsoleZapp
{
    public abstract class Control
    {
        public const int MinWidth = 40;
        public const int MaxWidth = 120;

        protected int Width = 80;

        // Sets the render width available to the control, clamped to a sane range
        public virtual void SetWidth(int width)
        {
            Width = Math.Max(MinWidth, Math.Min(MaxWidth, width));
        }

        // Renders control content
        public abstract string Render();

        // Writes the rendered content to the console
        public virtual void Print()
        {
            Console.Write(Render());
        }
    }
}
