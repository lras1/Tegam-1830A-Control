namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Configuration for automatic sampling mode.
    /// </summary>
    public class AutoSamplingConfig
    {
        /// <summary>
        /// Gets or sets the sample rate in milliseconds.
        /// </summary>
        public int SampleRateMs { get; set; }

        /// <summary>
        /// Gets or sets the total number of samples to collect.
        /// </summary>
        public int SampleCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic sampling is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the number of samples completed so far.
        /// </summary>
        public int CompletedCount { get; set; }
    }
}
