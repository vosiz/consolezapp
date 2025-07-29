using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleZapp.Cli;

namespace ConsoleZapp
{

    public partial class Printer
    {

        // Prints colored text
        public void Clrprintf(Conclr text, Conclr back, string fmt, params object[] args) {

            Console.ForegroundColor = (ConsoleColor)text;
            Console.BackgroundColor = (ConsoleColor)back;

            Sprintf(fmt, args);

            Console.ResetColor();
        }

        // Prints colored text by key
        public void Clrprintf(string coloringkey, string fmt, params object[] args) {

            var clr_set = new Config.Coloring();
            if (Cli.Config.Colorings.ContainsKey(coloringkey))
            {
                clr_set = Cli.Config.Colorings[coloringkey];
            }

            Clrprintf(clr_set.Foreground, clr_set.Background, fmt, args);
        }

        // Prints colored text with new line
        public void Clrprintfln(Conclr text, Conclr back, string fmt, params object[] args) {

            fmt += Environment.NewLine;
            Clrprintf(text, back, fmt, args);
        }

        // Prints colored text with new line using key
        public void Clrprintfln(string coloringkey, string fmt, params object[] args) {

            fmt += Environment.NewLine;
            Clrprintf(coloringkey, fmt, args);
        }


    }
}
