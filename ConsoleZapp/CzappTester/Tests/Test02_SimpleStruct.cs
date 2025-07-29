using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleZapp;

namespace CzappTester.Tests
{
    public static class Test02_SimpleStruct
    {
        // line lvls
        public static void Lines() {

            var lvls = Cli.Config.Lining
                .Where(x => x.Key.All(char.IsDigit))
                .Select(y => y.Key)
                .ToList();

            foreach (var lvl_key in lvls) {

                Cli.Print.Line(lvl_key);
            }
        }

        // headline lvls
        public static void HeadLines() {

            var lvls = Cli.Config.Lining
                .Where(x => x.Key.All(char.IsDigit))
                .Select(y => y.Key)
                .ToList();

            foreach (var lvl_key in lvls)
            {
                Cli.Print.Headline(lvl_key, lvl_key);
            }
        }

        // custom line
        public static void CustomLines() {

            var line = Config.Line.CreateCustom("====***{0}***===");
            Cli.Config.AddLine("mahline", line);
            Cli.Print.Headline("mahline", "Headline text");
        }

        // custom headline
        public static void CustomHeadlines() {

            var clr = new Config.Coloring(Cli.Conclr.Blued, Cli.Conclr.Cyan);
            var line = Config.Line.CreateCustom("---=== {0}: {1} ||", clr);
            Cli.Config.AddLine("complexline", line);
            Cli.Print.Headline("complexline", "Headline text", 123.ToString());
        }
    }
}
