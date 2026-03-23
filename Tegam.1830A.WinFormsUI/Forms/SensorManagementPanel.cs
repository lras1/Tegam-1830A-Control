using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam.WinFormsUI.Controllers;

namespace Tegam.WinFormsUI.Forms
{
    public partial class SensorManagementPanel : UserControl
    {
        private readonly SensorManagementController _controller;
        private readonly EnhancedLoggingController _loggingController;

        public SensorManagementPanel(SensorManagementController controller, EnhancedLoggingController loggingController = null)
        {
            _controller = controller;
            _loggingController = loggingController;
            InitializeComponent();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _controller.SensorSelected += Controller_SensorSelected;
            _controller.CurrentSensorQueried += Controller_CurrentSensorQueried;
            _controller.AvailableSensorsQueried += Controller_AvailableSensorsQueried;
            _controller.OperationError += Controller_OperationError;
        }

        private void Controller_SensorSelected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(Controller_SensorSelected), sender, e);
                return;
            }

            MessageBox.Show("Sensor selected successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Controller_CurrentSensorQueried(object sender, SensorInfo sensor)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, SensorInfo>(Controller_CurrentSensorQueried), sender, sensor);
                return;
            }

            lblCurrentSensorValue.Text = $"Sensor {sensor.SensorId}: {sensor.Name} " +
                $"({sensor.MinFrequency:F0} - {sensor.MaxFrequency:F0} Hz)";
        }

        private void Controller_AvailableSensorsQueried(object sender, List<SensorInfo> sensors)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, List<SensorInfo>>(Controller_AvailableSensorsQueried), sender, sensors);
                return;
            }

            lstAvailableSensors.Items.Clear();
            foreach (var sensor in sensors)
            {
                lstAvailableSensors.Items.Add(
                    $"Sensor {sensor.SensorId}: {sensor.Name} " +
                    $"({sensor.MinFrequency:F0} - {sensor.MaxFrequency:F0} Hz, " +
                    $"{sensor.MinPower:F2} - {sensor.MaxPower:F2} dBm)");
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

        private async void btnSelectSensor_Click(object sender, EventArgs e)
        {
            try
            {
                btnSelectSensor.Enabled = false;
                int sensorId = int.Parse(cmbSensor.SelectedItem?.ToString() ?? "1");
                await _controller.SelectSensorAsync(sensorId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSelectSensor.Enabled = true;
            }
        }

        private async void btnQueryCurrentSensor_Click(object sender, EventArgs e)
        {
            try
            {
                btnQueryCurrentSensor.Enabled = false;
                await _controller.QueryCurrentSensorAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnQueryCurrentSensor.Enabled = true;
            }
        }

        private async void btnQueryAvailableSensors_Click(object sender, EventArgs e)
        {
            try
            {
                btnQueryAvailableSensors.Enabled = false;
                await _controller.QueryAvailableSensorsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnQueryAvailableSensors.Enabled = true;
            }
        }
    }
}
