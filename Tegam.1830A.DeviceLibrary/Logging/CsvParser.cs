using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Logging
{
    /// <summary>
    /// Parses CSV log files with RFC 4180 compliant formatting.
    /// </summary>
    public class CsvParser : ICsvParser
    {
        /// <summary>
        /// Parses a CSV file and returns a collection of log entries.
        /// </summary>
        /// <param name="filename">The path to the CSV file.</param>
        /// <returns>A ParseResult containing the log entries or error information.</returns>
        public ParseResult<List<LogEntry>> Parse(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return ParseResult<List<LogEntry>>.Failure("Filename cannot be null or empty");

            if (!File.Exists(filename))
                return ParseResult<List<LogEntry>>.Failure("File does not exist: " + filename);

            try
            {
                var lines = File.ReadAllLines(filename);
                return ParseLines(lines);
            }
            catch (Exception ex)
            {
                return ParseResult<List<LogEntry>>.Failure("Error reading file: " + ex.Message);
            }
        }

        /// <summary>
        /// Parses CSV lines and returns a collection of log entries.
        /// </summary>
        /// <param name="lines">The CSV lines to parse.</param>
        /// <returns>A ParseResult containing the log entries or error information.</returns>
        public ParseResult<List<LogEntry>> ParseLines(IEnumerable<string> lines)
        {
            var entries = new List<LogEntry>();
            int lineNumber = 0;
            bool headerSkipped = false;

            foreach (var line in lines)
            {
                lineNumber++;

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Skip header line
                if (!headerSkipped)
                {
                    headerSkipped = true;
                    continue;
                }

                // Parse the line
                var parseResult = ParseLine(line, lineNumber);
                if (!parseResult.IsSuccess)
                    return ParseResult<List<LogEntry>>.Failure(parseResult.ErrorMessage, lineNumber);

                entries.Add(parseResult.Value);
            }

            return ParseResult<List<LogEntry>>.Success(entries);
        }

        /// <summary>
        /// Parses a single CSV line into a LogEntry.
        /// </summary>
        private ParseResult<LogEntry> ParseLine(string line, int lineNumber)
        {
            var fields = ParseCsvLine(line);

            if (fields.Count < 5)
                return ParseResult<LogEntry>.Failure("Invalid number of columns (expected 5)", lineNumber);

            string type = fields[0];
            string timestampStr = fields[1];

            // Validate Type column
            if (type != "Data" && type != "Setting")
                return ParseResult<LogEntry>.Failure("Invalid Type value: " + type + " (expected 'Data' or 'Setting')", lineNumber);

            // Parse timestamp
            DateTime timestamp;
            if (!DateTime.TryParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss.fff", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out timestamp))
            {
                return ParseResult<LogEntry>.Failure("Invalid timestamp format: " + timestampStr, lineNumber);
            }

            // Create appropriate entry type
            if (type == "Data")
            {
                return ParseDataEntry(fields, timestamp, lineNumber);
            }
            else
            {
                return ParseSettingEntry(fields, timestamp, lineNumber);
            }
        }

        /// <summary>
        /// Parses fields into a DataEntry.
        /// </summary>
        private ParseResult<LogEntry> ParseDataEntry(List<string> fields, DateTime timestamp, int lineNumber)
        {
            double frequency;
            if (!double.TryParse(fields[2], out frequency))
                return ParseResult<LogEntry>.Failure("Invalid frequency value: " + fields[2], lineNumber);

            double power;
            if (!double.TryParse(fields[3], out power))
                return ParseResult<LogEntry>.Failure("Invalid power value: " + fields[3], lineNumber);

            int sensorId;
            if (!int.TryParse(fields[4], out sensorId))
                return ParseResult<LogEntry>.Failure("Invalid sensor ID: " + fields[4], lineNumber);

            var entry = new DataEntry
            {
                Timestamp = timestamp,
                Frequency = frequency,
                Power = power,
                SensorId = sensorId
            };

            return ParseResult<LogEntry>.Success(entry);
        }

        /// <summary>
        /// Parses fields into a SettingEntry.
        /// </summary>
        private ParseResult<LogEntry> ParseSettingEntry(List<string> fields, DateTime timestamp, int lineNumber)
        {
            var entry = new SettingEntry
            {
                Timestamp = timestamp,
                SettingName = fields[2],
                SettingValue = fields[3],
                Context = fields[4]
            };

            return ParseResult<LogEntry>.Success(entry);
        }

        /// <summary>
        /// Parses a CSV line into fields, handling RFC 4180 escaping.
        /// </summary>
        private List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;
            bool previousWasQuote = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes)
                    {
                        // Check if this is an escaped quote (two quotes in a row)
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            currentField.Append('"');
                            i++; // Skip the next quote
                            previousWasQuote = false;
                        }
                        else
                        {
                            // End of quoted field
                            inQuotes = false;
                            previousWasQuote = true;
                        }
                    }
                    else
                    {
                        // Start of quoted field
                        inQuotes = true;
                        previousWasQuote = false;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // Field separator
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                    previousWasQuote = false;
                }
                else
                {
                    currentField.Append(c);
                    previousWasQuote = false;
                }
            }

            // Add the last field
            fields.Add(currentField.ToString());

            return fields;
        }
    }
}
