using System.Reflection;
using CzappTester.Tests;
using ConsoleZapp;

namespace CzappTester
{
    class Program
    {
        // MAIN
        static void Main(string[] args)
        {

            Cli.Init();

            TestRunner.RunAll(Assembly.GetExecutingAssembly());

            System.Console.ReadLine();
        }
    }
}
