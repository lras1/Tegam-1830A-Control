using System;
using System.ComponentModel;
using System.Windows.Forms;
using Siglent.SDG6052X.DeviceLibrary.Services;
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.WinFormsUI.Forms
{
    public partial class MainForm : Form
    {
        private readonly ISignalGeneratorService _service;
        private ErrorProvider _errorProvider;

        public MainForm(ISignalGeneratorService service)
        {
            InitializeComponent();
            _service = service ?? throw new ArgumentNullException(nameof(service));
            
            // Initialize error provider for validation feedback
            _errorProvider = new ErrorProvider();
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            
            // Subscribe to service events
            _service.ConnectionStateChanged += OnConnectionStateChanged;
            _service.DeviceError += OnDeviceError;
            
            // Initialize UI state
            InitializeWaveformControls();
            UpdateConnectionState(false);
        }

        private void InitializeWaveformControls()
        {
            // Initialize channel selector
            cmbChannel.SelectedIndex = 0; // Default to Channel 1
            
            // Initialize waveform type dropdown
            cmbWaveformType.Items.Clear();
            foreach (WaveformType type in Enum.GetValues(typeof(WaveformType)))
            {
                cmbWaveformType.Items.Add(type);
            }
            cmbWaveformType.SelectedIndex = 0; // Default to Sine
            
            // Initialize amplitude unit dropdown
            cmbAmplitudeUnit.Items.Clear();
            foreach (AmplitudeUnit unit in Enum.GetValues(typeof(AmplitudeUnit)))
            {
                cmbAmplitudeUnit.Items.Add(unit);
            }
            cmbAmplitudeUnit.SelectedIndex = 0; // Default to Vpp
            
            // Initialize load impedance dropdown
            cmbLoadImpedance.Items.Clear();
            cmbLoadImpedance.Items.Add("50Ω");
            cmbLoadImpedance.Items.Add("High-Z");
            cmbLoadImpedance.Items.Add("Custom");
            cmbLoadImpedance.SelectedIndex = 0; // Default to 50Ω
            
            // Initialize modulation controls
            InitializeModulationControls();
            
            // Initialize sweep controls
            InitializeSweepControls();
        }

        private void InitializeModulationControls()
        {
            // Initialize modulation channel selector
            cmbModChannel.SelectedIndex = 0; // Default to Channel 1
            
            // Initialize modulation type dropdown
            cmbModulationType.Items.Clear();
            foreach (ModulationType type in Enum.GetValues(typeof(ModulationType)))
            {
                cmbModulationType.Items.Add(type);
            }
            cmbModulationType.SelectedIndex = 0; // Default to AM
            
            // Initialize modulation source dropdown
            cmbModulationSource.Items.Clear();
            foreach (ModulationSource source in Enum.GetValues(typeof(ModulationSource)))
            {
                cmbModulationSource.Items.Add(source);
            }
            cmbModulationSource.SelectedIndex = 0; // Default to Internal
            
            // Initialize modulation waveform dropdown
            cmbModulationWaveform.Items.Clear();
            foreach (WaveformType type in Enum.GetValues(typeof(WaveformType)))
            {
                cmbModulationWaveform.Items.Add(type);
            }
            cmbModulationWaveform.SelectedIndex = 0; // Default to Sine
        }

        private void cmbWaveformType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show/hide controls based on waveform type
            WaveformType selectedType = (WaveformType)cmbWaveformType.SelectedItem;
            
            bool isSquareOrPulse = (selectedType == WaveformType.Square || selectedType == WaveformType.Pulse);
            bool isPulse = (selectedType == WaveformType.Pulse);
            
            // Duty cycle is visible for Square and Pulse
            lblDutyCycle.Visible = isSquareOrPulse;
            txtDutyCycle.Visible = isSquareOrPulse;
            lblDutyCycleUnit.Visible = isSquareOrPulse;
            
            // Pulse-specific controls
            lblPulseWidth.Visible = isPulse;
            txtPulseWidth.Visible = isPulse;
            lblPulseWidthUnit.Visible = isPulse;
            
            lblRiseTime.Visible = isPulse;
            txtRiseTime.Visible = isPulse;
            lblRiseTimeUnit.Visible = isPulse;
            
            lblFallTime.Visible = isPulse;
            txtFallTime.Visible = isPulse;
            lblFallTimeUnit.Visible = isPulse;
        }

        private async void btnSetWaveform_Click(object sender, EventArgs e)
        {
            // Validate all inputs first
            if (!ValidateChildren(ValidationConstraints.Enabled))
            {
                MessageBox.Show("Please correct the validation errors before proceeding.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // Get channel number (1 or 2)
                int channel = cmbChannel.SelectedIndex + 1;
                
                // Get waveform type
                WaveformType waveformType = (WaveformType)cmbWaveformType.SelectedItem;
                
                // Build waveform parameters
                var parameters = new WaveformParameters
                {
                    Frequency = double.Parse(txtFrequency.Text),
                    Amplitude = double.Parse(txtAmplitude.Text),
                    Offset = double.Parse(txtOffset.Text),
                    Phase = double.Parse(txtPhase.Text),
                    Unit = (AmplitudeUnit)cmbAmplitudeUnit.SelectedItem,
                    Load = GetSelectedLoadImpedance()
                };
                
                // Add waveform-specific parameters
                if (waveformType == WaveformType.Square || waveformType == WaveformType.Pulse)
                {
                    parameters.DutyCycle = double.Parse(txtDutyCycle.Text);
                }
                
                if (waveformType == WaveformType.Pulse)
                {
                    parameters.Width = double.Parse(txtPulseWidth.Text);
                    parameters.Rise = double.Parse(txtRiseTime.Text);
                    parameters.Fall = double.Parse(txtFallTime.Text);
                }
                
                // Disable button during operation
                btnSetWaveform.Enabled = false;
                
                // Set load impedance first
                var loadResult = await _service.SetLoadImpedanceAsync(channel, parameters.Load);
                if (!loadResult.Success)
                {
                    MessageBox.Show($"Failed to set load impedance: {loadResult.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Set waveform
                var result = await _service.SetBasicWaveformAsync(channel, waveformType, parameters);
                
                if (result.Success)
                {
                    MessageBox.Show($"Waveform configured successfully on Channel {channel}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to configure waveform: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid numeric input. Please check all values.", 
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSetWaveform.Enabled = true;
            }
        }

        private async void chkOutputEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!_service.IsConnected)
                return;
            
            try
            {
                int channel = cmbChannel.SelectedIndex + 1;
                var result = await _service.SetOutputStateAsync(channel, chkOutputEnable.Checked);
                
                if (!result.Success)
                {
                    // Revert checkbox state on failure
                    chkOutputEnable.CheckedChanged -= chkOutputEnable_CheckedChanged;
                    chkOutputEnable.Checked = !chkOutputEnable.Checked;
                    chkOutputEnable.CheckedChanged += chkOutputEnable_CheckedChanged;
                    
                    MessageBox.Show($"Failed to set output state: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnQueryState_Click(object sender, EventArgs e)
        {
            try
            {
                int channel = cmbChannel.SelectedIndex + 1;
                
                btnQueryState.Enabled = false;
                
                // Query current waveform state
                var state = await _service.GetWaveformStateAsync(channel);
                
                if (state != null)
                {
                    // Update UI with queried values
                    cmbWaveformType.SelectedItem = state.WaveformType;
                    txtFrequency.Text = state.Frequency.ToString("G");
                    txtAmplitude.Text = state.Amplitude.ToString("G");
                    txtOffset.Text = state.Offset.ToString("G");
                    txtPhase.Text = state.Phase.ToString("G");
                    cmbAmplitudeUnit.SelectedItem = state.Unit;
                    chkOutputEnable.Checked = state.OutputEnabled;
                    
                    if (state.WaveformType == WaveformType.Square || state.WaveformType == WaveformType.Pulse)
                    {
                        txtDutyCycle.Text = state.DutyCycle.ToString("G");
                    }
                    
                    MessageBox.Show($"Current state retrieved for Channel {channel}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to query device state", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error querying state: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnQueryState.Enabled = true;
            }
        }

        private LoadImpedance GetSelectedLoadImpedance()
        {
            switch (cmbLoadImpedance.SelectedIndex)
            {
                case 0: // 50Ω
                    return LoadImpedance.FiftyOhm;
                case 1: // High-Z
                    return LoadImpedance.HighZ;
                case 2: // Custom (default to 75Ω for now)
                    return LoadImpedance.Custom(75);
                default:
                    return LoadImpedance.FiftyOhm;
            }
        }

        // Validation event handlers
        private void txtFrequency_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtFrequency.Text, out double frequency) || frequency <= 0)
            {
                _errorProvider.SetError(txtFrequency, "Frequency must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtFrequency, string.Empty);
            }
        }

        private void txtAmplitude_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtAmplitude.Text, out double amplitude) || amplitude <= 0)
            {
                _errorProvider.SetError(txtAmplitude, "Amplitude must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtAmplitude, string.Empty);
            }
        }

        private void txtOffset_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtOffset.Text, out double offset))
            {
                _errorProvider.SetError(txtOffset, "Offset must be a valid number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtOffset, string.Empty);
            }
        }

        private void txtPhase_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtPhase.Text, out double phase) || phase < 0 || phase > 360)
            {
                _errorProvider.SetError(txtPhase, "Phase must be between 0 and 360 degrees");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtPhase, string.Empty);
            }
        }

        private void txtDutyCycle_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtDutyCycle.Text, out double duty) || duty < 0.01 || duty > 99.99)
            {
                _errorProvider.SetError(txtDutyCycle, "Duty cycle must be between 0.01 and 99.99%");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtDutyCycle, string.Empty);
            }
        }

        private void txtPulseWidth_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtPulseWidth.Text, out double width) || width <= 0)
            {
                _errorProvider.SetError(txtPulseWidth, "Pulse width must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtPulseWidth, string.Empty);
            }
        }

        private void txtRiseTime_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtRiseTime.Text, out double rise) || rise <= 0)
            {
                _errorProvider.SetError(txtRiseTime, "Rise time must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtRiseTime, string.Empty);
            }
        }

        private void txtFallTime_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtFallTime.Text, out double fall) || fall <= 0)
            {
                _errorProvider.SetError(txtFallTime, "Fall time must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtFallTime, string.Empty);
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            string ipAddress = txtIpAddress.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                MessageBox.Show("Please enter an IP address.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConnect.Enabled = false;
            btnDisconnect.Enabled = false;
            lblConnectionStatus.Text = "Connecting...";
            lblConnectionStatus.ForeColor = System.Drawing.Color.Orange;

            try
            {
                bool connected = await _service.ConnectAsync(ipAddress);
                
                if (connected)
                {
                    UpdateConnectionState(true);
                    DisplayDeviceInfo();
                }
                else
                {
                    UpdateConnectionState(false);
                    MessageBox.Show("Failed to connect to device. Please check the IP address and network connection.", 
                        "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                UpdateConnectionState(false);
                MessageBox.Show($"Connection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConnect.Enabled = !_service.IsConnected;
                btnDisconnect.Enabled = _service.IsConnected;
            }
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = false;
            lblConnectionStatus.Text = "Disconnecting...";
            lblConnectionStatus.ForeColor = System.Drawing.Color.Orange;

            try
            {
                await _service.DisconnectAsync();
                UpdateConnectionState(false);
                ClearDeviceInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Disconnection error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConnect.Enabled = !_service.IsConnected;
                btnDisconnect.Enabled = _service.IsConnected;
            }
        }

        private void OnConnectionStateChanged(object sender, EventArgs e)
        {
            // Ensure UI updates happen on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnConnectionStateChanged(sender, e)));
                return;
            }

            UpdateConnectionState(_service.IsConnected);
        }

        private void OnDeviceError(object sender, EventArgs e)
        {
            // Ensure UI updates happen on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnDeviceError(sender, e)));
                return;
            }

            // Display error message to user
            MessageBox.Show($"Device error occurred. Please check the device status.", 
                "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void UpdateConnectionState(bool isConnected)
        {
            if (isConnected)
            {
                lblConnectionStatus.Text = "Connected";
                lblConnectionStatus.ForeColor = System.Drawing.Color.Green;
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                txtIpAddress.Enabled = false;
                EnableControls(true);
            }
            else
            {
                lblConnectionStatus.Text = "Disconnected";
                lblConnectionStatus.ForeColor = System.Drawing.Color.Red;
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                txtIpAddress.Enabled = true;
                EnableControls(false);
            }
        }

        private void DisplayDeviceInfo()
        {
            if (_service.DeviceInfo != null)
            {
                lblDeviceInfo.Text = $"{_service.DeviceInfo.Manufacturer} {_service.DeviceInfo.Model}\n" +
                                    $"S/N: {_service.DeviceInfo.SerialNumber}\n" +
                                    $"Firmware: {_service.DeviceInfo.FirmwareVersion}";
            }
        }

        private void ClearDeviceInfo()
        {
            lblDeviceInfo.Text = "No device connected";
        }

        private void EnableControls(bool enabled)
        {
            // Enable/disable tab control and other controls based on connection state
            tabControl.Enabled = enabled;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Unsubscribe from events
            if (_service != null)
            {
                _service.ConnectionStateChanged -= OnConnectionStateChanged;
                _service.DeviceError -= OnDeviceError;
                
                // Disconnect if still connected
                if (_service.IsConnected)
                {
                    _service.DisconnectAsync().Wait();
                }
            }

            base.OnFormClosing(e);
        }

        // Modulation event handlers
        private void cmbModulationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show/hide controls based on modulation type
            ModulationType selectedType = (ModulationType)cmbModulationType.SelectedItem;
            
            // Depth is visible for AM and PWM
            bool showDepth = (selectedType == ModulationType.AM || selectedType == ModulationType.PWM);
            lblDepth.Visible = showDepth;
            txtDepth.Visible = showDepth;
            lblDepthUnit.Visible = showDepth;
            
            // Deviation is visible for FM and PM
            bool showDeviation = (selectedType == ModulationType.FM || selectedType == ModulationType.PM);
            lblDeviation.Visible = showDeviation;
            txtDeviation.Visible = showDeviation;
            lblDeviationUnit.Visible = showDeviation;
            
            // FSK-specific controls
            bool showFSK = (selectedType == ModulationType.FSK);
            lblHopFrequency.Visible = showFSK;
            txtHopFrequency.Visible = showFSK;
            lblHopFrequencyUnit.Visible = showFSK;
            
            // ASK-specific controls
            bool showASK = (selectedType == ModulationType.ASK);
            lblHopAmplitude.Visible = showASK;
            txtHopAmplitude.Visible = showASK;
            lblHopAmplitudeUnit.Visible = showASK;
            
            // PSK-specific controls
            bool showPSK = (selectedType == ModulationType.PSK);
            lblHopPhase.Visible = showPSK;
            txtHopPhase.Visible = showPSK;
            lblHopPhaseUnit.Visible = showPSK;
        }

        private async void btnConfigureModulation_Click(object sender, EventArgs e)
        {
            // Validate all inputs first
            if (!ValidateChildren(ValidationConstraints.Enabled))
            {
                MessageBox.Show("Please correct the validation errors before proceeding.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // Get channel number (1 or 2)
                int channel = cmbModChannel.SelectedIndex + 1;
                
                // Get modulation type
                ModulationType modulationType = (ModulationType)cmbModulationType.SelectedItem;
                
                // Build modulation parameters
                var parameters = new ModulationParameters
                {
                    Type = modulationType,
                    Source = (ModulationSource)cmbModulationSource.SelectedItem,
                    ModulationWaveform = (WaveformType)cmbModulationWaveform.SelectedItem,
                    Rate = double.Parse(txtRate.Text)
                };
                
                // Add type-specific parameters
                if (modulationType == ModulationType.AM || modulationType == ModulationType.PWM)
                {
                    parameters.Depth = double.Parse(txtDepth.Text);
                }
                
                if (modulationType == ModulationType.FM || modulationType == ModulationType.PM)
                {
                    parameters.Deviation = double.Parse(txtDeviation.Text);
                }
                
                if (modulationType == ModulationType.FSK)
                {
                    parameters.HopFrequency = double.Parse(txtHopFrequency.Text);
                }
                
                if (modulationType == ModulationType.ASK)
                {
                    parameters.HopAmplitude = double.Parse(txtHopAmplitude.Text);
                }
                
                if (modulationType == ModulationType.PSK)
                {
                    parameters.HopPhase = double.Parse(txtHopPhase.Text);
                }
                
                // Disable button during operation
                btnConfigureModulation.Enabled = false;
                
                // Configure modulation
                var result = await _service.ConfigureModulationAsync(channel, modulationType, parameters);
                
                if (result.Success)
                {
                    MessageBox.Show($"Modulation configured successfully on Channel {channel}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to configure modulation: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid numeric input. Please check all values.", 
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConfigureModulation.Enabled = true;
            }
        }

        private async void chkModulationEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!_service.IsConnected)
                return;
            
            try
            {
                int channel = cmbModChannel.SelectedIndex + 1;
                var result = await _service.SetModulationStateAsync(channel, chkModulationEnable.Checked);
                
                if (!result.Success)
                {
                    // Revert checkbox state on failure
                    chkModulationEnable.CheckedChanged -= chkModulationEnable_CheckedChanged;
                    chkModulationEnable.Checked = !chkModulationEnable.Checked;
                    chkModulationEnable.CheckedChanged += chkModulationEnable_CheckedChanged;
                    
                    MessageBox.Show($"Failed to set modulation state: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Modulation validation event handlers
        private void txtDepth_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtDepth.Text, out double depth) || depth < 0 || depth > 120)
            {
                _errorProvider.SetError(txtDepth, "Depth must be between 0 and 120%");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtDepth, string.Empty);
            }
        }

        private void txtDeviation_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtDeviation.Text, out double deviation) || deviation <= 0)
            {
                _errorProvider.SetError(txtDeviation, "Deviation must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtDeviation, string.Empty);
            }
        }

        private void txtRate_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtRate.Text, out double rate) || rate <= 0)
            {
                _errorProvider.SetError(txtRate, "Rate must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtRate, string.Empty);
            }
        }

        private void txtHopFrequency_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtHopFrequency.Text, out double freq) || freq <= 0)
            {
                _errorProvider.SetError(txtHopFrequency, "Hop frequency must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtHopFrequency, string.Empty);
            }
        }

        private void txtHopAmplitude_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtHopAmplitude.Text, out double amp) || amp <= 0)
            {
                _errorProvider.SetError(txtHopAmplitude, "Hop amplitude must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtHopAmplitude, string.Empty);
            }
        }

        private void txtHopPhase_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtHopPhase.Text, out double phase) || phase < 0 || phase > 360)
            {
                _errorProvider.SetError(txtHopPhase, "Hop phase must be between 0 and 360 degrees");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtHopPhase, string.Empty);
            }
        }

        private void InitializeSweepControls()
        {
            // Initialize sweep channel selector
            cmbSweepChannel.SelectedIndex = 0; // Default to Channel 1
            
            // Initialize sweep type dropdown
            cmbSweepType.Items.Clear();
            foreach (SweepType type in Enum.GetValues(typeof(SweepType)))
            {
                cmbSweepType.Items.Add(type);
            }
            cmbSweepType.SelectedIndex = 0; // Default to Linear
            
            // Initialize sweep direction dropdown
            cmbSweepDirection.Items.Clear();
            foreach (SweepDirection direction in Enum.GetValues(typeof(SweepDirection)))
            {
                cmbSweepDirection.Items.Add(direction);
            }
            cmbSweepDirection.SelectedIndex = 0; // Default to Up
            
            // Initialize trigger source dropdown
            cmbSweepTriggerSource.Items.Clear();
            foreach (TriggerSource source in Enum.GetValues(typeof(TriggerSource)))
            {
                cmbSweepTriggerSource.Items.Add(source);
            }
            cmbSweepTriggerSource.SelectedIndex = 0; // Default to Internal
            
            // Initialize burst controls
            InitializeBurstControls();
        }

        private void InitializeBurstControls()
        {
            // Initialize burst channel selector
            cmbBurstChannel.SelectedIndex = 0; // Default to Channel 1
            
            // Initialize burst mode dropdown
            cmbBurstMode.Items.Clear();
            foreach (BurstMode mode in Enum.GetValues(typeof(BurstMode)))
            {
                cmbBurstMode.Items.Add(mode);
            }
            cmbBurstMode.SelectedIndex = 0; // Default to NCycle
            
            // Initialize burst trigger source dropdown
            cmbBurstTriggerSource.Items.Clear();
            foreach (TriggerSource source in Enum.GetValues(typeof(TriggerSource)))
            {
                cmbBurstTriggerSource.Items.Add(source);
            }
            cmbBurstTriggerSource.SelectedIndex = 0; // Default to Internal
            
            // Initialize trigger edge dropdown
            cmbTriggerEdge.Items.Clear();
            foreach (TriggerEdge edge in Enum.GetValues(typeof(TriggerEdge)))
            {
                cmbTriggerEdge.Items.Add(edge);
            }
            cmbTriggerEdge.SelectedIndex = 0; // Default to Rising
            
            // Initialize gate polarity dropdown
            cmbGatePolarity.Items.Clear();
            foreach (GatePolarity polarity in Enum.GetValues(typeof(GatePolarity)))
            {
                cmbGatePolarity.Items.Add(polarity);
            }
            cmbGatePolarity.SelectedIndex = 0; // Default to Positive
            
            // Initialize arbitrary waveform controls
            InitializeArbitraryControls();
        }

        private void InitializeArbitraryControls()
        {
            // Initialize arbitrary channel selector
            cmbArbitraryChannel.SelectedIndex = 0; // Default to Channel 1
        }

        private async void btnConfigureSweep_Click(object sender, EventArgs e)
        {
            // Validate all inputs first
            if (!ValidateChildren(ValidationConstraints.Enabled))
            {
                MessageBox.Show("Please correct the validation errors before proceeding.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // Get channel number (1 or 2)
                int channel = cmbSweepChannel.SelectedIndex + 1;
                
                // Build sweep parameters
                var parameters = new SweepParameters
                {
                    StartFrequency = double.Parse(txtStartFrequency.Text),
                    StopFrequency = double.Parse(txtStopFrequency.Text),
                    Time = double.Parse(txtSweepTime.Text),
                    Type = (SweepType)cmbSweepType.SelectedItem,
                    Direction = (SweepDirection)cmbSweepDirection.SelectedItem,
                    TriggerSource = (TriggerSource)cmbSweepTriggerSource.SelectedItem,
                    ReturnTime = double.Parse(txtReturnTime.Text),
                    HoldTime = double.Parse(txtHoldTime.Text)
                };
                
                // Disable button during operation
                btnConfigureSweep.Enabled = false;
                
                // Configure sweep
                var result = await _service.ConfigureSweepAsync(channel, parameters);
                
                if (result.Success)
                {
                    MessageBox.Show($"Sweep configured successfully on Channel {channel}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to configure sweep: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid numeric input. Please check all values.", 
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConfigureSweep.Enabled = true;
            }
        }

        private async void chkSweepEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!_service.IsConnected)
                return;
            
            try
            {
                int channel = cmbSweepChannel.SelectedIndex + 1;
                var result = await _service.SetSweepStateAsync(channel, chkSweepEnable.Checked);
                
                if (!result.Success)
                {
                    // Revert checkbox state on failure
                    chkSweepEnable.CheckedChanged -= chkSweepEnable_CheckedChanged;
                    chkSweepEnable.Checked = !chkSweepEnable.Checked;
                    chkSweepEnable.CheckedChanged += chkSweepEnable_CheckedChanged;
                    
                    MessageBox.Show($"Failed to set sweep state: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sweep validation event handlers
        private void txtStartFrequency_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtStartFrequency.Text, out double freq) || freq <= 0)
            {
                _errorProvider.SetError(txtStartFrequency, "Start frequency must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtStartFrequency, string.Empty);
            }
        }

        private void txtStopFrequency_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtStopFrequency.Text, out double freq) || freq <= 0)
            {
                _errorProvider.SetError(txtStopFrequency, "Stop frequency must be a positive number");
                e.Cancel = true;
            }
            else if (double.TryParse(txtStartFrequency.Text, out double startFreq) && freq <= startFreq)
            {
                _errorProvider.SetError(txtStopFrequency, "Stop frequency must be greater than start frequency");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtStopFrequency, string.Empty);
            }
        }

        private void txtSweepTime_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtSweepTime.Text, out double time) || time <= 0)
            {
                _errorProvider.SetError(txtSweepTime, "Sweep time must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtSweepTime, string.Empty);
            }
        }

        private void txtReturnTime_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtReturnTime.Text, out double time) || time < 0)
            {
                _errorProvider.SetError(txtReturnTime, "Return time must be a non-negative number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtReturnTime, string.Empty);
            }
        }

        private void txtHoldTime_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtHoldTime.Text, out double time) || time < 0)
            {
                _errorProvider.SetError(txtHoldTime, "Hold time must be a non-negative number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtHoldTime, string.Empty);
            }
        }

        // Burst event handlers
        private void cmbBurstMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show/hide controls based on burst mode
            BurstMode selectedMode = (BurstMode)cmbBurstMode.SelectedItem;
            
            // Cycles is visible for N-Cycle mode
            bool isNCycle = (selectedMode == BurstMode.NCycle);
            lblCycles.Visible = isNCycle;
            txtCycles.Visible = isNCycle;
            
            // Gate polarity is visible for Gated mode
            bool isGated = (selectedMode == BurstMode.Gated);
            lblGatePolarity.Visible = isGated;
            cmbGatePolarity.Visible = isGated;
        }

        private async void btnConfigureBurst_Click(object sender, EventArgs e)
        {
            // Validate all inputs first
            if (!ValidateChildren(ValidationConstraints.Enabled))
            {
                MessageBox.Show("Please correct the validation errors before proceeding.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // Get channel number (1 or 2)
                int channel = cmbBurstChannel.SelectedIndex + 1;
                
                // Build burst parameters
                var parameters = new BurstParameters
                {
                    Mode = (BurstMode)cmbBurstMode.SelectedItem,
                    Period = double.Parse(txtPeriod.Text),
                    TriggerSource = (TriggerSource)cmbBurstTriggerSource.SelectedItem,
                    TriggerEdge = (TriggerEdge)cmbTriggerEdge.SelectedItem,
                    StartPhase = double.Parse(txtStartPhase.Text),
                    GatePolarity = (GatePolarity)cmbGatePolarity.SelectedItem
                };
                
                // Add mode-specific parameters
                if (parameters.Mode == BurstMode.NCycle)
                {
                    parameters.Cycles = int.Parse(txtCycles.Text);
                }
                
                // Disable button during operation
                btnConfigureBurst.Enabled = false;
                
                // Configure burst
                var result = await _service.ConfigureBurstAsync(channel, parameters);
                
                if (result.Success)
                {
                    MessageBox.Show($"Burst configured successfully on Channel {channel}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to configure burst: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid numeric input. Please check all values.", 
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConfigureBurst.Enabled = true;
            }
        }

        private async void chkBurstEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!_service.IsConnected)
                return;
            
            try
            {
                int channel = cmbBurstChannel.SelectedIndex + 1;
                var result = await _service.SetBurstStateAsync(channel, chkBurstEnable.Checked);
                
                if (!result.Success)
                {
                    // Revert checkbox state on failure
                    chkBurstEnable.CheckedChanged -= chkBurstEnable_CheckedChanged;
                    chkBurstEnable.Checked = !chkBurstEnable.Checked;
                    chkBurstEnable.CheckedChanged += chkBurstEnable_CheckedChanged;
                    
                    MessageBox.Show($"Failed to set burst state: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Burst validation event handlers
        private void txtCycles_Validating(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(txtCycles.Text, out int cycles) || cycles < 1 || cycles > 1000000)
            {
                _errorProvider.SetError(txtCycles, "Cycles must be between 1 and 1000000");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtCycles, string.Empty);
            }
        }

        private void txtPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtPeriod.Text, out double period) || period <= 0)
            {
                _errorProvider.SetError(txtPeriod, "Period must be a positive number");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtPeriod, string.Empty);
            }
        }

        private void txtStartPhase_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(txtStartPhase.Text, out double phase) || phase < 0 || phase > 360)
            {
                _errorProvider.SetError(txtStartPhase, "Start phase must be between 0 and 360 degrees");
                e.Cancel = true;
            }
            else
            {
                _errorProvider.SetError(txtStartPhase, string.Empty);
            }
        }

        // Arbitrary waveform event handlers
        private async void btnRefreshList_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefreshList.Enabled = false;
                
                // Get list of arbitrary waveforms from device
                var waveforms = await _service.GetArbitraryWaveformListAsync();
                
                // Update list box
                lstWaveforms.Items.Clear();
                foreach (var waveform in waveforms)
                {
                    lstWaveforms.Items.Add(waveform);
                }
                
                MessageBox.Show($"Found {waveforms.Count} waveforms", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing waveform list: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefreshList.Enabled = true;
            }
        }

        private async void btnUploadWaveform_Click(object sender, EventArgs e)
        {
            // Validate waveform name
            if (string.IsNullOrWhiteSpace(txtWaveformName.Text))
            {
                MessageBox.Show("Please enter a waveform name.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Open file dialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv|Binary Files (*.bin)|*.bin|All Files (*.*)|*.*";
                openFileDialog.Title = "Select Waveform File";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        btnUploadWaveform.Enabled = false;
                        progressBar.Value = 0;
                        progressBar.Visible = true;
                        
                        // Parse waveform file
                        double[] points = ParseWaveformFile(openFileDialog.FileName);
                        
                        if (points == null || points.Length == 0)
                        {
                            MessageBox.Show("Failed to parse waveform file or file is empty.", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        
                        progressBar.Value = 50;
                        
                        // Upload to device
                        var result = await _service.UploadArbitraryWaveformAsync(txtWaveformName.Text, points);
                        
                        progressBar.Value = 100;
                        
                        if (result.Success)
                        {
                            MessageBox.Show($"Waveform '{txtWaveformName.Text}' uploaded successfully ({points.Length} points)", 
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Refresh list
                            btnRefreshList_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show($"Failed to upload waveform: {result.Message}", 
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error uploading waveform: {ex.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnUploadWaveform.Enabled = true;
                        progressBar.Visible = false;
                        progressBar.Value = 0;
                    }
                }
            }
        }

        private async void btnDeleteWaveform_Click(object sender, EventArgs e)
        {
            if (lstWaveforms.SelectedItem == null)
            {
                MessageBox.Show("Please select a waveform to delete.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string waveformName = lstWaveforms.SelectedItem.ToString();
            
            // Confirmation dialog
            var confirmResult = MessageBox.Show($"Are you sure you want to delete waveform '{waveformName}'?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    btnDeleteWaveform.Enabled = false;
                    
                    var result = await _service.DeleteArbitraryWaveformAsync(waveformName);
                    
                    if (result.Success)
                    {
                        MessageBox.Show($"Waveform '{waveformName}' deleted successfully", 
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Refresh list
                        btnRefreshList_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete waveform: {result.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting waveform: {ex.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnDeleteWaveform.Enabled = true;
                }
            }
        }

        private async void btnSelectWaveform_Click(object sender, EventArgs e)
        {
            if (lstWaveforms.SelectedItem == null)
            {
                MessageBox.Show("Please select a waveform to assign to the channel.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                int channel = cmbArbitraryChannel.SelectedIndex + 1;
                string waveformName = lstWaveforms.SelectedItem.ToString();
                
                btnSelectWaveform.Enabled = false;
                
                var result = await _service.SelectArbitraryWaveformAsync(channel, waveformName);
                
                if (result.Success)
                {
                    MessageBox.Show($"Waveform '{waveformName}' selected for Channel {channel}", 
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to select waveform: {result.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting waveform: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSelectWaveform.Enabled = true;
            }
        }

        private double[] ParseWaveformFile(string filePath)
        {
            try
            {
                string extension = System.IO.Path.GetExtension(filePath).ToLower();
                
                if (extension == ".csv")
                {
                    // Parse CSV file (one value per line or comma-separated)
                    string[] lines = System.IO.File.ReadAllLines(filePath);
                    var points = new System.Collections.Generic.List<double>();
                    
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        
                        // Try comma-separated values first
                        string[] values = line.Split(',');
                        foreach (string value in values)
                        {
                            if (double.TryParse(value.Trim(), out double point))
                            {
                                points.Add(point);
                            }
                        }
                    }
                    
                    return points.ToArray();
                }
                else if (extension == ".bin")
                {
                    // Parse binary file (assume double precision floats)
                    byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                    int doubleCount = bytes.Length / sizeof(double);
                    double[] points = new double[doubleCount];
                    
                    for (int i = 0; i < doubleCount; i++)
                    {
                        points[i] = BitConverter.ToDouble(bytes, i * sizeof(double));
                    }
                    
                    return points;
                }
                else
                {
                    // Try to parse as text file with one value per line
                    string[] lines = System.IO.File.ReadAllLines(filePath);
                    var points = new System.Collections.Generic.List<double>();
                    
                    foreach (string line in lines)
                    {
                        if (double.TryParse(line.Trim(), out double point))
                        {
                            points.Add(point);
                        }
                    }
                    
                    return points.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing waveform file: {ex.Message}", 
                    "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
