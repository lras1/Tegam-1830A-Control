using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Logging
{
    /// <summary>
    /// Interface for writing log entries to CSV files.
    /// </summary>
    public interface ICsvWriter
    {
        /// <summary>
        /// Writes the CSV header row to the file.
        /// </summary>
        void WriteHeader();

        /// <summary>
        /// Writes a log entry to the CSV file.
        /// </summary>
        /// <param name="entry">The log entry to write.</param>
        void WriteEntry(LogEntry entry);

        /// <summary>
        /// Flushes the file buffer to disk.
        /// </summary>
        void Flush();

        /// <summary>
        /// Closes the CSV file.
        /// </summary>
        void Close();
    }
}
