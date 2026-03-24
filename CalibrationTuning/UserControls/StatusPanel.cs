using System;
using System.Drawing;
using System.Windows.Forms;
using CalibrationTuning.Controllers;
using CalibrationTuning.Events;
using CalibrationTuning.Models;

namespace CalibrationTuning.UserControls
{
    /// <summary>
    /// User control for displaying real-time tuning status and measurements.
    /// Validates Requirements 8.1, 8.2, 8.3, 8.4, 8.5, 8.6
    /// </summary>
    public partial class StatusPanel : UserControl
    {
        private readonly ITuningController _tuningController;

        // UI Controls - Status Display
        private GroupBox _statusGroup;
        private Label _tuningStatusLabel;
        private Label _tuningStatusValue;

        // UI Controls - Measurements
        private GroupBox _measurementsGroup;
        private Label _iterationLabel;
        private Label _iterationValue;
        private Label _measuredPowerLabel;
        private Label _measuredPowerValue;
        private Label _currentVoltageLabel;
        private Label _currentVoltageValue;
        private Label _powerErrorLabel;
        private Label _powerErrorValue;

        public StatusPanel(ITuningController tuningController)
        {
            _tuningController = tuningController ?? throw new ArgumentNullException(nameof(tuningController));

            InitializeComponent();
            InitializeControls();
            SubscribeToEvents();
            UpdateDisplay();
        }

        private void InitializeControls()
        {
            this.SuspendLayout();

            // Panel properties
            this.AutoScroll = false;
            this.Padding = new Padding(10);

            int yPosition = 10;

            // Status Group
            _statusGroup = new GroupBox
            {
                Text = "Tuning Status",
                Location = new Point(10, yPosition),
                Size = new Size(540, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _tuningStatusLabel = new Label
            {
                Text = "Status:",
                Location = new Point(15, 30),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            _tuningStatusValue = new Label
            {
                Text = "Idle",
                Location = new Point(120, 30),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Regular),
                ForeColor = Color.DarkGray
            };

            _statusGroup.Controls.Add(_tuningStatusLabel);
            _statusGroup.Controls.Add(_tuningStatusValue);

            this.Controls.Add(_statusGroup);
            yPosition += 80;

            // Measurements Group
            _measurementsGroup = new GroupBox
            {
                Text = "Current Measurements",
                Location = new Point(10, yPosition),
                Size = new Size(540, 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Iteration
            _iterationLabel = new Label
            {
                Text = "Iteration:",
                Location = new Point(15, 30),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _iterationValue = new Label
            {
                Text = "0",
                Location = new Point(170, 30),
                Size = new Size(350, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular)
            };

            // Measured Power
            _measuredPowerLabel = new Label
            {
                Text = "Measured Power:",
                Location = new Point(15, 60),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _measuredPowerValue = new Label
            {
                Text = "-- dBm",
                Location = new Point(170, 60),
                Size = new Size(350, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular)
            };

            // Current Voltage
            _currentVoltageLabel = new Label
            {
                Text = "Current Voltage:",
                Location = new Point(15, 90),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _currentVoltageValue = new Label
            {
                Text = "-- V",
                Location = new Point(170, 90),
                Size = new Size(350, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular)
            };

            // Power Error
            _powerErrorLabel = new Label
            {
                Text = "Power Error:",
                Location = new Point(15, 120),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _powerErrorValue = new Label
            {
                Text = "-- dB",
                Location = new Point(170, 120),
                Size = new Size(350, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(this.Font.FontFamily, 9, FontStyle.Regular)
            };

            _measurementsGroup.Controls.Add(_iterationLabel);
            _measurementsGroup.Controls.Add(_iterationValue);
            _measurementsGroup.Controls.Add(_measuredPowerLabel);
            _measurementsGroup.Controls.Add(_measuredPowerValue);
            _measurementsGroup.Controls.Add(_currentVoltageLabel);
            _measurementsGroup.Controls.Add(_currentVoltageValue);
            _measurementsGroup.Controls.Add(_powerErrorLabel);
            _measurementsGroup.Controls.Add(_powerErrorValue);

            this.Controls.Add(_measurementsGroup);

            this.ResumeLayout(false);
        }

        private void SubscribeToEvents()
        {
            // Subscribe to TuningController events
            _tuningController.StateChanged += TuningController_StateChanged;
            _tuningController.ProgressUpdated += TuningController_ProgressUpdated;
        }

        private void TuningController_StateChanged(object sender, TuningStateChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateStatusDisplay(e.NewState)));
            }
            else
            {
                UpdateStatusDisplay(e.NewState);
            }
        }

        private void TuningController_ProgressUpdated(object sender, TuningProgressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateMeasurementDisplay(e.Statistics)));
            }
            else
            {
                UpdateMeasurementDisplay(e.Statistics);
            }
        }

        private void UpdateStatusDisplay(TuningState state)
        {
            // Update status text
            _tuningStatusValue.Text = state.ToString();

            // Update status color based on state
            switch (state)
            {
                case TuningState.Idle:
                    _tuningStatusValue.ForeColor = Color.DarkGray;
                    break;
                case TuningState.Connecting:
                    _tuningStatusValue.ForeColor = Color.Orange;
                    break;
                case TuningState.Tuning:
                case TuningState.Measuring:
                case TuningState.Evaluating:
                    _tuningStatusValue.ForeColor = Color.Blue;
                    break;
                case TuningState.Converged:
                    _tuningStatusValue.ForeColor = Color.Green;
                    break;
                case TuningState.Timeout:
                    _tuningStatusValue.ForeColor = Color.DarkOrange;
                    break;
                case TuningState.Error:
                    _tuningStatusValue.ForeColor = Color.Red;
                    break;
                case TuningState.Aborted:
                    _tuningStatusValue.ForeColor = Color.DarkRed;
                    break;
                default:
                    _tuningStatusValue.ForeColor = Color.Black;
                    break;
            }

            // Reset measurements if returning to Idle
            if (state == TuningState.Idle)
            {
                ResetMeasurementDisplay();
            }
        }

        private void UpdateMeasurementDisplay(TuningStatistics statistics)
        {
            if (statistics == null)
            {
                ResetMeasurementDisplay();
                return;
            }

            // Update iteration
            _iterationValue.Text = statistics.CurrentIteration.ToString();

            // Update measured power
            _measuredPowerValue.Text = $"{statistics.CurrentPowerDbm:F3} dBm";

            // Update current voltage
            _currentVoltageValue.Text = $"{statistics.CurrentVoltage:F4} V";

            // Update power error with sign
            string errorSign = statistics.PowerError >= 0 ? "+" : "";
            _powerErrorValue.Text = $"{errorSign}{statistics.PowerError:F3} dB";

            // Color code power error
            if (Math.Abs(statistics.PowerError) < 0.1)
            {
                _powerErrorValue.ForeColor = Color.Green;
            }
            else if (Math.Abs(statistics.PowerError) < 1.0)
            {
                _powerErrorValue.ForeColor = Color.Orange;
            }
            else
            {
                _powerErrorValue.ForeColor = Color.Red;
            }
        }

        private void ResetMeasurementDisplay()
        {
            _iterationValue.Text = "0";
            _measuredPowerValue.Text = "-- dBm";
            _currentVoltageValue.Text = "-- V";
            _powerErrorValue.Text = "-- dB";
            _powerErrorValue.ForeColor = Color.Black;
        }

        private void UpdateDisplay()
        {
            // Initialize display with current controller state
            UpdateStatusDisplay(_tuningController.CurrentState);
            
            if (_tuningController.Statistics != null)
            {
                UpdateMeasurementDisplay(_tuningController.Statistics);
            }
            else
            {
                ResetMeasurementDisplay();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe from events
                _tuningController.StateChanged -= TuningController_StateChanged;
                _tuningController.ProgressUpdated -= TuningController_ProgressUpdated;
            }
            base.Dispose(disposing);
        }
    }
}
