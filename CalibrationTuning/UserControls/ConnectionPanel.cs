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
        private Button _powerMeterConnectButton;
        private Button _powerMeterDisconnectButton;

        private GroupBox _signalGenGroup;
        private Label _signalGenIpLabel;
        private TextBox _signalGenIpTextBox;
        private Label _signalGenStatusLabel;
        private Button _signalGenConnectButton;
        private Button _signalGenDisconnectButton;

        private bool _powerMeterConnected = false;
        private bool _signalGenConnected = false;

        public ConnectionPanel(ITuningController tuningController, MainForm mainForm)
        {
            _tuningController = tuningController ?? throw new ArgumentNullException(nameof(tuningController));
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));

            InitializeComponent();
            InitializeControls();
            SubscribeToEvents();
        }

        private void InitializeControls()
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
                Size = new Size(560, 120),
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

            _powerMeterConnectButton = new Button
            {
                Text = "Connect",
                Location = new Point(15, 85),
                Size = new Size(90, 25)
            };
            _powerMeterConnectButton.Click += PowerMeterConnectButton_Click;

            _powerMeterDisconnectButton = new Button
            {
                Text = "Disconnect",
                Location = new Point(115, 85),
                Size = new Size(90, 25),
                Enabled = false
            };
            _powerMeterDisconnectButton.Click += PowerMeterDisconnectButton_Click;

            _powerMeterGroup.Controls.Add(_powerMeterIpLabel);
            _powerMeterGroup.Controls.Add(_powerMeterIpTextBox);
            _powerMeterGroup.Controls.Add(_powerMeterStatusLabel);
            _powerMeterGroup.Controls.Add(_powerMeterConnectButton);
            _powerMeterGroup.Controls.Add(_powerMeterDisconnectButton);

            // Signal Generator Group
            _signalGenGroup = new GroupBox
            {
                Text = "Signal Generator (Siglent SDG6052X)",
                Location = new Point(10, 140),
                Size = new Size(560, 120),
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

            _signalGenConnectButton = new Button
            {
                Text = "Connect",
                Location = new Point(15, 85),
                Size = new Size(90, 25)
            };
            _signalGenConnectButton.Click += SignalGenConnectButton_Click;

            _signalGenDisconnectButton = new Button
            {
                Text = "Disconnect",
                Location = new Point(115, 85),
                Size = new Size(90, 25),
                Enabled = false
            };
            _signalGenDisconnectButton.Click += SignalGenDisconnectButton_Click;

            _signalGenGroup.Controls.Add(_signalGenIpLabel);
            _signalGenGroup.Controls.Add(_signalGenIpTextBox);
            _signalGenGroup.Controls.Add(_signalGenStatusLabel);
            _signalGenGroup.Controls.Add(_signalGenConnectButton);
            _signalGenGroup.Controls.Add(_signalGenDisconnectButton);

            // Add controls to panel
            this.Controls.Add(_powerMeterGroup);
            this.Controls.Add(_signalGenGroup);

            this.ResumeLayout(false);
        }

        private void SubscribeToEvents()
        {
            // Subscribe to tuning controller events for connection status updates
            _tuningController.StateChanged += TuningController_StateChanged;
            _tuningController.ErrorOccurred += TuningController_ErrorOccurred;
        }

        private async void PowerMeterConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                _powerMeterConnectButton.Enabled = false;
                _powerMeterIpTextBox.Enabled = false;

                string powerMeterIp = _powerMeterIpTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(powerMeterIp))
                {
                    MessageBox.Show("Please enter a valid Power Meter IP address.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _powerMeterStatusLabel.Text = "Status: Connecting...";
                _powerMeterStatusLabel.ForeColor = Color.Orange;

                bool success = await _tuningController.ConnectPowerMeterAsync(powerMeterIp);

                if (success)
                {
                    _powerMeterStatusLabel.Text = "Status: Connected";
                    _powerMeterStatusLabel.ForeColor = Color.Green;
                    _powerMeterDisconnectButton.Enabled = true;
                    _powerMeterConnected = true;
                    _mainForm.UpdateConnectionStatus(_powerMeterConnected, _signalGenConnected);
                }
                else
                {
                    _powerMeterStatusLabel.Text = "Status: Connection Failed";
                    _powerMeterStatusLabel.ForeColor = Color.Red;
                    _powerMeterConnected = false;
                    _mainForm.UpdateConnectionStatus(_powerMeterConnected, _signalGenConnected);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _powerMeterStatusLabel.Text = "Status: Error";
                _powerMeterStatusLabel.ForeColor = Color.Red;
                _powerMeterConnected = false;
                _mainForm.UpdateConnectionStatus(_powerMeterConnected, _signalGenConnected);
            }
            finally
            {
                _powerMeterConnectButton.Enabled = true;
                _powerMeterIpTextBox.Enabled = true;
            }
        }

        private void PowerMeterDisconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                _tuningController.DisconnectPowerMeter();
                _powerMeterStatusLabel.Text = "Status: Disconnected";
                _powerMeterStatusLabel.ForeColor = Color.Red;
                _powerMeterConnectButton.Enabled = true;
                _powerMeterDisconnectButton.Enabled = false;
                _powerMeterIpTextBox.Enabled = true;
                _powerMeterConnected = false;
                _mainForm.UpdateConnectionStatus(_powerMeterConnected, _signalGenConnected);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Disconnection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SignalGenConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                _signalGenConnectButton.Enabled = false;
                _signalGenIpTextBox.Enabled = false;

                string signalGenIp = _signalGenIpTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(signalGenIp))
                {
                    MessageBox.Show("Please enter a valid Signal Generator IP address.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _signalGenStatusLabel.Text = "Status: Connecting...";
                _signalGenStatusLabel.ForeColor = Color.Orange;

                bool success = await _tuningController.ConnectSignalGeneratorAsync(signalGenIp);

                if (success)
                {
                    _signalGenStatusLabel.Text = "Status: Connected";
                    _signalGenStatusLabel.ForeColor = Color.Green;
                    _signalGenDisconnectButton.Enabled = true;
                    _signalGenConnected = true;
                    _mainForm.UpdateConnectionStatus(_powerMeterConnected, _signalGenConnected);
                }
                else
                {
                    _signalGenStatusLabel.Text = "Status: Connection Failed";
                    _signalGenStatusLabel.ForeColor = Color.Red;
                    _signalGenConnected = false;
                    _mainForm.UpdateConnectionStatus(_powerMeterConnected, _signalGenConnected);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _signalGenStatusLabel.Text = "Status: Error";
                _signalGenStatusLabel.ForeColor = Color.Red;
                _signalGenConnected = false;
                _mainForm.UpdateConnectionStatus(_powerMeterConnected, _signalGenConnected);
            }
            finally
            {
                _signalGenConnectButton.Enabled = true;
                _signalGenIpTextBox.Enabled = true;
            }
        }

        private void SignalGenDisconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                _tuningController.DisconnectSignalGenerator();
                _signalGenStatusLabel.Text = "Status: Disconnected";
                _signalGenStatusLabel.ForeColor = Color.Red;
                _signalGenConnectButton.Enabled = true;
                _signalGenDisconnectButton.Enabled = false;
                _signalGenIpTextBox.Enabled = true;
                _signalGenConnected = false;
                _mainForm.UpdateConnectionStatus(_powerMeterConnected, _signalGenConnected);
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
