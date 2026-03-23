using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Services;

namespace Tegam.WinFormsUI.Controllers
{
    public class SensorManagementController
    {
        private readonly IPowerMeterService _powerMeterService;

        public event EventHandler SensorSelected;
        public event EventHandler<SensorInfo> CurrentSensorQueried;
        public event EventHandler<List<SensorInfo>> AvailableSensorsQueried;
        public event EventHandler<string> OperationError;

        public SensorManagementController(IPowerMeterService powerMeterService)
        {
            _powerMeterService = powerMeterService;
        }

        public async Task SelectSensorAsync(int sensorId)
        {
            try
            {
                var result = await Task.Run(() => _powerMeterService.SelectSensor(sensorId));
                if (result.IsSuccess)
                {
                    SensorSelected?.Invoke(this, EventArgs.Empty);
                    
                    // Query the sensor to get the actual value selected
                    await QueryCurrentSensorAsync();
                }
                else
                {
                    OperationError?.Invoke(this, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                OperationError?.Invoke(this, ex.Message);
            }
        }

        public async Task QueryCurrentSensorAsync()
        {
            try
            {
                var sensor = await Task.Run(() => _powerMeterService.GetCurrentSensor());
                CurrentSensorQueried?.Invoke(this, sensor);
            }
            catch (Exception ex)
            {
                OperationError?.Invoke(this, ex.Message);
            }
        }

        public async Task QueryAvailableSensorsAsync()
        {
            try
            {
                var sensors = await Task.Run(() => _powerMeterService.GetAvailableSensors());
                AvailableSensorsQueried?.Invoke(this, sensors);
            }
            catch (Exception ex)
            {
                OperationError?.Invoke(this, ex.Message);
            }
        }
    }
}
