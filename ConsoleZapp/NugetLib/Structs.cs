using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleZapp
{
    public partial class Printer
    {

        // Print line by level (0 based -> 0 is first)
        public void Line0(int level) {

            Line(level + 1);
        }

        // Print line by level
        public void Line(int level) {

            Line(level.ToString());
        }

        // Print line by key
        public void Line(string key) {

            try
            {
                if (!Cli.Config.Lining.ContainsKey(key))
                    throw new ArgumentException($"Line with key ({key}) not found");

                var lining = Cli.Config.Lining[key];
                PrintLine(lining);

            }
            catch (Exception exc) {

                throw new Exception("Line print exception", exc);
            }
        }

        // Prints headline
        public void Headline(int level, string headline) {

            Headline(level.ToString(), headline);
        }

        // Prints headline by key
        public void Headline(string key, params string[] str_pars) {

            try
            {
                if (!Cli.Config.Lining.ContainsKey(key))
                    throw new ArgumentException($"Headline with key ({key}) not found");

                var lining = Cli.Config.Lining[key];
                lining.Args = str_pars;
                PrintLine(lining);

            }
            catch (Exception exc) {

                throw new Exception("Line print exception", exc);
            }
        }


        // Prints line
        private void PrintLine(Config.Line line) {

            var clr = line.Coloring;
            Cli.Print.Clrprintfln(clr.Foreground, clr.Background, line.ToString());
        }

    }
}
