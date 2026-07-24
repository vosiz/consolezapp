using System;

namespace ConsoleZapp
{
    internal static class ColorWriter
    {
        // Writes text at the current cursor position, coloring it if a foreground color is set, resetting afterwards
        internal static void Write(Cli.Conclr? fg, Cli.Conclr? bg, string text)
        {
            var has_color = fg.HasValue;

            if (has_color)
            {
                Console.ForegroundColor = (ConsoleColor)fg.Value;
                Console.BackgroundColor = (ConsoleColor)bg.Value;
            }

            Console.Write(text);

            if (has_color)
                Console.ResetColor();
        }
    }
}
