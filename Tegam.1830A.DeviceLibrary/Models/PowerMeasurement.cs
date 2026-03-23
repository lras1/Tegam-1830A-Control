using System;

namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents a power measurement result from the Tegam 1830A device.
    /// </summary>
    public class PowerMeasurement
    {
        public double PowerValue { get; set; }
        public PowerUnit PowerUnit { get; set; }
        public double Frequency { get; set; }
        public FrequencyUnit FrequencyUnit { get; set; }
        public int SensorId { get; set; }
        public DateTime Timestamp { get; set; }

        public PowerMeasurement()
        {
            Timestamp = DateTime.Now;
        }

        public PowerMeasurement(double powerValue, PowerUnit powerUnit, double frequency, FrequencyUnit frequencyUnit, int sensorId)
        {
            PowerValue = powerValue;
            PowerUnit = powerUnit;
            Frequency = frequency;
            FrequencyUnit = frequencyUnit;
            SensorId = sensorId;
            Timestamp = DateTime.Now;
        }
    }
}
