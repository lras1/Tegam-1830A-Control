using System.Collections.Generic;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Logging
{
    /// <summary>
    /// Interface for parsing CSV log files.
    /// </summary>
    public interface ICsvParser
    {
        /// <summary>
        /// Parses a CSV file and returns a collection of log entries.
        /// </summary>
        /// <param name="filename">The path to the CSV file.</param>
        /// <returns>A ParseResult containing the log entries or error information.</returns>
        ParseResult<List<LogEntry>> Parse(string filename);

        /// <summary>
        /// Parses CSV lines and returns a collection of log entries.
        /// </summary>
        /// <param name="lines">The CSV lines to parse.</param>
        /// <returns>A ParseResult containing the log entries or error information.</returns>
        ParseResult<List<LogEntry>> ParseLines(IEnumerable<string> lines);
    }
}
