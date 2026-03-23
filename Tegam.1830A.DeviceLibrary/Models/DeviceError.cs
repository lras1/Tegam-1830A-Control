using System;

namespace Tegam._1830A.DeviceLibrary.Models
{
    /// <summary>
    /// Represents an error reported by the Tegam 1830A device.
    /// </summary>
    public class DeviceError
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }

        public DeviceError()
        {
            Timestamp = DateTime.Now;
        }

        public DeviceError(int errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Timestamp = DateTime.Now;
        }
    }
}
