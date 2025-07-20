using System;
using BoltSDK.Tests;
using System.Linq;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🚀 Bolt Unity SDK - Standalone Test Suite");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            try
            {
                // Run all tests
                Console.WriteLine("Running all tests...");
                var result = BoltSDKTestRunner.RunAllTests();

                // Display results
                Console.WriteLine();
                Console.WriteLine("📊 Test Results:");
                Console.WriteLine("=================");
                Console.WriteLine(result.ToString());
                Console.WriteLine();

                // Display detailed results
                Console.WriteLine("📋 Detailed Results:");
                Console.WriteLine("====================");
                foreach (var fixtureResult in result.FixtureResults)
                {
                    Console.WriteLine(fixtureResult.ToString());

                    // Show failed tests if any
                    var failedTests = fixtureResult.TestResults.Where(t => !t.Success);
                    if (failedTests.Any())
                    {
                        Console.WriteLine("  ❌ Failed Tests:");
                        foreach (var test in failedTests)
                        {
                            Console.WriteLine($"    - {test.TestName}: {test.ErrorMessage}");
                        }
                    }
                    Console.WriteLine();
                }

                // Summary
                Console.WriteLine("🎯 Summary:");
                Console.WriteLine("============");
                Console.WriteLine($"Total Tests: {result.TotalTests}");
                Console.WriteLine($"Passed: {result.PassedTests}");
                Console.WriteLine($"Failed: {result.FailedTests}");
                Console.WriteLine($"Success Rate: {(result.TotalTests > 0 ? (result.PassedTests * 100.0 / result.TotalTests) : 0):F1}%");
                Console.WriteLine();

                if (result.Success)
                {
                    Console.WriteLine("✅ All tests passed successfully!");
                }
                else
                {
                    Console.WriteLine("❌ Some tests failed. Check the detailed results above.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error running tests: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}