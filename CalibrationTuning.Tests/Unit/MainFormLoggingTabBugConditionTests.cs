using System;
using System.Linq;
using System.Windows.Forms;
using CalibrationTuning;
using CalibrationTuning.Controllers;
using CalibrationTuning.Models;
using CalibrationTuning.UserControls;
using NUnit.Framework;
using Moq;

namespace CalibrationTuning.Tests.Unit
{
    /// <summary>
    /// Bug condition exploration test for DataGridView placement on Logging tab.
    /// **Validates: Requirements 2.1, 2.2, 2.3, 2.4**
    /// 
    /// CRITICAL: This test MUST FAIL on unfixed code - failure confirms the bug exists.
    /// DO NOT attempt to fix the test or the code when it fails.
    /// 
    /// This test encodes the expected behavior - it will validate the fix when it passes after implementation.
    /// </summary>
    [TestFixture]
    public class MainFormLoggingTabBugConditionTests
    {
        private Mock<ITuningController> _mockTuningController;
        private Mock<IDataLoggingController> _mockDataLoggingController;
        private Mock<IConfigurationController> _mockConfigurationController;

        [SetUp]
        public void SetUp()
        {
            _mockTuningController = new Mock<ITuningController>();
            _mockDataLoggingController = new Mock<IDataLoggingController>();
            _mockConfigurationController = new Mock<IConfigurationController>();

            // Setup minimal configuration to allow form to load
            _mockConfigurationController
                .Setup(c => c.LoadDeviceConfiguration())
                .Returns(new DeviceConfiguration());

            _mockConfigurationController
                .Setup(c => c.LoadLastParameters())
                .Returns(new TuningParameters());

            _mockConfigurationController
                .Setup(c => c.LoadLastLogPath())
                .Returns(string.Empty);

            _mockDataLoggingController
                .Setup(c => c.CurrentLogFile)
                .Returns(string.Empty);
        }

        /// <summary>
        /// Bug Condition Exploration Test: Verifies expected behavior for DataGridView on Logging tab.
        /// 
        /// EXPECTED OUTCOME ON UNFIXED CODE: Test FAILS (this is correct - it proves the bug exists)
        /// 
        /// Expected counterexamples on unfixed code:
        /// - MainForm only has 3 tabs instead of 4
        /// - No Logging tab exists in the tab control
        /// - StatusPanel contains a DataGridView control
        /// - LoggingPanel class does not exist
        /// 
        /// **Validates: Requirements 2.1, 2.2, 2.3, 2.4**
        /// </summary>
        [Test]
        public void BugCondition_MainForm_ShouldHaveLoggingTabWithDataGridView_AndStatusPanelWithoutDataGridView()
        {
            // Arrange
            MainForm form = null;

            try
            {
                form = new MainForm(
                    _mockTuningController.Object,
                    _mockDataLoggingController.Object,
                    _mockConfigurationController.Object);

                form.Show();

                // Get the tab control using reflection to access private field
                var tabControlField = typeof(MainForm).GetField("_tabControl", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Assert.IsNotNull(tabControlField, "Could not find _tabControl field in MainForm");

                var tabControl = tabControlField.GetValue(form) as TabControl;
                Assert.IsNotNull(tabControl, "_tabControl is null");

                // **Property 1: Bug Condition - DataGridView on Logging Tab**
                // Test 1: MainForm has exactly 4 tabs (Connection, Tuning, Chart, Logging)
                // EXPECTED ON UNFIXED CODE: FAIL - only 3 tabs exist
                Assert.AreEqual(4, tabControl.TabPages.Count, 
                    "COUNTEREXAMPLE: MainForm should have exactly 4 tabs (Connection, Tuning, Chart, Logging), " +
                    $"but found {tabControl.TabPages.Count} tabs");

                // Test 2: MainForm._tabControl contains a tab named "Logging"
                // EXPECTED ON UNFIXED CODE: FAIL - Logging tab doesn't exist
                var loggingTab = tabControl.TabPages.Cast<TabPage>()
                    .FirstOrDefault(tp => tp.Text == "Logging");
                Assert.IsNotNull(loggingTab, 
                    "COUNTEREXAMPLE: MainForm should have a tab named 'Logging', but it does not exist");

                // Test 3: StatusPanel does NOT contain a DataGridView control
                // EXPECTED ON UNFIXED CODE: FAIL - StatusPanel contains DataGridView
                var tuningTab = tabControl.TabPages.Cast<TabPage>()
                    .FirstOrDefault(tp => tp.Text == "Tuning");
                Assert.IsNotNull(tuningTab, "Tuning tab should exist");

                var statusPanel = FindControlRecursive<StatusPanel>(tuningTab);
                Assert.IsNotNull(statusPanel, "StatusPanel should exist on Tuning tab");

                var dataGridInStatusPanel = FindControlRecursive<DataGridView>(statusPanel);
                Assert.IsNull(dataGridInStatusPanel, 
                    "COUNTEREXAMPLE: StatusPanel should NOT contain a DataGridView control, " +
                    "but a DataGridView was found in StatusPanel");

                // Test 4: MainForm has a LoggingPanel on the Logging tab
                // EXPECTED ON UNFIXED CODE: FAIL - LoggingPanel doesn't exist
                var loggingPanel = FindControlByTypeName(loggingTab, "LoggingPanel");
                Assert.IsNotNull(loggingPanel, 
                    "COUNTEREXAMPLE: MainForm should have a LoggingPanel on the Logging tab, " +
                    "but LoggingPanel was not found");

                // Additional verification: LoggingPanel should contain a DataGridView
                var dataGridInLoggingPanel = FindControlRecursive<DataGridView>(loggingPanel);
                Assert.IsNotNull(dataGridInLoggingPanel, 
                    "LoggingPanel should contain a DataGridView for measurement history");
            }
            finally
            {
                form?.Close();
                form?.Dispose();
            }
        }

        /// <summary>
        /// Helper method to recursively find a control of a specific type.
        /// </summary>
        private T FindControlRecursive<T>(Control parent) where T : Control
        {
            if (parent == null)
                return null;

            foreach (Control control in parent.Controls)
            {
                if (control is T typedControl)
                    return typedControl;

                var found = FindControlRecursive<T>(control);
                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        /// Helper method to recursively find a control by type name (for types that don't exist yet).
        /// </summary>
        private Control FindControlByTypeName(Control parent, string typeName)
        {
            if (parent == null)
                return null;

            foreach (Control control in parent.Controls)
            {
                if (control.GetType().Name == typeName)
                    return control;

                var found = FindControlByTypeName(control, typeName);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}
