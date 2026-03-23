using System;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam.WinFormsUI.Controllers;

namespace Tegam.WinFormsUI.Forms
{
    public partial class FrequencyConfigurationPanel : UserControl
    {
        private readonly FrequencyConfigurationController _controller;
        private readonly EnhancedLoggingController _loggingController;

        public FrequencyConfigurationPanel(FrequencyConfigurationController controller, EnhancedLoggingController loggingController = null)
        {
            _controller = controller;
            _loggingController = loggingController;
            InitializeComponent();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _controller.FrequencySet += Controller_FrequencySet;
            _controller.FrequencyQueried += Controller_FrequencyQueried;
            _controller.OperationError += Controller_OperationError;
        }

        private void Controller_FrequencySet(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(Controller_FrequencySet), sender, e);
                return;
            }

            MessageBox.Show("Frequency set successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Controller_FrequencyQueried(object sender, FrequencyResponse frequency)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, FrequencyResponse>(Controller_FrequencyQueried), sender, frequency);
                return;
            }

            lblCurrentFrequencyValue.Text = $"{frequency.Frequency:F2} {frequency.Unit}";
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

        private async void btnSetFrequency_Click(object sender, EventArgs e)
        {
            try
            {
                btnSetFrequency.Enabled = false;
                double frequency = (double)numFrequency.Value;
                string unitText = cmbUnit.SelectedItem?.ToString() ?? "MHz";

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

                await _controller.SetFrequencyAsync(frequency, unit);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSetFrequency.Enabled = true;
            }
        }

        private async void btnQueryFrequency_Click(object sender, EventArgs e)
        {
            try
            {
                btnQueryFrequency.Enabled = false;
                await _controller.QueryFrequencyAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnQueryFrequency.Enabled = true;
            }
        }
    }
}
