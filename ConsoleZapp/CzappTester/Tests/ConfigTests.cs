using ConsoleZapp;

namespace CzappTester.Tests
{
    public static class ConfigTests
    {
        public static void AddLineWithDuplicateLiningKeyThrows()
        {
            var config = new Config();

            Check.Throws<System.ArgumentException>(() =>
                config.AddLine("1", Config.Line.Create('=', '+', 10)));
        }

        public static void AddLineWithKeyOnlyInColoringsDoesNotThrow()
        {
            var config = new Config();

            // "debug" exists in Colorings but not in Lining - must not block AddLine
            config.AddLine("debug", Config.Line.Create('=', '+', 10));

            Check.True(config.Lining.ContainsKey("debug"));
        }
    }
}
