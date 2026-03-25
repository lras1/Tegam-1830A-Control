using System;
using CalibrationTuning.Tests.Unit;

namespace TestRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=== Bug Condition Exploration Test Runner ===");
            Console.WriteLine();

            var testFixture = new MainFormLoggingTabBugConditionTests();
            
            try
            {
                Console.WriteLine("Running SetUp...");
                testFixture.SetUp();
                
                Console.WriteLine("Running BugCondition test...");
                Console.WriteLine();
                
                testFixture.BugCondition_MainForm_ShouldHaveLoggingTabWithDataGridView_AndStatusPanelWithoutDataGridView();
                
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("TEST PASSED (UNEXPECTED - Bug may not exist or test is incorrect)");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("TEST FAILED (EXPECTED - This confirms the bug exists)");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Exception Details:");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
