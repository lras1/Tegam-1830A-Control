using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Logging;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam.WinFormsUI.Controllers;
using Tegam.WinFormsUI.Controls;

namespace Tegam.WinFormsUI.Forms
{
    /// <summary>
    /// Floating window for viewing and controlling log entries - mirrors the main logging panel.
    /// </summary>
    public partial class LogViewerForm : Form
    {
        private readonly ILogManager _logManager;
        private readonly EnhancedLoggingController _controller;
        private readonly EnhancedLoggingPanel _mainPanel;
        
        // Controls
        private GroupBox grpFileControl;
        private Label lblFilePath;
        private TextBox txtFilePath;
        private Button btnBrowse;
        
        private GroupBox grpStatus;
        private Panel pnlStatusIndicator;
        private Label lblStatusText;
        private Label lblFilePath2;
        private Label lblFilePathValue;
        private Label lblEntryCount;
        private Label lblEntryCountValue;
        private Button btnStartLogging;
        private Button btnStopLogging;
        
        private GroupBox grpManualSampling;
        private Button btnMeasureNow;
        
        private GroupBox grpAutomaticSampling;
        private Label lblSampleRate;
        private TextBox txtSampleRate;
        private Label lblSampleRateUnit;
        private Label lblSampleCount;
        private TextBox txtSampleCount;
        private Button btnStartAuto;
        private Button btnStopAuto;
        private Label lblProgress;
        private Label lblProgressValue;
        
        private GroupBox grpLogDisplay;
        private LogDisplayControl logDisplayControl;
        
        private SaveFileDialog saveFileDialog;

        public LogViewerForm(ILogManager logManager, EnhancedLoggingController controller, EnhancedLoggingPanel mainPanel)
        {
            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _mainPanel = mainPanel ?? throw new ArgumentNullException(nameof(mainPanel));
            
            InitializeComponent();
            
            // Disable auto-scroll in the floating window
            logDisplayControl.SetAutoScroll(false);
            
            SubscribeToEvents();
            SyncWithMainPanel();
            UpdateUIState();
            
            // Set form properties
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Log Viewer";
        }

        private void InitializeComponent()
        {
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
            this.grpLogDisplay = new System.Windows.Forms.GroupBox();
            this.logDisplayControl = new Tegam.WinFormsUI.Controls.LogDisplayControl();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.grpFileControl.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.grpManualSampling.SuspendLayout();
            this.grpAutomaticSampling.SuspendLayout();
            this.grpLogDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpFileControl
            // 
            this.grpFileControl.Controls.Add(this.lblFilePath);
            this.grpFileControl.Controls.Add(this.txtFilePath);
            this.grpFileControl.Controls.Add(this.btnBrowse);
            this.grpFileControl.Location = new System.Drawing.Point(10, 10);
            this.grpFileControl.Name = "grpFileControl";
            this.grpFileControl.Size = new System.Drawing.Size(760, 60);
            this.grpFileControl.TabIndex = 4;
            this.grpFileControl.TabStop = false;
            this.grpFileControl.Text = "File Configuration";
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Location = new System.Drawing.Point(10, 25);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(69, 32);
            this.lblFilePath.TabIndex = 0;
            this.lblFilePath.Text = "File:";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(42, 22);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(540, 38);
            this.txtFilePath.TabIndex = 1;
            this.txtFilePath.Text = "enhanced_log.csv";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(588, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            // 
            // grpStatus
            // 
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
            this.grpStatus.Size = new System.Drawing.Size(760, 100);
            this.grpStatus.TabIndex = 3;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Logging Status";
            // 
            // pnlStatusIndicator
            // 
            this.pnlStatusIndicator.BackColor = System.Drawing.Color.Gray;
            this.pnlStatusIndicator.Location = new System.Drawing.Point(10, 22);
            this.pnlStatusIndicator.Name = "pnlStatusIndicator";
            this.pnlStatusIndicator.Size = new System.Drawing.Size(20, 20);
            this.pnlStatusIndicator.TabIndex = 0;
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = true;
            this.lblStatusText.ForeColor = System.Drawing.Color.Gray;
            this.lblStatusText.Location = new System.Drawing.Point(36, 25);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(112, 32);
            this.lblStatusText.TabIndex = 1;
            this.lblStatusText.Text = "Inactive";
            // 
            // lblFilePath2
            // 
            this.lblFilePath2.AutoSize = true;
            this.lblFilePath2.Location = new System.Drawing.Point(10, 50);
            this.lblFilePath2.Name = "lblFilePath2";
            this.lblFilePath2.Size = new System.Drawing.Size(69, 32);
            this.lblFilePath2.TabIndex = 2;
            this.lblFilePath2.Text = "File:";
            // 
            // lblFilePathValue
            // 
            this.lblFilePathValue.AutoSize = true;
            this.lblFilePathValue.Location = new System.Drawing.Point(42, 50);
            this.lblFilePathValue.Name = "lblFilePathValue";
            this.lblFilePathValue.Size = new System.Drawing.Size(61, 32);
            this.lblFilePathValue.TabIndex = 3;
            this.lblFilePathValue.Text = "N/A";
            // 
            // lblEntryCount
            // 
            this.lblEntryCount.AutoSize = true;
            this.lblEntryCount.Location = new System.Drawing.Point(10, 70);
            this.lblEntryCount.Name = "lblEntryCount";
            this.lblEntryCount.Size = new System.Drawing.Size(111, 32);
            this.lblEntryCount.TabIndex = 4;
            this.lblEntryCount.Text = "Entries:";
            // 
            // lblEntryCountValue
            // 
            this.lblEntryCountValue.AutoSize = true;
            this.lblEntryCountValue.Location = new System.Drawing.Point(58, 70);
            this.lblEntryCountValue.Name = "lblEntryCountValue";
            this.lblEntryCountValue.Size = new System.Drawing.Size(30, 32);
            this.lblEntryCountValue.TabIndex = 5;
            this.lblEntryCountValue.Text = "0";
            // 
            // btnStartLogging
            // 
            this.btnStartLogging.Location = new System.Drawing.Point(540, 20);
            this.btnStartLogging.Name = "btnStartLogging";
            this.btnStartLogging.Size = new System.Drawing.Size(100, 30);
            this.btnStartLogging.TabIndex = 6;
            this.btnStartLogging.Text = "Start Logging";
            // 
            // btnStopLogging
            // 
            this.btnStopLogging.Enabled = false;
            this.btnStopLogging.Location = new System.Drawing.Point(646, 20);
            this.btnStopLogging.Name = "btnStopLogging";
            this.btnStopLogging.Size = new System.Drawing.Size(100, 30);
            this.btnStopLogging.TabIndex = 7;
            this.btnStopLogging.Text = "Stop Logging";
            // 
            // grpManualSampling
            // 
            this.grpManualSampling.Controls.Add(this.btnMeasureNow);
            this.grpManualSampling.Location = new System.Drawing.Point(10, 182);
            this.grpManualSampling.Name = "grpManualSampling";
            this.grpManualSampling.Size = new System.Drawing.Size(150, 60);
            this.grpManualSampling.TabIndex = 2;
            this.grpManualSampling.TabStop = false;
            this.grpManualSampling.Text = "Manual Sampling";
            // 
            // btnMeasureNow
            // 
            this.btnMeasureNow.Location = new System.Drawing.Point(10, 22);
            this.btnMeasureNow.Name = "btnMeasureNow";
            this.btnMeasureNow.Size = new System.Drawing.Size(120, 25);
            this.btnMeasureNow.TabIndex = 0;
            this.btnMeasureNow.Text = "Measure Now";
            // 
            // grpAutomaticSampling
            // 
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
            this.grpAutomaticSampling.Size = new System.Drawing.Size(600, 60);
            this.grpAutomaticSampling.TabIndex = 1;
            this.grpAutomaticSampling.TabStop = false;
            this.grpAutomaticSampling.Text = "Automatic Sampling";
            // 
            // lblSampleRate
            // 
            this.lblSampleRate.AutoSize = true;
            this.lblSampleRate.Location = new System.Drawing.Point(10, 25);
            this.lblSampleRate.Name = "lblSampleRate";
            this.lblSampleRate.Size = new System.Drawing.Size(186, 32);
            this.lblSampleRate.TabIndex = 0;
            this.lblSampleRate.Text = "Sample Rate:";
            // 
            // txtSampleRate
            // 
            this.txtSampleRate.Location = new System.Drawing.Point(86, 22);
            this.txtSampleRate.Name = "txtSampleRate";
            this.txtSampleRate.Size = new System.Drawing.Size(60, 38);
            this.txtSampleRate.TabIndex = 1;
            this.txtSampleRate.Text = "1000";
            // 
            // lblSampleRateUnit
            // 
            this.lblSampleRateUnit.AutoSize = true;
            this.lblSampleRateUnit.Location = new System.Drawing.Point(152, 25);
            this.lblSampleRateUnit.Name = "lblSampleRateUnit";
            this.lblSampleRateUnit.Size = new System.Drawing.Size(51, 32);
            this.lblSampleRateUnit.TabIndex = 2;
            this.lblSampleRateUnit.Text = "ms";
            // 
            // lblSampleCount
            // 
            this.lblSampleCount.AutoSize = true;
            this.lblSampleCount.Location = new System.Drawing.Point(190, 25);
            this.lblSampleCount.Name = "lblSampleCount";
            this.lblSampleCount.Size = new System.Drawing.Size(98, 32);
            this.lblSampleCount.TabIndex = 3;
            this.lblSampleCount.Text = "Count:";
            // 
            // txtSampleCount
            // 
            this.txtSampleCount.Location = new System.Drawing.Point(234, 22);
            this.txtSampleCount.Name = "txtSampleCount";
            this.txtSampleCount.Size = new System.Drawing.Size(60, 38);
            this.txtSampleCount.TabIndex = 4;
            this.txtSampleCount.Text = "100";
            // 
            // btnStartAuto
            // 
            this.btnStartAuto.Location = new System.Drawing.Point(310, 20);
            this.btnStartAuto.Name = "btnStartAuto";
            this.btnStartAuto.Size = new System.Drawing.Size(80, 25);
            this.btnStartAuto.TabIndex = 5;
            this.btnStartAuto.Text = "Start Auto";
            // 
            // btnStopAuto
            // 
            this.btnStopAuto.Enabled = false;
            this.btnStopAuto.Location = new System.Drawing.Point(396, 20);
            this.btnStopAuto.Name = "btnStopAuto";
            this.btnStopAuto.Size = new System.Drawing.Size(80, 25);
            this.btnStopAuto.TabIndex = 6;
            this.btnStopAuto.Text = "Stop Auto";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(482, 25);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(135, 32);
            this.lblProgress.TabIndex = 7;
            this.lblProgress.Text = "Progress:";
            // 
            // lblProgressValue
            // 
            this.lblProgressValue.AutoSize = true;
            this.lblProgressValue.Location = new System.Drawing.Point(539, 25);
            this.lblProgressValue.Name = "lblProgressValue";
            this.lblProgressValue.Size = new System.Drawing.Size(54, 32);
            this.lblProgressValue.TabIndex = 8;
            this.lblProgressValue.Text = "0/0";
            // 
            // grpLogDisplay
            // 
            this.grpLogDisplay.Controls.Add(this.logDisplayControl);
            this.grpLogDisplay.Location = new System.Drawing.Point(10, 328);
            this.grpLogDisplay.Name = "grpLogDisplay";
            this.grpLogDisplay.Size = new System.Drawing.Size(1268, 479);
            this.grpLogDisplay.TabIndex = 0;
            this.grpLogDisplay.TabStop = false;
            this.grpLogDisplay.Text = "Log Entries";
            // 
            // logDisplayControl
            // 
            this.logDisplayControl.Location = new System.Drawing.Point(42, 83);
            this.logDisplayControl.Name = "logDisplayControl";
            this.logDisplayControl.Size = new System.Drawing.Size(1260, 350);
            this.logDisplayControl.TabIndex = 0;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "csv";
            this.saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            // 
            // LogViewerForm
            // 
            this.ClientSize = new System.Drawing.Size(1394, 844);
            this.Controls.Add(this.grpLogDisplay);
            this.Controls.Add(this.grpAutomaticSampling);
            this.Controls.Add(this.grpManualSampling);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.grpFileControl);
            this.MinimumSize = new System.Drawing.Size(796, 679);
            this.Name = "LogViewerForm";
            this.grpFileControl.ResumeLayout(false);
            this.grpFileControl.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.grpManualSampling.ResumeLayout(false);
            this.grpAutomaticSampling.ResumeLayout(false);
            this.grpAutomaticSampling.PerformLayout();
            this.grpLogDisplay.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void SubscribeToEvents()
        {
            _logManager.EntryLogged += LogManager_EntryLogged;
            _logManager.StateChanged += LogManager_StateChanged;
            _logManager.WriteError += LogManager_WriteError;
            _controller.AutoSamplingProgress += Controller_AutoSamplingProgress;
            _controller.AutoSamplingCompleted += Controller_AutoSamplingCompleted;
        }

        private void SyncWithMainPanel()
        {
            // Sync file path from main panel
            var mainFilePath = _mainPanel.Controls.Find("txtFilePath", true);
            if (mainFilePath.Length > 0 && mainFilePath[0] is TextBox mainTxt)
            {
                txtFilePath.Text = mainTxt.Text;
            }
            
            // Sync sample rate from main panel
            var mainSampleRate = _mainPanel.Controls.Find("txtSampleRate", true);
            if (mainSampleRate.Length > 0 && mainSampleRate[0] is TextBox mainRate)
            {
                txtSampleRate.Text = mainRate.Text;
            }
            
            // Sync sample count from main panel
            var mainSampleCount = _mainPanel.Controls.Find("txtSampleCount", true);
            if (mainSampleCount.Length > 0 && mainSampleCount[0] is TextBox mainCount)
            {
                txtSampleCount.Text = mainCount.Text;
            }
        }

        private void LogManager_EntryLogged(object sender, LogEntryEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, LogEntryEventArgs>(LogManager_EntryLogged), sender, e);
                return;
            }

            logDisplayControl.AddEntry(e.Entry);
            lblEntryCountValue.Text = _logManager.TotalEntryCount.ToString();
        }

        private void LogManager_StateChanged(object sender, LoggingStateChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, LoggingStateChangedEventArgs>(LogManager_StateChanged), sender, e);
                return;
            }

            UpdateUIState();
        }

        private void LogManager_WriteError(object sender, WriteErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, WriteErrorEventArgs>(LogManager_WriteError), sender, e);
                return;
            }

            MessageBox.Show("Write error: " + e.ErrorMessage, "Logging Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Controller_AutoSamplingProgress(object sender, int completedCount)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, int>(Controller_AutoSamplingProgress), sender, completedCount);
                return;
            }

            var config = _controller.GetAutoSamplingConfig();
            if (config != null)
            {
                lblProgressValue.Text = string.Format("{0}/{1}", completedCount, config.SampleCount);
            }
        }

        private void Controller_AutoSamplingCompleted(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(Controller_AutoSamplingCompleted), sender, e);
                return;
            }

            UpdateUIState();
            MessageBox.Show("Automatic sampling completed.", "Info", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateUIState()
        {
            var state = _logManager.CurrentState;
            var autoConfig = _controller.GetAutoSamplingConfig();
            bool isAutoActive = autoConfig != null && autoConfig.IsActive;

            // Update status indicator
            if (state == LoggingState.Active)
            {
                pnlStatusIndicator.BackColor = Color.Green;
                lblStatusText.Text = "Active";
                lblStatusText.ForeColor = Color.Green;
            }
            else
            {
                pnlStatusIndicator.BackColor = Color.Gray;
                lblStatusText.Text = "Inactive";
                lblStatusText.ForeColor = Color.Gray;
            }

            // Update file path display
            lblFilePathValue.Text = _logManager.CurrentLogFile ?? "N/A";
            lblEntryCountValue.Text = _logManager.TotalEntryCount.ToString();

            // Update button states
            btnStartLogging.Enabled = state != LoggingState.Active;
            btnStopLogging.Enabled = state == LoggingState.Active;
            txtFilePath.Enabled = state != LoggingState.Active;
            btnBrowse.Enabled = state != LoggingState.Active;

            // Manual sampling always enabled
            btnMeasureNow.Enabled = true;

            // Automatic sampling controls
            btnStartAuto.Enabled = !isAutoActive;
            btnStopAuto.Enabled = isAutoActive;
            txtSampleRate.Enabled = !isAutoActive;
            txtSampleCount.Enabled = !isAutoActive;

            // Update progress display
            if (isAutoActive)
            {
                lblProgressValue.Text = string.Format("{0}/{1}", 
                    autoConfig.CompletedCount, autoConfig.SampleCount);
            }
            else
            {
                lblProgressValue.Text = "0/0";
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            _controller.LogButtonAction("Browse (Viewer)", "Clicked");
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = saveFileDialog.FileName;
                _controller.LogButtonAction("Browse (Viewer)", "File selected: " + saveFileDialog.FileName);
            }
        }

        private void btnStartLogging_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.LogButtonAction("Start Logging (Viewer)", "Clicked");
                string filename = txtFilePath.Text.Trim();
                if (string.IsNullOrEmpty(filename))
                {
                    MessageBox.Show("Please enter a filename.", "Input Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _logManager.StartLogging(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting logging: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStopLogging_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.LogButtonAction("Stop Logging (Viewer)", "Clicked");
                _logManager.StopLogging();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping logging: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMeasureNow_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.LogButtonAction("Measure Now (Viewer)", "Clicked");
                _controller.ManualSample();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during manual sample: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartAuto_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.LogButtonAction("Start Auto (Viewer)", "Clicked");
                if (!int.TryParse(txtSampleRate.Text, out int rateMs) || rateMs < 100 || rateMs > 60000)
                {
                    MessageBox.Show("Sample rate must be between 100 and 60000 milliseconds.", 
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtSampleCount.Text, out int count) || count < 1 || count > 10000)
                {
                    MessageBox.Show("Sample count must be between 1 and 10000.", 
                        "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _controller.StartAutomaticSampling(rateMs, count);
                UpdateUIState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting automatic sampling: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStopAuto_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.LogButtonAction("Stop Auto (Viewer)", "Clicked");
                _controller.StopAutomaticSampling();
                UpdateUIState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping automatic sampling: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Unsubscribe from events
            _logManager.EntryLogged -= LogManager_EntryLogged;
            _logManager.StateChanged -= LogManager_StateChanged;
            _logManager.WriteError -= LogManager_WriteError;
            _controller.AutoSamplingProgress -= Controller_AutoSamplingProgress;
            _controller.AutoSamplingCompleted -= Controller_AutoSamplingCompleted;
            
            base.OnFormClosing(e);
        }
    }
}
