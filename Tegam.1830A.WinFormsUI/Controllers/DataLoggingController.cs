using System;
using System.IO;
using System.Threading.Tasks;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Services;

namespace Tegam.WinFormsUI.Controllers
{
    public class DataLoggingController
    {
        private readonly IPowerMeterService _powerMeterService;
        private string _currentLogFile;
        private int _measurementCount;
        private readonly object _fileLock = new object();

        public event EventHandler<string> LoggingStarted;
        public event EventHandler LoggingStopped;
        public event EventHandler<int> MeasurementLogged;
        public event EventHandler<string> OperationError;

        public DataLoggingController(IPowerMeterService powerMeterService)
        {
            _powerMeterService = powerMeterService;
            _powerMeterService.MeasurementReceived += PowerMeterService_MeasurementReceived;
        }

        private void PowerMeterService_MeasurementReceived(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentLogFile) && e is MeasurementEventArgs measurementArgs)
            {
                try
                {
                    // Write measurement to CSV file
                    lock (_fileLock)
                    {
                        using (var writer = new StreamWriter(_currentLogFile, true))
                        {
                            var measurement = measurementArgs.Measurement;
                            writer.WriteLine($"{measurement.Timestamp:yyyy-MM-dd HH:mm:ss.fff}," +
                                           $"{measurement.Frequency}," +
                                           $"{measurement.PowerValue}," +
                                           $"{measurement.SensorId}");
                        }
                    }

                    _measurementCount++;
                    MeasurementLogged?.Invoke(this, _measurementCount);
                }
                catch (Exception ex)
                {
                    OperationError?.Invoke(this, $"Error writing to log file: {ex.Message}");
                }
            }
        }

        public async Task StartLoggingAsync(string filename)
        {
            try
            {
                var result = await Task.Run(() => _powerMeterService.StartLogging(filename));
                if (result.IsSuccess)
                {
                    _currentLogFile = filename;
                    _measurementCount = 0;
                    
                    // Write header to file
                    using (var writer = new StreamWriter(filename, false))
                    {
                        writer.WriteLine("Timestamp,Frequency (Hz),Power (dBm),Sensor ID");
                    }

                    LoggingStarted?.Invoke(this, filename);
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

        public async Task StopLoggingAsync()
        {
            try
            {
                var result = await Task.Run(() => _powerMeterService.StopLogging());
                if (result.IsSuccess)
                {
                    _currentLogFile = null;
                    LoggingStopped?.Invoke(this, EventArgs.Empty);
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
    }
}
