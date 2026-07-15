using System;
using System.IO;
using ConsoleZapp;

namespace CzappTester.Tests.Tui
{
    public static class TuiTests
    {
        public static void ReadCommandReturnsNullWithoutBody()
        {
            var header = new Header();
            var tui = new global::ConsoleZapp.Tui(header);

            Check.Equal(null, tui.ReadCommand());
        }

        public static void ReadCommandDelegatesToBody()
        {
            var original_in = Console.In;

            try
            {
                Console.SetIn(new StringReader("status\n"));

                var header = new Header();
                var body = new Body();
                var tui = new global::ConsoleZapp.Tui(header, body);

                Check.Equal("status", tui.ReadCommand());
            }
            finally
            {
                Console.SetIn(original_in);
            }
        }
    }
}
