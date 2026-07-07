using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class Test04_ProgressBar
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
    }
}
