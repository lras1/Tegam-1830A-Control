using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Services;

namespace Tegam.WinFormsUI.Controllers
{
    public class PowerMeasurementController
    {
        private readonly IPowerMeterService _powerMeterService;

        public event EventHandler<PowerMeasurement> MeasurementCompleted;
        public event EventHandler<string> MeasurementError;
        public event EventHandler<int> MultipleProgressChanged;

        public PowerMeasurementController(IPowerMeterService powerMeterService)
        {
            _powerMeterService = powerMeterService;
            _powerMeterService.MeasurementReceived += PowerMeterService_MeasurementReceived;
            _powerMeterService.DeviceError += PowerMeterService_DeviceError;
        }

        private void PowerMeterService_MeasurementReceived(object sender, EventArgs e)
        {
            // This will be called when measurements are received
        }

        private void PowerMeterService_DeviceError(object sender, EventArgs e)
        {
            // This will be called when device errors occur
        }

        public async Task MeasurePowerAsync(double frequency, FrequencyUnit unit, int sensorId)
        {
            try
            {
                await Task.Run(() =>
                {
                    // Set frequency
                    _powerMeterService.SetFrequency(frequency, unit);

                    // Select sensor
                    _powerMeterService.SelectSensor(sensorId);

                    // Measure power
                    var measurement = _powerMeterService.MeasurePower();
                    MeasurementCompleted?.Invoke(this, measurement);
                });
            }
            catch (Exception ex)
            {
                MeasurementError?.Invoke(this, ex.Message);
            }
        }

        public async Task MeasureMultipleAsync(int count, int delayMs)
        {
            try
            {
                await Task.Run(() =>
                {
                    var measurements = _powerMeterService.MeasureMultiple(count, delayMs);
                    
                    for (int i = 0; i < measurements.Count; i++)
                    {
                        MeasurementCompleted?.Invoke(this, measurements[i]);
                        int progress = (int)((i + 1) / (double)count * 100);
                        MultipleProgressChanged?.Invoke(this, progress);
                    }
                });
            }
            catch (Exception ex)
            {
                MeasurementError?.Invoke(this, ex.Message);
            }
        }
    }
}
