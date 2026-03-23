using System;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam.WinFormsUI.Controllers;

namespace Tegam.WinFormsUI.Forms
{
    public partial class PowerMeasurementPanel : UserControl
    {
        private readonly PowerMeasurementController _controller;
        private readonly EnhancedLoggingController _loggingController;

        public PowerMeasurementPanel(PowerMeasurementController controller, EnhancedLoggingController loggingController = null)
        {
            _controller = controller;
            _loggingController = loggingController;
            InitializeComponent();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _controller.MeasurementCompleted += Controller_MeasurementCompleted;
            _controller.MeasurementError += Controller_MeasurementError;
            _controller.MultipleProgressChanged += Controller_MultipleProgressChanged;
        }

        private void Controller_MeasurementCompleted(object sender, PowerMeasurement measurement)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, PowerMeasurement>(Controller_MeasurementCompleted), sender, measurement);
                return;
            }

            lblPowerValueDisplay.Text = $"{measurement.PowerValue:F2} {measurement.PowerUnit}";
            lblTimestampDisplay.Text = measurement.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private void Controller_MeasurementError(object sender, string error)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, string>(Controller_MeasurementError), sender, error);
                return;
            }

            MessageBox.Show($"Measurement error: {error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Controller_MultipleProgressChanged(object sender, int progress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, int>(Controller_MultipleProgressChanged), sender, progress);
                return;
            }

            progressBar.Value = progress;
        }

        private async void btnMeasure_Click(object sender, EventArgs e)
        {
            try
            {
                btnMeasure.Enabled = false;
                double frequency = (double)numFrequency.Value;
                string unitText = cmbFrequencyUnit.SelectedItem?.ToString() ?? "MHz";
                int sensorId = int.Parse(cmbSensor.SelectedItem?.ToString() ?? "1");

                FrequencyUnit unit;
                switch (unitText)
                {
                    case "Hz":
                        unit = FrequencyUnit.Hz;
                        break;
                    case "kHz":
                        unit = FrequencyUnit.kHz;
                        break;
                    case "MHz":
                        unit = FrequencyUnit.MHz;
                        break;
                    case "GHz":
                        unit = FrequencyUnit.GHz;
                        break;
                    default:
                        unit = FrequencyUnit.MHz;
                        break;
                }

                await _controller.MeasurePowerAsync(frequency, unit, sensorId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnMeasure.Enabled = true;
            }
        }

        private async void btnMeasureMultiple_Click(object sender, EventArgs e)
        {
            try
            {
                btnMeasureMultiple.Enabled = false;
                progressBar.Value = 0;
                int count = (int)numCount.Value;
                int delay = (int)numDelay.Value;

                await _controller.MeasureMultipleAsync(count, delay);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnMeasureMultiple.Enabled = true;
            }
        }
    }
}
