using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Services;
using Tegam.WinFormsUI.Controllers;

namespace Tegam.WinFormsUI.Forms
{
    public partial class MainForm : Form
    {
        private readonly IPowerMeterService _powerMeterService;
        private readonly MainFormController _controller;
        private readonly PowerMeasurementPanel _powerMeasurementPanel;
        private readonly FrequencyConfigurationPanel _frequencyConfigurationPanel;
        private readonly SensorManagementPanel _sensorManagementPanel;
        private readonly CalibrationPanel _calibrationPanel;
        private readonly EnhancedLoggingPanel _enhancedLoggingPanel;

        public MainForm(
            IPowerMeterService powerMeterService,
            MainFormController controller,
            PowerMeasurementPanel powerMeasurementPanel,
            FrequencyConfigurationPanel frequencyConfigurationPanel,
            SensorManagementPanel sensorManagementPanel,
            CalibrationPanel calibrationPanel,
            EnhancedLoggingPanel enhancedLoggingPanel)
        {
            _powerMeterService = powerMeterService;
            _controller = controller;
            _powerMeasurementPanel = powerMeasurementPanel;
            _frequencyConfigurationPanel = frequencyConfigurationPanel;
            _sensorManagementPanel = sensorManagementPanel;
            _calibrationPanel = calibrationPanel;
            _enhancedLoggingPanel = enhancedLoggingPanel;

            InitializeComponent();
            InitializePanels();
            SubscribeToEvents();
        }

        private void InitializePanels()
        {
            // Add panels to tabs
            _powerMeasurementPanel.Dock = DockStyle.Fill;
            tabPower.Controls.Add(_powerMeasurementPanel);

            _frequencyConfigurationPanel.Dock = DockStyle.Fill;
            tabFrequency.Controls.Add(_frequencyConfigurationPanel);

            _sensorManagementPanel.Dock = DockStyle.Fill;
            tabSensors.Controls.Add(_sensorManagementPanel);

            _calibrationPanel.Dock = DockStyle.Fill;
            tabCalibration.Controls.Add(_calibrationPanel);

            _enhancedLoggingPanel.Dock = DockStyle.Fill;
            tabLogging.Controls.Add(_enhancedLoggingPanel);

            // Enable all tabs and panels before connection
            // This allows logging to start before device connection
            tabControl.Enabled = true;
            _powerMeasurementPanel.Enabled = false;  // Only disable power measurement
            _frequencyConfigurationPanel.Enabled = false;  // Only disable frequency config
            _sensorManagementPanel.Enabled = false;  // Only disable sensor management
            _calibrationPanel.Enabled = false;  // Only disable calibration
            _enhancedLoggingPanel.Enabled = true;  // Keep logging enabled
        }

        private void SubscribeToEvents()
        {
            _powerMeterService.ConnectionStateChanged += PowerMeterService_ConnectionStateChanged;
            _powerMeterService.DeviceError += PowerMeterService_DeviceError;
        }

        private void PowerMeterService_ConnectionStateChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(PowerMeterService_ConnectionStateChanged), sender, e);
                return;
            }

            if (_powerMeterService.IsConnected)
            {
                lblStatus.Text = "Connected";
                lblStatus.ForeColor = System.Drawing.Color.Green;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                txtIpAddress.Enabled = false;
                EnableControls(true);

                // Display device information
                if (_powerMeterService.DeviceInfo != null)
                {
                    lblDeviceInfoValue.Text = $"Model: {_powerMeterService.DeviceInfo.Model}, " +
                        $"Serial: {_powerMeterService.DeviceInfo.SerialNumber}, " +
                        $"Firmware: {_powerMeterService.DeviceInfo.FirmwareVersion}";
                }
            }
            else
            {
                lblStatus.Text = "Disconnected";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                txtIpAddress.Enabled = true;
                EnableControls(false);
                lblDeviceInfoValue.Text = "Not connected";
            }
        }

        private void PowerMeterService_DeviceError(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(PowerMeterService_DeviceError), sender, e);
                return;
            }

            MessageBox.Show("Device error occurred. Check logs for details.", "Device Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void EnableControls(bool enabled)
        {
            // Don't disable the tab control itself - keep tabs accessible
            // Only enable/disable individual panels based on connection state
            _powerMeasurementPanel.Enabled = enabled;
            _frequencyConfigurationPanel.Enabled = enabled;
            _sensorManagementPanel.Enabled = enabled;
            _calibrationPanel.Enabled = enabled;
            // Keep logging panel always enabled (connection-independent)
            _enhancedLoggingPanel.Enabled = true;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                toolStripStatusLabel.Text = "Connecting...";
                string ipAddress = txtIpAddress.Text.Trim();

                if (string.IsNullOrEmpty(ipAddress))
                {
                    MessageBox.Show("Please enter an IP address.", "Input Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool connected = await Task.Run(() => _powerMeterService.Connect(ipAddress));
                if (!connected)
                {
                    MessageBox.Show("Failed to connect to device.", "Connection Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toolStripStatusLabel.Text = "Connection failed";
                }
                else
                {
                    toolStripStatusLabel.Text = "Connected";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel.Text = "Error";
            }
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                toolStripStatusLabel.Text = "Disconnecting...";
                await Task.Run(() => _powerMeterService.Disconnect());
                toolStripStatusLabel.Text = "Disconnected";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Disconnection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel.Text = "Error";
            }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_powerMeterService.IsConnected)
                {
                    await Task.Run(() => _powerMeterService.Disconnect());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during form closing: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
