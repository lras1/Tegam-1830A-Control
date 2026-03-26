using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalibrationTuning.Controllers;
using CalibrationTuning.Models;

namespace CalibrationTuning.UserControls
{
    /// <summary>
    /// User control for configuring tuning parameters and controlling the tuning process.
    /// </summary>
    public partial class TuningPanel : UserControl
    {
        private readonly ITuningController _tuningController;
        private readonly MainForm _mainForm;

        // UI Controls - Status Panel
        private StatusPanel _statusPanel;

        // UI Controls - Signal Configuration
        private GroupBox _signalConfigGroup;
        private Label _frequencyLabel;
        private NumericUpDown _frequencyNumeric;
        private Label _frequencyUnitLabel;
        private Label _initialVoltageLabel;
        private NumericUpDown _initialVoltageNumeric;
        private Label _initialVoltageUnitLabel;

        // UI Controls - Tuning Parameters
        private GroupBox _tuningParamsGroup;
        private Label _setpointLabel;
        private NumericUpDown _setpointNumeric;
        private Label _setpointUnitLabel;
        private Label _toleranceLabel;
        private NumericUpDown _toleranceNumeric;
        private Label _toleranceUnitLabel;
        private Label _voltageStepLabel;
        private NumericUpDown _voltageStepNumeric;
        private Label _voltageStepUnitLabel;

        // UI Controls - Safety Limits
        private GroupBox _safetyLimitsGroup;
        private Label _minVoltageLabel;
        private NumericUpDown _minVoltageNumeric;
        private Label _minVoltageUnitLabel;
        private Label _maxVoltageLabel;
        private NumericUpDown _maxVoltageNumeric;
        private Label _maxVoltageUnitLabel;
        private Label _maxIterationsLabel;
        private NumericUpDown _maxIterationsNumeric;
        private Label _sampleDelayLabel;
        private NumericUpDown _sampleDelayNumeric;
        private Label _sampleDelayUnitLabel;

        // UI Controls - Sensor Selection
        private GroupBox _sensorGroup;
        private Label _sensorLabel;
        private ComboBox _sensorComboBox;

        // UI Controls - Actions
        private Button _startTuningButton;
        private Button _stopTuningButton;
        private Button _manualMeasureButton;
        private Button _resetIterButton;

        private bool _devicesConnected = false;

        public TuningPanel(ITuningController tuningController, MainForm mainForm)
        {
            _tuningController = tuningController ?? throw new ArgumentNullException(nameof(tuningController));
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));

            InitializeComponent();
            InitializeControls();
            SubscribeToEvents();
            UpdateControlStates();
        }

        private void InitializeControls()
        {
            this.SuspendLayout();

            // Panel properties
            this.AutoScroll = true;
            this.Padding = new Padding(10);

            int yPosition = 10;

            // Signal Configuration Group
            _signalConfigGroup = new GroupBox
            {
                Text = "Signal Configuration",
                Location = new Point(10, yPosition),
                Size = new Size(560, 90),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _frequencyLabel = new Label
            {
                Text = "Frequency:",
                Location = new Point(15, 30),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _frequencyNumeric = new NumericUpDown
            {
                Location = new Point(120, 28),
                Size = new Size(150, 20),
                Minimum = 1,
                Maximum = 500000000,  // 500 MHz (Siglent SDG6052X max for Sine)
                DecimalPlaces = 0,
                Value = 100000000,    // 100 MHz
                ThousandsSeparator = true
            };

            _frequencyUnitLabel = new Label
            {
                Text = "Hz",
                Location = new Point(275, 30),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _initialVoltageLabel = new Label
            {
                Text = "Initial Voltage:",
                Location = new Point(15, 60),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _initialVoltageNumeric = new NumericUpDown
            {
                Location = new Point(120, 58),
                Size = new Size(150, 20),
                Minimum = 0.001M,
                Maximum = 10.0M,
                DecimalPlaces = 3,
                Value = 0.5M,
                Increment = 0.1M
            };

            _initialVoltageUnitLabel = new Label
            {
                Text = "V",
                Location = new Point(275, 60),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _signalConfigGroup.Controls.Add(_frequencyLabel);
            _signalConfigGroup.Controls.Add(_frequencyNumeric);
            _signalConfigGroup.Controls.Add(_frequencyUnitLabel);
            _signalConfigGroup.Controls.Add(_initialVoltageLabel);
            _signalConfigGroup.Controls.Add(_initialVoltageNumeric);
            _signalConfigGroup.Controls.Add(_initialVoltageUnitLabel);

            this.Controls.Add(_signalConfigGroup);
            yPosition += 100;

            // Tuning Parameters Group
            _tuningParamsGroup = new GroupBox
            {
                Text = "Tuning Parameters",
                Location = new Point(10, yPosition),
                Size = new Size(560, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _setpointLabel = new Label
            {
                Text = "Setpoint:",
                Location = new Point(15, 30),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _setpointNumeric = new NumericUpDown
            {
                Location = new Point(120, 28),
                Size = new Size(150, 20),
                Minimum = -100M,
                Maximum = 100M,
                DecimalPlaces = 2,
                Value = -10.0M,
                Increment = 0.5M
            };

            _setpointUnitLabel = new Label
            {
                Text = "dBm",
                Location = new Point(275, 30),
                Size = new Size(40, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _toleranceLabel = new Label
            {
                Text = "Tolerance:",
                Location = new Point(15, 60),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _toleranceNumeric = new NumericUpDown
            {
                Location = new Point(120, 58),
                Size = new Size(150, 20),
                Minimum = 0.01M,
                Maximum = 10M,
                DecimalPlaces = 2,
                Value = 0.5M,
                Increment = 0.1M
            };

            _toleranceUnitLabel = new Label
            {
                Text = "dB",
                Location = new Point(275, 60),
                Size = new Size(40, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _voltageStepLabel = new Label
            {
                Text = "Voltage Step:",
                Location = new Point(15, 90),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _voltageStepNumeric = new NumericUpDown
            {
                Location = new Point(120, 88),
                Size = new Size(150, 20),
                Minimum = 0.001M,
                Maximum = 1M,
                DecimalPlaces = 3,
                Value = 0.05M,
                Increment = 0.01M
            };

            _voltageStepUnitLabel = new Label
            {
                Text = "V",
                Location = new Point(275, 90),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _tuningParamsGroup.Controls.Add(_setpointLabel);
            _tuningParamsGroup.Controls.Add(_setpointNumeric);
            _tuningParamsGroup.Controls.Add(_setpointUnitLabel);
            _tuningParamsGroup.Controls.Add(_toleranceLabel);
            _tuningParamsGroup.Controls.Add(_toleranceNumeric);
            _tuningParamsGroup.Controls.Add(_toleranceUnitLabel);
            _tuningParamsGroup.Controls.Add(_voltageStepLabel);
            _tuningParamsGroup.Controls.Add(_voltageStepNumeric);
            _tuningParamsGroup.Controls.Add(_voltageStepUnitLabel);

            this.Controls.Add(_tuningParamsGroup);
            yPosition += 130;

            // Safety Limits Group
            _safetyLimitsGroup = new GroupBox
            {
                Text = "Safety Limits",
                Location = new Point(10, yPosition),
                Size = new Size(560, 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _minVoltageLabel = new Label
            {
                Text = "Min Voltage:",
                Location = new Point(15, 30),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _minVoltageNumeric = new NumericUpDown
            {
                Location = new Point(120, 28),
                Size = new Size(150, 20),
                Minimum = 0M,
                Maximum = 10M,
                DecimalPlaces = 3,
                Value = 0.1M,
                Increment = 0.1M
            };

            _minVoltageUnitLabel = new Label
            {
                Text = "V",
                Location = new Point(275, 30),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _maxVoltageLabel = new Label
            {
                Text = "Max Voltage:",
                Location = new Point(15, 60),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _maxVoltageNumeric = new NumericUpDown
            {
                Location = new Point(120, 58),
                Size = new Size(150, 20),
                Minimum = 0M,
                Maximum = 10M,
                DecimalPlaces = 3,
                Value = 5.0M,
                Increment = 0.1M
            };

            _maxVoltageUnitLabel = new Label
            {
                Text = "V",
                Location = new Point(275, 60),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _maxIterationsLabel = new Label
            {
                Text = "Max Iterations:",
                Location = new Point(15, 90),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _maxIterationsNumeric = new NumericUpDown
            {
                Location = new Point(120, 88),
                Size = new Size(150, 20),
                Minimum = 0,
                Maximum = 1000000,
                DecimalPlaces = 0,
                Value = 0,
                Increment = 100
            };

            _safetyLimitsGroup.Controls.Add(_minVoltageLabel);
            _safetyLimitsGroup.Controls.Add(_minVoltageNumeric);
            _safetyLimitsGroup.Controls.Add(_minVoltageUnitLabel);
            _safetyLimitsGroup.Controls.Add(_maxVoltageLabel);
            _safetyLimitsGroup.Controls.Add(_maxVoltageNumeric);
            _safetyLimitsGroup.Controls.Add(_maxVoltageUnitLabel);
            _safetyLimitsGroup.Controls.Add(_maxIterationsLabel);
            _safetyLimitsGroup.Controls.Add(_maxIterationsNumeric);

            var maxIterHint = new Label
            {
                Text = "(0 = continuous)",
                Location = new Point(275, 90),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gray,
                Font = new Font(this.Font.FontFamily, 7.5f)
            };
            _safetyLimitsGroup.Controls.Add(maxIterHint);

            _sampleDelayLabel = new Label
            {
                Text = "Sample Delay:",
                Location = new Point(15, 120),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _sampleDelayNumeric = new NumericUpDown
            {
                Location = new Point(120, 118),
                Size = new Size(150, 20),
                Minimum = 100,
                Maximum = 10000,
                DecimalPlaces = 0,
                Value = 500,
                Increment = 100
            };

            _sampleDelayUnitLabel = new Label
            {
                Text = "ms",
                Location = new Point(275, 120),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _safetyLimitsGroup.Controls.Add(_sampleDelayLabel);
            _safetyLimitsGroup.Controls.Add(_sampleDelayNumeric);
            _safetyLimitsGroup.Controls.Add(_sampleDelayUnitLabel);

            this.Controls.Add(_safetyLimitsGroup);
            yPosition += 160;

            // Sensor Selection Group
            _sensorGroup = new GroupBox
            {
                Text = "Sensor Selection",
                Location = new Point(10, yPosition),
                Size = new Size(560, 70),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _sensorLabel = new Label
            {
                Text = "Sensor:",
                Location = new Point(15, 30),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _sensorComboBox = new ComboBox
            {
                Location = new Point(120, 28),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _sensorComboBox.Items.AddRange(new object[] { "Sensor 1", "Sensor 2" });
            _sensorComboBox.SelectedIndex = 0;

            _sensorGroup.Controls.Add(_sensorLabel);
            _sensorGroup.Controls.Add(_sensorComboBox);

            this.Controls.Add(_sensorGroup);
            yPosition += 80;

            // Action Buttons
            _startTuningButton = new Button
            {
                Text = "Start Tuning",
                Location = new Point(10, yPosition),
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            _startTuningButton.Click += StartTuningButton_Click;

            _stopTuningButton = new Button
            {
                Text = "Stop Tuning",
                Location = new Point(140, yPosition),
                Size = new Size(120, 35),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            _stopTuningButton.Click += StopTuningButton_Click;

            _manualMeasureButton = new Button
            {
                Text = "Manual Measure",
                Location = new Point(270, yPosition),
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            _manualMeasureButton.Click += ManualMeasureButton_Click;

            _resetIterButton = new Button
            {
                Text = "Reset Counter",
                Location = new Point(400, yPosition),
                Size = new Size(120, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            _resetIterButton.Click += (s, ev) => _tuningController.ResetIterationCounter();

            this.Controls.Add(_startTuningButton);
            this.Controls.Add(_stopTuningButton);
            this.Controls.Add(_manualMeasureButton);
            this.Controls.Add(_resetIterButton);

            yPosition += 45;

            // Status Panel
            _statusPanel = new StatusPanel(_tuningController)
            {
                Location = new Point(10, yPosition),
                Size = new Size(560, 250),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(_statusPanel);

            this.ResumeLayout(false);
        }

        private void SubscribeToEvents()
        {
            // Subscribe to tuning controller events
            _tuningController.StateChanged += TuningController_StateChanged;
            _tuningController.ErrorOccurred += TuningController_ErrorOccurred;
        }

        private async void StartTuningButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Build tuning parameters from UI inputs
                TuningParameters parameters = new TuningParameters
                {
                    FrequencyHz = (double)_frequencyNumeric.Value,
                    InitialVoltage = (double)_initialVoltageNumeric.Value,
                    TargetPowerDbm = (double)_setpointNumeric.Value,
                    ToleranceDb = (double)_toleranceNumeric.Value,
                    VoltageStepSize = (double)_voltageStepNumeric.Value,
                    MinVoltage = (double)_minVoltageNumeric.Value,
                    MaxVoltage = (double)_maxVoltageNumeric.Value,
                    MaxIterations = (int)_maxIterationsNumeric.Value,
                    SensorId = _sensorComboBox.SelectedIndex + 1,
                    SampleDelayMs = (int)_sampleDelayNumeric.Value
                };

                // Validate ranges
                if (parameters.MinVoltage >= parameters.MaxVoltage)
                {
                    MessageBox.Show("Minimum voltage must be less than maximum voltage.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (parameters.InitialVoltage < parameters.MinVoltage || parameters.InitialVoltage > parameters.MaxVoltage)
                {
                    MessageBox.Show("Initial voltage must be between minimum and maximum voltage.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Disable controls during tuning
                UpdateControlStates(isTuning: true);

                // Start tuning asynchronously on background thread
                await Task.Run(async () => await _tuningController.StartTuningAsync(parameters));
                
                // Re-enable controls after tuning completes
                UpdateControlStates(isTuning: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting tuning: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateControlStates(isTuning: false);
            }
        }

        private void StopTuningButton_Click(object sender, EventArgs e)
        {
            try
            {
                _tuningController.StopTuning();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping tuning: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ManualMeasureButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable button during measurement
                _manualMeasureButton.Enabled = false;

                // Perform manual measurement with full details
                var measurement = await _tuningController.MeasureManualWithDetailsAsync();

                // Display result
                if (measurement.IsValid)
                {
                    // Add data row to LoggingPanel
                    if (_mainForm.LoggingPanel != null)
                    {
                        _mainForm.LoggingPanel.AddDataRow(
                            0, // iteration 0 for manual measurement
                            measurement.FrequencyHz,
                            measurement.Voltage,
                            measurement.PowerDbm,
                            "Manual"
                        );
                    }

                    // Add data point to Chart
                    var chartPanel = _mainForm.ChartTab?.Controls.Count > 0 
                        ? _mainForm.ChartTab.Controls[0] as ChartPanel 
                        : null;
                    if (chartPanel != null)
                    {
                        chartPanel.AddDataPoint(0, measurement.PowerDbm);
                    }

                    MessageBox.Show(
                        $"Manual Measurement:\n\n" +
                        $"Frequency: {measurement.FrequencyHz:F0} Hz\n" +
                        $"Voltage: {measurement.Voltage:F4} V\n" +
                        $"Power: {measurement.PowerDbm:F3} dBm\n" +
                        $"Timestamp: {measurement.Timestamp:yyyy-MM-dd HH:mm:ss.fff}",
                        "Manual Measurement Result",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"Manual Measurement Failed:\n\n{measurement.ErrorMessage}",
                        "Manual Measurement Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during manual measurement: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable button
                _manualMeasureButton.Enabled = _devicesConnected;
            }
        }

        private void TuningController_StateChanged(object sender, Events.TuningStateChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleStateChange(e.NewState)));
            }
            else
            {
                HandleStateChange(e.NewState);
            }
        }

        private void HandleStateChange(TuningState newState)
        {
            // Update control states based on tuning state
            bool isTuning = newState == TuningState.Tuning || 
                           newState == TuningState.Measuring || 
                           newState == TuningState.Evaluating;

            bool isConnecting = newState == TuningState.Connecting;

            // Track connection state
            if (newState == TuningState.Idle && !isConnecting)
            {
                // Check if we just finished tuning or if devices are still connected
                // For now, assume devices remain connected after tuning
            }

            UpdateControlStates(isTuning);
        }

        private void TuningController_ErrorOccurred(object sender, System.IO.ErrorEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleError(e.GetException().Message)));
            }
            else
            {
                HandleError(e.GetException().Message);
            }
        }

        private void HandleError(string errorMessage)
        {
            System.Diagnostics.Debug.WriteLine($"[TuningPanel] Error: {errorMessage}");
            // Re-enable controls after error
            UpdateControlStates(isTuning: false);
        }

        private void UpdateControlStates(bool isTuning = false)
        {
            // Input controls should always be enabled when not tuning
            // Users need to configure parameters before connecting devices
            bool enableInputs = !isTuning;

            _frequencyNumeric.Enabled = enableInputs;
            _initialVoltageNumeric.Enabled = enableInputs;
            _setpointNumeric.Enabled = enableInputs;
            _toleranceNumeric.Enabled = enableInputs;
            _voltageStepNumeric.Enabled = enableInputs;
            _minVoltageNumeric.Enabled = enableInputs;
            _maxVoltageNumeric.Enabled = enableInputs;
            _maxIterationsNumeric.Enabled = enableInputs;
            _sampleDelayNumeric.Enabled = enableInputs;
            _sensorComboBox.Enabled = enableInputs;

            // Start button enabled only when devices connected and not tuning
            _startTuningButton.Enabled = _devicesConnected && !isTuning;

            // Stop button enabled only when tuning
            _stopTuningButton.Enabled = isTuning;

            // Manual measure button enabled only when devices connected and not tuning
            _manualMeasureButton.Enabled = _devicesConnected && !isTuning;
        }

        /// <summary>
        /// Updates the connection state and refreshes control states accordingly.
        /// </summary>
        public void SetDevicesConnected(bool connected)
        {
            _devicesConnected = connected;
            UpdateControlStates();
        }

        /// <summary>
        /// Gets the current tuning parameters from the UI.
        /// </summary>
        public TuningParameters GetTuningParameters()
        {
            return new TuningParameters
            {
                FrequencyHz = (double)_frequencyNumeric.Value,
                InitialVoltage = (double)_initialVoltageNumeric.Value,
                TargetPowerDbm = (double)_setpointNumeric.Value,
                ToleranceDb = (double)_toleranceNumeric.Value,
                VoltageStepSize = (double)_voltageStepNumeric.Value,
                MinVoltage = (double)_minVoltageNumeric.Value,
                MaxVoltage = (double)_maxVoltageNumeric.Value,
                MaxIterations = (int)_maxIterationsNumeric.Value,
                SensorId = _sensorComboBox.SelectedIndex + 1,
                SampleDelayMs = (int)_sampleDelayNumeric.Value
            };
        }

        /// <summary>
        /// Sets the tuning parameters in the UI.
        /// </summary>
        public void SetTuningParameters(TuningParameters parameters)
        {
            if (parameters == null) return;

            _frequencyNumeric.Value = (decimal)parameters.FrequencyHz;
            _initialVoltageNumeric.Value = (decimal)parameters.InitialVoltage;
            _setpointNumeric.Value = (decimal)parameters.TargetPowerDbm;
            _toleranceNumeric.Value = (decimal)parameters.ToleranceDb;
            _voltageStepNumeric.Value = (decimal)parameters.VoltageStepSize;
            _minVoltageNumeric.Value = (decimal)parameters.MinVoltage;
            _maxVoltageNumeric.Value = (decimal)parameters.MaxVoltage;
            _maxIterationsNumeric.Value = parameters.MaxIterations;
            _sampleDelayNumeric.Value = parameters.SampleDelayMs > 0 ? parameters.SampleDelayMs : 500;
            
            if (parameters.SensorId >= 1 && parameters.SensorId <= _sensorComboBox.Items.Count)
            {
                _sensorComboBox.SelectedIndex = parameters.SensorId - 1;
            }
        }
    }
}
