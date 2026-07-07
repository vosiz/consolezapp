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

        public static void PartsDoneAddsToCurrent()
        {
            var progress = new Progress("MB");
            progress.SetTotal(10);
            progress.SetCurrent(3);

            progress.PartsDone(2);
            progress.PartsDone(1);

            Cli.Print.WriteLine(progress.Render());
        }
    }
}
