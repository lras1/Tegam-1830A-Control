using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
#if !NO_VISA
using Ivi.Visa;
#endif
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Communication
{
    /// <summary>
    /// Real VISA communication manager using NI-VISA library
    /// </summary>
    public class VisaCommunicationManager : IVisaCommunicationManager
    {
#if !NO_VISA
        private IMessageBasedSession _session;
#else
        private object _session;
#endif
        private bool _isConnected;
        private int _timeout;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Event raised when a communication error occurs
        /// </summary>
        public event EventHandler<CommunicationErrorEventArgs> CommunicationError;

        /// <summary>
        /// Gets whether the device is currently connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                lock (_lockObject)
                {
                    return _isConnected && _session != null;
                }
            }
        }

        /// <summary>
        /// Connect to the device using the specified resource name
        /// </summary>
        public bool Connect(string resourceName, int timeout = 5000)
        {
#if NO_VISA
            throw new NotImplementedException("VISA support is not available. Use MockVisaCommunicationManager for testing.");
#else
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                RaiseCommunicationError("Resource name cannot be null or empty", null);
                return false;
            }

            try
            {
                lock (_lockObject)
                {
                    // Disconnect if already connected
                    if (_isConnected)
                    {
                        Disconnect();
                    }

                    _timeout = timeout;

                    // Open VISA session
                    var resourceManager = new ResourceManager();
                    _session = (IMessageBasedSession)resourceManager.Open(resourceName);

                    // Configure session
                    _session.TimeoutMilliseconds = timeout;
                    _session.TerminationCharacterEnabled = true;
                    _session.TerminationCharacter = 0x0A; // Line feed

                    // Clear device status
                    _session.FormattedIO.WriteLine("*CLS");
                    _session.FormattedIO.FlushWrite(true);

                    _isConnected = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                RaiseCommunicationError($"Failed to connect to {resourceName}", ex);
                _isConnected = false;
                return false;
            }
#endif
        }

        /// <summary>
        /// Disconnect from the device
        /// </summary>
        public void Disconnect()
        {
#if NO_VISA
            _isConnected = false;
#else
            lock (_lockObject)
            {
                if (_session != null)
                {
                    try
                    {
                        _session.Dispose();
                    }
                    catch (Exception ex)
                    {
                        RaiseCommunicationError("Error during disconnect", ex);
                    }
                    finally
                    {
                        _session = null;
                        _isConnected = false;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Send a command to the device without expecting a response
        /// </summary>
        public CommandResult SendCommand(string command)
        {
#if NO_VISA
            throw new NotImplementedException("VISA support is not available. Use MockVisaCommunicationManager for testing.");
#else
            if (string.IsNullOrWhiteSpace(command))
            {
                return new CommandResult
                {
                    Success = false,
                    Response = "Command cannot be null or empty"
                };
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                lock (_lockObject)
                {
                    if (!_isConnected || _session == null)
                    {
                        return new CommandResult
                        {
                            Success = false,
                            Response = "Not connected to device"
                        };
                    }

                    _session.FormattedIO.WriteLine(command);
                    _session.FormattedIO.FlushWrite(true);

                    stopwatch.Stop();

                    return new CommandResult
                    {
                        Success = true,
                        Response = "Command sent successfully",
                        ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds
                    };
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                RaiseCommunicationError($"Error sending command: {command}", ex);

                return new CommandResult
                {
                    Success = false,
                    Response = ex.Message,
                    Exception = ex,
                    ExecutionTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }
#endif
        }

        /// <summary>
        /// Send a query to the device and receive a string response
        /// </summary>
        public string Query(string query)
        {
#if NO_VISA
            throw new NotImplementedException("VISA support is not available. Use MockVisaCommunicationManager for testing.");
#else
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be null or empty", nameof(query));
            }

            try
            {
                lock (_lockObject)
                {
                    if (!_isConnected || _session == null)
                    {
                        throw new InvalidOperationException("Not connected to device");
                    }

                    _session.FormattedIO.WriteLine(query);
                    _session.FormattedIO.FlushWrite(true);

                    string response = _session.FormattedIO.ReadLine();
                    return response?.Trim();
                }
            }
            catch (Exception ex)
            {
                RaiseCommunicationError($"Error executing query: {query}", ex);
                throw;
            }
#endif
        }

        /// <summary>
        /// Send a query to the device and receive a binary response
        /// </summary>
        public byte[] QueryBinary(string query)
        {
#if NO_VISA
            throw new NotImplementedException("VISA support is not available. Use MockVisaCommunicationManager for testing.");
#else
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be null or empty", nameof(query));
            }

            try
            {
                lock (_lockObject)
                {
                    if (!_isConnected || _session == null)
                    {
                        throw new InvalidOperationException("Not connected to device");
                    }

                    _session.FormattedIO.WriteLine(query);
                    _session.FormattedIO.FlushWrite(true);

                    // Read binary data
                    byte[] data = _session.RawIO.Read();
                    return data;
                }
            }
            catch (Exception ex)
            {
                RaiseCommunicationError($"Error executing binary query: {query}", ex);
                throw;
            }
#endif
        }

        /// <summary>
        /// Send a command asynchronously
        /// </summary>
        public Task<CommandResult> SendCommandAsync(string command)
        {
            return Task.Factory.StartNew(() => SendCommand(command));
        }

        /// <summary>
        /// Send a query asynchronously
        /// </summary>
        public Task<string> QueryAsync(string query)
        {
            return Task.Factory.StartNew(() => Query(query));
        }

        /// <summary>
        /// Get the device identity string
        /// </summary>
        public string GetDeviceIdentity()
        {
            try
            {
                return Query("*IDN?");
            }
            catch (Exception ex)
            {
                RaiseCommunicationError("Error getting device identity", ex);
                throw;
            }
        }

        /// <summary>
        /// Raise communication error event
        /// </summary>
        private void RaiseCommunicationError(string message, Exception exception)
        {
            CommunicationError?.Invoke(this, new CommunicationErrorEventArgs
            {
                Message = message,
                Exception = exception,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
    }
}
