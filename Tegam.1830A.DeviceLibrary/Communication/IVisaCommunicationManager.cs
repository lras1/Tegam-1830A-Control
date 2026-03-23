using System;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Communication
{
    /// <summary>
    /// Interface for VISA communication manager that handles low-level TCPIP communication with the Tegam 1830A device.
    /// </summary>
    public interface IVisaCommunicationManager : IDisposable
    {
        /// <summary>
        /// Establishes a connection to the device via TCPIP using NI-VISA.
        /// </summary>
        /// <param name="resourceName">The VISA resource name (e.g., "TCPIP::192.168.1.100::INSTR")</param>
        /// <param name="timeout">Connection timeout in milliseconds (default: 5000ms)</param>
        /// <returns>True if connection was successful, false otherwise</returns>
        bool Connect(string resourceName, int timeout = 5000);

        /// <summary>
        /// Closes the connection to the device and releases all resources.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets a value indicating whether the device is currently connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Sends a command to the device and waits for a response.
        /// </summary>
        /// <param name="command">The SCPI command to send</param>
        /// <returns>A CommandResult containing the response or error information</returns>
        CommandResult SendCommand(string command);

        /// <summary>
        /// Sends a query command to the device and returns the response as a string.
        /// </summary>
        /// <param name="query">The SCPI query command to send</param>
        /// <returns>The response string from the device</returns>
        string Query(string query);

        /// <summary>
        /// Sends a query command to the device and returns the response as binary data.
        /// </summary>
        /// <param name="query">The SCPI query command to send</param>
        /// <returns>The response as a byte array</returns>
        byte[] QueryBinary(string query);

        /// <summary>
        /// Gets the device identity string (typically from *IDN? query).
        /// </summary>
        /// <returns>The device identity string</returns>
        string GetDeviceIdentity();

        /// <summary>
        /// Event raised when a communication error occurs.
        /// </summary>
        event EventHandler<CommunicationErrorEventArgs> CommunicationError;
    }

    /// <summary>
    /// Event arguments for communication errors.
    /// </summary>
    public class CommunicationErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the exception that caused the error.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Initializes a new instance of the CommunicationErrorEventArgs class.
        /// </summary>
        public CommunicationErrorEventArgs(string errorMessage, Exception exception = null)
        {
            ErrorMessage = errorMessage;
            Exception = exception;
        }
    }
}
