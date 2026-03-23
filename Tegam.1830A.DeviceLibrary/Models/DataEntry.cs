using System;

namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents a data measurement log entry.
    /// </summary>
    public class DataEntry : LogEntry
    {
        /// <summary>
        /// Gets the type of this log entry.
        /// </summary>
        public override string Type => "Data";

        /// <summary>
        /// Gets or sets the frequency value.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the frequency unit.
        /// </summary>
        public FrequencyUnit FrequencyUnit { get; set; }

        /// <summary>
        /// Gets or sets the power value in dBm.
        /// </summary>
        public double Power { get; set; }

        /// <summary>
        /// Gets or sets the sensor ID.
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// Converts this data entry to a CSV line format.
        /// Format: Data,Timestamp,Frequency,FrequencyUnit,Power,SensorId
        /// </summary>
        /// <returns>CSV formatted string.</returns>
        public override string ToCsvLine()
        {
            return string.Format("Data,{0:yyyy-MM-dd HH:mm:ss.fff},{1},{2},{3:F2},{4}",
                Timestamp, Frequency, FrequencyUnit, Power, SensorId);
        }

        /// <summary>
        /// Converts this data entry to a display-friendly string.
        /// Format: Power: {value} dBm @ {frequency} {unit}
        /// </summary>
        /// <returns>Display formatted string.</returns>
        public override string ToDisplayString()
        {
            return string.Format("Power: {0:F2} dBm @ {1} {2}", Power, Frequency, FrequencyUnit);
        }
    }
}
