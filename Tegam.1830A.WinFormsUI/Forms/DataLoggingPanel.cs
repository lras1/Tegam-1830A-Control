using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Tegam.WinFormsUI.Controllers;

namespace Tegam.WinFormsUI.Forms
{
    public partial class DataLoggingPanel : UserControl
    {
        private readonly DataLoggingController _controller;

        public DataLoggingPanel(DataLoggingController controller)
        {
            _controller = controller;
            InitializeComponent();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _controller.LoggingStarted += Controller_LoggingStarted;
            _controller.LoggingStopped += Controller_LoggingStopped;
            _controller.MeasurementLogged += Controller_MeasurementLogged;
            _controller.OperationError += Controller_OperationError;
        }

        private void Controller_LoggingStarted(object sender, string filename)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, string>(Controller_LoggingStarted), sender, filename);
                return;
            }

            lblStatusValue.Text = "Logging active";
            lblFileLocationValue.Text = filename;
            lblMeasurementCountValue.Text = "0";
            btnStartLogging.Enabled = false;
            btnStopLogging.Enabled = true;
            txtFilename.Enabled = false;
            btnBrowse.Enabled = false;
        }

        private void Controller_LoggingStopped(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(Controller_LoggingStopped), sender, e);
                return;
            }

            lblStatusValue.Text = "Not logging";
            btnStartLogging.Enabled = true;
            btnStopLogging.Enabled = false;
            txtFilename.Enabled = true;
            btnBrowse.Enabled = true;
            MessageBox.Show("Logging stopped.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Controller_MeasurementLogged(object sender, int count)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, int>(Controller_MeasurementLogged), sender, count);
                return;
            }

            lblMeasurementCountValue.Text = count.ToString();
        }

        private void Controller_OperationError(object sender, string error)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, string>(Controller_OperationError), sender, error);
                return;
            }

            MessageBox.Show($"Error: {error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilename.Text = saveFileDialog.FileName;
            }
        }

        private async void btnStartLogging_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartLogging.Enabled = false;
                string filename = txtFilename.Text.Trim();

                if (string.IsNullOrEmpty(filename))
                {
                    MessageBox.Show("Please enter a filename.", "Input Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnStartLogging.Enabled = true;
                    return;
                }

                await _controller.StartLoggingAsync(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStartLogging.Enabled = true;
            }
        }

        private async void btnStopLogging_Click(object sender, EventArgs e)
        {
            try
            {
                btnStopLogging.Enabled = false;
                await _controller.StopLoggingAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStopLogging.Enabled = true;
            }
        }

        private void btnOpenLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = lblFileLocationValue.Text;
                if (string.IsNullOrEmpty(filename) || filename == "N/A")
                {
                    MessageBox.Show("No log file is currently open.", "Info", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
