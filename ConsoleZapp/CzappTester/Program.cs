using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CzappTester.Tests;
using ConsoleZapp;


namespace CzappTester
{
    class Program
    {

        static void RunTest(int testn) {

            switch (testn) {

                case 0:
                    Cli.Init();
                    Test00_SimplePrint.Write("test");
                    Test00_SimplePrint.WriteNl("test NL");
                    Test00_SimplePrint.WriteNl("int {0} as string", 123);
                    break;

                case 1:
                    Cli.Init();
                    Test01_Coloring.ColorOnce();
                    Test01_Coloring.Classics();
                    Test01_Coloring.Custom();
                    break;

                case 2:
                    Cli.Init();
                    Test02_SimpleStruct.Lines();
                    Test02_SimpleStruct.HeadLines();
                    Test02_SimpleStruct.CustomLines();
                    Test02_SimpleStruct.CustomHeadlines();
                    break;

                case 3:
                    Cli.Init();
                    // TODO
                    break;

                default:
                    Console.WriteLine($"Test level ({testn}) is not defined");
                    break;
            }
        }

        static void Test(params int[] nums) {

            foreach (var n in nums) {

                RunTest(n);
            }
        }

        // MAIN
        static void Main(string[] args)
        {

            Test(0, 1, 2);

            Console.ReadLine();
        }
    }
}
