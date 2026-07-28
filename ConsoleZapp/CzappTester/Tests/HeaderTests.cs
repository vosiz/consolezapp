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
    }
}
