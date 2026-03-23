using System;
using System.Threading.Tasks;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Services;

namespace Tegam.WinFormsUI.Controllers
{
    public class FrequencyConfigurationController
    {
        private readonly IPowerMeterService _powerMeterService;

        public event EventHandler FrequencySet;
        public event EventHandler<FrequencyResponse> FrequencyQueried;
        public event EventHandler<string> OperationError;

        public FrequencyConfigurationController(IPowerMeterService powerMeterService)
        {
            _powerMeterService = powerMeterService;
        }

        public async Task SetFrequencyAsync(double frequency, FrequencyUnit unit)
        {
            try
            {
                var result = await Task.Run(() => _powerMeterService.SetFrequency(frequency, unit));
                if (result.IsSuccess)
                {
                    FrequencySet?.Invoke(this, EventArgs.Empty);
                    
                    // Query the frequency to get the actual value set
                    await QueryFrequencyAsync();
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

        public async Task QueryFrequencyAsync()
        {
            try
            {
                var frequency = await Task.Run(() => _powerMeterService.GetFrequency());
                FrequencyQueried?.Invoke(this, frequency);
            }
            catch (Exception ex)
            {
                OperationError?.Invoke(this, ex.Message);
            }
        }
    }
}
