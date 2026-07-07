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

        public static void RendersFromRatio()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(0.5f);

            Cli.Print.WriteLine(percentage.Render());
        }

        public static void RendersWithZeroDecimals()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(1f / 3f);

            Cli.Print.WriteLine(percentage.Render(0));
        }

        public static void RendersWithOneDecimal()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(1f / 3f);

            Cli.Print.WriteLine(percentage.Render(1));
        }

        public static void RendersWithTwoDecimals()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(1f / 3f);

            Cli.Print.WriteLine(percentage.Render(2));
        }

        public static void RendersWithThreeDecimals()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(1f / 3f);

            Cli.Print.WriteLine(percentage.Render(3));
        }

        public static void RendersSmallValueWithMilliPrefix()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(0.00005f);

            Cli.Print.WriteLine(percentage.Render());
        }

        public static void RendersSmallValueWithMicroPrefix()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(0.000005f);

            Cli.Print.WriteLine(percentage.Render());
        }

        public static void RendersLargeValueWithKiloPrefix()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(15000);

            Cli.Print.WriteLine(percentage.Render());
        }
    }
}
