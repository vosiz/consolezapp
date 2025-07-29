using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleZapp;

namespace CzappTester.Tests
{
    public static class Test00_SimplePrint
    {
        public static void Write(string fmt, params object[] args)
        {
            Cli.Print.Sprintf(fmt, args);
        }

        public static void WriteNl(string fmt, params object[] args)
        {
            Cli.Print.Sprintfln(fmt, args);
        }
    }
}
