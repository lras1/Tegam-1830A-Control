using System;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Logging;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam.WinFormsUI.Controllers;
using Tegam.WinFormsUI.Controls;

namespace Tegam.WinFormsUI.Forms
{
    /// <summary>
    /// Enhanced logging panel with manual/automatic sampling and log display.
    /// </summary>
    public partial class EnhancedLoggingPanel : UserControl
    {
        private readonly EnhancedLoggingController _controller;
        private readonly ILogManager _logManager;
        private LogViewerForm _logViewerForm;

        public EnhancedLoggingPanel(EnhancedLoggingController controller, ILogManager logManager)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
            
            InitializeComponent();
            SubscribeToEvents();
            UpdateUIState();
        }

        private void SubscribeToEvents()
        {
            _logManager.EntryLogged += LogManager_EntryLogged;
            _logManager.StateChanged += LogManager_StateChanged;
            _logManager.WriteError += LogManager_WriteError;
            _controller.AutoSamplingProgress += Controller_AutoSamplingProgress;
            _controller.AutoSamplingCompleted += Controller_AutoSamplingCompleted;
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
                pnlStatusIndicator.BackColor = System.Drawing.Color.Green;
                lblStatusText.Text = "Active";
                lblStatusText.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                pnlStatusIndicator.BackColor = System.Drawing.Color.Gray;
                lblStatusText.Text = "Inactive";
                lblStatusText.ForeColor = System.Drawing.Color.Gray;
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
            _controller.LogButtonAction("Browse", "Clicked");
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = saveFileDialog.FileName;
                _controller.LogButtonAction("Browse", "File selected: " + saveFileDialog.FileName);
            }
        }

        private void btnStartLogging_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.LogButtonAction("Start Logging", "Clicked");
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
                _controller.LogButtonAction("Stop Logging", "Clicked");
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
                _controller.LogButtonAction("Measure Now", "Clicked");
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
                _controller.LogButtonAction("Start Auto", "Clicked");
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
                _controller.LogButtonAction("Stop Auto", "Clicked");
                _controller.StopAutomaticSampling();
                UpdateUIState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping automatic sampling: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewLogWindow_Click(object sender, EventArgs e)
        {
            try
            {
                _controller.LogButtonAction("View Log Window", "Clicked");
                if (_logViewerForm == null || _logViewerForm.IsDisposed)
                {
                    _logViewerForm = new LogViewerForm(_logManager, _controller, this);
                    _logViewerForm.Show();
                }
                else
                {
                    _logViewerForm.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening log viewer: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe from events
                _logManager.EntryLogged -= LogManager_EntryLogged;
                _logManager.StateChanged -= LogManager_StateChanged;
                _logManager.WriteError -= LogManager_WriteError;
                _controller.AutoSamplingProgress -= Controller_AutoSamplingProgress;
                _controller.AutoSamplingCompleted -= Controller_AutoSamplingCompleted;

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
