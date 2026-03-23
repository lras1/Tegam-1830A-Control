using Tegam.WinFormsUI.Controls;

namespace Tegam.WinFormsUI.Forms
{
    partial class EnhancedLoggingPanel
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpFileControl = new System.Windows.Forms.GroupBox();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.pnlStatusIndicator = new System.Windows.Forms.Panel();
            this.lblStatusText = new System.Windows.Forms.Label();
            this.lblFilePath2 = new System.Windows.Forms.Label();
            this.lblFilePathValue = new System.Windows.Forms.Label();
            this.lblEntryCount = new System.Windows.Forms.Label();
            this.lblEntryCountValue = new System.Windows.Forms.Label();
            this.btnStartLogging = new System.Windows.Forms.Button();
            this.btnStopLogging = new System.Windows.Forms.Button();
            this.grpManualSampling = new System.Windows.Forms.GroupBox();
            this.btnMeasureNow = new System.Windows.Forms.Button();
            this.grpAutomaticSampling = new System.Windows.Forms.GroupBox();
            this.lblSampleRate = new System.Windows.Forms.Label();
            this.txtSampleRate = new System.Windows.Forms.TextBox();
            this.lblSampleRateUnit = new System.Windows.Forms.Label();
            this.lblSampleCount = new System.Windows.Forms.Label();
            this.txtSampleCount = new System.Windows.Forms.TextBox();
            this.btnStartAuto = new System.Windows.Forms.Button();
            this.btnStopAuto = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblProgressValue = new System.Windows.Forms.Label();
            this.grpRecentEntries = new System.Windows.Forms.GroupBox();
            this.logDisplayControl = new LogDisplayControl();
            this.btnViewLogWindow = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();

            this.grpFileControl.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.grpManualSampling.SuspendLayout();
            this.grpAutomaticSampling.SuspendLayout();
            this.grpRecentEntries.SuspendLayout();
            this.SuspendLayout();

            // grpFileControl
            this.grpFileControl.Controls.Add(this.lblFilePath);
            this.grpFileControl.Controls.Add(this.txtFilePath);
            this.grpFileControl.Controls.Add(this.btnBrowse);
            this.grpFileControl.Location = new System.Drawing.Point(10, 10);
            this.grpFileControl.Name = "grpFileControl";
            this.grpFileControl.Size = new System.Drawing.Size(720, 60);
            this.grpFileControl.TabIndex = 0;
            this.grpFileControl.TabStop = false;
            this.grpFileControl.Text = "File Configuration";

            // lblFilePath
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Location = new System.Drawing.Point(10, 25);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(26, 13);
            this.lblFilePath.TabIndex = 0;
            this.lblFilePath.Text = "File:";

            // txtFilePath
            this.txtFilePath.Location = new System.Drawing.Point(42, 22);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(500, 20);
            this.txtFilePath.TabIndex = 1;
            this.txtFilePath.Text = "enhanced_log.csv";

            // btnBrowse
            this.btnBrowse.Location = new System.Drawing.Point(548, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // grpStatus
            this.grpStatus.Controls.Add(this.pnlStatusIndicator);
            this.grpStatus.Controls.Add(this.lblStatusText);
            this.grpStatus.Controls.Add(this.lblFilePath2);
            this.grpStatus.Controls.Add(this.lblFilePathValue);
            this.grpStatus.Controls.Add(this.lblEntryCount);
            this.grpStatus.Controls.Add(this.lblEntryCountValue);
            this.grpStatus.Controls.Add(this.btnStartLogging);
            this.grpStatus.Controls.Add(this.btnStopLogging);
            this.grpStatus.Location = new System.Drawing.Point(10, 76);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(720, 100);
            this.grpStatus.TabIndex = 1;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Logging Status";

            // pnlStatusIndicator
            this.pnlStatusIndicator.BackColor = System.Drawing.Color.Gray;
            this.pnlStatusIndicator.Location = new System.Drawing.Point(10, 22);
            this.pnlStatusIndicator.Name = "pnlStatusIndicator";
            this.pnlStatusIndicator.Size = new System.Drawing.Size(20, 20);
            this.pnlStatusIndicator.TabIndex = 0;

            // lblStatusText
            this.lblStatusText.AutoSize = true;
            this.lblStatusText.ForeColor = System.Drawing.Color.Gray;
            this.lblStatusText.Location = new System.Drawing.Point(36, 25);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(43, 13);
            this.lblStatusText.TabIndex = 1;
            this.lblStatusText.Text = "Inactive";

            // lblFilePath2
            this.lblFilePath2.AutoSize = true;
            this.lblFilePath2.Location = new System.Drawing.Point(10, 50);
            this.lblFilePath2.Name = "lblFilePath2";
            this.lblFilePath2.Size = new System.Drawing.Size(26, 13);
            this.lblFilePath2.TabIndex = 2;
            this.lblFilePath2.Text = "File:";

            // lblFilePathValue
            this.lblFilePathValue.AutoSize = true;
            this.lblFilePathValue.Location = new System.Drawing.Point(42, 50);
            this.lblFilePathValue.Name = "lblFilePathValue";
            this.lblFilePathValue.Size = new System.Drawing.Size(27, 13);
            this.lblFilePathValue.TabIndex = 3;
            this.lblFilePathValue.Text = "N/A";

            // lblEntryCount
            this.lblEntryCount.AutoSize = true;
            this.lblEntryCount.Location = new System.Drawing.Point(10, 70);
            this.lblEntryCount.Name = "lblEntryCount";
            this.lblEntryCount.Size = new System.Drawing.Size(42, 13);
            this.lblEntryCount.TabIndex = 4;
            this.lblEntryCount.Text = "Entries:";

            // lblEntryCountValue
            this.lblEntryCountValue.AutoSize = true;
            this.lblEntryCountValue.Location = new System.Drawing.Point(58, 70);
            this.lblEntryCountValue.Name = "lblEntryCountValue";
            this.lblEntryCountValue.Size = new System.Drawing.Size(13, 13);
            this.lblEntryCountValue.TabIndex = 5;
            this.lblEntryCountValue.Text = "0";

            // btnStartLogging
            this.btnStartLogging.Location = new System.Drawing.Point(500, 20);
            this.btnStartLogging.Name = "btnStartLogging";
            this.btnStartLogging.Size = new System.Drawing.Size(100, 30);
            this.btnStartLogging.TabIndex = 6;
            this.btnStartLogging.Text = "Start Logging";
            this.btnStartLogging.UseVisualStyleBackColor = true;
            this.btnStartLogging.Click += new System.EventHandler(this.btnStartLogging_Click);

            // btnStopLogging
            this.btnStopLogging.Enabled = false;
            this.btnStopLogging.Location = new System.Drawing.Point(606, 20);
            this.btnStopLogging.Name = "btnStopLogging";
            this.btnStopLogging.Size = new System.Drawing.Size(100, 30);
            this.btnStopLogging.TabIndex = 7;
            this.btnStopLogging.Text = "Stop Logging";
            this.btnStopLogging.UseVisualStyleBackColor = true;
            this.btnStopLogging.Click += new System.EventHandler(this.btnStopLogging_Click);

            // grpManualSampling
            this.grpManualSampling.Controls.Add(this.btnMeasureNow);
            this.grpManualSampling.Location = new System.Drawing.Point(10, 182);
            this.grpManualSampling.Name = "grpManualSampling";
            this.grpManualSampling.Size = new System.Drawing.Size(150, 60);
            this.grpManualSampling.TabIndex = 2;
            this.grpManualSampling.TabStop = false;
            this.grpManualSampling.Text = "Manual Sampling";

            // btnMeasureNow
            this.btnMeasureNow.Location = new System.Drawing.Point(10, 22);
            this.btnMeasureNow.Name = "btnMeasureNow";
            this.btnMeasureNow.Size = new System.Drawing.Size(120, 25);
            this.btnMeasureNow.TabIndex = 0;
            this.btnMeasureNow.Text = "Measure Now";
            this.btnMeasureNow.UseVisualStyleBackColor = true;
            this.btnMeasureNow.Click += new System.EventHandler(this.btnMeasureNow_Click);

            // grpAutomaticSampling
            this.grpAutomaticSampling.Controls.Add(this.lblSampleRate);
            this.grpAutomaticSampling.Controls.Add(this.txtSampleRate);
            this.grpAutomaticSampling.Controls.Add(this.lblSampleRateUnit);
            this.grpAutomaticSampling.Controls.Add(this.lblSampleCount);
            this.grpAutomaticSampling.Controls.Add(this.txtSampleCount);
            this.grpAutomaticSampling.Controls.Add(this.btnStartAuto);
            this.grpAutomaticSampling.Controls.Add(this.btnStopAuto);
            this.grpAutomaticSampling.Controls.Add(this.lblProgress);
            this.grpAutomaticSampling.Controls.Add(this.lblProgressValue);
            this.grpAutomaticSampling.Location = new System.Drawing.Point(170, 182);
            this.grpAutomaticSampling.Name = "grpAutomaticSampling";
            this.grpAutomaticSampling.Size = new System.Drawing.Size(560, 60);
            this.grpAutomaticSampling.TabIndex = 3;
            this.grpAutomaticSampling.TabStop = false;
            this.grpAutomaticSampling.Text = "Automatic Sampling";

            // lblSampleRate
            this.lblSampleRate.AutoSize = true;
            this.lblSampleRate.Location = new System.Drawing.Point(10, 25);
            this.lblSampleRate.Name = "lblSampleRate";
            this.lblSampleRate.Size = new System.Drawing.Size(70, 13);
            this.lblSampleRate.TabIndex = 0;
            this.lblSampleRate.Text = "Sample Rate:";

            // txtSampleRate
            this.txtSampleRate.Location = new System.Drawing.Point(86, 22);
            this.txtSampleRate.Name = "txtSampleRate";
            this.txtSampleRate.Size = new System.Drawing.Size(60, 20);
            this.txtSampleRate.TabIndex = 1;
            this.txtSampleRate.Text = "1000";

            // lblSampleRateUnit
            this.lblSampleRateUnit.AutoSize = true;
            this.lblSampleRateUnit.Location = new System.Drawing.Point(152, 25);
            this.lblSampleRateUnit.Name = "lblSampleRateUnit";
            this.lblSampleRateUnit.Size = new System.Drawing.Size(20, 13);
            this.lblSampleRateUnit.TabIndex = 2;
            this.lblSampleRateUnit.Text = "ms";

            // lblSampleCount
            this.lblSampleCount.AutoSize = true;
            this.lblSampleCount.Location = new System.Drawing.Point(190, 25);
            this.lblSampleCount.Name = "lblSampleCount";
            this.lblSampleCount.Size = new System.Drawing.Size(38, 13);
            this.lblSampleCount.TabIndex = 3;
            this.lblSampleCount.Text = "Count:";

            // txtSampleCount
            this.txtSampleCount.Location = new System.Drawing.Point(234, 22);
            this.txtSampleCount.Name = "txtSampleCount";
            this.txtSampleCount.Size = new System.Drawing.Size(60, 20);
            this.txtSampleCount.TabIndex = 4;
            this.txtSampleCount.Text = "100";

            // btnStartAuto
            this.btnStartAuto.Location = new System.Drawing.Point(310, 20);
            this.btnStartAuto.Name = "btnStartAuto";
            this.btnStartAuto.Size = new System.Drawing.Size(80, 25);
            this.btnStartAuto.TabIndex = 5;
            this.btnStartAuto.Text = "Start Auto";
            this.btnStartAuto.UseVisualStyleBackColor = true;
            this.btnStartAuto.Click += new System.EventHandler(this.btnStartAuto_Click);

            // btnStopAuto
            this.btnStopAuto.Enabled = false;
            this.btnStopAuto.Location = new System.Drawing.Point(396, 20);
            this.btnStopAuto.Name = "btnStopAuto";
            this.btnStopAuto.Size = new System.Drawing.Size(80, 25);
            this.btnStopAuto.TabIndex = 6;
            this.btnStopAuto.Text = "Stop Auto";
            this.btnStopAuto.UseVisualStyleBackColor = true;
            this.btnStopAuto.Click += new System.EventHandler(this.btnStopAuto_Click);

            // lblProgress
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(482, 25);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(51, 13);
            this.lblProgress.TabIndex = 7;
            this.lblProgress.Text = "Progress:";

            // lblProgressValue
            this.lblProgressValue.AutoSize = true;
            this.lblProgressValue.Location = new System.Drawing.Point(539, 25);
            this.lblProgressValue.Name = "lblProgressValue";
            this.lblProgressValue.Size = new System.Drawing.Size(24, 13);
            this.lblProgressValue.TabIndex = 8;
            this.lblProgressValue.Text = "0/0";

            // grpRecentEntries
            this.grpRecentEntries.Controls.Add(this.btnViewLogWindow);
            this.grpRecentEntries.Controls.Add(this.logDisplayControl);
            this.grpRecentEntries.Location = new System.Drawing.Point(10, 248);
            this.grpRecentEntries.Name = "grpRecentEntries";
            this.grpRecentEntries.Size = new System.Drawing.Size(720, 380);
            this.grpRecentEntries.TabIndex = 4;
            this.grpRecentEntries.TabStop = false;
            this.grpRecentEntries.Text = "Recent Log Entries";

            // btnViewLogWindow
            this.btnViewLogWindow.Location = new System.Drawing.Point(10, 22);
            this.btnViewLogWindow.Name = "btnViewLogWindow";
            this.btnViewLogWindow.Size = new System.Drawing.Size(140, 30);
            this.btnViewLogWindow.TabIndex = 0;
            this.btnViewLogWindow.Text = "View Log Window";
            this.btnViewLogWindow.UseVisualStyleBackColor = true;
            this.btnViewLogWindow.Click += new System.EventHandler(this.btnViewLogWindow_Click);

            // logDisplayControl
            this.logDisplayControl.Location = new System.Drawing.Point(10, 58);
            this.logDisplayControl.Name = "logDisplayControl";
            this.logDisplayControl.Size = new System.Drawing.Size(700, 310);
            this.logDisplayControl.TabIndex = 1;

            // saveFileDialog
            this.saveFileDialog.DefaultExt = "csv";
            this.saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            // EnhancedLoggingPanel
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.grpRecentEntries);
            this.Controls.Add(this.grpAutomaticSampling);
            this.Controls.Add(this.grpManualSampling);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.grpFileControl);
            this.Name = "EnhancedLoggingPanel";
            this.Size = new System.Drawing.Size(740, 640);

            this.grpFileControl.ResumeLayout(false);
            this.grpFileControl.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.grpManualSampling.ResumeLayout(false);
            this.grpAutomaticSampling.ResumeLayout(false);
            this.grpAutomaticSampling.PerformLayout();
            this.grpRecentEntries.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpFileControl;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Panel pnlStatusIndicator;
        private System.Windows.Forms.Label lblStatusText;
        private System.Windows.Forms.Label lblFilePath2;
        private System.Windows.Forms.Label lblFilePathValue;
        private System.Windows.Forms.Label lblEntryCount;
        private System.Windows.Forms.Label lblEntryCountValue;
        private System.Windows.Forms.Button btnStartLogging;
        private System.Windows.Forms.Button btnStopLogging;
        private System.Windows.Forms.GroupBox grpManualSampling;
        private System.Windows.Forms.Button btnMeasureNow;
        private System.Windows.Forms.GroupBox grpAutomaticSampling;
        private System.Windows.Forms.Label lblSampleRate;
        private System.Windows.Forms.TextBox txtSampleRate;
        private System.Windows.Forms.Label lblSampleRateUnit;
        private System.Windows.Forms.Label lblSampleCount;
        private System.Windows.Forms.TextBox txtSampleCount;
        private System.Windows.Forms.Button btnStartAuto;
        private System.Windows.Forms.Button btnStopAuto;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblProgressValue;
        private System.Windows.Forms.GroupBox grpRecentEntries;
        private LogDisplayControl logDisplayControl;
        private System.Windows.Forms.Button btnViewLogWindow;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}
