namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents the system status of the Tegam 1830A device.
    /// </summary>
    public class SystemStatus
    {
        public bool IsReady { get; set; }
        public double Temperature { get; set; }
        public int ErrorCount { get; set; }

        public SystemStatus()
        {
        }

        public SystemStatus(bool isReady, double temperature, int errorCount)
        {
            IsReady = isReady;
            Temperature = temperature;
            ErrorCount = errorCount;
        }
    }
}
