using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CzappTester.Tests
{

    public static class TestRunner
    {

        // Runs every test method found in the assembly
        // - public static parameterless void, on any *Tests class
        public static void RunAll(Assembly assembly) {

            var test_classes = assembly.GetTypes()
                .Where(t => t.IsClass && t.IsAbstract && t.IsSealed && t.Name.EndsWith("Tests"))
                .OrderBy(t => t.FullName);

            var passed = 0;
            var failures = new List<(string Name, string Reason)>();

            foreach (var test_class in test_classes) {

                var methods = test_class.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.GetParameters().Length == 0 && m.ReturnType == typeof(void))
                    .OrderBy(m => m.Name);

                foreach (var method in methods) {

                    var name = string.Format("{0}.{1}", test_class.FullName, method.Name);

                    try {

                        method.Invoke(null, null);
                        Console.WriteLine(string.Format("[PASS] {0}", name));
                        passed++;

                    } catch (TargetInvocationException exc) {

                        var reason = exc.InnerException?.Message;
                        Console.WriteLine(string.Format("[FAIL] {0} - {1}", name, reason));
                        failures.Add((name, reason));
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine(string.Format("Passed: {0}, Failed: {1}", passed, failures.Count));

            if (failures.Count > 0) {

                Console.WriteLine();
                Console.WriteLine("Failed tests:");

                foreach (var failure in failures)
                    Console.WriteLine(string.Format("{0} - {1}", failure.Name, failure.Reason));
            }
        }

    }
}
