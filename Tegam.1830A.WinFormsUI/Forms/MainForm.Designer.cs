namespace Tegam.WinFormsUI.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpDeviceInfo = new System.Windows.Forms.GroupBox();
            this.lblDeviceInfoValue = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPower = new System.Windows.Forms.TabPage();
            this.tabFrequency = new System.Windows.Forms.TabPage();
            this.tabSensors = new System.Windows.Forms.TabPage();
            this.tabCalibration = new System.Windows.Forms.TabPage();
            this.tabLogging = new System.Windows.Forms.TabPage();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();

            this.grpConnection.SuspendLayout();
            this.grpDeviceInfo.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Tegam 1830A Power Meter Control";

            // grpConnection
            this.grpConnection.Controls.Add(this.lblIpAddress);
            this.grpConnection.Controls.Add(this.txtIpAddress);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.btnDisconnect);
            this.grpConnection.Controls.Add(this.lblConnectionStatus);
            this.grpConnection.Controls.Add(this.lblStatus);
            this.grpConnection.Location = new System.Drawing.Point(12, 36);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(760, 80);
            this.grpConnection.TabIndex = 1;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Connection";

            // lblIpAddress
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Location = new System.Drawing.Point(6, 22);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Size = new System.Drawing.Size(58, 13);
            this.lblIpAddress.TabIndex = 0;
            this.lblIpAddress.Text = "IP Address:";

            // txtIpAddress
            this.txtIpAddress.Location = new System.Drawing.Point(70, 19);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(150, 20);
            this.txtIpAddress.TabIndex = 1;
            this.txtIpAddress.Text = "192.168.1.100";

            // btnConnect
            this.btnConnect.Location = new System.Drawing.Point(226, 19);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            // btnDisconnect
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(307, 19);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);

            // lblConnectionStatus
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Location = new System.Drawing.Point(6, 48);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(40, 13);
            this.lblConnectionStatus.TabIndex = 4;
            this.lblConnectionStatus.Text = "Status:";

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(52, 48);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(68, 13);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Disconnected";

            // grpDeviceInfo
            this.grpDeviceInfo.Controls.Add(this.lblDeviceInfoValue);
            this.grpDeviceInfo.Location = new System.Drawing.Point(12, 122);
            this.grpDeviceInfo.Name = "grpDeviceInfo";
            this.grpDeviceInfo.Size = new System.Drawing.Size(760, 60);
            this.grpDeviceInfo.TabIndex = 2;
            this.grpDeviceInfo.TabStop = false;
            this.grpDeviceInfo.Text = "Device Information";

            // lblDeviceInfoValue
            this.lblDeviceInfoValue.AutoSize = true;
            this.lblDeviceInfoValue.Location = new System.Drawing.Point(6, 22);
            this.lblDeviceInfoValue.Name = "lblDeviceInfoValue";
            this.lblDeviceInfoValue.Size = new System.Drawing.Size(100, 13);
            this.lblDeviceInfoValue.TabIndex = 0;
            this.lblDeviceInfoValue.Text = "Not connected";

            // tabControl
            this.tabControl.Controls.Add(this.tabPower);
            this.tabControl.Controls.Add(this.tabFrequency);
            this.tabControl.Controls.Add(this.tabSensors);
            this.tabControl.Controls.Add(this.tabCalibration);
            this.tabControl.Controls.Add(this.tabLogging);
            this.tabControl.Location = new System.Drawing.Point(12, 188);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(760, 350);
            this.tabControl.TabIndex = 3;

            // tabPower
            this.tabPower.Location = new System.Drawing.Point(4, 22);
            this.tabPower.Name = "tabPower";
            this.tabPower.Padding = new System.Windows.Forms.Padding(3);
            this.tabPower.Size = new System.Drawing.Size(752, 324);
            this.tabPower.TabIndex = 0;
            this.tabPower.Text = "Power Measurement";
            this.tabPower.UseVisualStyleBackColor = true;

            // tabFrequency
            this.tabFrequency.Location = new System.Drawing.Point(4, 22);
            this.tabFrequency.Name = "tabFrequency";
            this.tabFrequency.Padding = new System.Windows.Forms.Padding(3);
            this.tabFrequency.Size = new System.Drawing.Size(752, 324);
            this.tabFrequency.TabIndex = 1;
            this.tabFrequency.Text = "Frequency Configuration";
            this.tabFrequency.UseVisualStyleBackColor = true;

            // tabSensors
            this.tabSensors.Location = new System.Drawing.Point(4, 22);
            this.tabSensors.Name = "tabSensors";
            this.tabSensors.Padding = new System.Windows.Forms.Padding(3);
            this.tabSensors.Size = new System.Drawing.Size(752, 324);
            this.tabSensors.TabIndex = 2;
            this.tabSensors.Text = "Sensor Management";
            this.tabSensors.UseVisualStyleBackColor = true;

            // tabCalibration
            this.tabCalibration.Location = new System.Drawing.Point(4, 22);
            this.tabCalibration.Name = "tabCalibration";
            this.tabCalibration.Padding = new System.Windows.Forms.Padding(3);
            this.tabCalibration.Size = new System.Drawing.Size(752, 324);
            this.tabCalibration.TabIndex = 3;
            this.tabCalibration.Text = "Calibration";
            this.tabCalibration.UseVisualStyleBackColor = true;

            // tabLogging
            this.tabLogging.Location = new System.Drawing.Point(4, 22);
            this.tabLogging.Name = "tabLogging";
            this.tabLogging.Padding = new System.Windows.Forms.Padding(3);
            this.tabLogging.Size = new System.Drawing.Size(752, 324);
            this.tabLogging.TabIndex = 4;
            this.tabLogging.Text = "Data Logging";
            this.tabLogging.UseVisualStyleBackColor = true;

            // statusStrip
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 539);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";

            // toolStripStatusLabel
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel.Text = "Ready";

            // MainForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.grpDeviceInfo);
            this.Controls.Add(this.grpConnection);
            this.Controls.Add(this.lblTitle);
            this.Name = "MainForm";
            this.Text = "Tegam 1830A Power Meter Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.grpDeviceInfo.ResumeLayout(false);
            this.grpDeviceInfo.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblIpAddress;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox grpDeviceInfo;
        private System.Windows.Forms.Label lblDeviceInfoValue;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPower;
        private System.Windows.Forms.TabPage tabFrequency;
        private System.Windows.Forms.TabPage tabSensors;
        private System.Windows.Forms.TabPage tabCalibration;
        private System.Windows.Forms.TabPage tabLogging;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}
