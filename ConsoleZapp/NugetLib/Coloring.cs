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

            SprintfCore("Clrprintf", fmt, args);

            Console.ResetColor();
        }

        // Prints colored text by key
        public void Clrprintf(string coloringkey, string fmt, params object[] args) {

            var clr_set = ResolveColoring(coloringkey);
            Clrprintf(clr_set.Foreground, clr_set.Background, fmt, args);
        }

        // Prints colored text with new line
        public void Clrprintfln(Conclr text, Conclr back, string fmt, params object[] args) {

            Console.ForegroundColor = (ConsoleColor)text;
            Console.BackgroundColor = (ConsoleColor)back;

            SprintfCore("Clrprintfln", fmt + Environment.NewLine, args);

            Console.ResetColor();
        }

        // Prints colored text with new line using key
        public void Clrprintfln(string coloringkey, string fmt, params object[] args) {

            var clr_set = ResolveColoring(coloringkey);
            Clrprintfln(clr_set.Foreground, clr_set.Background, fmt, args);
        }

        // Resolves a coloring key to its Config.Coloring, falling back to a default Coloring if the key isn't registered
        private Config.Coloring ResolveColoring(string key) {

            return Cli.Config.Colorings.TryGetValue(key, out var coloring) ? coloring : new Config.Coloring();
        }

    }
}
