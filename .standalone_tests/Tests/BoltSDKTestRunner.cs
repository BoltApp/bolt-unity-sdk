using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace BoltSDK.Tests
{
    /// <summary>
    /// Test runner for Bolt SDK tests that can be used programmatically
    /// </summary>
    public static class BoltSDKTestRunner
    {
        /// <summary>
        /// Runs all Bolt SDK tests and returns the results
        /// </summary>
        /// <returns>Test results summary</returns>
        public static TestRunResult RunAllTests()
        {
            var result = new TestRunResult();
            var testAssembly = Assembly.GetExecutingAssembly();

            try
            {
                // Discover and run all test fixtures
                var testFixtures = DiscoverTestFixtures(testAssembly);

                foreach (var fixture in testFixtures)
                {
                    var fixtureResult = RunTestFixture(fixture);
                    result.AddFixtureResult(fixtureResult);
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Runs a specific test fixture
        /// </summary>
        /// <param name="fixtureType">The test fixture type to run</param>
        /// <returns>Test fixture result</returns>
        public static TestFixtureResult RunTestFixture(Type fixtureType)
        {
            var result = new TestFixtureResult
            {
                FixtureName = fixtureType.Name
            };

            try
            {
                var instance = Activator.CreateInstance(fixtureType);
                var methods = fixtureType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                // Run setup if exists
                var setupMethod = methods.FirstOrDefault(m => m.GetCustomAttribute<SetUpAttribute>() != null);
                if (setupMethod != null)
                {
                    setupMethod.Invoke(instance, null);
                }

                // Run all test methods
                var testMethods = methods.Where(m => m.GetCustomAttribute<TestAttribute>() != null);
                foreach (var testMethod in testMethods)
                {
                    var testResult = RunTestMethod(instance, testMethod);
                    result.AddTestResult(testResult);
                }

                // Run teardown if exists
                var teardownMethod = methods.FirstOrDefault(m => m.GetCustomAttribute<TearDownAttribute>() != null);
                if (teardownMethod != null)
                {
                    teardownMethod.Invoke(instance, null);
                }

                result.Success = result.TestResults.All(r => r.Success);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Runs a specific test method
        /// </summary>
        /// <param name="instance">Test fixture instance</param>
        /// <param name="testMethod">Test method to run</param>
        /// <returns>Test result</returns>
        public static TestResult RunTestMethod(object? instance, MethodInfo testMethod)
        {
            var result = new TestResult
            {
                TestName = testMethod.Name
            };

            try
            {
                testMethod.Invoke(instance, null);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Discovers all test fixtures in the assembly
        /// </summary>
        /// <param name="assembly">Assembly to search</param>
        /// <returns>List of test fixture types</returns>
        private static List<Type> DiscoverTestFixtures(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TestFixtureAttribute>() != null)
                .ToList();
        }
    }

    /// <summary>
    /// Result of running all tests
    /// </summary>
    public class TestRunResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<TestFixtureResult> FixtureResults { get; } = new();

        public void AddFixtureResult(TestFixtureResult result)
        {
            FixtureResults.Add(result);
        }

        public int TotalTests => FixtureResults.Sum(f => f.TestResults.Count);
        public int PassedTests => FixtureResults.Sum(f => f.TestResults.Count(t => t.Success));
        public int FailedTests => TotalTests - PassedTests;

        public override string ToString()
        {
            return $"Test Run: {PassedTests}/{TotalTests} tests passed. {(Success ? "SUCCESS" : "FAILED")}";
        }
    }

    /// <summary>
    /// Result of running a test fixture
    /// </summary>
    public class TestFixtureResult
    {
        public string FixtureName { get; set; } = "";
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<TestResult> TestResults { get; } = new();

        public void AddTestResult(TestResult result)
        {
            TestResults.Add(result);
        }

        public override string ToString()
        {
            return $"{FixtureName}: {TestResults.Count(t => t.Success)}/{TestResults.Count} tests passed. {(Success ? "SUCCESS" : "FAILED")}";
        }
    }

    /// <summary>
    /// Result of running a single test
    /// </summary>
    public class TestResult
    {
        public string TestName { get; set; } = "";
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public override string ToString()
        {
            return $"{TestName}: {(Success ? "PASSED" : "FAILED")}";
        }
    }
}