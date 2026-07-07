using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class PercentageTests
    {
        public static void RendersValue()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(42);

            Cli.Print.WriteLine(percentage.Render());
        }
    }
}
