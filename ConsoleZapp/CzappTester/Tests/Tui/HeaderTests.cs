using ConsoleZapp;

namespace CzappTester.Tests.Tui
{
    public static class HeaderTests
    {
        public static void RendersAllControlTypesInMainContainer()
        {
            var header = new Header();

            var text = header.AddControl("text", new Text());
            text.SetText("Plain text row");

            var labeled = header.AddControl("labeled", new LabeledControl("Velicina", "jednotek"));
            labeled.SetValue("{0}", 25);

            var progress = header.AddControl("progress", new Progress("items"));
            progress.SetTotal(10);
            progress.PartsDone(4);

            var percentage = header.AddControl("percentage", new Percentage("Loaded"));
            percentage.SetValue(0.42f);

            var bar = header.AddControl("bar", new ProgressBar("Download"));
            bar.SetProgress(0.6f);

            var tui = new global::ConsoleZapp.Tui(header);
            tui.Print();
        }
    }
}
