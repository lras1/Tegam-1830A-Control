using System;

namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Event arguments for write error events.
    /// </summary>
    public class WriteErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the WriteErrorEventArgs class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public WriteErrorEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
