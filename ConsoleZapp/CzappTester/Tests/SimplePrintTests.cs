using ConsoleZapp;

namespace CzappTester.Tests
{
    public static class SimplePrintTests
    {
        // plain write without newline
        public static void WritesPlainText() {

            Cli.Print.Sprintf("test");
        }

        // write with newline
        public static void WritesWithNewline() {

            Cli.Print.Sprintfln("test NL");
        }

        // write with format substitution
        public static void WritesFormattedValue() {

            Cli.Print.Sprintfln("int {0} as string", 123);
        }
    }
}
