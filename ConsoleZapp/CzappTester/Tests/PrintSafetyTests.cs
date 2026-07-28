using ConsoleZapp;

namespace CzappTester.Tests.Basic
{
    public static class PrintSafetyTests
    {
        public static void DoubleFormatFix()
        {
            // Sprintf used to format its own already-formatted output a second time,
            // choking on any literal braces that ended up in the substituted text.
            var json = "{\"type\":\"error\",\"error\":{\"type\":\"authentication_error\",\"message\":\"invalid x-api-key\"}}";

            Cli.Print.WriteLine("Printing raw JSON as an arg (should NOT throw):");
            Cli.Print.Error("{0}", json);
        }

        public static void LegitFormatStillWorks()
        {
            Cli.Print.WriteLine("Legit format placeholders still work:");
            Cli.Print.Success("Value is {0}, count is {1}", "test", 42);
        }

        public static void ThrowsPrintException()
        {
            Cli.Print.WriteLine("Triggering a real format error on purpose (expect PrintException):");

            try
            {
                Cli.Print.Write("Missing arg: {0} {1}", "only one");
            }
            catch (PrintException exc)
            {
                Cli.Print.WriteLine("Caught PrintException as expected:");
                Cli.Print.WriteLine(exc.Message.EscapeFormat());
                Cli.Print.WriteLine("Inner: {0}", exc.InnerException?.GetType().Name);
            }
        }

        public static void SprintflnFailureReportsItsOwnName()
        {
            Cli.Print.WriteLine("Triggering Sprintfln() with a bad format on purpose (expect PrintException):");

            PrintException caught = null;

            try
            {
                Cli.Print.Sprintfln("Missing arg: {0} {1}", "only one");
            }
            catch (PrintException exc)
            {
                caught = exc;
            }

            Check.True(caught != null, "Expected PrintException to be thrown");
            Check.True(caught.Message.StartsWith("Sprintfln failed"),
                $"Expected message to start with 'Sprintfln failed', got: {caught.Message}");
        }

        public static void ClrprintflnFailureReportsItsOwnName()
        {
            Cli.Print.WriteLine("Triggering Clrprintfln() with a bad format on purpose (expect PrintException):");

            PrintException caught = null;

            try
            {
                Cli.Print.Clrprintfln(Cli.Conclr.Red, Cli.Conclr.DefBg, "Missing arg: {0} {1}", "only one");
            }
            catch (PrintException exc)
            {
                caught = exc;
            }

            Check.True(caught != null, "Expected PrintException to be thrown");
            Check.True(caught.Message.StartsWith("Clrprintfln failed"),
                $"Expected message to start with 'Clrprintfln failed', got: {caught.Message}");
        }

        public static void LineWithUnknownKeyThrowsPrintException()
        {
            Cli.Print.WriteLine("Triggering Line() with an unknown key on purpose (expect PrintException):");

            Check.Throws<PrintException>(() => Cli.Print.Line("no_such_key"));
        }

        public static void HeadlineWithUnknownKeyThrowsPrintException()
        {
            Cli.Print.WriteLine("Triggering Headline() with an unknown key on purpose (expect PrintException):");

            Check.Throws<PrintException>(() => Cli.Print.Headline("no_such_key", "text"));
        }

        public static void EscapeHelpers()
        {
            var raw = "curly {braces} and \"quotes\" and a\nnewline";

            Cli.Print.WriteLine("Raw text printed via EscapeFormat (should NOT throw):");
            Cli.Print.WriteLine(raw.EscapeFormat());

            // EscapeJson only escapes quotes/backslashes/control chars for JSON embedding -
            // it does NOT escape braces, so its output must still go through the "{0}" arg
            // pattern (or EscapeFormat) when printed, same as any other unescaped text.
            Cli.Print.WriteLine("Same text escaped for JSON:");
            Cli.Print.WriteLine("{0}", raw.EscapeJson());
        }
    }
}
