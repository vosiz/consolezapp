using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class Test01_LabeledControl
    {
        public static void RendersWithUnit()
        {
            var control = new LabeledControl("Velicina", "jednotek");
            control.SetValue("{0}", 25);

            Cli.Print.WriteLine(control.Render());
        }

        public static void RendersWithoutUnit()
        {
            var control = new LabeledControl("Jmeno");
            control.SetValue("{0}", "Petr");

            Cli.Print.WriteLine(control.Render());
        }
    }
}
