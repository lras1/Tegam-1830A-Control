using System;

namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents a configuration setting change log entry.
    /// </summary>
    public class SettingEntry : LogEntry
    {
        /// <summary>
        /// Gets the type of this log entry.
        /// </summary>
        public override string Type => "Setting";

        /// <summary>
        /// Gets or sets the name of the setting that changed.
        /// </summary>
        public string SettingName { get; set; }

        /// <summary>
        /// Gets or sets the value of the setting.
        /// </summary>
        public string SettingValue { get; set; }

        /// <summary>
        /// Gets or sets additional context information about the setting change.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Converts this setting entry to a CSV line format.
        /// Format: Setting,Timestamp,SettingName,SettingValue,Context
        /// </summary>
        /// <returns>CSV formatted string with proper escaping.</returns>
        public override string ToCsvLine()
        {
            return string.Format("Setting,{0:yyyy-MM-dd HH:mm:ss.fff},{1},{2},{3}",
                Timestamp, 
                EscapeCsv(SettingName), 
                EscapeCsv(SettingValue), 
                EscapeCsv(Context));
        }

        /// <summary>
        /// Converts this setting entry to a display-friendly string.
        /// Format: {SettingName}: {SettingValue}
        /// </summary>
        /// <returns>Display formatted string.</returns>
        public override string ToDisplayString()
        {
            return string.Format("{0}: {1}", SettingName, SettingValue);
        }

        /// <summary>
        /// Escapes a CSV field value according to RFC 4180.
        /// Fields containing commas, quotes, or newlines are enclosed in double quotes.
        /// Double quotes within fields are escaped by doubling them.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>Escaped CSV field value.</returns>
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Check if escaping is needed
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                // Escape double quotes by doubling them
                string escaped = value.Replace("\"", "\"\"");
                // Enclose in double quotes
                return "\"" + escaped + "\"";
            }

            return value;
        }
    }
}
