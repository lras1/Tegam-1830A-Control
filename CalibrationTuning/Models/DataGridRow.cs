using System;

namespace CalibrationTuning.Models
{
    /// <summary>
    /// Represents a row in the data grid view, either a setting row or a data row.
    /// </summary>
    public class DataGridRow
    {
        /// <summary>
        /// Type of row: "setting" or "data".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Timestamp when the row was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Iteration number (null for setting rows).
        /// </summary>
        public int? Iteration { get; set; }

        /// <summary>
        /// Signal frequency in Hz (null for setting rows).
        /// </summary>
        public double? Frequency { get; set; }

        /// <summary>
        /// Signal voltage (null for setting rows).
        /// </summary>
        public double? Voltage { get; set; }

        /// <summary>
        /// Measured power in dBm (null for setting rows).
        /// </summary>
        public double? PowerDbm { get; set; }

        /// <summary>
        /// Status or action name.
        /// </summary>
        public string Status { get; set; }
    }
}
