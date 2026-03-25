using System;
using CalibrationTuning.Tests.Unit;

namespace TestRunner
{
    class PreservationTestRunner
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=== Preservation Property Tests Runner ===");
            Console.WriteLine("Running tests on UNFIXED code to establish baseline behavior");
            Console.WriteLine();

            var testFixture = new MainFormLoggingTabPreservationTests();
            int passedTests = 0;
            int failedTests = 0;

            // Test 1: Start Tuning
            RunTest("Preservation_StartTuning_ClearsDataGridViewAndAddsSettingRow",
                () => {
                    testFixture.SetUp();
                    testFixture.Preservation_StartTuning_ClearsDataGridViewAndAddsSettingRow();
                },
                ref passedTests, ref failedTests);

            // Test 2: Tuning Iterations
            RunTest("Preservation_TuningIterations_AddDataRowsWithAllColumns",
                () => {
                    testFixture.SetUp();
                    testFixture.Preservation_TuningIterations_AddDataRowsWithAllColumns();
                },
                ref passedTests, ref failedTests);

            // Test 3: Stop Tuning
            RunTest("Preservation_StopTuning_AddsSettingRow",
                () => {
                    testFixture.SetUp();
                    testFixture.Preservation_StopTuning_AddsSettingRow();
                },
                ref passedTests, ref failedTests);

            // Test 4: Auto-Scroll
            RunTest("Preservation_DataGridView_AutoScrollsToLatestEntry",
                () => {
                    testFixture.SetUp();
                    testFixture.Preservation_DataGridView_AutoScrollsToLatestEntry();
                },
                ref passedTests, ref failedTests);

            // Test 5: Status Panel
            RunTest("Preservation_StatusPanel_DisplaysTuningStatusAndMeasurements",
                () => {
                    testFixture.SetUp();
                    testFixture.Preservation_StatusPanel_DisplaysTuningStatusAndMeasurements();
                },
                ref passedTests, ref failedTests);

            // Test 6: Connection Tab
            RunTest("Preservation_ConnectionTab_FunctionsCorrectly",
                () => {
                    testFixture.SetUp();
                    testFixture.Preservation_ConnectionTab_FunctionsCorrectly();
                },
                ref passedTests, ref failedTests);

            // Test 7: Chart Tab
            RunTest("Preservation_ChartTab_FunctionsCorrectly",
                () => {
                    testFixture.SetUp();
                    testFixture.Preservation_ChartTab_FunctionsCorrectly();
                },
                ref passedTests, ref failedTests);

            // Summary
            Console.WriteLine();
            Console.WriteLine("=== Test Summary ===");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Passed: {passedTests}");
            Console.ResetColor();
            
            if (failedTests > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed: {failedTests}");
                Console.ResetColor();
            }
            
            Console.WriteLine($"Total: {passedTests + failedTests}");
            Console.WriteLine();

            if (passedTests == 7 && failedTests == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("SUCCESS: All preservation tests PASSED on unfixed code.");
                Console.WriteLine("Baseline behavior has been documented and will be preserved after the fix.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: Some preservation tests failed on unfixed code.");
                Console.WriteLine("This may indicate issues with the test implementation or unexpected baseline behavior.");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void RunTest(string testName, Action testAction, ref int passedTests, ref int failedTests)
        {
            Console.Write($"Running {testName}... ");
            
            try
            {
                testAction();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("PASSED");
                Console.ResetColor();
                passedTests++;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("FAILED");
                Console.ResetColor();
                Console.WriteLine($"  Error: {ex.Message}");
                failedTests++;
            }
        }
    }
}
