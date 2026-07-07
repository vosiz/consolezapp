using System;
using ConsoleZapp;

namespace CzappTester.Tests.Controls
{
    public static class TextTests
    {
        public static void RendersPlainText()
        {
            var text = new Text();
            text.SetText("Plain text, no formatting");

            text.Print();
            Console.WriteLine();
        }

        public static void RendersFormattedText()
        {
            var text = new Text();
            text.SetText("Hello, {0}! Count: {1}", "world", 42);

            text.Print();
            Console.WriteLine();
        }
    }
}
