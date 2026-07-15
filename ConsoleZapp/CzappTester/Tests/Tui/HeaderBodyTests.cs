using System;
using System.IO;
using ConsoleZapp;

namespace CzappTester.Tests.Tui
{
    public static class HeaderBodyTests
    {
        public static void PrintsHeaderThenReadsBodyCommand()
        {
            var header = new Header();
            header.AddControl("status", new Text()).SetText("Status: Ready");

            var body = new Body();
            body.SetPrompt("cmd> ");

            var tui = new global::ConsoleZapp.Tui(header, body);
            tui.Print();

            var original_in = Console.In;
            string command;

            try
            {
                Console.SetIn(new StringReader("help\n"));
                command = tui.ReadCommand();
            }
            finally
            {
                Console.SetIn(original_in);
            }

            Console.WriteLine();
            Cli.Print.WriteLine("Entered command: {0}", command);

            Check.Equal("help", command);
        }
    }
}
