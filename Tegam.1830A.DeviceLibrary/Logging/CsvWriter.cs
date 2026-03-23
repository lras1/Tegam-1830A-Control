using System;
using System.IO;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Logging
{
    /// <summary>
    /// Writes log entries to CSV files with RFC 4180 compliant formatting.
    /// </summary>
    public class CsvWriter : ICsvWriter
    {
        private readonly string _filename;
        private readonly object _fileLock = new object();
        private StreamWriter _writer;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the CsvWriter class.
        /// </summary>
        /// <param name="filename">The path to the CSV file.</param>
        /// <param name="append">True to append to existing file, false to create new file.</param>
        public CsvWriter(string filename, bool append)
        {
            _filename = filename;
            
            lock (_fileLock)
            {
                _writer = new StreamWriter(filename, append);
            }
        }

        /// <summary>
        /// Writes the CSV header row to the file.
        /// Header format: Type,Timestamp,Column3,Column4,Column5
        /// </summary>
        public void WriteHeader()
        {
            lock (_fileLock)
            {
                if (_writer != null && !_isDisposed)
                {
                    _writer.WriteLine("Type,Timestamp,Column3,Column4,Column5");
                }
            }
        }

        /// <summary>
        /// Writes a log entry to the CSV file.
        /// </summary>
        /// <param name="entry">The log entry to write.</param>
        public void WriteEntry(LogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            lock (_fileLock)
            {
                if (_writer != null && !_isDisposed)
                {
                    _writer.WriteLine(entry.ToCsvLine());
                }
            }
        }

        /// <summary>
        /// Flushes the file buffer to disk.
        /// </summary>
        public void Flush()
        {
            lock (_fileLock)
            {
                if (_writer != null && !_isDisposed)
                {
                    _writer.Flush();
                }
            }
        }

        /// <summary>
        /// Closes the CSV file and releases resources.
        /// </summary>
        public void Close()
        {
            lock (_fileLock)
            {
                if (_writer != null && !_isDisposed)
                {
                    _writer.Flush();
                    _writer.Close();
                    _writer.Dispose();
                    _writer = null;
                    _isDisposed = true;
                }
            }
        }

        /// <summary>
        /// Disposes of the CsvWriter and closes the file.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}
