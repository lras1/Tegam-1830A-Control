namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents information about a measurement sensor in the Tegam 1830A device.
    /// </summary>
    public class SensorInfo
    {
        public int SensorId { get; set; }
        public string Name { get; set; }
        public double MinFrequency { get; set; }
        public double MaxFrequency { get; set; }
        public double MinPower { get; set; }
        public double MaxPower { get; set; }

        public SensorInfo()
        {
        }

        public SensorInfo(int sensorId, string name, double minFrequency, double maxFrequency, double minPower, double maxPower)
        {
            SensorId = sensorId;
            Name = name;
            MinFrequency = minFrequency;
            MaxFrequency = maxFrequency;
            MinPower = minPower;
            MaxPower = maxPower;
        }
    }
}
