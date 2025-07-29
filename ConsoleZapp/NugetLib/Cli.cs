using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleZapp
{
    public static class Cli
    {
        public enum Conclr {

            // defaults
            DefaultForeground   = ConsoleColor.White,
            DefaultBackground   = ConsoleColor.Black,
            DefFg               = DefaultForeground,
            DefBg               = DefaultBackground,

            // BW
            White   = ConsoleColor.White,
            Gray    = ConsoleColor.Gray,
            Black   = ConsoleColor.Black,

            // RGB
            Red     = ConsoleColor.Red,
            Green   = ConsoleColor.Green,
            Blue    = ConsoleColor.Blue,

            // CMY
            Cyan    = ConsoleColor.Cyan,
            Magenta = ConsoleColor.Magenta,
            Yellow  = ConsoleColor.Yellow,

            // darkened
            Grayd       = ConsoleColor.DarkGray,
            Redd        = ConsoleColor.DarkRed,
            Greend      = ConsoleColor.DarkGreen,
            Blued       = ConsoleColor.DarkBlue,
            Cyand       = ConsoleColor.DarkCyan,
            Magentad    = ConsoleColor.DarkMagenta,
            Yellowd     = ConsoleColor.DarkYellow,
        }

        public static Printer Print { get; private set; }
        public static Config Config { get; private set; }

        public static void Init(Config config = null)
        {
            Config = config;
            if (config == null)
                Config = new Config();  // default

            Print = new Printer();
        }

    }
}
