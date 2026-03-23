namespace CalibrationTuning.Models
{
    /// <summary>
    /// Device connection configuration.
    /// </summary>
    public class DeviceConfiguration
    {
        /// <summary>
        /// IP address of the power meter.
        /// </summary>
        public string PowerMeterIpAddress { get; set; }

        /// <summary>
        /// IP address of the signal generator.
        /// </summary>
        public string SignalGeneratorIpAddress { get; set; }
    }
}
