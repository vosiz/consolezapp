using System;

namespace ConsoleZapp
{
    public static class BarColorExt
    {
        // Converts a BarColor to the matching ConsoleColor
        public static ConsoleColor ToConsoleColor(this BarColor color)
        {
            switch (color)
            {
                case BarColor.White:
                    return ConsoleColor.White;

                case BarColor.Green:
                    return ConsoleColor.Green;

                case BarColor.Yellow:
                    return ConsoleColor.Yellow;

                case BarColor.LightBlue:
                    return ConsoleColor.Blue;

                case BarColor.Red:
                    return ConsoleColor.Red;

                default:
                    throw new NotSupportedException(string.Format("Unsupported color {0}.", color));
            }
        }
    }
}
