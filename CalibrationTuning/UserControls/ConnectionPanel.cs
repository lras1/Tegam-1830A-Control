using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CalibrationTuning.Controllers;

namespace CalibrationTuning.UserControls
{
    /// <summary>
    /// User control for managing device connections.
    /// Provides IP address inputs and connection controls for both power meter and signal generator.
    /// </summary>
    public partial class ConnectionPanel : UserControl
    {
        private readonly ITuningController _tuningController;
        private readonly MainForm _mainForm;

        // UI Controls
        private GroupBox _powerMeterGroup;
        private Label _powerMeterIpLabel;
        private TextBox _powerMeterIpTextBox;
        private Label _powerMeterStatusLabel;

        private GroupBox _signalGenGroup;
        private Label _signalGenIpLabel;
        private TextBox _signalGenIpTextBox;
        private Label _signalGenStatusLabel;

        private Button _connectButton;
        private Button _disconnectButton;

        public ConnectionPanel(ITuningController tuningController, MainForm mainForm)
        {
            _tuningController = tuningController ?? throw new ArgumentNullException(nameof(tuningController));
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));

            InitializeComponent();
            SubscribeToEvents();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Panel properties
            this.Size = new Size(600, 300);
            this.Padding = new Padding(10);

            // Power Meter Group
            _powerMeterGroup = new GroupBox
            {
                Text = "Power Meter (Tegam 1830A)",
                Location = new Point(10, 10),
                Size = new Size(560, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _powerMeterIpLabel = new Label
            {
                Text = "IP Address:",
                Location = new Point(15, 30),
                Size = new Size(80, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _powerMeterIpTextBox = new TextBox
            {
                Location = new Point(100, 28),
                Size = new Size(200, 20),
                Text = "192.168.1.100"
            };

            _powerMeterStatusLabel = new Label
            {
                Text = "Status: Disconnected",
                Location = new Point(15, 60),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Red
            };

            _powerMeterGroup.Controls.Add(_powerMeterIpLabel);
            _powerMeterGroup.Controls.Add(_powerMeterIpTextBox);
            _powerMeterGroup.Controls.Add(_powerMeterStatusLabel);

            // Signal Generator Group
            _signalGenGroup = new GroupBox
            {
                Text = "Signal Generator (Siglent SDG6052X)",
                Location = new Point(10, 120),
                Size = new Size(560, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _signalGenIpLabel = new Label
            {
                Text = "IP Address:",
                Location = new Point(15, 30),
                Size = new Size(80, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _signalGenIpTextBox = new TextBox
            {
                Location = new Point(100, 28),
                Size = new Size(200, 20),
                Text = "192.168.1.101"
            };

            _signalGenStatusLabel = new Label
            {
                Text = "Status: Disconnected",
                Location = new Point(15, 60),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Red
            };

            _signalGenGroup.Controls.Add(_signalGenIpLabel);
            _signalGenGroup.Controls.Add(_signalGenIpTextBox);
            _signalGenGroup.Controls.Add(_signalGenStatusLabel);

            // Connect Button
            _connectButton = new Button
            {
                Text = "Connect",
                Location = new Point(10, 230),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            _connectButton.Click += ConnectButton_Click;

            // Disconnect Button
            _disconnectButton = new Button
            {
                Text = "Disconnect",
                Location = new Point(120, 230),
                Size = new Size(100, 30),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            _disconnectButton.Click += DisconnectButton_Click;

            // Add controls to panel
            this.Controls.Add(_powerMeterGroup);
            this.Controls.Add(_signalGenGroup);
            this.Controls.Add(_connectButton);
            this.Controls.Add(_disconnectButton);

            this.ResumeLayout(false);
        }

        private void SubscribeToEvents()
        {
            // Subscribe to tuning controller events for connection status updates
            _tuningController.StateChanged += TuningController_StateChanged;
            _tuningController.ErrorOccurred += TuningController_ErrorOccurred;
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable controls during connection attempt
                _connectButton.Enabled = false;
                _powerMeterIpTextBox.Enabled = false;
                _signalGenIpTextBox.Enabled = false;

                string powerMeterIp = _powerMeterIpTextBox.Text.Trim();
                string signalGenIp = _signalGenIpTextBox.Text.Trim();

                // Validate IP addresses
                if (string.IsNullOrWhiteSpace(powerMeterIp))
                {
                    MessageBox.Show("Please enter a valid Power Meter IP address.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(signalGenIp))
                {
                    MessageBox.Show("Please enter a valid Signal Generator IP address.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update status labels
                _powerMeterStatusLabel.Text = "Status: Connecting...";
                _powerMeterStatusLabel.ForeColor = Color.Orange;
                _signalGenStatusLabel.Text = "Status: Connecting...";
                _signalGenStatusLabel.ForeColor = Color.Orange;

                // Attempt connection
                bool success = await _tuningController.ConnectDevicesAsync(powerMeterIp, signalGenIp);

                if (success)
                {
                    // Update status labels
                    _powerMeterStatusLabel.Text = "Status: Connected";
                    _powerMeterStatusLabel.ForeColor = Color.Green;
                    _signalGenStatusLabel.Text = "Status: Connected";
                    _signalGenStatusLabel.ForeColor = Color.Green;

                    // Update button states
                    _disconnectButton.Enabled = true;

                    // Update main form status bar
                    _mainForm.UpdateConnectionStatus(true, true);

                    MessageBox.Show("Successfully connected to both devices.", "Connection Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Error occurred - status labels will be updated by error event handler
                    _powerMeterStatusLabel.Text = "Status: Connection Failed";
                    _powerMeterStatusLabel.ForeColor = Color.Red;
                    _signalGenStatusLabel.Text = "Status: Connection Failed";
                    _signalGenStatusLabel.ForeColor = Color.Red;

                    // Update main form status bar
                    _mainForm.UpdateConnectionStatus(false, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                _powerMeterStatusLabel.Text = "Status: Error";
                _powerMeterStatusLabel.ForeColor = Color.Red;
                _signalGenStatusLabel.Text = "Status: Error";
                _signalGenStatusLabel.ForeColor = Color.Red;

                _mainForm.UpdateConnectionStatus(false, false);
            }
            finally
            {
                // Re-enable controls
                _connectButton.Enabled = true;
                _powerMeterIpTextBox.Enabled = true;
                _signalGenIpTextBox.Enabled = true;
            }
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Disconnect devices
                _tuningController.DisconnectDevices();

                // Update status labels
                _powerMeterStatusLabel.Text = "Status: Disconnected";
                _powerMeterStatusLabel.ForeColor = Color.Red;
                _signalGenStatusLabel.Text = "Status: Disconnected";
                _signalGenStatusLabel.ForeColor = Color.Red;

                // Update button states
                _connectButton.Enabled = true;
                _disconnectButton.Enabled = false;
                _powerMeterIpTextBox.Enabled = true;
                _signalGenIpTextBox.Enabled = true;

                // Update main form status bar
                _mainForm.UpdateConnectionStatus(false, false);

                MessageBox.Show("Devices disconnected successfully.", "Disconnection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Disconnection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TuningController_StateChanged(object sender, Events.TuningStateChangedEventArgs e)
        {
            // Handle state changes if needed
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleStateChange(e.NewState)));
            }
            else
            {
                HandleStateChange(e.NewState);
            }
        }

        private void HandleStateChange(Models.TuningState newState)
        {
            // Disable connection controls while tuning is active
            bool isTuningActive = newState == Models.TuningState.Tuning || 
                                  newState == Models.TuningState.Measuring || 
                                  newState == Models.TuningState.Evaluating;

            _disconnectButton.Enabled = !isTuningActive && _disconnectButton.Enabled;
        }

        private void TuningController_ErrorOccurred(object sender, ErrorEventArgs e)
        {
            // Handle errors if needed
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
            // Check if error is connection-related
            if (errorMessage.Contains("connect") || errorMessage.Contains("connection"))
            {
                _powerMeterStatusLabel.Text = "Status: Connection Error";
                _powerMeterStatusLabel.ForeColor = Color.Red;
                _signalGenStatusLabel.Text = "Status: Connection Error";
                _signalGenStatusLabel.ForeColor = Color.Red;

                _mainForm.UpdateConnectionStatus(false, false);
            }
        }

        /// <summary>
        /// Gets the current power meter IP address.
        /// </summary>
        public string PowerMeterIpAddress
        {
            get { return _powerMeterIpTextBox.Text.Trim(); }
            set { _powerMeterIpTextBox.Text = value; }
        }

        /// <summary>
        /// Gets the current signal generator IP address.
        /// </summary>
        public string SignalGeneratorIpAddress
        {
            get { return _signalGenIpTextBox.Text.Trim(); }
            set { _signalGenIpTextBox.Text = value; }
        }
    }
}
