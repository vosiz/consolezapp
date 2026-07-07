using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class Test00_Text
    {
        public static void RendersPlainText()
        {
            var text = new Text();
            text.SetText("Plain text, no formatting");

            Cli.Print.WriteLine(text.Render());
        }

        public static void RendersFormattedText()
        {
            var text = new Text();
            text.SetText("Hello, {0}! Count: {1}", "world", 42);

            Cli.Print.WriteLine(text.Render());
        }
    }
}
