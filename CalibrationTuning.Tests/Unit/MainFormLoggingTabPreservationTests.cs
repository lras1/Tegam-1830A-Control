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
    /// Preservation property tests for DataGridView logging functionality.
    /// **Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8**
    /// 
    /// IMPORTANT: These tests observe and document baseline behavior on UNFIXED code.
    /// These tests should PASS on the unfixed code to establish the baseline behavior that must be preserved.
    /// 
    /// After the fix is implemented, these same tests should still PASS to confirm no regressions.
    /// </summary>
    [TestFixture]
    public class MainFormLoggingTabPreservationTests
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
        /// Preservation Test: Start Tuning clears DataGridView and adds "Start Tuning" setting row.
        /// 
        /// EXPECTED OUTCOME ON UNFIXED CODE: Test PASSES (confirms baseline behavior)
        /// 
        /// **Validates: Requirements 3.2, 3.5**
        /// </summary>
        [Test]
        public void Preservation_StartTuning_ClearsDataGridViewAndAddsSettingRow()
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

                // Get LoggingPanel (DataGridView is now in LoggingPanel on Logging tab)
                var loggingPanel = FindLoggingPanel(form);
                Assert.IsNotNull(loggingPanel, "LoggingPanel should exist");

                // Get DataGridView from LoggingPanel
                var dataGridView = FindDataGridView(loggingPanel);
                Assert.IsNotNull(dataGridView, "DataGridView should exist in LoggingPanel");

                // Add some initial rows to verify clearing
                loggingPanel.AddDataRow(1, 1000000, 0.5, -10.5, "OK");
                loggingPanel.AddDataRow(2, 1000000, 0.6, -9.8, "OK");
                Assert.AreEqual(2, dataGridView.Rows.Count, "Should have 2 initial rows");

                // Act: Clear and add setting row (simulating Start Tuning)
                loggingPanel.ClearDataGrid();
                loggingPanel.AddSettingRow("Start Tuning", DateTime.Now);

                // Assert: DataGridView should be cleared and have 1 setting row
                Assert.AreEqual(1, dataGridView.Rows.Count, 
                    "DataGridView should have exactly 1 row after clearing and adding Start Tuning setting row");

                var row = dataGridView.Rows[0];
                Assert.AreEqual("setting", row.Cells["Type"].Value, 
                    "First row should have Type='setting'");
                Assert.AreEqual("Start Tuning", row.Cells["Status"].Value, 
                    "First row should have Status='Start Tuning'");
            }
            finally
            {
                form?.Close();
                form?.Dispose();
            }
        }

        /// <summary>
        /// Preservation Test: Tuning iterations add data rows with all columns populated.
        /// 
        /// EXPECTED OUTCOME ON UNFIXED CODE: Test PASSES (confirms baseline behavior)
        /// 
        /// **Validates: Requirements 3.1, 3.3**
        /// </summary>
        [Test]
        public void Preservation_TuningIterations_AddDataRowsWithAllColumns()
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

                // Get LoggingPanel (DataGridView is now in LoggingPanel on Logging tab)
                var loggingPanel = FindLoggingPanel(form);
                Assert.IsNotNull(loggingPanel, "LoggingPanel should exist");

                // Get DataGridView from LoggingPanel
                var dataGridView = FindDataGridView(loggingPanel);
                Assert.IsNotNull(dataGridView, "DataGridView should exist in LoggingPanel");

                // Act: Add data rows (simulating tuning iterations)
                loggingPanel.AddDataRow(1, 1000000, 0.5, -10.5, "OK");
                loggingPanel.AddDataRow(2, 1500000, 0.6, -9.8, "OK");
                loggingPanel.AddDataRow(3, 2000000, 0.7, -9.2, "OK");

                // Assert: DataGridView should have 3 data rows with all columns populated
                Assert.AreEqual(3, dataGridView.Rows.Count, 
                    "DataGridView should have exactly 3 rows after adding 3 data rows");

                // Verify first data row
                var row1 = dataGridView.Rows[0];
                Assert.AreEqual("data", row1.Cells["Type"].Value, 
                    "Row 1 should have Type='data'");
                Assert.IsNotNull(row1.Cells["Timestamp"].Value, 
                    "Row 1 should have Timestamp populated");
                Assert.AreEqual("1", row1.Cells["Iteration"].Value, 
                    "Row 1 should have Iteration='1'");
                Assert.AreEqual("1000000", row1.Cells["Frequency"].Value, 
                    "Row 1 should have Frequency='1000000'");
                Assert.AreEqual("0.5000", row1.Cells["Voltage"].Value, 
                    "Row 1 should have Voltage='0.5000'");
                Assert.AreEqual("-10.500", row1.Cells["Power_dBm"].Value, 
                    "Row 1 should have Power_dBm='-10.500'");
                Assert.AreEqual("OK", row1.Cells["Status"].Value, 
                    "Row 1 should have Status='OK'");

                // Verify second data row
                var row2 = dataGridView.Rows[1];
                Assert.AreEqual("data", row2.Cells["Type"].Value, 
                    "Row 2 should have Type='data'");
                Assert.AreEqual("2", row2.Cells["Iteration"].Value, 
                    "Row 2 should have Iteration='2'");
                Assert.AreEqual("1500000", row2.Cells["Frequency"].Value, 
                    "Row 2 should have Frequency='1500000'");

                // Verify third data row
                var row3 = dataGridView.Rows[2];
                Assert.AreEqual("data", row3.Cells["Type"].Value, 
                    "Row 3 should have Type='data'");
                Assert.AreEqual("3", row3.Cells["Iteration"].Value, 
                    "Row 3 should have Iteration='3'");
                Assert.AreEqual("2000000", row3.Cells["Frequency"].Value, 
                    "Row 3 should have Frequency='2000000'");
            }
            finally
            {
                form?.Close();
                form?.Dispose();
            }
        }

        /// <summary>
        /// Preservation Test: Stop Tuning adds "Stop Tuning" setting row.
        /// 
        /// EXPECTED OUTCOME ON UNFIXED CODE: Test PASSES (confirms baseline behavior)
        /// 
        /// **Validates: Requirements 3.2**
        /// </summary>
        [Test]
        public void Preservation_StopTuning_AddsSettingRow()
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

                // Get LoggingPanel (DataGridView is now in LoggingPanel on Logging tab)
                var loggingPanel = FindLoggingPanel(form);
                Assert.IsNotNull(loggingPanel, "LoggingPanel should exist");

                // Get DataGridView from LoggingPanel
                var dataGridView = FindDataGridView(loggingPanel);
                Assert.IsNotNull(dataGridView, "DataGridView should exist in LoggingPanel");

                // Add some data rows first
                loggingPanel.AddDataRow(1, 1000000, 0.5, -10.5, "OK");
                loggingPanel.AddDataRow(2, 1500000, 0.6, -9.8, "OK");

                // Act: Add Stop Tuning setting row
                loggingPanel.AddSettingRow("Stop Tuning", DateTime.Now);

                // Assert: DataGridView should have 3 rows (2 data + 1 setting)
                Assert.AreEqual(3, dataGridView.Rows.Count, 
                    "DataGridView should have exactly 3 rows (2 data + 1 setting)");

                var lastRow = dataGridView.Rows[2];
                Assert.AreEqual("setting", lastRow.Cells["Type"].Value, 
                    "Last row should have Type='setting'");
                Assert.AreEqual("Stop Tuning", lastRow.Cells["Status"].Value, 
                    "Last row should have Status='Stop Tuning'");
            }
            finally
            {
                form?.Close();
                form?.Dispose();
            }
        }

        /// <summary>
        /// Preservation Test: DataGridView auto-scrolls to latest entry.
        /// 
        /// EXPECTED OUTCOME ON UNFIXED CODE: Test PASSES (confirms baseline behavior)
        /// 
        /// **Validates: Requirements 3.4**
        /// </summary>
        [Test]
        public void Preservation_DataGridView_AutoScrollsToLatestEntry()
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

                // Get LoggingPanel (DataGridView is now in LoggingPanel on Logging tab)
                var loggingPanel = FindLoggingPanel(form);
                Assert.IsNotNull(loggingPanel, "LoggingPanel should exist");

                // Get DataGridView from LoggingPanel
                var dataGridView = FindDataGridView(loggingPanel);
                Assert.IsNotNull(dataGridView, "DataGridView should exist in LoggingPanel");

                // Act: Add multiple rows to trigger auto-scroll
                for (int i = 1; i <= 10; i++)
                {
                    loggingPanel.AddDataRow(i, 1000000 + (i * 100000), 0.5 + (i * 0.01), -10.5 + (i * 0.1), "OK");
                }

                // Assert: DataGridView should auto-scroll to show the last row
                // FirstDisplayedScrollingRowIndex should be set to the last row index
                int lastRowIndex = dataGridView.Rows.Count - 1;
                Assert.AreEqual(lastRowIndex, dataGridView.FirstDisplayedScrollingRowIndex, 
                    "DataGridView should auto-scroll to the latest entry (last row)");
            }
            finally
            {
                form?.Close();
                form?.Dispose();
            }
        }

        /// <summary>
        /// Preservation Test: StatusPanel displays tuning status and measurements.
        /// 
        /// EXPECTED OUTCOME ON UNFIXED CODE: Test PASSES (confirms baseline behavior)
        /// 
        /// **Validates: Requirements 3.6**
        /// </summary>
        [Test]
        public void Preservation_StatusPanel_DisplaysTuningStatusAndMeasurements()
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

                // Get StatusPanel
                var statusPanel = FindStatusPanel(form);
                Assert.IsNotNull(statusPanel, "StatusPanel should exist on Tuning tab");

                // Assert: StatusPanel should contain status and measurement labels
                var statusGroup = FindControlByName<GroupBox>(statusPanel, "_statusGroup");
                Assert.IsNotNull(statusGroup, "StatusPanel should contain status group");
                Assert.AreEqual("Tuning Status", statusGroup.Text, 
                    "Status group should have text 'Tuning Status'");

                var measurementsGroup = FindControlByName<GroupBox>(statusPanel, "_measurementsGroup");
                Assert.IsNotNull(measurementsGroup, "StatusPanel should contain measurements group");
                Assert.AreEqual("Current Measurements", measurementsGroup.Text, 
                    "Measurements group should have text 'Current Measurements'");

                // Verify measurement labels exist
                var iterationLabel = FindControlByText<Label>(measurementsGroup, "Iteration:");
                Assert.IsNotNull(iterationLabel, "Measurements group should contain Iteration label");

                var measuredPowerLabel = FindControlByText<Label>(measurementsGroup, "Measured Power:");
                Assert.IsNotNull(measuredPowerLabel, "Measurements group should contain Measured Power label");

                var currentVoltageLabel = FindControlByText<Label>(measurementsGroup, "Current Voltage:");
                Assert.IsNotNull(currentVoltageLabel, "Measurements group should contain Current Voltage label");

                var powerErrorLabel = FindControlByText<Label>(measurementsGroup, "Power Error:");
                Assert.IsNotNull(powerErrorLabel, "Measurements group should contain Power Error label");
            }
            finally
            {
                form?.Close();
                form?.Dispose();
            }
        }

        /// <summary>
        /// Preservation Test: Connection tab functions correctly.
        /// 
        /// EXPECTED OUTCOME ON UNFIXED CODE: Test PASSES (confirms baseline behavior)
        /// 
        /// **Validates: Requirements 3.7**
        /// </summary>
        [Test]
        public void Preservation_ConnectionTab_FunctionsCorrectly()
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

                // Get the tab control
                var tabControl = GetTabControl(form);
                Assert.IsNotNull(tabControl, "TabControl should exist");

                // Assert: Connection tab should exist
                var connectionTab = tabControl.TabPages.Cast<TabPage>()
                    .FirstOrDefault(tp => tp.Text == "Connection");
                Assert.IsNotNull(connectionTab, "Connection tab should exist");

                // Verify ConnectionPanel exists on Connection tab
                var connectionPanel = FindControlRecursive<ConnectionPanel>(connectionTab);
                Assert.IsNotNull(connectionPanel, 
                    "ConnectionPanel should exist on Connection tab");
            }
            finally
            {
                form?.Close();
                form?.Dispose();
            }
        }

        /// <summary>
        /// Preservation Test: Chart tab functions correctly.
        /// 
        /// EXPECTED OUTCOME ON UNFIXED CODE: Test PASSES (confirms baseline behavior)
        /// 
        /// **Validates: Requirements 3.8**
        /// </summary>
        [Test]
        public void Preservation_ChartTab_FunctionsCorrectly()
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

                // Get the tab control
                var tabControl = GetTabControl(form);
                Assert.IsNotNull(tabControl, "TabControl should exist");

                // Assert: Chart tab should exist
                var chartTab = tabControl.TabPages.Cast<TabPage>()
                    .FirstOrDefault(tp => tp.Text == "Chart");
                Assert.IsNotNull(chartTab, "Chart tab should exist");

                // Verify ChartPanel exists on Chart tab
                var chartPanel = FindControlRecursive<ChartPanel>(chartTab);
                Assert.IsNotNull(chartPanel, 
                    "ChartPanel should exist on Chart tab");
            }
            finally
            {
                form?.Close();
                form?.Dispose();
            }
        }

        #region Helper Methods

        /// <summary>
        /// Helper method to get the TabControl from MainForm.
        /// </summary>
        private TabControl GetTabControl(MainForm form)
        {
            var tabControlField = typeof(MainForm).GetField("_tabControl",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return tabControlField?.GetValue(form) as TabControl;
        }

        /// <summary>
        /// Helper method to find StatusPanel on the Tuning tab.
        /// </summary>
        private StatusPanel FindStatusPanel(MainForm form)
        {
            var tabControl = GetTabControl(form);
            if (tabControl == null)
                return null;

            var tuningTab = tabControl.TabPages.Cast<TabPage>()
                .FirstOrDefault(tp => tp.Text == "Tuning");
            if (tuningTab == null)
                return null;

            return FindControlRecursive<StatusPanel>(tuningTab);
        }

        /// <summary>
        /// Helper method to find LoggingPanel on the Logging tab.
        /// </summary>
        private LoggingPanel FindLoggingPanel(MainForm form)
        {
            var tabControl = GetTabControl(form);
            if (tabControl == null)
                return null;

            var loggingTab = tabControl.TabPages.Cast<TabPage>()
                .FirstOrDefault(tp => tp.Text == "Logging");
            if (loggingTab == null)
                return null;

            return FindControlRecursive<LoggingPanel>(loggingTab);
        }

        /// <summary>
        /// Helper method to find DataGridView within a control.
        /// </summary>
        private DataGridView FindDataGridView(Control parent)
        {
            return FindControlRecursive<DataGridView>(parent);
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
        /// Helper method to find a control by name using reflection.
        /// </summary>
        private T FindControlByName<T>(Control parent, string fieldName) where T : Control
        {
            if (parent == null)
                return null;

            var field = parent.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field?.GetValue(parent) as T;
        }

        /// <summary>
        /// Helper method to find a control by its Text property.
        /// </summary>
        private T FindControlByText<T>(Control parent, string text) where T : Control
        {
            if (parent == null)
                return null;

            foreach (Control control in parent.Controls)
            {
                if (control is T typedControl && control.Text == text)
                    return typedControl;

                var found = FindControlByText<T>(control, text);
                if (found != null)
                    return found;
            }

            return null;
        }

        #endregion
    }
}
