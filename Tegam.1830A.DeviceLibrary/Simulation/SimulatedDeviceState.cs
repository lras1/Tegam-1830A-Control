using System;
using System.Collections.Generic;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Simulation
{
    /// <summary>
    /// Represents the simulated state of a Tegam 1830A device for testing purposes.
    /// </summary>
    public class SimulatedDeviceState
    {
        private double _currentFrequency;
        private FrequencyUnit _currentFrequencyUnit;
        private int _currentSensorId;
        private bool _isCalibrating;
        private bool _isLogging;
        private string _logFilename;
        private Random _random;
        private bool _shouldSimulateError;
        private bool _shouldSimulateConnectionLoss;
        private bool _shouldSimulateTimeout;

        /// <summary>
        /// Gets or sets the current measurement frequency.
        /// </summary>
        public double CurrentFrequency
        {
            get { return _currentFrequency; }
            set { _currentFrequency = value; }
        }

        /// <summary>
        /// Gets or sets the current frequency unit.
        /// </summary>
        public FrequencyUnit CurrentFrequencyUnit
        {
            get { return _currentFrequencyUnit; }
            set { _currentFrequencyUnit = value; }
        }

        /// <summary>
        /// Gets or sets the currently selected sensor ID.
        /// </summary>
        public int CurrentSensorId
        {
            get { return _currentSensorId; }
            set { _currentSensorId = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether calibration is in progress.
        /// </summary>
        public bool IsCalibrating
        {
            get { return _isCalibrating; }
            set { _isCalibrating = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data logging is active.
        /// </summary>
        public bool IsLogging
        {
            get { return _isLogging; }
            set { _isLogging = value; }
        }

        /// <summary>
        /// Gets or sets the current log filename.
        /// </summary>
        public string LogFilename
        {
            get { return _logFilename; }
            set { _logFilename = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to simulate an error on the next command.
        /// </summary>
        public bool ShouldSimulateError
        {
            get { return _shouldSimulateError; }
            set { _shouldSimulateError = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to simulate a connection loss.
        /// </summary>
        public bool ShouldSimulateConnectionLoss
        {
            get { return _shouldSimulateConnectionLoss; }
            set { _shouldSimulateConnectionLoss = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to simulate a timeout.
        /// </summary>
        public bool ShouldSimulateTimeout
        {
            get { return _shouldSimulateTimeout; }
            set { _shouldSimulateTimeout = value; }
        }

        /// <summary>
        /// Initializes a new instance of the SimulatedDeviceState class.
        /// </summary>
        public SimulatedDeviceState()
        {
            _currentFrequency = 2.4;
            _currentFrequencyUnit = FrequencyUnit.GHz;
            _currentSensorId = 1;
            _isCalibrating = false;
            _isLogging = false;
            _logFilename = null;
            _random = new Random();
            _shouldSimulateError = false;
            _shouldSimulateConnectionLoss = false;
            _shouldSimulateTimeout = false;
        }

        /// <summary>
        /// Generates a simulated power measurement response.
        /// </summary>
        public string GeneratePowerMeasurementResponse()
        {
            // Generate a realistic power measurement between -50 dBm and +20 dBm
            double powerValue = _random.NextDouble() * 70 - 50;
            return string.Format("{0:F2} dBm", powerValue);
        }

        /// <summary>
        /// Generates a simulated device identity response.
        /// </summary>
        public string GenerateIdentityResponse()
        {
            return "Tegam,1830A,SN123456,1.0.0";
        }

        /// <summary>
        /// Generates a simulated system status response.
        /// </summary>
        public string GenerateSystemStatusResponse()
        {
            // Format: isReady, temperature, errorCount
            double temperature = 20 + _random.NextDouble() * 10;
            return string.Format("1,{0:F1},0", temperature);
        }

        /// <summary>
        /// Generates a simulated calibration status response.
        /// </summary>
        public string GenerateCalibrationStatusResponse()
        {
            // Format: isCalibrating, isComplete, isSuccessful, errorMessage
            if (_isCalibrating)
                return "1,0,0,Calibration in progress";
            else
                return "0,1,1,OK";
        }

        /// <summary>
        /// Generates a simulated frequency response.
        /// </summary>
        public string GenerateFrequencyResponse()
        {
            string unitString = ConvertFrequencyUnitToString(_currentFrequencyUnit);
            return string.Format("{0} {1}", _currentFrequency, unitString);
        }

        /// <summary>
        /// Generates a simulated sensor selection response.
        /// </summary>
        public string GenerateSensorSelectionResponse()
        {
            return _currentSensorId.ToString();
        }

        /// <summary>
        /// Generates a simulated available sensors response.
        /// </summary>
        public string GenerateAvailableSensorsResponse()
        {
            // Format: sensor1;sensor2;sensor3;sensor4
            // Each sensor: id,name,minFreq,maxFreq,minPower,maxPower
            return "1,Sensor1,100KHZ,40GHZ,-50DBM,+20DBM;" +
                   "2,Sensor2,100KHZ,40GHZ,-50DBM,+20DBM;" +
                   "3,Sensor3,100KHZ,40GHZ,-50DBM,+20DBM;" +
                   "4,Sensor4,100KHZ,40GHZ,-50DBM,+20DBM";
        }

        /// <summary>
        /// Generates a simulated logging status response.
        /// </summary>
        public string GenerateLoggingStatusResponse()
        {
            // Format: isLogging, filename
            if (_isLogging)
                return string.Format("1,{0}", _logFilename);
            else
                return "0,";
        }

        /// <summary>
        /// Generates a simulated error response.
        /// </summary>
        public string GenerateErrorResponse()
        {
            return "0,No error";
        }

        /// <summary>
        /// Converts a FrequencyUnit enum value to its SCPI string representation.
        /// </summary>
        private string ConvertFrequencyUnitToString(FrequencyUnit unit)
        {
            switch (unit)
            {
                case FrequencyUnit.Hz:
                    return "HZ";
                case FrequencyUnit.kHz:
                    return "KHZ";
                case FrequencyUnit.MHz:
                    return "MHZ";
                case FrequencyUnit.GHz:
                    return "GHZ";
                default:
                    return "GHZ";
            }
        }

        /// <summary>
        /// Resets the device state to default values.
        /// </summary>
        public void Reset()
        {
            _currentFrequency = 2.4;
            _currentFrequencyUnit = FrequencyUnit.GHz;
            _currentSensorId = 1;
            _isCalibrating = false;
            _isLogging = false;
            _logFilename = null;
            _shouldSimulateError = false;
            _shouldSimulateConnectionLoss = false;
            _shouldSimulateTimeout = false;
        }
    }
}
