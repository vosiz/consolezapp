using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleZapp;

namespace CzappTester.Tests
{
    public static class Test01_Coloring
    {

        public static void ColorOnce() {

            Cli.Print.Sprintfln("Default");
            Cli.Print.Clrprintfln(Cli.Conclr.Magenta, Cli.Conclr.Cyand, "Colored with format? {0}", "yes");
            Cli.Print.Sprintfln("Default");
        }

        public static void Classics() {

            Cli.Print.Debug("This is {0}", "debug");
            Cli.Print.Info("This is {0}", "info");
            Cli.Print.Warning("This is {0}", "warning");
            Cli.Print.Error("This is {0}", "error");
            Cli.Print.Exception("This is {0}", "exception");
            Cli.Print.Success("This is {0}", "success");
            Cli.Print.Fail("This is {0}", "fail");
        }

        public static void Custom() {

            Cli.Config.AddColoring("my_color", new Config.Coloring(Cli.Conclr.White, Cli.Conclr.Blued));
            Cli.Print.Custom("my_color", "Custom color with key -- {0}", "my_color");
        }
    }
}
