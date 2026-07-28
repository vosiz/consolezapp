using ConsoleZapp;

namespace CzappTester.Tests
{
    public static class HeaderTests
    {
        public static void AddContainerLetsControlsTargetIt()
        {
            var header = new Header();
            header.AddContainer("side");

            var control = header.AddControl("greeting", new Text(), "side");
            control.SetText("Hello from the side container");

            Check.Equal(control, header.GetControl("greeting", "side"));
        }

        public static void AddContainerWithDuplicateIdThrows()
        {
            var header = new Header();

            Check.Throws<System.ArgumentException>(() => header.AddContainer("main"));
        }

        public static void AddControlWithUnknownContainerThrows()
        {
            var header = new Header();

            Check.Throws<System.Collections.Generic.KeyNotFoundException>(() =>
                header.AddControl("orphan", new Text(), "unknown"));
        }

        public static void PrintWithOnlyForegroundPartColorDoesNotThrow()
        {
            var header = new Header();
            var richText = header.AddControl("only_fg", new RichText());
            richText.AddText("part", Cli.Conclr.Red, null, "Only foreground set");

            header.Print(40);
        }

        public static void PrintWithOnlyBackgroundPartColorDoesNotThrow()
        {
            var header = new Header();
            var richText = header.AddControl("only_bg", new RichText());
            richText.AddText("part", null, Cli.Conclr.Blue, "Only background set");

            header.Print(40);
        }
    }
}
