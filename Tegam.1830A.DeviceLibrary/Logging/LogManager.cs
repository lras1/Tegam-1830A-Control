using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Logging
{
    /// <summary>
    /// Manages log operations with queuing and automatic flushing.
    /// </summary>
    public class LogManager : ILogManager
    {
        private readonly ConcurrentQueue<LogEntry> _entryQueue;
        private readonly object _stateLock = new object();
        private readonly Timer _flushTimer;
        
        private ICsvWriter _csvWriter;
        private LoggingState _currentState;
        private string _currentLogFile;
        private int _totalEntryCount;
        private int _entriesSinceFlush;
        private DateTime _lastFlushTime;
        private bool _isProcessing;

        /// <summary>
        /// Gets the current logging state.
        /// </summary>
        public LoggingState CurrentState
        {
            get { lock (_stateLock) { return _currentState; } }
        }

        /// <summary>
        /// Gets the total number of entries logged.
        /// </summary>
        public int TotalEntryCount
        {
            get { return _totalEntryCount; }
        }

        /// <summary>
        /// Gets the current log file path.
        /// </summary>
        public string CurrentLogFile
        {
            get { lock (_stateLock) { return _currentLogFile; } }
        }

        /// <summary>
        /// Event raised when an entry is logged.
        /// </summary>
        public event EventHandler<LogEntryEventArgs> EntryLogged;

        /// <summary>
        /// Event raised when the logging state changes.
        /// </summary>
        public event EventHandler<LoggingStateChangedEventArgs> StateChanged;

        /// <summary>
        /// Event raised when a write error occurs.
        /// </summary>
        public event EventHandler<WriteErrorEventArgs> WriteError;

        /// <summary>
        /// Initializes a new instance of the LogManager class.
        /// </summary>
        public LogManager()
        {
            _entryQueue = new ConcurrentQueue<LogEntry>();
            _currentState = LoggingState.NotStarted;
            _lastFlushTime = DateTime.Now;
            
            // Create flush timer (checks every second)
            _flushTimer = new Timer(FlushTimerCallback, null, 1000, 1000);
        }

        /// <summary>
        /// Starts logging to the specified file.
        /// </summary>
        public void StartLogging(string filename)
        {
            lock (_stateLock)
            {
                if (_currentState == LoggingState.Active)
                    throw new InvalidOperationException("Logging is already active");

                try
                {
                    // Determine if we should append or create new file
                    bool append = File.Exists(filename);
                    
                    _csvWriter = new CsvWriter(filename, append);
                    
                    // Write header if new file
                    if (!append)
                    {
                        _csvWriter.WriteHeader();
                    }

                    var oldState = _currentState;
                    _currentState = LoggingState.Active;
                    _currentLogFile = filename;
                    _totalEntryCount = 0;
                    _entriesSinceFlush = 0;
                    _lastFlushTime = DateTime.Now;

                    // Log the logging start event
                    var startEntry = new SettingEntry
                    {
                        SettingName = "Logging",
                        SettingValue = "Started: " + Path.GetFileName(filename),
                        Context = "User initiated"
                    };
                    LogEntry(startEntry);

                    // Process any queued entries
                    ProcessQueue();

                    // Raise state changed event
                    OnStateChanged(new LoggingStateChangedEventArgs
                    {
                        OldState = oldState,
                        NewState = _currentState,
                        Filename = filename
                    });
                }
                catch (Exception ex)
                {
                    OnWriteError("Failed to start logging: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Stops logging and closes the log file.
        /// </summary>
        public void StopLogging()
        {
            lock (_stateLock)
            {
                if (_currentState != LoggingState.Active)
                    return;

                try
                {
                    // Log the logging stop event
                    var stopEntry = new SettingEntry
                    {
                        SettingName = "Logging",
                        SettingValue = string.Format("Stopped: {0} entries", _totalEntryCount),
                        Context = "User initiated"
                    };
                    LogEntry(stopEntry);

                    // Process remaining queue
                    ProcessQueue();

                    // Flush and close
                    if (_csvWriter != null)
                    {
                        _csvWriter.Flush();
                        _csvWriter.Close();
                        _csvWriter = null;
                    }

                    var oldState = _currentState;
                    _currentState = LoggingState.Stopped;
                    string filename = _currentLogFile;
                    _currentLogFile = null;

                    // Raise state changed event
                    OnStateChanged(new LoggingStateChangedEventArgs
                    {
                        OldState = oldState,
                        NewState = _currentState,
                        Filename = filename
                    });
                }
                catch (Exception ex)
                {
                    OnWriteError("Error stopping logging: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Logs an entry (queues it for writing).
        /// </summary>
        public void LogEntry(LogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            _entryQueue.Enqueue(entry);

            // Raise event immediately
            OnEntryLogged(entry);

            // Process queue if logging is active
            if (_currentState == LoggingState.Active)
            {
                ProcessQueue();
            }
        }

        /// <summary>
        /// Processes the entry queue and writes to file.
        /// </summary>
        private void ProcessQueue()
        {
            // Prevent concurrent processing
            if (_isProcessing)
                return;

            _isProcessing = true;

            try
            {
                LogEntry entry;
                while (_entryQueue.TryDequeue(out entry))
                {
                    if (_csvWriter != null && _currentState == LoggingState.Active)
                    {
                        if (WriteEntryWithRetry(entry))
                        {
                            _totalEntryCount++;
                            _entriesSinceFlush++;
                            
                            // Check if we should flush
                            CheckFlush();
                        }
                    }
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        /// <summary>
        /// Writes an entry with retry logic.
        /// </summary>
        private bool WriteEntryWithRetry(LogEntry entry, int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    _csvWriter.WriteEntry(entry);
                    return true;
                }
                catch (IOException ex)
                {
                    if (attempt == maxRetries)
                    {
                        OnWriteError(string.Format("Failed to write entry after {0} attempts: {1}", 
                            maxRetries, ex.Message));
                        return false;
                    }
                    Thread.Sleep(100);
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if flush is needed and performs it.
        /// Flush triggers: 10 entries or 5 seconds.
        /// </summary>
        private void CheckFlush()
        {
            bool shouldFlush = _entriesSinceFlush >= 10 ||
                               (DateTime.Now - _lastFlushTime).TotalSeconds >= 5;

            if (shouldFlush && _csvWriter != null)
            {
                try
                {
                    _csvWriter.Flush();
                    _entriesSinceFlush = 0;
                    _lastFlushTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    OnWriteError("Error flushing file: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Timer callback for periodic flush check.
        /// </summary>
        private void FlushTimerCallback(object state)
        {
            if (_currentState == LoggingState.Active)
            {
                CheckFlush();
            }
        }

        /// <summary>
        /// Raises the EntryLogged event.
        /// </summary>
        protected virtual void OnEntryLogged(LogEntry entry)
        {
            var handler = EntryLogged;
            if (handler != null)
            {
                handler(this, new LogEntryEventArgs(entry));
            }
        }

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        protected virtual void OnStateChanged(LoggingStateChangedEventArgs e)
        {
            var handler = StateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the WriteError event.
        /// </summary>
        protected virtual void OnWriteError(string error)
        {
            var handler = WriteError;
            if (handler != null)
            {
                handler(this, new WriteErrorEventArgs(error));
            }
        }
    }
}
