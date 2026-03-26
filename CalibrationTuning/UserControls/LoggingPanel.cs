using System;
using System.Drawing;
using System.Windows.Forms;
using CalibrationTuning.Controllers;
using CalibrationTuning.Events;
using CalibrationTuning.Models;

namespace CalibrationTuning.UserControls
{
    /// <summary>
    /// User control for displaying measurement history logging data.
    /// Extracted from StatusPanel to provide dedicated logging view on Logging tab.
    /// </summary>
    public partial class LoggingPanel : UserControl
    {
        private readonly ITuningController _tuningController;

        // UI Controls - Data Grid
        private GroupBox _dataGridGroup;
        private DataGridView _dataGridView;

        // Track pending scroll position for when control isn't visible
        private int _pendingScrollRowIndex = -1;

        public LoggingPanel(ITuningController tuningController)
        {
            _tuningController = tuningController ?? throw new ArgumentNullException(nameof(tuningController));

            InitializeComponent();
            InitializeControls();
            SubscribeToEvents();
        }

        private void InitializeControls()
        {
            this.SuspendLayout();

            // Panel properties
            this.AutoScroll = false;
            this.Padding = new Padding(10);

            // Data Grid Group
            _dataGridGroup = new GroupBox
            {
                Text = "Measurement History",
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 25, 10, 10)
            };

            _dataGridView = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(520, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ScrollBars = ScrollBars.Both
            };

            // Configure columns
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Type",
                HeaderText = "Type",
                Width = 80,
                ReadOnly = true
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Timestamp",
                HeaderText = "Timestamp",
                Width = 180,
                ReadOnly = true
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Iteration",
                HeaderText = "Iteration",
                Width = 80,
                ReadOnly = true
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Frequency",
                HeaderText = "Frequency (Hz)",
                Width = 120,
                ReadOnly = true
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Voltage",
                HeaderText = "Voltage (V)",
                Width = 100,
                ReadOnly = true
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Power_dBm",
                HeaderText = "Power (dBm)",
                Width = 100,
                ReadOnly = true
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 120,
                ReadOnly = true
            });

            _dataGridGroup.Controls.Add(_dataGridView);
            this.Controls.Add(_dataGridGroup);

            this.ResumeLayout(false);
        }

        private void SubscribeToEvents()
        {
            // Subscribe to TuningController events
            _tuningController.UserActionOccurred += TuningController_UserActionOccurred;
            _tuningController.ProgressUpdated += TuningController_ProgressUpdated;

            // Subscribe to VisibleChanged to apply pending scroll when control becomes visible
            _dataGridView.VisibleChanged += DataGridView_VisibleChanged;
        }

        private void DataGridView_VisibleChanged(object sender, EventArgs e)
        {
            if (_dataGridView.Visible)
            {
                // Resize DataGridView to fill the GroupBox when it becomes visible
                if (_dataGridGroup != null)
                {
                    var padding = _dataGridGroup.Padding;
                    _dataGridView.Size = new Size(
                        _dataGridGroup.ClientSize.Width - padding.Left - padding.Right,
                        _dataGridGroup.ClientSize.Height - _dataGridView.Location.Y - padding.Bottom);
                    _dataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                }

                // Apply pending scroll position
                if (_pendingScrollRowIndex >= 0 && _dataGridView.Rows.Count > 0)
                {
                    try
                    {
                        _dataGridView.FirstDisplayedScrollingRowIndex = _pendingScrollRowIndex;
                    }
                    catch { }
                    _pendingScrollRowIndex = -1;
                }
            }
        }

        /// <summary>
        /// Auto-scrolls the DataGridView to the last row, handling the case where the control isn't visible.
        /// </summary>
        private void AutoScrollToLastRow()
        {
            if (_dataGridView.Rows.Count > 0)
            {
                int targetIndex = _dataGridView.Rows.Count - 1;
                _pendingScrollRowIndex = targetIndex;
                try
                {
                    _dataGridView.FirstDisplayedScrollingRowIndex = targetIndex;
                }
                catch
                {
                    // Scroll will be applied when control becomes visible
                }
            }
        }

        private void TuningController_UserActionOccurred(object sender, Events.UserActionEventArgs e)
        {
            AddSettingRow(e.ActionName, e.Timestamp);
        }

        private void TuningController_ProgressUpdated(object sender, TuningProgressEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[LoggingPanel] ProgressUpdated received. Stats null? {e.Statistics == null}, Params null? {_tuningController.Parameters == null}");
            // Add data row to grid
            if (e.Statistics != null && _tuningController.Parameters != null)
            {
                System.Diagnostics.Debug.WriteLine($"[LoggingPanel] Adding data row: Iter={e.Statistics.CurrentIteration}, Power={e.Statistics.CurrentPowerDbm}");
                AddDataRow(
                    e.Statistics.CurrentIteration,
                    _tuningController.Parameters.FrequencyHz,
                    e.Statistics.CurrentVoltage,
                    e.Statistics.CurrentPowerDbm,
                    "Tuning"
                );
            }
        }

        /// <summary>
        /// Adds a setting row to the data grid view.
        /// </summary>
        /// <param name="actionName">Name of the user action.</param>
        /// <param name="timestamp">Timestamp of the action.</param>
        public void AddSettingRow(string actionName, DateTime timestamp)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddSettingRow(actionName, timestamp)));
                return;
            }

            try
            {
                var row = new DataGridRow
                {
                    Type = "setting",
                    Timestamp = timestamp,
                    Iteration = null,
                    Frequency = null,
                    Voltage = null,
                    PowerDbm = null,
                    Status = actionName
                };

                _dataGridView.Rows.Add(
                    row.Type,
                    row.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    "",
                    "",
                    "",
                    "",
                    row.Status
                );

                // Auto-scroll to latest entry
                AutoScrollToLastRow();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding setting row: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a data row to the data grid view.
        /// </summary>
        /// <param name="iteration">Iteration number.</param>
        /// <param name="frequency">Signal frequency in Hz.</param>
        /// <param name="voltage">Signal voltage.</param>
        /// <param name="powerDbm">Measured power in dBm.</param>
        /// <param name="status">Status indicator.</param>
        public void AddDataRow(int iteration, double frequency, double voltage, double powerDbm, string status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddDataRow(iteration, frequency, voltage, powerDbm, status)));
                return;
            }

            try
            {
                var row = new DataGridRow
                {
                    Type = "data",
                    Timestamp = DateTime.Now,
                    Iteration = iteration,
                    Frequency = frequency,
                    Voltage = voltage,
                    PowerDbm = powerDbm,
                    Status = status
                };

                _dataGridView.Rows.Add(
                    row.Type,
                    row.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    row.Iteration.HasValue ? row.Iteration.Value.ToString() : "",
                    row.Frequency.HasValue ? row.Frequency.Value.ToString("F0") : "",
                    row.Voltage.HasValue ? row.Voltage.Value.ToString("F4") : "",
                    row.PowerDbm.HasValue ? row.PowerDbm.Value.ToString("F3") : "",
                    row.Status
                );

                // Auto-scroll to latest entry
                AutoScrollToLastRow();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding data row: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all rows from the data grid view.
        /// </summary>
        public void ClearDataGrid()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ClearDataGrid));
                return;
            }

            try
            {
                _dataGridView.Rows.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing data grid: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe from events
                _tuningController.UserActionOccurred -= TuningController_UserActionOccurred;
                _tuningController.ProgressUpdated -= TuningController_ProgressUpdated;
                if (_dataGridView != null)
                {
                    _dataGridView.VisibleChanged -= DataGridView_VisibleChanged;
                }
            }
            base.Dispose(disposing);
        }
    }
}
