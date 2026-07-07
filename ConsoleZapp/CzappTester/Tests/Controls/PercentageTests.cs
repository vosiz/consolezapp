using System;
using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class PercentageTests
    {
        public static void RendersValue()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(42);

            percentage.Print();
            Console.WriteLine();
        }

        public static void RendersFromRatio()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(0.5f);

            percentage.Print();
            Console.WriteLine();
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

            percentage.Print();
            Console.WriteLine();
        }

        public static void RendersSmallValueWithMicroPrefix()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(0.000005f);

            percentage.Print();
            Console.WriteLine();
        }

        public static void RendersLargeValueWithKiloPrefix()
        {
            var percentage = new Percentage("Progress");
            percentage.SetValue(15000);

            percentage.Print();
            Console.WriteLine();
        }
    }
}
