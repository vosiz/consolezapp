using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class ProgressTests
    {
        public static void RendersWithoutUnit()
        {
            var progress = new Progress();
            progress.SetCurrent(3);
            progress.SetTotal(10);

            Cli.Print.WriteLine(progress.Render());
        }

        public static void RendersWithUnit()
        {
            var progress = new Progress("MB");
            progress.SetCurrent(3);
            progress.SetTotal(10);

            Cli.Print.WriteLine(progress.Render());
        }
    }
}
