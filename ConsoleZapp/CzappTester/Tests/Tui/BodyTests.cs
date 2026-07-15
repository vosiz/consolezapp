using System;
using System.IO;
using ConsoleZapp;

namespace CzappTester.Tests.Tui
{
    public static class BodyTests
    {
        public static void ReadCommandReturnsTypedLine()
        {
            var original_in = Console.In;

            try
            {
                Console.SetIn(new StringReader("do something\n"));

                var body = new Body();
                var command = body.ReadCommand();

                Check.Equal("do something", command);
            }
            finally
            {
                Console.SetIn(original_in);
            }
        }

        public static void ReadCommandPrintsCustomPrompt()
        {
            var original_in = Console.In;
            var original_out = Console.Out;

            try
            {
                Console.SetIn(new StringReader("quit\n"));

                var output = new StringWriter();
                Console.SetOut(output);

                var body = new Body();
                body.SetPrompt("cmd> ");
                var command = body.ReadCommand();

                Check.Equal("cmd> ", output.ToString());
                Check.Equal("quit", command);
            }
            finally
            {
                Console.SetIn(original_in);
                Console.SetOut(original_out);
            }
        }
    }
}
