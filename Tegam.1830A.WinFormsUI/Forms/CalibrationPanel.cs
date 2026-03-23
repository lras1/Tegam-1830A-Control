using System;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam.WinFormsUI.Controllers;

namespace Tegam.WinFormsUI.Forms
{
    public partial class CalibrationPanel : UserControl
    {
        private readonly CalibrationController _controller;
        private readonly EnhancedLoggingController _loggingController;

        public CalibrationPanel(CalibrationController controller, EnhancedLoggingController loggingController = null)
        {
            _controller = controller;
            _loggingController = loggingController;
            InitializeComponent();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _controller.CalibrationStarted += Controller_CalibrationStarted;
            _controller.CalibrationStatusQueried += Controller_CalibrationStatusQueried;
            _controller.OperationError += Controller_OperationError;
        }

        private void Controller_CalibrationStarted(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(Controller_CalibrationStarted), sender, e);
                return;
            }

            MessageBox.Show("Calibration started.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar.Value = 0;
        }

        private void Controller_CalibrationStatusQueried(object sender, CalibrationStatus status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, CalibrationStatus>(Controller_CalibrationStatusQueried), sender, status);
                return;
            }

            if (status.IsCalibrating)
            {
                lblStatusValue.Text = "Calibration in progress...";
                progressBar.Value = 50;
            }
            else if (status.IsComplete)
            {
                if (status.IsSuccessful)
                {
                    lblStatusValue.Text = "Calibration completed successfully";
                    progressBar.Value = 100;
                }
                else
                {
                    lblStatusValue.Text = $"Calibration failed: {status.ErrorMessage}";
                    progressBar.Value = 0;
                }
            }
            else
            {
                lblStatusValue.Text = "Idle";
                progressBar.Value = 0;
            }
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

        private async void btnStartCalibration_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartCalibration.Enabled = false;
                string modeText = cmbMode.SelectedItem?.ToString() ?? "Internal";
                CalibrationMode mode = modeText == "Internal" ? CalibrationMode.Internal : CalibrationMode.External;
                await _controller.StartCalibrationAsync(mode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnStartCalibration.Enabled = true;
            }
        }

        private async void btnQueryStatus_Click(object sender, EventArgs e)
        {
            try
            {
                btnQueryStatus.Enabled = false;
                await _controller.QueryCalibrationStatusAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnQueryStatus.Enabled = true;
            }
        }
    }
}
