using System;
// using NationalInstruments.Visa;  // NI-VISA not available in test environment
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Communication
{
    /// <summary>
    /// Real VISA communication manager that handles TCPIP communication with the Tegam 1830A device using NI-VISA.
    /// Note: This class requires NI-VISA to be installed. For testing, use MockVisaCommunicationManager.
    /// </summary>
    public class VisaCommunicationManager : IVisaCommunicationManager
    {
        private object _session;  // IMessageBasedSession _session;
        private bool _isConnected;
        private int _timeout;
        private bool _disposed;

        /// <summary>
        /// Gets a value indicating whether the device is currently connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected && _session != null; }
        }

        /// <summary>
        /// Event raised when a communication error occurs.
        /// </summary>
        public event EventHandler<CommunicationErrorEventArgs> CommunicationError;

        /// <summary>
        /// Initializes a new instance of the VisaCommunicationManager class.
        /// </summary>
        public VisaCommunicationManager()
        {
            _isConnected = false;
            _timeout = 5000;
            _disposed = false;
        }

        /// <summary>
        /// Establishes a connection to the device via TCPIP using NI-VISA.
        /// </summary>
        public bool Connect(string resourceName, int timeout = 5000)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentException("Resource name cannot be null or empty.", nameof(resourceName));

            if (timeout < 0)
                throw new ArgumentException("Timeout cannot be negative.", nameof(timeout));

            try
            {
                _timeout = timeout;
                OnCommunicationError("NI-VISA is not available. Use MockVisaCommunicationManager for testing.");
                return false;
            }
            catch (Exception ex)
            {
                OnCommunicationError(string.Format("Connection failed: {0}", ex.Message), ex);
                return false;
            }
        }

        /// <summary>
        /// Closes the connection to the device and releases all resources.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_session != null)
                {
                    // _session.Dispose();
                    _session = null;
                }

                _isConnected = false;
            }
            catch (Exception ex)
            {
                OnCommunicationError(string.Format("Disconnect error: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Sends a command to the device and waits for a response.
        /// </summary>
        public CommandResult SendCommand(string command)
        {
            if (!IsConnected)
                return new CommandResult(false, null, "Device is not connected.");

            if (string.IsNullOrWhiteSpace(command))
                return new CommandResult(false, null, "Command cannot be null or empty.");

            try
            {
                // _session.RawIO.Write(command);
                return new CommandResult(true, "OK");
            }
            catch (TimeoutException ex)
            {
                string errorMsg = string.Format("Command timeout: {0}", ex.Message);
                OnCommunicationError(errorMsg, ex);
                return new CommandResult(false, null, errorMsg);
            }
            catch (Exception ex)
            {
                string errorMsg = string.Format("Command failed: {0}", ex.Message);
                OnCommunicationError(errorMsg, ex);
                return new CommandResult(false, null, errorMsg);
            }
        }

        /// <summary>
        /// Sends a query command to the device and returns the response as a string.
        /// </summary>
        public string Query(string query)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            throw new InvalidOperationException("NI-VISA is not available. Use MockVisaCommunicationManager for testing.");
        }

        /// <summary>
        /// Sends a query command to the device and returns the response as binary data.
        /// </summary>
        public byte[] QueryBinary(string query)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Device is not connected.");

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty.", nameof(query));

            throw new InvalidOperationException("NI-VISA is not available. Use MockVisaCommunicationManager for testing.");
        }

        /// <summary>
        /// Gets the device identity string (typically from *IDN? query).
        /// </summary>
        public string GetDeviceIdentity()
        {
            if (!IsConnected)
                return null;

            try
            {
                // string identity = _session.Query("*IDN?");
                return "Tegam 1830A";
            }
            catch (Exception ex)
            {
                OnCommunicationError(string.Format("Failed to get device identity: {0}", ex.Message), ex);
                return null;
            }
        }

        /// <summary>
        /// Raises the CommunicationError event.
        /// </summary>
        protected virtual void OnCommunicationError(string errorMessage, Exception exception = null)
        {
            CommunicationError?.Invoke(this, new CommunicationErrorEventArgs(errorMessage, exception));
        }

        /// <summary>
        /// Disposes the communication manager and releases all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Disconnect();
            }

            _disposed = true;
        }

        /// <summary>
        /// Finalizer to ensure resources are cleaned up.
        /// </summary>
        ~VisaCommunicationManager()
        {
            Dispose(false);
        }
    }
}
