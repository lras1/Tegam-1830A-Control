namespace Siglent.SDG6052X.WinFormsUI.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.lblDeviceInfo = new System.Windows.Forms.Label();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabWaveform = new System.Windows.Forms.TabPage();
            this.grpWaveformConfig = new System.Windows.Forms.GroupBox();
            this.grpModulationConfig = new System.Windows.Forms.GroupBox();
            this.lblModChannel = new System.Windows.Forms.Label();
            this.cmbModChannel = new System.Windows.Forms.ComboBox();
            this.lblModulationType = new System.Windows.Forms.Label();
            this.cmbModulationType = new System.Windows.Forms.ComboBox();
            this.lblModulationSource = new System.Windows.Forms.Label();
            this.cmbModulationSource = new System.Windows.Forms.ComboBox();
            this.lblModulationWaveform = new System.Windows.Forms.Label();
            this.cmbModulationWaveform = new System.Windows.Forms.ComboBox();
            this.lblDepth = new System.Windows.Forms.Label();
            this.txtDepth = new System.Windows.Forms.TextBox();
            this.lblDepthUnit = new System.Windows.Forms.Label();
            this.lblDeviation = new System.Windows.Forms.Label();
            this.txtDeviation = new System.Windows.Forms.TextBox();
            this.lblDeviationUnit = new System.Windows.Forms.Label();
            this.lblRate = new System.Windows.Forms.Label();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.lblRateUnit = new System.Windows.Forms.Label();
            this.lblHopFrequency = new System.Windows.Forms.Label();
            this.txtHopFrequency = new System.Windows.Forms.TextBox();
            this.lblHopFrequencyUnit = new System.Windows.Forms.Label();
            this.lblHopAmplitude = new System.Windows.Forms.Label();
            this.txtHopAmplitude = new System.Windows.Forms.TextBox();
            this.lblHopAmplitudeUnit = new System.Windows.Forms.Label();
            this.lblHopPhase = new System.Windows.Forms.Label();
            this.txtHopPhase = new System.Windows.Forms.TextBox();
            this.lblHopPhaseUnit = new System.Windows.Forms.Label();
            this.chkModulationEnable = new System.Windows.Forms.CheckBox();
            this.btnConfigureModulation = new System.Windows.Forms.Button();
            this.grpSweepConfig = new System.Windows.Forms.GroupBox();
            this.grpBurstConfig = new System.Windows.Forms.GroupBox();
            this.grpArbitraryConfig = new System.Windows.Forms.GroupBox();
            this.lblSweepChannel = new System.Windows.Forms.Label();
            this.cmbSweepChannel = new System.Windows.Forms.ComboBox();
            this.lblStartFrequency = new System.Windows.Forms.Label();
            this.txtStartFrequency = new System.Windows.Forms.TextBox();
            this.lblStartFrequencyUnit = new System.Windows.Forms.Label();
            this.lblStopFrequency = new System.Windows.Forms.Label();
            this.txtStopFrequency = new System.Windows.Forms.TextBox();
            this.lblStopFrequencyUnit = new System.Windows.Forms.Label();
            this.lblSweepTime = new System.Windows.Forms.Label();
            this.txtSweepTime = new System.Windows.Forms.TextBox();
            this.lblSweepTimeUnit = new System.Windows.Forms.Label();
            this.lblSweepType = new System.Windows.Forms.Label();
            this.cmbSweepType = new System.Windows.Forms.ComboBox();
            this.lblSweepDirection = new System.Windows.Forms.Label();
            this.cmbSweepDirection = new System.Windows.Forms.ComboBox();
            this.lblSweepTriggerSource = new System.Windows.Forms.Label();
            this.cmbSweepTriggerSource = new System.Windows.Forms.ComboBox();
            this.lblReturnTime = new System.Windows.Forms.Label();
            this.txtReturnTime = new System.Windows.Forms.TextBox();
            this.lblReturnTimeUnit = new System.Windows.Forms.Label();
            this.lblHoldTime = new System.Windows.Forms.Label();
            this.txtHoldTime = new System.Windows.Forms.TextBox();
            this.lblHoldTimeUnit = new System.Windows.Forms.Label();
            this.chkSweepEnable = new System.Windows.Forms.CheckBox();
            this.btnConfigureSweep = new System.Windows.Forms.Button();
            this.lblBurstChannel = new System.Windows.Forms.Label();
            this.cmbBurstChannel = new System.Windows.Forms.ComboBox();
            this.lblBurstMode = new System.Windows.Forms.Label();
            this.cmbBurstMode = new System.Windows.Forms.ComboBox();
            this.lblCycles = new System.Windows.Forms.Label();
            this.txtCycles = new System.Windows.Forms.TextBox();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.txtPeriod = new System.Windows.Forms.TextBox();
            this.lblPeriodUnit = new System.Windows.Forms.Label();
            this.lblBurstTriggerSource = new System.Windows.Forms.Label();
            this.cmbBurstTriggerSource = new System.Windows.Forms.ComboBox();
            this.lblTriggerEdge = new System.Windows.Forms.Label();
            this.cmbTriggerEdge = new System.Windows.Forms.ComboBox();
            this.lblStartPhase = new System.Windows.Forms.Label();
            this.txtStartPhase = new System.Windows.Forms.TextBox();
            this.lblStartPhaseUnit = new System.Windows.Forms.Label();
            this.lblGatePolarity = new System.Windows.Forms.Label();
            this.cmbGatePolarity = new System.Windows.Forms.ComboBox();
            this.chkBurstEnable = new System.Windows.Forms.CheckBox();
            this.btnConfigureBurst = new System.Windows.Forms.Button();
            this.lblArbitraryChannel = new System.Windows.Forms.Label();
            this.cmbArbitraryChannel = new System.Windows.Forms.ComboBox();
            this.lstWaveforms = new System.Windows.Forms.ListBox();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.btnUploadWaveform = new System.Windows.Forms.Button();
            this.btnDeleteWaveform = new System.Windows.Forms.Button();
            this.btnSelectWaveform = new System.Windows.Forms.Button();
            this.lblWaveformName = new System.Windows.Forms.Label();
            this.txtWaveformName = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.txtReturnTime = new System.Windows.Forms.TextBox();
            this.lblReturnTimeUnit = new System.Windows.Forms.Label();
            this.lblHoldTime = new System.Windows.Forms.Label();
            this.txtHoldTime = new System.Windows.Forms.TextBox();
            this.lblHoldTimeUnit = new System.Windows.Forms.Label();
            this.chkSweepEnable = new System.Windows.Forms.CheckBox();
            this.btnConfigureSweep = new System.Windows.Forms.Button();
            this.btnQueryState = new System.Windows.Forms.Button();
            this.btnSetWaveform = new System.Windows.Forms.Button();
            this.chkOutputEnable = new System.Windows.Forms.CheckBox();
            this.cmbLoadImpedance = new System.Windows.Forms.ComboBox();
            this.lblLoadImpedance = new System.Windows.Forms.Label();
            this.lblFallTimeUnit = new System.Windows.Forms.Label();
            this.txtFallTime = new System.Windows.Forms.TextBox();
            this.lblFallTime = new System.Windows.Forms.Label();
            this.lblRiseTimeUnit = new System.Windows.Forms.Label();
            this.txtRiseTime = new System.Windows.Forms.TextBox();
            this.lblRiseTime = new System.Windows.Forms.Label();
            this.lblPulseWidthUnit = new System.Windows.Forms.Label();
            this.txtPulseWidth = new System.Windows.Forms.TextBox();
            this.lblPulseWidth = new System.Windows.Forms.Label();
            this.lblDutyCycleUnit = new System.Windows.Forms.Label();
            this.txtDutyCycle = new System.Windows.Forms.TextBox();
            this.lblDutyCycle = new System.Windows.Forms.Label();
            this.lblPhaseUnit = new System.Windows.Forms.Label();
            this.txtPhase = new System.Windows.Forms.TextBox();
            this.lblPhase = new System.Windows.Forms.Label();
            this.lblOffsetUnit = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.lblOffset = new System.Windows.Forms.Label();
            this.cmbAmplitudeUnit = new System.Windows.Forms.ComboBox();
            this.txtAmplitude = new System.Windows.Forms.TextBox();
            this.lblAmplitude = new System.Windows.Forms.Label();
            this.lblFrequencyUnit = new System.Windows.Forms.Label();
            this.txtFrequency = new System.Windows.Forms.TextBox();
            this.lblFrequency = new System.Windows.Forms.Label();
            this.cmbWaveformType = new System.Windows.Forms.ComboBox();
            this.lblWaveformType = new System.Windows.Forms.Label();
            this.cmbChannel = new System.Windows.Forms.ComboBox();
            this.lblChannel = new System.Windows.Forms.Label();
            this.tabModulation = new System.Windows.Forms.TabPage();
            this.tabSweep = new System.Windows.Forms.TabPage();
            this.tabBurst = new System.Windows.Forms.TabPage();
            this.tabArbitrary = new System.Windows.Forms.TabPage();
            this.grpConnection.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabWaveform.SuspendLayout();
            this.grpWaveformConfig.SuspendLayout();
            this.tabModulation.SuspendLayout();
            this.grpModulationConfig.SuspendLayout();
            this.tabSweep.SuspendLayout();
            this.grpSweepConfig.SuspendLayout();
            this.tabBurst.SuspendLayout();
            this.grpBurstConfig.SuspendLayout();
            this.tabArbitrary.SuspendLayout();
            this.grpArbitraryConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.lblDeviceInfo);
            this.grpConnection.Controls.Add(this.lblConnectionStatus);
            this.grpConnection.Controls.Add(this.btnDisconnect);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.txtIpAddress);
            this.grpConnection.Controls.Add(this.lblIpAddress);
            this.grpConnection.Location = new System.Drawing.Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(760, 120);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Device Connection";
            // 
            // lblDeviceInfo
            // 
            this.lblDeviceInfo.AutoSize = true;
            this.lblDeviceInfo.Location = new System.Drawing.Point(400, 25);
            this.lblDeviceInfo.Name = "lblDeviceInfo";
            this.lblDeviceInfo.Size = new System.Drawing.Size(110, 13);
            this.lblDeviceInfo.TabIndex = 5;
            this.lblDeviceInfo.Text = "No device connected";
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionStatus.ForeColor = System.Drawing.Color.Red;
            this.lblConnectionStatus.Location = new System.Drawing.Point(20, 85);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(102, 16);
            this.lblConnectionStatus.TabIndex = 4;
            this.lblConnectionStatus.Text = "Disconnected";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(290, 50);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(90, 25);
            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(290, 20);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(90, 25);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Location = new System.Drawing.Point(90, 22);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(180, 20);
            this.txtIpAddress.TabIndex = 1;
            this.txtIpAddress.Text = "192.168.1.100";
            // 
            // lblIpAddress
            // 
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Location = new System.Drawing.Point(20, 25);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Size = new System.Drawing.Size(61, 13);
            this.lblIpAddress.TabIndex = 0;
            this.lblIpAddress.Text = "IP Address:";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabWaveform);
            this.tabControl.Controls.Add(this.tabModulation);
            this.tabControl.Controls.Add(this.tabSweep);
            this.tabControl.Controls.Add(this.tabBurst);
            this.tabControl.Controls.Add(this.tabArbitrary);
            this.tabControl.Enabled = false;
            this.tabControl.Location = new System.Drawing.Point(12, 138);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(760, 400);
            this.tabControl.TabIndex = 1;
            // 
            // tabWaveform
            // 
            this.tabWaveform.Controls.Add(this.grpWaveformConfig);
            this.tabWaveform.Location = new System.Drawing.Point(4, 22);
            this.tabWaveform.Name = "tabWaveform";
            this.tabWaveform.Padding = new System.Windows.Forms.Padding(3);
            this.tabWaveform.Size = new System.Drawing.Size(752, 374);
            this.tabWaveform.TabIndex = 0;
            this.tabWaveform.Text = "Waveform";
            this.tabWaveform.UseVisualStyleBackColor = true;
            // 
            // grpWaveformConfig
            // 
            this.grpWaveformConfig.Controls.Add(this.btnQueryState);
            this.grpWaveformConfig.Controls.Add(this.btnSetWaveform);
            this.grpWaveformConfig.Controls.Add(this.chkOutputEnable);
            this.grpWaveformConfig.Controls.Add(this.cmbLoadImpedance);
            this.grpWaveformConfig.Controls.Add(this.lblLoadImpedance);
            this.grpWaveformConfig.Controls.Add(this.lblFallTimeUnit);
            this.grpWaveformConfig.Controls.Add(this.txtFallTime);
            this.grpWaveformConfig.Controls.Add(this.lblFallTime);
            this.grpWaveformConfig.Controls.Add(this.lblRiseTimeUnit);
            this.grpWaveformConfig.Controls.Add(this.txtRiseTime);
            this.grpWaveformConfig.Controls.Add(this.lblRiseTime);
            this.grpWaveformConfig.Controls.Add(this.lblPulseWidthUnit);
            this.grpWaveformConfig.Controls.Add(this.txtPulseWidth);
            this.grpWaveformConfig.Controls.Add(this.lblPulseWidth);
            this.grpWaveformConfig.Controls.Add(this.lblDutyCycleUnit);
            this.grpWaveformConfig.Controls.Add(this.txtDutyCycle);
            this.grpWaveformConfig.Controls.Add(this.lblDutyCycle);
            this.grpWaveformConfig.Controls.Add(this.lblPhaseUnit);
            this.grpWaveformConfig.Controls.Add(this.txtPhase);
            this.grpWaveformConfig.Controls.Add(this.lblPhase);
            this.grpWaveformConfig.Controls.Add(this.lblOffsetUnit);
            this.grpWaveformConfig.Controls.Add(this.txtOffset);
            this.grpWaveformConfig.Controls.Add(this.lblOffset);
            this.grpWaveformConfig.Controls.Add(this.cmbAmplitudeUnit);
            this.grpWaveformConfig.Controls.Add(this.txtAmplitude);
            this.grpWaveformConfig.Controls.Add(this.lblAmplitude);
            this.grpWaveformConfig.Controls.Add(this.lblFrequencyUnit);
            this.grpWaveformConfig.Controls.Add(this.txtFrequency);
            this.grpWaveformConfig.Controls.Add(this.lblFrequency);
            this.grpWaveformConfig.Controls.Add(this.cmbWaveformType);
            this.grpWaveformConfig.Controls.Add(this.lblWaveformType);
            this.grpWaveformConfig.Controls.Add(this.cmbChannel);
            this.grpWaveformConfig.Controls.Add(this.lblChannel);
            this.grpWaveformConfig.Location = new System.Drawing.Point(10, 10);
            this.grpWaveformConfig.Name = "grpWaveformConfig";
            this.grpWaveformConfig.Size = new System.Drawing.Size(730, 350);
            this.grpWaveformConfig.TabIndex = 0;
            this.grpWaveformConfig.TabStop = false;
            this.grpWaveformConfig.Text = "Waveform Configuration";
            // 
            // lblChannel
            // 
            this.lblChannel.AutoSize = true;
            this.lblChannel.Location = new System.Drawing.Point(20, 30);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(49, 13);
            this.lblChannel.TabIndex = 0;
            this.lblChannel.Text = "Channel:";
            // 
            // cmbChannel
            // 
            this.cmbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannel.FormattingEnabled = true;
            this.cmbChannel.Items.AddRange(new object[] {
            "Channel 1",
            "Channel 2"});
            this.cmbChannel.Location = new System.Drawing.Point(120, 27);
            this.cmbChannel.Name = "cmbChannel";
            this.cmbChannel.Size = new System.Drawing.Size(120, 21);
            this.cmbChannel.TabIndex = 1;
            // 
            // lblWaveformType
            // 
            this.lblWaveformType.AutoSize = true;
            this.lblWaveformType.Location = new System.Drawing.Point(20, 60);
            this.lblWaveformType.Name = "lblWaveformType";
            this.lblWaveformType.Size = new System.Drawing.Size(83, 13);
            this.lblWaveformType.TabIndex = 2;
            this.lblWaveformType.Text = "Waveform Type:";
            // 
            // cmbWaveformType
            // 
            this.cmbWaveformType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaveformType.FormattingEnabled = true;
            this.cmbWaveformType.Location = new System.Drawing.Point(120, 57);
            this.cmbWaveformType.Name = "cmbWaveformType";
            this.cmbWaveformType.Size = new System.Drawing.Size(120, 21);
            this.cmbWaveformType.TabIndex = 3;
            this.cmbWaveformType.SelectedIndexChanged += new System.EventHandler(this.cmbWaveformType_SelectedIndexChanged);
            // 
            // lblFrequency
            // 
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.Location = new System.Drawing.Point(20, 95);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(60, 13);
            this.lblFrequency.TabIndex = 4;
            this.lblFrequency.Text = "Frequency:";
            // 
            // txtFrequency
            // 
            this.txtFrequency.Location = new System.Drawing.Point(120, 92);
            this.txtFrequency.Name = "txtFrequency";
            this.txtFrequency.Size = new System.Drawing.Size(120, 20);
            this.txtFrequency.TabIndex = 5;
            this.txtFrequency.Text = "1000";
            this.txtFrequency.Validating += new System.ComponentModel.CancelEventHandler(this.txtFrequency_Validating);
            // 
            // lblFrequencyUnit
            // 
            this.lblFrequencyUnit.AutoSize = true;
            this.lblFrequencyUnit.Location = new System.Drawing.Point(246, 95);
            this.lblFrequencyUnit.Name = "lblFrequencyUnit";
            this.lblFrequencyUnit.Size = new System.Drawing.Size(20, 13);
            this.lblFrequencyUnit.TabIndex = 6;
            this.lblFrequencyUnit.Text = "Hz";
            // 
            // lblAmplitude
            // 
            this.lblAmplitude.AutoSize = true;
            this.lblAmplitude.Location = new System.Drawing.Point(20, 125);
            this.lblAmplitude.Name = "lblAmplitude";
            this.lblAmplitude.Size = new System.Drawing.Size(56, 13);
            this.lblAmplitude.TabIndex = 7;
            this.lblAmplitude.Text = "Amplitude:";
            // 
            // txtAmplitude
            // 
            this.txtAmplitude.Location = new System.Drawing.Point(120, 122);
            this.txtAmplitude.Name = "txtAmplitude";
            this.txtAmplitude.Size = new System.Drawing.Size(120, 20);
            this.txtAmplitude.TabIndex = 8;
            this.txtAmplitude.Text = "5.0";
            this.txtAmplitude.Validating += new System.ComponentModel.CancelEventHandler(this.txtAmplitude_Validating);
            // 
            // cmbAmplitudeUnit
            // 
            this.cmbAmplitudeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAmplitudeUnit.FormattingEnabled = true;
            this.cmbAmplitudeUnit.Location = new System.Drawing.Point(246, 122);
            this.cmbAmplitudeUnit.Name = "cmbAmplitudeUnit";
            this.cmbAmplitudeUnit.Size = new System.Drawing.Size(70, 21);
            this.cmbAmplitudeUnit.TabIndex = 9;
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(20, 155);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(38, 13);
            this.lblOffset.TabIndex = 10;
            this.lblOffset.Text = "Offset:";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(120, 152);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(120, 20);
            this.txtOffset.TabIndex = 11;
            this.txtOffset.Text = "0.0";
            this.txtOffset.Validating += new System.ComponentModel.CancelEventHandler(this.txtOffset_Validating);
            // 
            // lblOffsetUnit
            // 
            this.lblOffsetUnit.AutoSize = true;
            this.lblOffsetUnit.Location = new System.Drawing.Point(246, 155);
            this.lblOffsetUnit.Name = "lblOffsetUnit";
            this.lblOffsetUnit.Size = new System.Drawing.Size(14, 13);
            this.lblOffsetUnit.TabIndex = 12;
            this.lblOffsetUnit.Text = "V";
            // 
            // lblPhase
            // 
            this.lblPhase.AutoSize = true;
            this.lblPhase.Location = new System.Drawing.Point(20, 185);
            this.lblPhase.Name = "lblPhase";
            this.lblPhase.Size = new System.Drawing.Size(40, 13);
            this.lblPhase.TabIndex = 13;
            this.lblPhase.Text = "Phase:";
            // 
            // txtPhase
            // 
            this.txtPhase.Location = new System.Drawing.Point(120, 182);
            this.txtPhase.Name = "txtPhase";
            this.txtPhase.Size = new System.Drawing.Size(120, 20);
            this.txtPhase.TabIndex = 14;
            this.txtPhase.Text = "0.0";
            this.txtPhase.Validating += new System.ComponentModel.CancelEventHandler(this.txtPhase_Validating);
            // 
            // lblPhaseUnit
            // 
            this.lblPhaseUnit.AutoSize = true;
            this.lblPhaseUnit.Location = new System.Drawing.Point(246, 185);
            this.lblPhaseUnit.Name = "lblPhaseUnit";
            this.lblPhaseUnit.Size = new System.Drawing.Size(47, 13);
            this.lblPhaseUnit.TabIndex = 15;
            this.lblPhaseUnit.Text = "Degrees";
            // 
            // lblDutyCycle
            // 
            this.lblDutyCycle.AutoSize = true;
            this.lblDutyCycle.Location = new System.Drawing.Point(380, 95);
            this.lblDutyCycle.Name = "lblDutyCycle";
            this.lblDutyCycle.Size = new System.Drawing.Size(62, 13);
            this.lblDutyCycle.TabIndex = 16;
            this.lblDutyCycle.Text = "Duty Cycle:";
            this.lblDutyCycle.Visible = false;
            // 
            // txtDutyCycle
            // 
            this.txtDutyCycle.Location = new System.Drawing.Point(480, 92);
            this.txtDutyCycle.Name = "txtDutyCycle";
            this.txtDutyCycle.Size = new System.Drawing.Size(120, 20);
            this.txtDutyCycle.TabIndex = 17;
            this.txtDutyCycle.Text = "50.0";
            this.txtDutyCycle.Visible = false;
            this.txtDutyCycle.Validating += new System.ComponentModel.CancelEventHandler(this.txtDutyCycle_Validating);
            // 
            // lblDutyCycleUnit
            // 
            this.lblDutyCycleUnit.AutoSize = true;
            this.lblDutyCycleUnit.Location = new System.Drawing.Point(606, 95);
            this.lblDutyCycleUnit.Name = "lblDutyCycleUnit";
            this.lblDutyCycleUnit.Size = new System.Drawing.Size(15, 13);
            this.lblDutyCycleUnit.TabIndex = 18;
            this.lblDutyCycleUnit.Text = "%";
            this.lblDutyCycleUnit.Visible = false;
            // 
            // lblPulseWidth
            // 
            this.lblPulseWidth.AutoSize = true;
            this.lblPulseWidth.Location = new System.Drawing.Point(380, 125);
            this.lblPulseWidth.Name = "lblPulseWidth";
            this.lblPulseWidth.Size = new System.Drawing.Size(68, 13);
            this.lblPulseWidth.TabIndex = 19;
            this.lblPulseWidth.Text = "Pulse Width:";
            this.lblPulseWidth.Visible = false;
            // 
            // txtPulseWidth
            // 
            this.txtPulseWidth.Location = new System.Drawing.Point(480, 122);
            this.txtPulseWidth.Name = "txtPulseWidth";
            this.txtPulseWidth.Size = new System.Drawing.Size(120, 20);
            this.txtPulseWidth.TabIndex = 20;
            this.txtPulseWidth.Text = "0.0001";
            this.txtPulseWidth.Visible = false;
            this.txtPulseWidth.Validating += new System.ComponentModel.CancelEventHandler(this.txtPulseWidth_Validating);
            // 
            // lblPulseWidthUnit
            // 
            this.lblPulseWidthUnit.AutoSize = true;
            this.lblPulseWidthUnit.Location = new System.Drawing.Point(606, 125);
            this.lblPulseWidthUnit.Name = "lblPulseWidthUnit";
            this.lblPulseWidthUnit.Size = new System.Drawing.Size(12, 13);
            this.lblPulseWidthUnit.TabIndex = 21;
            this.lblPulseWidthUnit.Text = "s";
            this.lblPulseWidthUnit.Visible = false;
            // 
            // lblRiseTime
            // 
            this.lblRiseTime.AutoSize = true;
            this.lblRiseTime.Location = new System.Drawing.Point(380, 155);
            this.lblRiseTime.Name = "lblRiseTime";
            this.lblRiseTime.Size = new System.Drawing.Size(60, 13);
            this.lblRiseTime.TabIndex = 22;
            this.lblRiseTime.Text = "Rise Time:";
            this.lblRiseTime.Visible = false;
            // 
            // txtRiseTime
            // 
            this.txtRiseTime.Location = new System.Drawing.Point(480, 152);
            this.txtRiseTime.Name = "txtRiseTime";
            this.txtRiseTime.Size = new System.Drawing.Size(120, 20);
            this.txtRiseTime.TabIndex = 23;
            this.txtRiseTime.Text = "0.00001";
            this.txtRiseTime.Visible = false;
            this.txtRiseTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtRiseTime_Validating);
            // 
            // lblRiseTimeUnit
            // 
            this.lblRiseTimeUnit.AutoSize = true;
            this.lblRiseTimeUnit.Location = new System.Drawing.Point(606, 155);
            this.lblRiseTimeUnit.Name = "lblRiseTimeUnit";
            this.lblRiseTimeUnit.Size = new System.Drawing.Size(12, 13);
            this.lblRiseTimeUnit.TabIndex = 24;
            this.lblRiseTimeUnit.Text = "s";
            this.lblRiseTimeUnit.Visible = false;
            // 
            // lblFallTime
            // 
            this.lblFallTime.AutoSize = true;
            this.lblFallTime.Location = new System.Drawing.Point(380, 185);
            this.lblFallTime.Name = "lblFallTime";
            this.lblFallTime.Size = new System.Drawing.Size(54, 13);
            this.lblFallTime.TabIndex = 25;
            this.lblFallTime.Text = "Fall Time:";
            this.lblFallTime.Visible = false;
            // 
            // txtFallTime
            // 
            this.txtFallTime.Location = new System.Drawing.Point(480, 182);
            this.txtFallTime.Name = "txtFallTime";
            this.txtFallTime.Size = new System.Drawing.Size(120, 20);
            this.txtFallTime.TabIndex = 26;
            this.txtFallTime.Text = "0.00001";
            this.txtFallTime.Visible = false;
            this.txtFallTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtFallTime_Validating);
            // 
            // lblFallTimeUnit
            // 
            this.lblFallTimeUnit.AutoSize = true;
            this.lblFallTimeUnit.Location = new System.Drawing.Point(606, 185);
            this.lblFallTimeUnit.Name = "lblFallTimeUnit";
            this.lblFallTimeUnit.Size = new System.Drawing.Size(12, 13);
            this.lblFallTimeUnit.TabIndex = 27;
            this.lblFallTimeUnit.Text = "s";
            this.lblFallTimeUnit.Visible = false;
            // 
            // lblLoadImpedance
            // 
            this.lblLoadImpedance.AutoSize = true;
            this.lblLoadImpedance.Location = new System.Drawing.Point(20, 220);
            this.lblLoadImpedance.Name = "lblLoadImpedance";
            this.lblLoadImpedance.Size = new System.Drawing.Size(91, 13);
            this.lblLoadImpedance.TabIndex = 28;
            this.lblLoadImpedance.Text = "Load Impedance:";
            // 
            // cmbLoadImpedance
            // 
            this.cmbLoadImpedance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLoadImpedance.FormattingEnabled = true;
            this.cmbLoadImpedance.Location = new System.Drawing.Point(120, 217);
            this.cmbLoadImpedance.Name = "cmbLoadImpedance";
            this.cmbLoadImpedance.Size = new System.Drawing.Size(120, 21);
            this.cmbLoadImpedance.TabIndex = 29;
            // 
            // chkOutputEnable
            // 
            this.chkOutputEnable.AutoSize = true;
            this.chkOutputEnable.Location = new System.Drawing.Point(120, 255);
            this.chkOutputEnable.Name = "chkOutputEnable";
            this.chkOutputEnable.Size = new System.Drawing.Size(95, 17);
            this.chkOutputEnable.TabIndex = 30;
            this.chkOutputEnable.Text = "Output Enable";
            this.chkOutputEnable.UseVisualStyleBackColor = true;
            this.chkOutputEnable.CheckedChanged += new System.EventHandler(this.chkOutputEnable_CheckedChanged);
            // 
            // btnSetWaveform
            // 
            this.btnSetWaveform.Location = new System.Drawing.Point(120, 290);
            this.btnSetWaveform.Name = "btnSetWaveform";
            this.btnSetWaveform.Size = new System.Drawing.Size(120, 30);
            this.btnSetWaveform.TabIndex = 31;
            this.btnSetWaveform.Text = "Set Waveform";
            this.btnSetWaveform.UseVisualStyleBackColor = true;
            this.btnSetWaveform.Click += new System.EventHandler(this.btnSetWaveform_Click);
            // 
            // btnQueryState
            // 
            this.btnQueryState.Location = new System.Drawing.Point(260, 290);
            this.btnQueryState.Name = "btnQueryState";
            this.btnQueryState.Size = new System.Drawing.Size(120, 30);
            this.btnQueryState.TabIndex = 32;
            this.btnQueryState.Text = "Query Current State";
            this.btnQueryState.UseVisualStyleBackColor = true;
            this.btnQueryState.Click += new System.EventHandler(this.btnQueryState_Click);
            // 
            // tabModulation
            // 
            this.tabModulation.Controls.Add(this.grpModulationConfig);
            this.tabModulation.Location = new System.Drawing.Point(4, 22);
            this.tabModulation.Name = "tabModulation";
            this.tabModulation.Padding = new System.Windows.Forms.Padding(3);
            this.tabModulation.Size = new System.Drawing.Size(752, 374);
            this.tabModulation.TabIndex = 1;
            this.tabModulation.Text = "Modulation";
            this.tabModulation.UseVisualStyleBackColor = true;
            // 
            // grpModulationConfig
            // 
            this.grpModulationConfig.Controls.Add(this.btnConfigureModulation);
            this.grpModulationConfig.Controls.Add(this.chkModulationEnable);
            this.grpModulationConfig.Controls.Add(this.lblHopPhaseUnit);
            this.grpModulationConfig.Controls.Add(this.txtHopPhase);
            this.grpModulationConfig.Controls.Add(this.lblHopPhase);
            this.grpModulationConfig.Controls.Add(this.lblHopAmplitudeUnit);
            this.grpModulationConfig.Controls.Add(this.txtHopAmplitude);
            this.grpModulationConfig.Controls.Add(this.lblHopAmplitude);
            this.grpModulationConfig.Controls.Add(this.lblHopFrequencyUnit);
            this.grpModulationConfig.Controls.Add(this.txtHopFrequency);
            this.grpModulationConfig.Controls.Add(this.lblHopFrequency);
            this.grpModulationConfig.Controls.Add(this.lblRateUnit);
            this.grpModulationConfig.Controls.Add(this.txtRate);
            this.grpModulationConfig.Controls.Add(this.lblRate);
            this.grpModulationConfig.Controls.Add(this.lblDeviationUnit);
            this.grpModulationConfig.Controls.Add(this.txtDeviation);
            this.grpModulationConfig.Controls.Add(this.lblDeviation);
            this.grpModulationConfig.Controls.Add(this.lblDepthUnit);
            this.grpModulationConfig.Controls.Add(this.txtDepth);
            this.grpModulationConfig.Controls.Add(this.lblDepth);
            this.grpModulationConfig.Controls.Add(this.cmbModulationWaveform);
            this.grpModulationConfig.Controls.Add(this.lblModulationWaveform);
            this.grpModulationConfig.Controls.Add(this.cmbModulationSource);
            this.grpModulationConfig.Controls.Add(this.lblModulationSource);
            this.grpModulationConfig.Controls.Add(this.cmbModulationType);
            this.grpModulationConfig.Controls.Add(this.lblModulationType);
            this.grpModulationConfig.Controls.Add(this.cmbModChannel);
            this.grpModulationConfig.Controls.Add(this.lblModChannel);
            this.grpModulationConfig.Location = new System.Drawing.Point(10, 10);
            this.grpModulationConfig.Name = "grpModulationConfig";
            this.grpModulationConfig.Size = new System.Drawing.Size(730, 350);
            this.grpModulationConfig.TabIndex = 0;
            this.grpModulationConfig.TabStop = false;
            this.grpModulationConfig.Text = "Modulation Configuration";
            // 
            // lblModChannel
            // 
            this.lblModChannel.AutoSize = true;
            this.lblModChannel.Location = new System.Drawing.Point(20, 30);
            this.lblModChannel.Name = "lblModChannel";
            this.lblModChannel.Size = new System.Drawing.Size(49, 13);
            this.lblModChannel.TabIndex = 0;
            this.lblModChannel.Text = "Channel:";
            // 
            // cmbModChannel
            // 
            this.cmbModChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModChannel.FormattingEnabled = true;
            this.cmbModChannel.Items.AddRange(new object[] {
            "Channel 1",
            "Channel 2"});
            this.cmbModChannel.Location = new System.Drawing.Point(140, 27);
            this.cmbModChannel.Name = "cmbModChannel";
            this.cmbModChannel.Size = new System.Drawing.Size(120, 21);
            this.cmbModChannel.TabIndex = 1;
            // 
            // lblModulationType
            // 
            this.lblModulationType.AutoSize = true;
            this.lblModulationType.Location = new System.Drawing.Point(20, 60);
            this.lblModulationType.Name = "lblModulationType";
            this.lblModulationType.Size = new System.Drawing.Size(91, 13);
            this.lblModulationType.TabIndex = 2;
            this.lblModulationType.Text = "Modulation Type:";
            // 
            // cmbModulationType
            // 
            this.cmbModulationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModulationType.FormattingEnabled = true;
            this.cmbModulationType.Location = new System.Drawing.Point(140, 57);
            this.cmbModulationType.Name = "cmbModulationType";
            this.cmbModulationType.Size = new System.Drawing.Size(120, 21);
            this.cmbModulationType.TabIndex = 3;
            this.cmbModulationType.SelectedIndexChanged += new System.EventHandler(this.cmbModulationType_SelectedIndexChanged);
            // 
            // lblModulationSource
            // 
            this.lblModulationSource.AutoSize = true;
            this.lblModulationSource.Location = new System.Drawing.Point(20, 90);
            this.lblModulationSource.Name = "lblModulationSource";
            this.lblModulationSource.Size = new System.Drawing.Size(103, 13);
            this.lblModulationSource.TabIndex = 4;
            this.lblModulationSource.Text = "Modulation Source:";
            // 
            // cmbModulationSource
            // 
            this.cmbModulationSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModulationSource.FormattingEnabled = true;
            this.cmbModulationSource.Location = new System.Drawing.Point(140, 87);
            this.cmbModulationSource.Name = "cmbModulationSource";
            this.cmbModulationSource.Size = new System.Drawing.Size(120, 21);
            this.cmbModulationSource.TabIndex = 5;
            // 
            // lblModulationWaveform
            // 
            this.lblModulationWaveform.AutoSize = true;
            this.lblModulationWaveform.Location = new System.Drawing.Point(20, 120);
            this.lblModulationWaveform.Name = "lblModulationWaveform";
            this.lblModulationWaveform.Size = new System.Drawing.Size(120, 13);
            this.lblModulationWaveform.TabIndex = 6;
            this.lblModulationWaveform.Text = "Modulation Waveform:";
            // 
            // cmbModulationWaveform
            // 
            this.cmbModulationWaveform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModulationWaveform.FormattingEnabled = true;
            this.cmbModulationWaveform.Location = new System.Drawing.Point(140, 117);
            this.cmbModulationWaveform.Name = "cmbModulationWaveform";
            this.cmbModulationWaveform.Size = new System.Drawing.Size(120, 21);
            this.cmbModulationWaveform.TabIndex = 7;
            // 
            // lblDepth
            // 
            this.lblDepth.AutoSize = true;
            this.lblDepth.Location = new System.Drawing.Point(20, 155);
            this.lblDepth.Name = "lblDepth";
            this.lblDepth.Size = new System.Drawing.Size(39, 13);
            this.lblDepth.TabIndex = 8;
            this.lblDepth.Text = "Depth:";
            // 
            // txtDepth
            // 
            this.txtDepth.Location = new System.Drawing.Point(140, 152);
            this.txtDepth.Name = "txtDepth";
            this.txtDepth.Size = new System.Drawing.Size(120, 20);
            this.txtDepth.TabIndex = 9;
            this.txtDepth.Text = "50.0";
            this.txtDepth.Validating += new System.ComponentModel.CancelEventHandler(this.txtDepth_Validating);
            // 
            // lblDepthUnit
            // 
            this.lblDepthUnit.AutoSize = true;
            this.lblDepthUnit.Location = new System.Drawing.Point(266, 155);
            this.lblDepthUnit.Name = "lblDepthUnit";
            this.lblDepthUnit.Size = new System.Drawing.Size(15, 13);
            this.lblDepthUnit.TabIndex = 10;
            this.lblDepthUnit.Text = "%";
            // 
            // lblDeviation
            // 
            this.lblDeviation.AutoSize = true;
            this.lblDeviation.Location = new System.Drawing.Point(20, 185);
            this.lblDeviation.Name = "lblDeviation";
            this.lblDeviation.Size = new System.Drawing.Size(56, 13);
            this.lblDeviation.TabIndex = 11;
            this.lblDeviation.Text = "Deviation:";
            this.lblDeviation.Visible = false;
            // 
            // txtDeviation
            // 
            this.txtDeviation.Location = new System.Drawing.Point(140, 182);
            this.txtDeviation.Name = "txtDeviation";
            this.txtDeviation.Size = new System.Drawing.Size(120, 20);
            this.txtDeviation.TabIndex = 12;
            this.txtDeviation.Text = "1000";
            this.txtDeviation.Visible = false;
            this.txtDeviation.Validating += new System.ComponentModel.CancelEventHandler(this.txtDeviation_Validating);
            // 
            // lblDeviationUnit
            // 
            this.lblDeviationUnit.AutoSize = true;
            this.lblDeviationUnit.Location = new System.Drawing.Point(266, 185);
            this.lblDeviationUnit.Name = "lblDeviationUnit";
            this.lblDeviationUnit.Size = new System.Drawing.Size(20, 13);
            this.lblDeviationUnit.TabIndex = 13;
            this.lblDeviationUnit.Text = "Hz";
            this.lblDeviationUnit.Visible = false;
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Location = new System.Drawing.Point(20, 215);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(33, 13);
            this.lblRate.TabIndex = 14;
            this.lblRate.Text = "Rate:";
            // 
            // txtRate
            // 
            this.txtRate.Location = new System.Drawing.Point(140, 212);
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(120, 20);
            this.txtRate.TabIndex = 15;
            this.txtRate.Text = "100";
            this.txtRate.Validating += new System.ComponentModel.CancelEventHandler(this.txtRate_Validating);
            // 
            // lblRateUnit
            // 
            this.lblRateUnit.AutoSize = true;
            this.lblRateUnit.Location = new System.Drawing.Point(266, 215);
            this.lblRateUnit.Name = "lblRateUnit";
            this.lblRateUnit.Size = new System.Drawing.Size(20, 13);
            this.lblRateUnit.TabIndex = 16;
            this.lblRateUnit.Text = "Hz";
            // 
            // lblHopFrequency
            // 
            this.lblHopFrequency.AutoSize = true;
            this.lblHopFrequency.Location = new System.Drawing.Point(380, 155);
            this.lblHopFrequency.Name = "lblHopFrequency";
            this.lblHopFrequency.Size = new System.Drawing.Size(83, 13);
            this.lblHopFrequency.TabIndex = 17;
            this.lblHopFrequency.Text = "Hop Frequency:";
            this.lblHopFrequency.Visible = false;
            // 
            // txtHopFrequency
            // 
            this.txtHopFrequency.Location = new System.Drawing.Point(480, 152);
            this.txtHopFrequency.Name = "txtHopFrequency";
            this.txtHopFrequency.Size = new System.Drawing.Size(120, 20);
            this.txtHopFrequency.TabIndex = 18;
            this.txtHopFrequency.Text = "2000";
            this.txtHopFrequency.Visible = false;
            this.txtHopFrequency.Validating += new System.ComponentModel.CancelEventHandler(this.txtHopFrequency_Validating);
            // 
            // lblHopFrequencyUnit
            // 
            this.lblHopFrequencyUnit.AutoSize = true;
            this.lblHopFrequencyUnit.Location = new System.Drawing.Point(606, 155);
            this.lblHopFrequencyUnit.Name = "lblHopFrequencyUnit";
            this.lblHopFrequencyUnit.Size = new System.Drawing.Size(20, 13);
            this.lblHopFrequencyUnit.TabIndex = 19;
            this.lblHopFrequencyUnit.Text = "Hz";
            this.lblHopFrequencyUnit.Visible = false;
            // 
            // lblHopAmplitude
            // 
            this.lblHopAmplitude.AutoSize = true;
            this.lblHopAmplitude.Location = new System.Drawing.Point(380, 185);
            this.lblHopAmplitude.Name = "lblHopAmplitude";
            this.lblHopAmplitude.Size = new System.Drawing.Size(79, 13);
            this.lblHopAmplitude.TabIndex = 20;
            this.lblHopAmplitude.Text = "Hop Amplitude:";
            this.lblHopAmplitude.Visible = false;
            // 
            // txtHopAmplitude
            // 
            this.txtHopAmplitude.Location = new System.Drawing.Point(480, 182);
            this.txtHopAmplitude.Name = "txtHopAmplitude";
            this.txtHopAmplitude.Size = new System.Drawing.Size(120, 20);
            this.txtHopAmplitude.TabIndex = 21;
            this.txtHopAmplitude.Text = "2.5";
            this.txtHopAmplitude.Visible = false;
            this.txtHopAmplitude.Validating += new System.ComponentModel.CancelEventHandler(this.txtHopAmplitude_Validating);
            // 
            // lblHopAmplitudeUnit
            // 
            this.lblHopAmplitudeUnit.AutoSize = true;
            this.lblHopAmplitudeUnit.Location = new System.Drawing.Point(606, 185);
            this.lblHopAmplitudeUnit.Name = "lblHopAmplitudeUnit";
            this.lblHopAmplitudeUnit.Size = new System.Drawing.Size(14, 13);
            this.lblHopAmplitudeUnit.TabIndex = 22;
            this.lblHopAmplitudeUnit.Text = "V";
            this.lblHopAmplitudeUnit.Visible = false;
            // 
            // lblHopPhase
            // 
            this.lblHopPhase.AutoSize = true;
            this.lblHopPhase.Location = new System.Drawing.Point(380, 215);
            this.lblHopPhase.Name = "lblHopPhase";
            this.lblHopPhase.Size = new System.Drawing.Size(63, 13);
            this.lblHopPhase.TabIndex = 23;
            this.lblHopPhase.Text = "Hop Phase:";
            this.lblHopPhase.Visible = false;
            // 
            // txtHopPhase
            // 
            this.txtHopPhase.Location = new System.Drawing.Point(480, 212);
            this.txtHopPhase.Name = "txtHopPhase";
            this.txtHopPhase.Size = new System.Drawing.Size(120, 20);
            this.txtHopPhase.TabIndex = 24;
            this.txtHopPhase.Text = "90";
            this.txtHopPhase.Visible = false;
            this.txtHopPhase.Validating += new System.ComponentModel.CancelEventHandler(this.txtHopPhase_Validating);
            // 
            // lblHopPhaseUnit
            // 
            this.lblHopPhaseUnit.AutoSize = true;
            this.lblHopPhaseUnit.Location = new System.Drawing.Point(606, 215);
            this.lblHopPhaseUnit.Name = "lblHopPhaseUnit";
            this.lblHopPhaseUnit.Size = new System.Drawing.Size(47, 13);
            this.lblHopPhaseUnit.TabIndex = 25;
            this.lblHopPhaseUnit.Text = "Degrees";
            this.lblHopPhaseUnit.Visible = false;
            // 
            // chkModulationEnable
            // 
            this.chkModulationEnable.AutoSize = true;
            this.chkModulationEnable.Location = new System.Drawing.Point(140, 255);
            this.chkModulationEnable.Name = "chkModulationEnable";
            this.chkModulationEnable.Size = new System.Drawing.Size(115, 17);
            this.chkModulationEnable.TabIndex = 26;
            this.chkModulationEnable.Text = "Modulation Enable";
            this.chkModulationEnable.UseVisualStyleBackColor = true;
            this.chkModulationEnable.CheckedChanged += new System.EventHandler(this.chkModulationEnable_CheckedChanged);
            // 
            // btnConfigureModulation
            // 
            this.btnConfigureModulation.Location = new System.Drawing.Point(140, 290);
            this.btnConfigureModulation.Name = "btnConfigureModulation";
            this.btnConfigureModulation.Size = new System.Drawing.Size(150, 30);
            this.btnConfigureModulation.TabIndex = 27;
            this.btnConfigureModulation.Text = "Configure Modulation";
            this.btnConfigureModulation.UseVisualStyleBackColor = true;
            this.btnConfigureModulation.Click += new System.EventHandler(this.btnConfigureModulation_Click);
            // 
            // tabSweep
            // 
            this.tabSweep.Controls.Add(this.grpSweepConfig);
            this.tabSweep.Location = new System.Drawing.Point(4, 22);
            this.tabSweep.Name = "tabSweep";
            this.tabSweep.Size = new System.Drawing.Size(752, 374);
            this.tabSweep.TabIndex = 2;
            this.tabSweep.Text = "Sweep";
            this.tabSweep.UseVisualStyleBackColor = true;
            // 
            // grpSweepConfig
            // 
            this.grpSweepConfig.Controls.Add(this.btnConfigureSweep);
            this.grpSweepConfig.Controls.Add(this.chkSweepEnable);
            this.grpSweepConfig.Controls.Add(this.lblHoldTimeUnit);
            this.grpSweepConfig.Controls.Add(this.txtHoldTime);
            this.grpSweepConfig.Controls.Add(this.lblHoldTime);
            this.grpSweepConfig.Controls.Add(this.lblReturnTimeUnit);
            this.grpSweepConfig.Controls.Add(this.txtReturnTime);
            this.grpSweepConfig.Controls.Add(this.lblReturnTime);
            this.grpSweepConfig.Controls.Add(this.cmbSweepTriggerSource);
            this.grpSweepConfig.Controls.Add(this.lblSweepTriggerSource);
            this.grpSweepConfig.Controls.Add(this.cmbSweepDirection);
            this.grpSweepConfig.Controls.Add(this.lblSweepDirection);
            this.grpSweepConfig.Controls.Add(this.cmbSweepType);
            this.grpSweepConfig.Controls.Add(this.lblSweepType);
            this.grpSweepConfig.Controls.Add(this.lblSweepTimeUnit);
            this.grpSweepConfig.Controls.Add(this.txtSweepTime);
            this.grpSweepConfig.Controls.Add(this.lblSweepTime);
            this.grpSweepConfig.Controls.Add(this.lblStopFrequencyUnit);
            this.grpSweepConfig.Controls.Add(this.txtStopFrequency);
            this.grpSweepConfig.Controls.Add(this.lblStopFrequency);
            this.grpSweepConfig.Controls.Add(this.lblStartFrequencyUnit);
            this.grpSweepConfig.Controls.Add(this.txtStartFrequency);
            this.grpSweepConfig.Controls.Add(this.lblStartFrequency);
            this.grpSweepConfig.Controls.Add(this.cmbSweepChannel);
            this.grpSweepConfig.Controls.Add(this.lblSweepChannel);
            this.grpSweepConfig.Location = new System.Drawing.Point(10, 10);
            this.grpSweepConfig.Name = "grpSweepConfig";
            this.grpSweepConfig.Size = new System.Drawing.Size(730, 350);
            this.grpSweepConfig.TabIndex = 0;
            this.grpSweepConfig.TabStop = false;
            this.grpSweepConfig.Text = "Sweep Configuration";
            // 
            // lblSweepChannel
            // 
            this.lblSweepChannel.AutoSize = true;
            this.lblSweepChannel.Location = new System.Drawing.Point(20, 30);
            this.lblSweepChannel.Name = "lblSweepChannel";
            this.lblSweepChannel.Size = new System.Drawing.Size(49, 13);
            this.lblSweepChannel.TabIndex = 0;
            this.lblSweepChannel.Text = "Channel:";
            // 
            // cmbSweepChannel
            // 
            this.cmbSweepChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSweepChannel.FormattingEnabled = true;
            this.cmbSweepChannel.Items.AddRange(new object[] {
            "Channel 1",
            "Channel 2"});
            this.cmbSweepChannel.Location = new System.Drawing.Point(140, 27);
            this.cmbSweepChannel.Name = "cmbSweepChannel";
            this.cmbSweepChannel.Size = new System.Drawing.Size(120, 21);
            this.cmbSweepChannel.TabIndex = 1;
            // 
            // lblStartFrequency
            // 
            this.lblStartFrequency.AutoSize = true;
            this.lblStartFrequency.Location = new System.Drawing.Point(20, 60);
            this.lblStartFrequency.Name = "lblStartFrequency";
            this.lblStartFrequency.Size = new System.Drawing.Size(87, 13);
            this.lblStartFrequency.TabIndex = 2;
            this.lblStartFrequency.Text = "Start Frequency:";
            // 
            // txtStartFrequency
            // 
            this.txtStartFrequency.Location = new System.Drawing.Point(140, 57);
            this.txtStartFrequency.Name = "txtStartFrequency";
            this.txtStartFrequency.Size = new System.Drawing.Size(120, 20);
            this.txtStartFrequency.TabIndex = 3;
            this.txtStartFrequency.Text = "100";
            this.txtStartFrequency.Validating += new System.ComponentModel.CancelEventHandler(this.txtStartFrequency_Validating);
            // 
            // lblStartFrequencyUnit
            // 
            this.lblStartFrequencyUnit.AutoSize = true;
            this.lblStartFrequencyUnit.Location = new System.Drawing.Point(266, 60);
            this.lblStartFrequencyUnit.Name = "lblStartFrequencyUnit";
            this.lblStartFrequencyUnit.Size = new System.Drawing.Size(20, 13);
            this.lblStartFrequencyUnit.TabIndex = 4;
            this.lblStartFrequencyUnit.Text = "Hz";
            // 
            // lblStopFrequency
            // 
            this.lblStopFrequency.AutoSize = true;
            this.lblStopFrequency.Location = new System.Drawing.Point(20, 90);
            this.lblStopFrequency.Name = "lblStopFrequency";
            this.lblStopFrequency.Size = new System.Drawing.Size(84, 13);
            this.lblStopFrequency.TabIndex = 5;
            this.lblStopFrequency.Text = "Stop Frequency:";
            // 
            // txtStopFrequency
            // 
            this.txtStopFrequency.Location = new System.Drawing.Point(140, 87);
            this.txtStopFrequency.Name = "txtStopFrequency";
            this.txtStopFrequency.Size = new System.Drawing.Size(120, 20);
            this.txtStopFrequency.TabIndex = 6;
            this.txtStopFrequency.Text = "10000";
            this.txtStopFrequency.Validating += new System.ComponentModel.CancelEventHandler(this.txtStopFrequency_Validating);
            // 
            // lblStopFrequencyUnit
            // 
            this.lblStopFrequencyUnit.AutoSize = true;
            this.lblStopFrequencyUnit.Location = new System.Drawing.Point(266, 90);
            this.lblStopFrequencyUnit.Name = "lblStopFrequencyUnit";
            this.lblStopFrequencyUnit.Size = new System.Drawing.Size(20, 13);
            this.lblStopFrequencyUnit.TabIndex = 7;
            this.lblStopFrequencyUnit.Text = "Hz";
            // 
            // lblSweepTime
            // 
            this.lblSweepTime.AutoSize = true;
            this.lblSweepTime.Location = new System.Drawing.Point(20, 120);
            this.lblSweepTime.Name = "lblSweepTime";
            this.lblSweepTime.Size = new System.Drawing.Size(70, 13);
            this.lblSweepTime.TabIndex = 8;
            this.lblSweepTime.Text = "Sweep Time:";
            // 
            // txtSweepTime
            // 
            this.txtSweepTime.Location = new System.Drawing.Point(140, 117);
            this.txtSweepTime.Name = "txtSweepTime";
            this.txtSweepTime.Size = new System.Drawing.Size(120, 20);
            this.txtSweepTime.TabIndex = 9;
            this.txtSweepTime.Text = "1.0";
            this.txtSweepTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtSweepTime_Validating);
            // 
            // lblSweepTimeUnit
            // 
            this.lblSweepTimeUnit.AutoSize = true;
            this.lblSweepTimeUnit.Location = new System.Drawing.Point(266, 120);
            this.lblSweepTimeUnit.Name = "lblSweepTimeUnit";
            this.lblSweepTimeUnit.Size = new System.Drawing.Size(12, 13);
            this.lblSweepTimeUnit.TabIndex = 10;
            this.lblSweepTimeUnit.Text = "s";
            // 
            // lblSweepType
            // 
            this.lblSweepType.AutoSize = true;
            this.lblSweepType.Location = new System.Drawing.Point(20, 150);
            this.lblSweepType.Name = "lblSweepType";
            this.lblSweepType.Size = new System.Drawing.Size(70, 13);
            this.lblSweepType.TabIndex = 11;
            this.lblSweepType.Text = "Sweep Type:";
            // 
            // cmbSweepType
            // 
            this.cmbSweepType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSweepType.FormattingEnabled = true;
            this.cmbSweepType.Location = new System.Drawing.Point(140, 147);
            this.cmbSweepType.Name = "cmbSweepType";
            this.cmbSweepType.Size = new System.Drawing.Size(120, 21);
            this.cmbSweepType.TabIndex = 12;
            // 
            // lblSweepDirection
            // 
            this.lblSweepDirection.AutoSize = true;
            this.lblSweepDirection.Location = new System.Drawing.Point(20, 180);
            this.lblSweepDirection.Name = "lblSweepDirection";
            this.lblSweepDirection.Size = new System.Drawing.Size(92, 13);
            this.lblSweepDirection.TabIndex = 13;
            this.lblSweepDirection.Text = "Sweep Direction:";
            // 
            // cmbSweepDirection
            // 
            this.cmbSweepDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSweepDirection.FormattingEnabled = true;
            this.cmbSweepDirection.Location = new System.Drawing.Point(140, 177);
            this.cmbSweepDirection.Name = "cmbSweepDirection";
            this.cmbSweepDirection.Size = new System.Drawing.Size(120, 21);
            this.cmbSweepDirection.TabIndex = 14;
            // 
            // lblSweepTriggerSource
            // 
            this.lblSweepTriggerSource.AutoSize = true;
            this.lblSweepTriggerSource.Location = new System.Drawing.Point(20, 210);
            this.lblSweepTriggerSource.Name = "lblSweepTriggerSource";
            this.lblSweepTriggerSource.Size = new System.Drawing.Size(80, 13);
            this.lblSweepTriggerSource.TabIndex = 15;
            this.lblSweepTriggerSource.Text = "Trigger Source:";
            // 
            // cmbSweepTriggerSource
            // 
            this.cmbSweepTriggerSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSweepTriggerSource.FormattingEnabled = true;
            this.cmbSweepTriggerSource.Location = new System.Drawing.Point(140, 207);
            this.cmbSweepTriggerSource.Name = "cmbSweepTriggerSource";
            this.cmbSweepTriggerSource.Size = new System.Drawing.Size(120, 21);
            this.cmbSweepTriggerSource.TabIndex = 16;
            // 
            // lblReturnTime
            // 
            this.lblReturnTime.AutoSize = true;
            this.lblReturnTime.Location = new System.Drawing.Point(380, 120);
            this.lblReturnTime.Name = "lblReturnTime";
            this.lblReturnTime.Size = new System.Drawing.Size(70, 13);
            this.lblReturnTime.TabIndex = 17;
            this.lblReturnTime.Text = "Return Time:";
            // 
            // txtReturnTime
            // 
            this.txtReturnTime.Location = new System.Drawing.Point(480, 117);
            this.txtReturnTime.Name = "txtReturnTime";
            this.txtReturnTime.Size = new System.Drawing.Size(120, 20);
            this.txtReturnTime.TabIndex = 18;
            this.txtReturnTime.Text = "0.0";
            this.txtReturnTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtReturnTime_Validating);
            // 
            // lblReturnTimeUnit
            // 
            this.lblReturnTimeUnit.AutoSize = true;
            this.lblReturnTimeUnit.Location = new System.Drawing.Point(606, 120);
            this.lblReturnTimeUnit.Name = "lblReturnTimeUnit";
            this.lblReturnTimeUnit.Size = new System.Drawing.Size(12, 13);
            this.lblReturnTimeUnit.TabIndex = 19;
            this.lblReturnTimeUnit.Text = "s";
            // 
            // lblHoldTime
            // 
            this.lblHoldTime.AutoSize = true;
            this.lblHoldTime.Location = new System.Drawing.Point(380, 150);
            this.lblHoldTime.Name = "lblHoldTime";
            this.lblHoldTime.Size = new System.Drawing.Size(57, 13);
            this.lblHoldTime.TabIndex = 20;
            this.lblHoldTime.Text = "Hold Time:";
            // 
            // txtHoldTime
            // 
            this.txtHoldTime.Location = new System.Drawing.Point(480, 147);
            this.txtHoldTime.Name = "txtHoldTime";
            this.txtHoldTime.Size = new System.Drawing.Size(120, 20);
            this.txtHoldTime.TabIndex = 21;
            this.txtHoldTime.Text = "0.0";
            this.txtHoldTime.Validating += new System.ComponentModel.CancelEventHandler(this.txtHoldTime_Validating);
            // 
            // lblHoldTimeUnit
            // 
            this.lblHoldTimeUnit.AutoSize = true;
            this.lblHoldTimeUnit.Location = new System.Drawing.Point(606, 150);
            this.lblHoldTimeUnit.Name = "lblHoldTimeUnit";
            this.lblHoldTimeUnit.Size = new System.Drawing.Size(12, 13);
            this.lblHoldTimeUnit.TabIndex = 22;
            this.lblHoldTimeUnit.Text = "s";
            // 
            // chkSweepEnable
            // 
            this.chkSweepEnable.AutoSize = true;
            this.chkSweepEnable.Location = new System.Drawing.Point(140, 250);
            this.chkSweepEnable.Name = "chkSweepEnable";
            this.chkSweepEnable.Size = new System.Drawing.Size(95, 17);
            this.chkSweepEnable.TabIndex = 23;
            this.chkSweepEnable.Text = "Sweep Enable";
            this.chkSweepEnable.UseVisualStyleBackColor = true;
            this.chkSweepEnable.CheckedChanged += new System.EventHandler(this.chkSweepEnable_CheckedChanged);
            // 
            // btnConfigureSweep
            // 
            this.btnConfigureSweep.Location = new System.Drawing.Point(140, 285);
            this.btnConfigureSweep.Name = "btnConfigureSweep";
            this.btnConfigureSweep.Size = new System.Drawing.Size(150, 30);
            this.btnConfigureSweep.TabIndex = 24;
            this.btnConfigureSweep.Text = "Configure Sweep";
            this.btnConfigureSweep.UseVisualStyleBackColor = true;
            this.btnConfigureSweep.Click += new System.EventHandler(this.btnConfigureSweep_Click);
            // 
            // grpBurstConfig
            // 
            this.grpBurstConfig.Controls.Add(this.btnConfigureBurst);
            this.grpBurstConfig.Controls.Add(this.chkBurstEnable);
            this.grpBurstConfig.Controls.Add(this.cmbGatePolarity);
            this.grpBurstConfig.Controls.Add(this.lblGatePolarity);
            this.grpBurstConfig.Controls.Add(this.lblStartPhaseUnit);
            this.grpBurstConfig.Controls.Add(this.txtStartPhase);
            this.grpBurstConfig.Controls.Add(this.lblStartPhase);
            this.grpBurstConfig.Controls.Add(this.cmbTriggerEdge);
            this.grpBurstConfig.Controls.Add(this.lblTriggerEdge);
            this.grpBurstConfig.Controls.Add(this.cmbBurstTriggerSource);
            this.grpBurstConfig.Controls.Add(this.lblBurstTriggerSource);
            this.grpBurstConfig.Controls.Add(this.lblPeriodUnit);
            this.grpBurstConfig.Controls.Add(this.txtPeriod);
            this.grpBurstConfig.Controls.Add(this.lblPeriod);
            this.grpBurstConfig.Controls.Add(this.txtCycles);
            this.grpBurstConfig.Controls.Add(this.lblCycles);
            this.grpBurstConfig.Controls.Add(this.cmbBurstMode);
            this.grpBurstConfig.Controls.Add(this.lblBurstMode);
            this.grpBurstConfig.Controls.Add(this.cmbBurstChannel);
            this.grpBurstConfig.Controls.Add(this.lblBurstChannel);
            this.grpBurstConfig.Location = new System.Drawing.Point(10, 10);
            this.grpBurstConfig.Name = "grpBurstConfig";
            this.grpBurstConfig.Size = new System.Drawing.Size(730, 350);
            this.grpBurstConfig.TabIndex = 0;
            this.grpBurstConfig.TabStop = false;
            this.grpBurstConfig.Text = "Burst Configuration";
            // 
            // lblBurstChannel
            // 
            this.lblBurstChannel.AutoSize = true;
            this.lblBurstChannel.Location = new System.Drawing.Point(20, 30);
            this.lblBurstChannel.Name = "lblBurstChannel";
            this.lblBurstChannel.Size = new System.Drawing.Size(49, 13);
            this.lblBurstChannel.TabIndex = 0;
            this.lblBurstChannel.Text = "Channel:";
            // 
            // cmbBurstChannel
            // 
            this.cmbBurstChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBurstChannel.FormattingEnabled = true;
            this.cmbBurstChannel.Items.AddRange(new object[] {
            "Channel 1",
            "Channel 2"});
            this.cmbBurstChannel.Location = new System.Drawing.Point(140, 27);
            this.cmbBurstChannel.Name = "cmbBurstChannel";
            this.cmbBurstChannel.Size = new System.Drawing.Size(120, 21);
            this.cmbBurstChannel.TabIndex = 1;
            // 
            // lblBurstMode
            // 
            this.lblBurstMode.AutoSize = true;
            this.lblBurstMode.Location = new System.Drawing.Point(20, 60);
            this.lblBurstMode.Name = "lblBurstMode";
            this.lblBurstMode.Size = new System.Drawing.Size(65, 13);
            this.lblBurstMode.TabIndex = 2;
            this.lblBurstMode.Text = "Burst Mode:";
            // 
            // cmbBurstMode
            // 
            this.cmbBurstMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBurstMode.FormattingEnabled = true;
            this.cmbBurstMode.Location = new System.Drawing.Point(140, 57);
            this.cmbBurstMode.Name = "cmbBurstMode";
            this.cmbBurstMode.Size = new System.Drawing.Size(120, 21);
            this.cmbBurstMode.TabIndex = 3;
            this.cmbBurstMode.SelectedIndexChanged += new System.EventHandler(this.cmbBurstMode_SelectedIndexChanged);
            // 
            // lblCycles
            // 
            this.lblCycles.AutoSize = true;
            this.lblCycles.Location = new System.Drawing.Point(20, 90);
            this.lblCycles.Name = "lblCycles";
            this.lblCycles.Size = new System.Drawing.Size(42, 13);
            this.lblCycles.TabIndex = 4;
            this.lblCycles.Text = "Cycles:";
            // 
            // txtCycles
            // 
            this.txtCycles.Location = new System.Drawing.Point(140, 87);
            this.txtCycles.Name = "txtCycles";
            this.txtCycles.Size = new System.Drawing.Size(120, 20);
            this.txtCycles.TabIndex = 5;
            this.txtCycles.Text = "1";
            this.txtCycles.Validating += new System.ComponentModel.CancelEventHandler(this.txtCycles_Validating);
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Location = new System.Drawing.Point(20, 120);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(40, 13);
            this.lblPeriod.TabIndex = 6;
            this.lblPeriod.Text = "Period:";
            // 
            // txtPeriod
            // 
            this.txtPeriod.Location = new System.Drawing.Point(140, 117);
            this.txtPeriod.Name = "txtPeriod";
            this.txtPeriod.Size = new System.Drawing.Size(120, 20);
            this.txtPeriod.TabIndex = 7;
            this.txtPeriod.Text = "0.001";
            this.txtPeriod.Validating += new System.ComponentModel.CancelEventHandler(this.txtPeriod_Validating);
            // 
            // lblPeriodUnit
            // 
            this.lblPeriodUnit.AutoSize = true;
            this.lblPeriodUnit.Location = new System.Drawing.Point(266, 120);
            this.lblPeriodUnit.Name = "lblPeriodUnit";
            this.lblPeriodUnit.Size = new System.Drawing.Size(12, 13);
            this.lblPeriodUnit.TabIndex = 8;
            this.lblPeriodUnit.Text = "s";
            // 
            // lblBurstTriggerSource
            // 
            this.lblBurstTriggerSource.AutoSize = true;
            this.lblBurstTriggerSource.Location = new System.Drawing.Point(20, 150);
            this.lblBurstTriggerSource.Name = "lblBurstTriggerSource";
            this.lblBurstTriggerSource.Size = new System.Drawing.Size(80, 13);
            this.lblBurstTriggerSource.TabIndex = 9;
            this.lblBurstTriggerSource.Text = "Trigger Source:";
            // 
            // cmbBurstTriggerSource
            // 
            this.cmbBurstTriggerSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBurstTriggerSource.FormattingEnabled = true;
            this.cmbBurstTriggerSource.Location = new System.Drawing.Point(140, 147);
            this.cmbBurstTriggerSource.Name = "cmbBurstTriggerSource";
            this.cmbBurstTriggerSource.Size = new System.Drawing.Size(120, 21);
            this.cmbBurstTriggerSource.TabIndex = 10;
            // 
            // lblTriggerEdge
            // 
            this.lblTriggerEdge.AutoSize = true;
            this.lblTriggerEdge.Location = new System.Drawing.Point(20, 180);
            this.lblTriggerEdge.Name = "lblTriggerEdge";
            this.lblTriggerEdge.Size = new System.Drawing.Size(71, 13);
            this.lblTriggerEdge.TabIndex = 11;
            this.lblTriggerEdge.Text = "Trigger Edge:";
            // 
            // cmbTriggerEdge
            // 
            this.cmbTriggerEdge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTriggerEdge.FormattingEnabled = true;
            this.cmbTriggerEdge.Location = new System.Drawing.Point(140, 177);
            this.cmbTriggerEdge.Name = "cmbTriggerEdge";
            this.cmbTriggerEdge.Size = new System.Drawing.Size(120, 21);
            this.cmbTriggerEdge.TabIndex = 12;
            // 
            // lblStartPhase
            // 
            this.lblStartPhase.AutoSize = true;
            this.lblStartPhase.Location = new System.Drawing.Point(20, 210);
            this.lblStartPhase.Name = "lblStartPhase";
            this.lblStartPhase.Size = new System.Drawing.Size(66, 13);
            this.lblStartPhase.TabIndex = 13;
            this.lblStartPhase.Text = "Start Phase:";
            // 
            // txtStartPhase
            // 
            this.txtStartPhase.Location = new System.Drawing.Point(140, 207);
            this.txtStartPhase.Name = "txtStartPhase";
            this.txtStartPhase.Size = new System.Drawing.Size(120, 20);
            this.txtStartPhase.TabIndex = 14;
            this.txtStartPhase.Text = "0.0";
            this.txtStartPhase.Validating += new System.ComponentModel.CancelEventHandler(this.txtStartPhase_Validating);
            // 
            // lblStartPhaseUnit
            // 
            this.lblStartPhaseUnit.AutoSize = true;
            this.lblStartPhaseUnit.Location = new System.Drawing.Point(266, 210);
            this.lblStartPhaseUnit.Name = "lblStartPhaseUnit";
            this.lblStartPhaseUnit.Size = new System.Drawing.Size(47, 13);
            this.lblStartPhaseUnit.TabIndex = 15;
            this.lblStartPhaseUnit.Text = "Degrees";
            // 
            // lblGatePolarity
            // 
            this.lblGatePolarity.AutoSize = true;
            this.lblGatePolarity.Location = new System.Drawing.Point(380, 90);
            this.lblGatePolarity.Name = "lblGatePolarity";
            this.lblGatePolarity.Size = new System.Drawing.Size(72, 13);
            this.lblGatePolarity.TabIndex = 16;
            this.lblGatePolarity.Text = "Gate Polarity:";
            this.lblGatePolarity.Visible = false;
            // 
            // cmbGatePolarity
            // 
            this.cmbGatePolarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGatePolarity.FormattingEnabled = true;
            this.cmbGatePolarity.Location = new System.Drawing.Point(480, 87);
            this.cmbGatePolarity.Name = "cmbGatePolarity";
            this.cmbGatePolarity.Size = new System.Drawing.Size(120, 21);
            this.cmbGatePolarity.TabIndex = 17;
            this.cmbGatePolarity.Visible = false;
            // 
            // chkBurstEnable
            // 
            this.chkBurstEnable.AutoSize = true;
            this.chkBurstEnable.Location = new System.Drawing.Point(140, 250);
            this.chkBurstEnable.Name = "chkBurstEnable";
            this.chkBurstEnable.Size = new System.Drawing.Size(85, 17);
            this.chkBurstEnable.TabIndex = 18;
            this.chkBurstEnable.Text = "Burst Enable";
            this.chkBurstEnable.UseVisualStyleBackColor = true;
            this.chkBurstEnable.CheckedChanged += new System.EventHandler(this.chkBurstEnable_CheckedChanged);
            // 
            // btnConfigureBurst
            // 
            this.btnConfigureBurst.Location = new System.Drawing.Point(140, 285);
            this.btnConfigureBurst.Name = "btnConfigureBurst";
            this.btnConfigureBurst.Size = new System.Drawing.Size(150, 30);
            this.btnConfigureBurst.TabIndex = 19;
            this.btnConfigureBurst.Text = "Configure Burst";
            this.btnConfigureBurst.UseVisualStyleBackColor = true;
            this.btnConfigureBurst.Click += new System.EventHandler(this.btnConfigureBurst_Click);
            // 
            // grpArbitraryConfig
            // 
            this.grpArbitraryConfig.Controls.Add(this.progressBar);
            this.grpArbitraryConfig.Controls.Add(this.txtWaveformName);
            this.grpArbitraryConfig.Controls.Add(this.lblWaveformName);
            this.grpArbitraryConfig.Controls.Add(this.btnSelectWaveform);
            this.grpArbitraryConfig.Controls.Add(this.btnDeleteWaveform);
            this.grpArbitraryConfig.Controls.Add(this.btnUploadWaveform);
            this.grpArbitraryConfig.Controls.Add(this.btnRefreshList);
            this.grpArbitraryConfig.Controls.Add(this.lstWaveforms);
            this.grpArbitraryConfig.Controls.Add(this.cmbArbitraryChannel);
            this.grpArbitraryConfig.Controls.Add(this.lblArbitraryChannel);
            this.grpArbitraryConfig.Location = new System.Drawing.Point(10, 10);
            this.grpArbitraryConfig.Name = "grpArbitraryConfig";
            this.grpArbitraryConfig.Size = new System.Drawing.Size(730, 350);
            this.grpArbitraryConfig.TabIndex = 0;
            this.grpArbitraryConfig.TabStop = false;
            this.grpArbitraryConfig.Text = "Arbitrary Waveform Management";
            // 
            // lblArbitraryChannel
            // 
            this.lblArbitraryChannel.AutoSize = true;
            this.lblArbitraryChannel.Location = new System.Drawing.Point(20, 30);
            this.lblArbitraryChannel.Name = "lblArbitraryChannel";
            this.lblArbitraryChannel.Size = new System.Drawing.Size(49, 13);
            this.lblArbitraryChannel.TabIndex = 0;
            this.lblArbitraryChannel.Text = "Channel:";
            // 
            // cmbArbitraryChannel
            // 
            this.cmbArbitraryChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArbitraryChannel.FormattingEnabled = true;
            this.cmbArbitraryChannel.Items.AddRange(new object[] {
            "Channel 1",
            "Channel 2"});
            this.cmbArbitraryChannel.Location = new System.Drawing.Point(120, 27);
            this.cmbArbitraryChannel.Name = "cmbArbitraryChannel";
            this.cmbArbitraryChannel.Size = new System.Drawing.Size(120, 21);
            this.cmbArbitraryChannel.TabIndex = 1;
            // 
            // lstWaveforms
            // 
            this.lstWaveforms.FormattingEnabled = true;
            this.lstWaveforms.Location = new System.Drawing.Point(20, 60);
            this.lstWaveforms.Name = "lstWaveforms";
            this.lstWaveforms.Size = new System.Drawing.Size(300, 160);
            this.lstWaveforms.TabIndex = 2;
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.Location = new System.Drawing.Point(340, 60);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(120, 30);
            this.btnRefreshList.TabIndex = 3;
            this.btnRefreshList.Text = "Refresh List";
            this.btnRefreshList.UseVisualStyleBackColor = true;
            this.btnRefreshList.Click += new System.EventHandler(this.btnRefreshList_Click);
            // 
            // btnUploadWaveform
            // 
            this.btnUploadWaveform.Location = new System.Drawing.Point(340, 100);
            this.btnUploadWaveform.Name = "btnUploadWaveform";
            this.btnUploadWaveform.Size = new System.Drawing.Size(120, 30);
            this.btnUploadWaveform.TabIndex = 4;
            this.btnUploadWaveform.Text = "Upload Waveform";
            this.btnUploadWaveform.UseVisualStyleBackColor = true;
            this.btnUploadWaveform.Click += new System.EventHandler(this.btnUploadWaveform_Click);
            // 
            // btnDeleteWaveform
            // 
            this.btnDeleteWaveform.Location = new System.Drawing.Point(340, 140);
            this.btnDeleteWaveform.Name = "btnDeleteWaveform";
            this.btnDeleteWaveform.Size = new System.Drawing.Size(120, 30);
            this.btnDeleteWaveform.TabIndex = 5;
            this.btnDeleteWaveform.Text = "Delete Waveform";
            this.btnDeleteWaveform.UseVisualStyleBackColor = true;
            this.btnDeleteWaveform.Click += new System.EventHandler(this.btnDeleteWaveform_Click);
            // 
            // btnSelectWaveform
            // 
            this.btnSelectWaveform.Location = new System.Drawing.Point(340, 180);
            this.btnSelectWaveform.Name = "btnSelectWaveform";
            this.btnSelectWaveform.Size = new System.Drawing.Size(120, 30);
            this.btnSelectWaveform.TabIndex = 6;
            this.btnSelectWaveform.Text = "Select Waveform";
            this.btnSelectWaveform.UseVisualStyleBackColor = true;
            this.btnSelectWaveform.Click += new System.EventHandler(this.btnSelectWaveform_Click);
            // 
            // lblWaveformName
            // 
            this.lblWaveformName.AutoSize = true;
            this.lblWaveformName.Location = new System.Drawing.Point(20, 240);
            this.lblWaveformName.Name = "lblWaveformName";
            this.lblWaveformName.Size = new System.Drawing.Size(88, 13);
            this.lblWaveformName.TabIndex = 7;
            this.lblWaveformName.Text = "Waveform Name:";
            // 
            // txtWaveformName
            // 
            this.txtWaveformName.Location = new System.Drawing.Point(120, 237);
            this.txtWaveformName.Name = "txtWaveformName";
            this.txtWaveformName.Size = new System.Drawing.Size(200, 20);
            this.txtWaveformName.TabIndex = 8;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(20, 280);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(440, 23);
            this.progressBar.TabIndex = 9;
            this.progressBar.Visible = false;
            // 
            // tabBurst
            // 
            this.tabBurst.Controls.Add(this.grpBurstConfig);
            this.tabBurst.Location = new System.Drawing.Point(4, 22);
            this.tabBurst.Name = "tabBurst";
            this.tabBurst.Size = new System.Drawing.Size(752, 374);
            this.tabBurst.TabIndex = 3;
            this.tabBurst.Text = "Burst";
            this.tabBurst.UseVisualStyleBackColor = true;
            // 
            // tabArbitrary
            // 
            this.tabArbitrary.Controls.Add(this.grpArbitraryConfig);
            this.tabArbitrary.Location = new System.Drawing.Point(4, 22);
            this.tabArbitrary.Name = "tabArbitrary";
            this.tabArbitrary.Size = new System.Drawing.Size(752, 374);
            this.tabArbitrary.TabIndex = 4;
            this.tabArbitrary.Text = "Arbitrary Waveform";
            this.tabArbitrary.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.grpConnection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Siglent SDG6052X Control Application";
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabWaveform.ResumeLayout(false);
            this.grpWaveformConfig.ResumeLayout(false);
            this.grpWaveformConfig.PerformLayout();
            this.tabModulation.ResumeLayout(false);
            this.grpModulationConfig.ResumeLayout(false);
            this.grpModulationConfig.PerformLayout();
            this.tabSweep.ResumeLayout(false);
            this.grpSweepConfig.ResumeLayout(false);
            this.grpSweepConfig.PerformLayout();
            this.tabBurst.ResumeLayout(false);
            this.grpBurstConfig.ResumeLayout(false);
            this.grpBurstConfig.PerformLayout();
            this.tabArbitrary.ResumeLayout(false);
            this.grpArbitraryConfig.ResumeLayout(false);
            this.grpArbitraryConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblIpAddress;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Label lblDeviceInfo;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabWaveform;
        private System.Windows.Forms.TabPage tabModulation;
        private System.Windows.Forms.TabPage tabSweep;
        private System.Windows.Forms.TabPage tabBurst;
        private System.Windows.Forms.TabPage tabArbitrary;
        private System.Windows.Forms.GroupBox grpWaveformConfig;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.ComboBox cmbChannel;
        private System.Windows.Forms.Label lblWaveformType;
        private System.Windows.Forms.ComboBox cmbWaveformType;
        private System.Windows.Forms.Label lblFrequency;
        private System.Windows.Forms.TextBox txtFrequency;
        private System.Windows.Forms.Label lblFrequencyUnit;
        private System.Windows.Forms.Label lblAmplitude;
        private System.Windows.Forms.TextBox txtAmplitude;
        private System.Windows.Forms.ComboBox cmbAmplitudeUnit;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Label lblOffsetUnit;
        private System.Windows.Forms.Label lblPhase;
        private System.Windows.Forms.TextBox txtPhase;
        private System.Windows.Forms.Label lblPhaseUnit;
        private System.Windows.Forms.Label lblDutyCycle;
        private System.Windows.Forms.TextBox txtDutyCycle;
        private System.Windows.Forms.Label lblDutyCycleUnit;
        private System.Windows.Forms.Label lblPulseWidth;
        private System.Windows.Forms.TextBox txtPulseWidth;
        private System.Windows.Forms.Label lblPulseWidthUnit;
        private System.Windows.Forms.Label lblRiseTime;
        private System.Windows.Forms.TextBox txtRiseTime;
        private System.Windows.Forms.Label lblRiseTimeUnit;
        private System.Windows.Forms.Label lblFallTime;
        private System.Windows.Forms.TextBox txtFallTime;
        private System.Windows.Forms.Label lblFallTimeUnit;
        private System.Windows.Forms.Label lblLoadImpedance;
        private System.Windows.Forms.ComboBox cmbLoadImpedance;
        private System.Windows.Forms.CheckBox chkOutputEnable;
        private System.Windows.Forms.Button btnSetWaveform;
        private System.Windows.Forms.Button btnQueryState;
        private System.Windows.Forms.GroupBox grpModulationConfig;
        private System.Windows.Forms.Label lblModChannel;
        private System.Windows.Forms.ComboBox cmbModChannel;
        private System.Windows.Forms.Label lblModulationType;
        private System.Windows.Forms.ComboBox cmbModulationType;
        private System.Windows.Forms.Label lblModulationSource;
        private System.Windows.Forms.ComboBox cmbModulationSource;
        private System.Windows.Forms.Label lblModulationWaveform;
        private System.Windows.Forms.ComboBox cmbModulationWaveform;
        private System.Windows.Forms.Label lblDepth;
        private System.Windows.Forms.TextBox txtDepth;
        private System.Windows.Forms.Label lblDepthUnit;
        private System.Windows.Forms.Label lblDeviation;
        private System.Windows.Forms.TextBox txtDeviation;
        private System.Windows.Forms.Label lblDeviationUnit;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Label lblRateUnit;
        private System.Windows.Forms.Label lblHopFrequency;
        private System.Windows.Forms.TextBox txtHopFrequency;
        private System.Windows.Forms.Label lblHopFrequencyUnit;
        private System.Windows.Forms.Label lblHopAmplitude;
        private System.Windows.Forms.TextBox txtHopAmplitude;
        private System.Windows.Forms.Label lblHopAmplitudeUnit;
        private System.Windows.Forms.Label lblHopPhase;
        private System.Windows.Forms.TextBox txtHopPhase;
        private System.Windows.Forms.Label lblHopPhaseUnit;
        private System.Windows.Forms.CheckBox chkModulationEnable;
        private System.Windows.Forms.Button btnConfigureModulation;
        private System.Windows.Forms.GroupBox grpSweepConfig;
        private System.Windows.Forms.Label lblSweepChannel;
        private System.Windows.Forms.ComboBox cmbSweepChannel;
        private System.Windows.Forms.Label lblStartFrequency;
        private System.Windows.Forms.TextBox txtStartFrequency;
        private System.Windows.Forms.Label lblStartFrequencyUnit;
        private System.Windows.Forms.Label lblStopFrequency;
        private System.Windows.Forms.TextBox txtStopFrequency;
        private System.Windows.Forms.Label lblStopFrequencyUnit;
        private System.Windows.Forms.Label lblSweepTime;
        private System.Windows.Forms.TextBox txtSweepTime;
        private System.Windows.Forms.Label lblSweepTimeUnit;
        private System.Windows.Forms.Label lblSweepType;
        private System.Windows.Forms.ComboBox cmbSweepType;
        private System.Windows.Forms.Label lblSweepDirection;
        private System.Windows.Forms.ComboBox cmbSweepDirection;
        private System.Windows.Forms.Label lblSweepTriggerSource;
        private System.Windows.Forms.ComboBox cmbSweepTriggerSource;
        private System.Windows.Forms.Label lblReturnTime;
        private System.Windows.Forms.TextBox txtReturnTime;
        private System.Windows.Forms.Label lblReturnTimeUnit;
        private System.Windows.Forms.Label lblHoldTime;
        private System.Windows.Forms.TextBox txtHoldTime;
        private System.Windows.Forms.Label lblHoldTimeUnit;
        private System.Windows.Forms.CheckBox chkSweepEnable;
        private System.Windows.Forms.Button btnConfigureSweep;
        private System.Windows.Forms.GroupBox grpBurstConfig;
        private System.Windows.Forms.Label lblBurstChannel;
        private System.Windows.Forms.ComboBox cmbBurstChannel;
        private System.Windows.Forms.Label lblBurstMode;
        private System.Windows.Forms.ComboBox cmbBurstMode;
        private System.Windows.Forms.Label lblCycles;
        private System.Windows.Forms.TextBox txtCycles;
        private System.Windows.Forms.Label lblPeriod;
        private System.Windows.Forms.TextBox txtPeriod;
        private System.Windows.Forms.Label lblPeriodUnit;
        private System.Windows.Forms.Label lblBurstTriggerSource;
        private System.Windows.Forms.ComboBox cmbBurstTriggerSource;
        private System.Windows.Forms.Label lblTriggerEdge;
        private System.Windows.Forms.ComboBox cmbTriggerEdge;
        private System.Windows.Forms.Label lblStartPhase;
        private System.Windows.Forms.TextBox txtStartPhase;
        private System.Windows.Forms.Label lblStartPhaseUnit;
        private System.Windows.Forms.Label lblGatePolarity;
        private System.Windows.Forms.ComboBox cmbGatePolarity;
        private System.Windows.Forms.CheckBox chkBurstEnable;
        private System.Windows.Forms.Button btnConfigureBurst;
        private System.Windows.Forms.GroupBox grpArbitraryConfig;
        private System.Windows.Forms.Label lblArbitraryChannel;
        private System.Windows.Forms.ComboBox cmbArbitraryChannel;
        private System.Windows.Forms.ListBox lstWaveforms;
        private System.Windows.Forms.Button btnRefreshList;
        private System.Windows.Forms.Button btnUploadWaveform;
        private System.Windows.Forms.Button btnDeleteWaveform;
        private System.Windows.Forms.Button btnSelectWaveform;
        private System.Windows.Forms.Label lblWaveformName;
        private System.Windows.Forms.TextBox txtWaveformName;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
