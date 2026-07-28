using System;

namespace ConsoleZapp
{
    internal static class ColorWriter
    {
        // Writes text at the current cursor position, coloring it if a foreground color is set, resetting afterwards
        internal static void Write(Cli.Conclr? fg, Cli.Conclr? bg, string text)
        {
            var has_foreground = fg.HasValue;
            var has_background = bg.HasValue;

            if (has_foreground)
                Console.ForegroundColor = (ConsoleColor)fg.Value;
            if (has_background)
                Console.BackgroundColor = (ConsoleColor)bg.Value;

            Console.Write(text);

            if (has_foreground || has_background)
                Console.ResetColor();
        }
    }
}
