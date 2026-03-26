namespace CalibrationTuning.Models
{
    /// <summary>
    /// Configuration parameters for a tuning session.
    /// </summary>
    public class TuningParameters
    {
        /// <summary>
        /// Signal frequency in Hz.
        /// </summary>
        public double FrequencyHz { get; set; }

        /// <summary>
        /// Initial signal voltage to start tuning.
        /// </summary>
        public double InitialVoltage { get; set; }

        /// <summary>
        /// Target power setpoint in dBm.
        /// </summary>
        public double TargetPowerDbm { get; set; }

        /// <summary>
        /// Maximum acceptable standard deviation in dB for stability.
        /// Measurements must be repeatable within this spread.
        /// </summary>
        public double MaxStdDevDb { get; set; } = 0.5;

        /// <summary>
        /// Confidence level multiplier (k-factor).
        /// 2 = ~95% confidence, 3 = ~99.7% confidence.
        /// Stability requires |mean - target| &lt;= k * stdDev.
        /// </summary>
        public double ConfidenceK { get; set; } = 2.0;

        /// <summary>
        /// Number of recent samples to evaluate for stability.
        /// </summary>
        public int StabilityWindow { get; set; } = 10;

        /// <summary>
        /// Voltage adjustment step size.
        /// </summary>
        public double VoltageStepSize { get; set; }

        /// <summary>
        /// Minimum allowed voltage limit.
        /// </summary>
        public double MinVoltage { get; set; }

        /// <summary>
        /// Maximum allowed voltage limit.
        /// </summary>
        public double MaxVoltage { get; set; }

        /// <summary>
        /// Maximum number of tuning iterations before timeout.
        /// Set to 0 for continuous sampling (runs until stopped).
        /// </summary>
        public int MaxIterations { get; set; }

        /// <summary>
        /// Power meter sensor ID to use for measurements.
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// Delay between measurement samples in milliseconds.
        /// </summary>
        public int SampleDelayMs { get; set; } = 500;
    }
}
