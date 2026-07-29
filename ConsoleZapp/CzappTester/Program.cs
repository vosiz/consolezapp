using System.Reflection;
using CzappTester.Tests;
using ConsoleZapp;

namespace CzappTester
{
    public class Program
    {
        // Entry point
        private static void Main(string[] args)
        {

            Cli.Init();

            TestRunner.RunAll(Assembly.GetExecutingAssembly());

            System.Console.ReadLine();
        }
    }
}
