using System;
using System.Drawing;
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
        private TextBox _frequencyTextBox;
        private Label _frequencyUnitLabel;
        private Label _initialVoltageLabel;
        private TextBox _initialVoltageTextBox;
        private Label _initialVoltageUnitLabel;

        // UI Controls - Tuning Parameters
        private GroupBox _tuningParamsGroup;
        private Label _setpointLabel;
        private TextBox _setpointTextBox;
        private Label _setpointUnitLabel;
        private Label _toleranceLabel;
        private TextBox _toleranceTextBox;
        private Label _toleranceUnitLabel;
        private Label _voltageStepLabel;
        private TextBox _voltageStepTextBox;
        private Label _voltageStepUnitLabel;

        // UI Controls - Safety Limits
        private GroupBox _safetyLimitsGroup;
        private Label _minVoltageLabel;
        private TextBox _minVoltageTextBox;
        private Label _minVoltageUnitLabel;
        private Label _maxVoltageLabel;
        private TextBox _maxVoltageTextBox;
        private Label _maxVoltageUnitLabel;
        private Label _maxIterationsLabel;
        private TextBox _maxIterationsTextBox;

        // UI Controls - Sensor Selection
        private GroupBox _sensorGroup;
        private Label _sensorLabel;
        private ComboBox _sensorComboBox;

        // UI Controls - Actions
        private Button _startTuningButton;
        private Button _stopTuningButton;
        private Button _manualMeasureButton;

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

            _frequencyTextBox = new TextBox
            {
                Location = new Point(120, 28),
                Size = new Size(150, 20),
                Text = "2400000000"
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

            _initialVoltageTextBox = new TextBox
            {
                Location = new Point(120, 58),
                Size = new Size(150, 20),
                Text = "0.5"
            };

            _initialVoltageUnitLabel = new Label
            {
                Text = "V",
                Location = new Point(275, 60),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _signalConfigGroup.Controls.Add(_frequencyLabel);
            _signalConfigGroup.Controls.Add(_frequencyTextBox);
            _signalConfigGroup.Controls.Add(_frequencyUnitLabel);
            _signalConfigGroup.Controls.Add(_initialVoltageLabel);
            _signalConfigGroup.Controls.Add(_initialVoltageTextBox);
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

            _setpointTextBox = new TextBox
            {
                Location = new Point(120, 28),
                Size = new Size(150, 20),
                Text = "-10.0"
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

            _toleranceTextBox = new TextBox
            {
                Location = new Point(120, 58),
                Size = new Size(150, 20),
                Text = "0.5"
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

            _voltageStepTextBox = new TextBox
            {
                Location = new Point(120, 88),
                Size = new Size(150, 20),
                Text = "0.05"
            };

            _voltageStepUnitLabel = new Label
            {
                Text = "V",
                Location = new Point(275, 90),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _tuningParamsGroup.Controls.Add(_setpointLabel);
            _tuningParamsGroup.Controls.Add(_setpointTextBox);
            _tuningParamsGroup.Controls.Add(_setpointUnitLabel);
            _tuningParamsGroup.Controls.Add(_toleranceLabel);
            _tuningParamsGroup.Controls.Add(_toleranceTextBox);
            _tuningParamsGroup.Controls.Add(_toleranceUnitLabel);
            _tuningParamsGroup.Controls.Add(_voltageStepLabel);
            _tuningParamsGroup.Controls.Add(_voltageStepTextBox);
            _tuningParamsGroup.Controls.Add(_voltageStepUnitLabel);

            this.Controls.Add(_tuningParamsGroup);
            yPosition += 130;

            // Safety Limits Group
            _safetyLimitsGroup = new GroupBox
            {
                Text = "Safety Limits",
                Location = new Point(10, yPosition),
                Size = new Size(560, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _minVoltageLabel = new Label
            {
                Text = "Min Voltage:",
                Location = new Point(15, 30),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _minVoltageTextBox = new TextBox
            {
                Location = new Point(120, 28),
                Size = new Size(150, 20),
                Text = "0.1"
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

            _maxVoltageTextBox = new TextBox
            {
                Location = new Point(120, 58),
                Size = new Size(150, 20),
                Text = "5.0"
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

            _maxIterationsTextBox = new TextBox
            {
                Location = new Point(120, 88),
                Size = new Size(150, 20),
                Text = "100"
            };

            _safetyLimitsGroup.Controls.Add(_minVoltageLabel);
            _safetyLimitsGroup.Controls.Add(_minVoltageTextBox);
            _safetyLimitsGroup.Controls.Add(_minVoltageUnitLabel);
            _safetyLimitsGroup.Controls.Add(_maxVoltageLabel);
            _safetyLimitsGroup.Controls.Add(_maxVoltageTextBox);
            _safetyLimitsGroup.Controls.Add(_maxVoltageUnitLabel);
            _safetyLimitsGroup.Controls.Add(_maxIterationsLabel);
            _safetyLimitsGroup.Controls.Add(_maxIterationsTextBox);

            this.Controls.Add(_safetyLimitsGroup);
            yPosition += 130;

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

            this.Controls.Add(_startTuningButton);
            this.Controls.Add(_stopTuningButton);
            this.Controls.Add(_manualMeasureButton);

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
                // Validate all inputs
                if (!ValidateInputs(out string validationError))
                {
                    MessageBox.Show(validationError, "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Build tuning parameters from UI inputs
                TuningParameters parameters = new TuningParameters
                {
                    FrequencyHz = double.Parse(_frequencyTextBox.Text),
                    InitialVoltage = double.Parse(_initialVoltageTextBox.Text),
                    TargetPowerDbm = double.Parse(_setpointTextBox.Text),
                    ToleranceDb = double.Parse(_toleranceTextBox.Text),
                    VoltageStepSize = double.Parse(_voltageStepTextBox.Text),
                    MinVoltage = double.Parse(_minVoltageTextBox.Text),
                    MaxVoltage = double.Parse(_maxVoltageTextBox.Text),
                    MaxIterations = int.Parse(_maxIterationsTextBox.Text),
                    SensorId = _sensorComboBox.SelectedIndex + 1
                };

                // Disable controls during tuning
                UpdateControlStates(isTuning: true);

                // Start tuning
                await _tuningController.StartTuningAsync(parameters);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid numeric input. Please check all values.", "Input Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // Perform manual measurement
                var measurement = await _tuningController.MeasureManualAsync();

                // Display result
                MessageBox.Show(
                    $"Manual Measurement:\n\n" +
                    $"Power: {measurement.PowerDbm:F3} dBm\n" +
                    $"Timestamp: {measurement.Timestamp:yyyy-MM-dd HH:mm:ss.fff}",
                    "Manual Measurement Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
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

        private bool ValidateInputs(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Validate frequency
            if (!double.TryParse(_frequencyTextBox.Text, out double frequency) || frequency < 1 || frequency > 500e6)
            {
                errorMessage = "Frequency must be between 1 Hz and 500 MHz.";
                return false;
            }

            // Validate initial voltage
            if (!double.TryParse(_initialVoltageTextBox.Text, out double initialVoltage) || initialVoltage <= 0)
            {
                errorMessage = "Initial voltage must be greater than 0.";
                return false;
            }

            // Validate setpoint
            if (!double.TryParse(_setpointTextBox.Text, out double setpoint))
            {
                errorMessage = "Setpoint must be a valid number.";
                return false;
            }

            // Validate tolerance
            if (!double.TryParse(_toleranceTextBox.Text, out double tolerance) || tolerance <= 0)
            {
                errorMessage = "Tolerance must be greater than 0.";
                return false;
            }

            // Validate voltage step
            if (!double.TryParse(_voltageStepTextBox.Text, out double voltageStep) || voltageStep <= 0)
            {
                errorMessage = "Voltage step must be greater than 0.";
                return false;
            }

            // Validate min voltage
            if (!double.TryParse(_minVoltageTextBox.Text, out double minVoltage) || minVoltage < 0)
            {
                errorMessage = "Minimum voltage must be greater than or equal to 0.";
                return false;
            }

            // Validate max voltage
            if (!double.TryParse(_maxVoltageTextBox.Text, out double maxVoltage) || maxVoltage <= 0)
            {
                errorMessage = "Maximum voltage must be greater than 0.";
                return false;
            }

            // Validate voltage range
            if (minVoltage >= maxVoltage)
            {
                errorMessage = "Minimum voltage must be less than maximum voltage.";
                return false;
            }

            // Validate initial voltage within range
            if (initialVoltage < minVoltage || initialVoltage > maxVoltage)
            {
                errorMessage = "Initial voltage must be between minimum and maximum voltage.";
                return false;
            }

            // Validate voltage step
            if (voltageStep >= maxVoltage)
            {
                errorMessage = "Voltage step must be less than maximum voltage.";
                return false;
            }

            // Validate max iterations
            if (!int.TryParse(_maxIterationsTextBox.Text, out int maxIterations) || maxIterations < 1 || maxIterations > 10000)
            {
                errorMessage = "Maximum iterations must be between 1 and 10000.";
                return false;
            }

            return true;
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
            // Re-enable controls after error
            UpdateControlStates(isTuning: false);
        }

        private void UpdateControlStates(bool isTuning = false)
        {
            // Disable all input controls when tuning is active or devices are disconnected
            bool enableInputs = !isTuning && _devicesConnected;

            _frequencyTextBox.Enabled = enableInputs;
            _initialVoltageTextBox.Enabled = enableInputs;
            _setpointTextBox.Enabled = enableInputs;
            _toleranceTextBox.Enabled = enableInputs;
            _voltageStepTextBox.Enabled = enableInputs;
            _minVoltageTextBox.Enabled = enableInputs;
            _maxVoltageTextBox.Enabled = enableInputs;
            _maxIterationsTextBox.Enabled = enableInputs;
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
                FrequencyHz = double.Parse(_frequencyTextBox.Text),
                InitialVoltage = double.Parse(_initialVoltageTextBox.Text),
                TargetPowerDbm = double.Parse(_setpointTextBox.Text),
                ToleranceDb = double.Parse(_toleranceTextBox.Text),
                VoltageStepSize = double.Parse(_voltageStepTextBox.Text),
                MinVoltage = double.Parse(_minVoltageTextBox.Text),
                MaxVoltage = double.Parse(_maxVoltageTextBox.Text),
                MaxIterations = int.Parse(_maxIterationsTextBox.Text),
                SensorId = _sensorComboBox.SelectedIndex + 1
            };
        }

        /// <summary>
        /// Sets the tuning parameters in the UI.
        /// </summary>
        public void SetTuningParameters(TuningParameters parameters)
        {
            if (parameters == null) return;

            _frequencyTextBox.Text = parameters.FrequencyHz.ToString();
            _initialVoltageTextBox.Text = parameters.InitialVoltage.ToString();
            _setpointTextBox.Text = parameters.TargetPowerDbm.ToString();
            _toleranceTextBox.Text = parameters.ToleranceDb.ToString();
            _voltageStepTextBox.Text = parameters.VoltageStepSize.ToString();
            _minVoltageTextBox.Text = parameters.MinVoltage.ToString();
            _maxVoltageTextBox.Text = parameters.MaxVoltage.ToString();
            _maxIterationsTextBox.Text = parameters.MaxIterations.ToString();
            
            if (parameters.SensorId >= 1 && parameters.SensorId <= _sensorComboBox.Items.Count)
            {
                _sensorComboBox.SelectedIndex = parameters.SensorId - 1;
            }
        }
    }
}
