using System;
using System.Threading.Tasks;
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Communication
{
    /// <summary>
    /// Event arguments for communication errors
    /// </summary>
    public class CommunicationErrorEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Interface for managing VISA communication with the SDG6052X signal generator
    /// </summary>
    public interface IVisaCommunicationManager : IDisposable
    {
        /// <summary>
        /// Connect to the device using the specified resource name
        /// </summary>
        /// <param name="resourceName">VISA resource name (e.g., "TCPIP::192.168.1.100::INSTR")</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <returns>True if connection successful, false otherwise</returns>
        bool Connect(string resourceName, int timeout = 5000);

        /// <summary>
        /// Disconnect from the device
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets whether the device is currently connected
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Send a command to the device without expecting a response
        /// </summary>
        /// <param name="command">SCPI command string</param>
        /// <returns>Command result indicating success or failure</returns>
        CommandResult SendCommand(string command);

        /// <summary>
        /// Send a query to the device and receive a string response
        /// </summary>
        /// <param name="query">SCPI query string</param>
        /// <returns>Response string from the device</returns>
        string Query(string query);

        /// <summary>
        /// Send a query to the device and receive a binary response
        /// </summary>
        /// <param name="query">SCPI query string</param>
        /// <returns>Binary data from the device</returns>
        byte[] QueryBinary(string query);

        /// <summary>
        /// Send a command asynchronously
        /// </summary>
        /// <param name="command">SCPI command string</param>
        /// <returns>Task returning command result</returns>
        Task<CommandResult> SendCommandAsync(string command);

        /// <summary>
        /// Send a query asynchronously
        /// </summary>
        /// <param name="query">SCPI query string</param>
        /// <returns>Task returning response string</returns>
        Task<string> QueryAsync(string query);

        /// <summary>
        /// Get the device identity string
        /// </summary>
        /// <returns>Device identity string from *IDN? query</returns>
        string GetDeviceIdentity();

        /// <summary>
        /// Event raised when a communication error occurs
        /// </summary>
        event EventHandler<CommunicationErrorEventArgs> CommunicationError;
    }
}
