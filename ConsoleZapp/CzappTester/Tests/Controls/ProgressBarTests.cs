using System;
using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class ProgressBarTests
    {
        public static void RendersFromFraction()
        {
            var bar = new ProgressBar("Download");
            bar.SetProgress(0.25f);

            Cli.Print.WriteLine(bar.Render());
        }

        public static void RendersFromPercent()
        {
            var bar = new ProgressBar("Upload");
            bar.SetProgress(75);

            Cli.Print.WriteLine(bar.Render());
        }

        public static void RendersEmptyAndFull()
        {
            var empty = new ProgressBar("Empty");
            empty.SetProgress(0f);
            Cli.Print.WriteLine(empty.Render());

            var full = new ProgressBar("Full");
            full.SetProgress(1f);
            Cli.Print.WriteLine(full.Render());
        }

        public static void RendersFromRange()
        {
            var bar = new ProgressBar("Range");
            bar.SetProgress(25.0, 0.0, 200.0);

            Cli.Print.WriteLine(bar.Render());
        }

        public static void RendersWithCustomMarginAndChars()
        {
            var bar = new ProgressBar("Custom");
            bar.SetProgress(0.5f);
            bar.SetMargin(3);
            bar.SetEmptyChar('.');
            bar.SetFullChar('#');

            Cli.Print.WriteLine(bar.Render());
        }

        public static void RendersAtFixedWidthAcrossPercentDigits()
        {
            var bar = new ProgressBar("Width", 40);

            bar.SetProgress(0);
            Cli.Print.WriteLine("[{0}] length {1}", bar.Render(), bar.Render().Length);

            bar.SetProgress(5);
            Cli.Print.WriteLine("[{0}] length {1}", bar.Render(), bar.Render().Length);

            bar.SetProgress(100);
            Cli.Print.WriteLine("[{0}] length {1}", bar.Render(), bar.Render().Length);
        }

        public static void AdjustsWidthAfterConstruction()
        {
            var bar = new ProgressBar("Adjustable");
            bar.SetProgress(0.5f);

            Cli.Print.WriteLine("[{0}] length {1}", bar.Render(), bar.Render().Length);

            bar.SetWidth(60);
            Cli.Print.WriteLine("[{0}] length {1}", bar.Render(), bar.Render().Length);

            bar.SetWidth(40);
            Cli.Print.WriteLine("[{0}] length {1}", bar.Render(), bar.Render().Length);
        }

        public static void ClampsWidthToSaneRange()
        {
            // Below Control.MinWidth (40) should clamp up
            var narrow = new ProgressBar("Narrow", 10);
            narrow.SetProgress(0.5f);
            Cli.Print.WriteLine("[{0}] length {1}", narrow.Render(), narrow.Render().Length);

            // Above Control.MaxWidth (120) should clamp down
            var wide = new ProgressBar("Wide", 500);
            wide.SetProgress(0.5f);
            Cli.Print.WriteLine("[{0}] length {1}", wide.Render(), wide.Render().Length);

            // SetWidth after construction is clamped the same way
            var adjusted = new ProgressBar("Adjusted");
            adjusted.SetProgress(0.5f);
            adjusted.SetWidth(5);
            Cli.Print.WriteLine("[{0}] length {1}", adjusted.Render(), adjusted.Render().Length);
        }

        public static void DoesNotCrashWhenLabelExceedsWidth()
        {
            var bar = new ProgressBar("A very long label that eats up all the space", Control.MinWidth);
            bar.SetProgress(0.5f);

            Cli.Print.WriteLine("[{0}] length {1}", bar.Render(), bar.Render().Length);
        }

        public static void SetsColors()
        {
            var bar = new ProgressBar("Colors");
            bar.SetBracketColor(BarColor.Yellow);
            bar.SetLabelColor(BarColor.Green);
            bar.SetPercentColor(BarColor.Red);
            bar.SetEmptyColor(BarColor.LightBlue);
            bar.SetFullColor(BarColor.White);

            Cli.Print.WriteLine("Bracket: {0}, Label: {1}, Percent: {2}, Empty: {3}, Full: {4}",
                bar.BracketColor, bar.LabelColor, bar.PercentColor, bar.EmptyColor, bar.FullColor);
        }

        public static void SetsAllColorsAtOnce()
        {
            var bar = new ProgressBar("Rainbow");
            bar.SetProgress(0.6f);

            bar.SetBracketColor(BarColor.White);
            bar.SetLabelColor(BarColor.Green);
            bar.SetPercentColor(BarColor.Yellow);
            bar.SetEmptyColor(BarColor.LightBlue);
            bar.SetFullColor(BarColor.Red);

            bar.Print();
            Console.WriteLine();

            Cli.Print.WriteLine("Bracket: {0} ({1})", bar.BracketColor, bar.BracketColor.ToConsoleColor());
            Cli.Print.WriteLine("Label: {0} ({1})", bar.LabelColor, bar.LabelColor.ToConsoleColor());
            Cli.Print.WriteLine("Percent: {0} ({1})", bar.PercentColor, bar.PercentColor.ToConsoleColor());
            Cli.Print.WriteLine("Empty: {0} ({1})", bar.EmptyColor, bar.EmptyColor.ToConsoleColor());
            Cli.Print.WriteLine("Full: {0} ({1})", bar.FullColor, bar.FullColor.ToConsoleColor());
        }

        public static void ConvertsColorsToConsoleColor()
        {
            foreach (BarColor color in Enum.GetValues(typeof(BarColor)))
                Cli.Print.WriteLine("{0} -> {1}", color, color.ToConsoleColor());
        }
    }
}
