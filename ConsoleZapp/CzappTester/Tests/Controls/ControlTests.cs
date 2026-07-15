using System;
using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class ControlTests
    {
        public static void DefaultPrintDelegatesToRender()
        {
            var text = new Text();
            text.SetText("Default Print via base Control");

            text.Print();
            Console.WriteLine();
        }

        public static void SetWidthIsNoOpForControlsThatDontUseIt()
        {
            var labeled = new LabeledControl("Velicina", "jednotek");
            labeled.SetValue("{0}", 25);

            Cli.Print.WriteLine(labeled.Render());

            labeled.SetWidth(20);
            Cli.Print.WriteLine(labeled.Render());
        }
    }
}
