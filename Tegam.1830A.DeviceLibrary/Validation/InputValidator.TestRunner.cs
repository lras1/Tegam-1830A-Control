using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tegam._1830A.DeviceLibrary.Validation.Tests
{
    /// <summary>
    /// Simple test runner for InputValidator tests.
    /// </summary>
    public class InputValidatorTestRunner
    {
        public static void RunAllTests()
        {
            var testClass = new InputValidatorTests();
            var methods = typeof(InputValidatorTests).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            
            int passed = 0;
            int failed = 0;
            var failures = new List<string>();

            foreach (var method in methods)
            {
                if (method.Name.StartsWith("Validate") && method.ReturnType == typeof(void))
                {
                    try
                    {
                        method.Invoke(testClass, null);
                        passed++;
                        Console.WriteLine($"✓ {method.Name}");
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        Console.WriteLine($"✗ {method.Name}");
                        failures.Add($"{method.Name}: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }
            }

            Console.WriteLine($"\n{passed} passed, {failed} failed");
            if (failures.Count > 0)
            {
                Console.WriteLine("\nFailures:");
                foreach (var failure in failures)
                {
                    Console.WriteLine($"  - {failure}");
                }
            }
        }
    }
}
