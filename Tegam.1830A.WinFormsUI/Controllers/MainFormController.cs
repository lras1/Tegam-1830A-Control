using System;
using Tegam._1830A.DeviceLibrary.Services;

namespace Tegam.WinFormsUI.Controllers
{
    public class MainFormController
    {
        private readonly IPowerMeterService _powerMeterService;

        public MainFormController(IPowerMeterService powerMeterService)
        {
            _powerMeterService = powerMeterService;
        }

        public IPowerMeterService PowerMeterService => _powerMeterService;
    }
}
