using System;
using System.Threading.Tasks;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Services;

namespace Tegam.WinFormsUI.Controllers
{
    public class CalibrationController
    {
        private readonly IPowerMeterService _powerMeterService;

        public event EventHandler CalibrationStarted;
        public event EventHandler<CalibrationStatus> CalibrationStatusQueried;
        public event EventHandler<string> OperationError;

        public CalibrationController(IPowerMeterService powerMeterService)
        {
            _powerMeterService = powerMeterService;
        }

        public async Task StartCalibrationAsync(CalibrationMode mode)
        {
            try
            {
                var result = await Task.Run(() => _powerMeterService.Calibrate(mode));
                if (result.IsSuccess)
                {
                    CalibrationStarted?.Invoke(this, EventArgs.Empty);
                    
                    // Query status to get the actual calibration state
                    await QueryCalibrationStatusAsync();
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

        public async Task QueryCalibrationStatusAsync()
        {
            try
            {
                var status = await Task.Run(() => _powerMeterService.GetCalibrationStatus());
                CalibrationStatusQueried?.Invoke(this, status);
            }
            catch (Exception ex)
            {
                OperationError?.Invoke(this, ex.Message);
            }
        }
    }
}
