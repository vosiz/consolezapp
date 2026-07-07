using System;
using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class LabeledControlTests
    {
        public static void RendersWithUnit()
        {
            var control = new LabeledControl("Velicina", "jednotek");
            control.SetValue("{0}", 25);

            control.Print();
            Console.WriteLine();
        }

        public static void RendersWithoutUnit()
        {
            var control = new LabeledControl("Jmeno");
            control.SetValue("{0}", "Petr");

            control.Print();
            Console.WriteLine();
        }
    }
}
