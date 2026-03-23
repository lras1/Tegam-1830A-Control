using System;

namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DeviceError Error { get; set; }
        public DateTime Timestamp { get; set; }
        
        public static OperationResult Successful(string message = "Operation completed successfully")
        {
            return new OperationResult 
            { 
                Success = true, 
                Message = message,
                Timestamp = DateTime.Now
            };
        }
        
        public static OperationResult Failed(string message, DeviceError error = null)
        {
            return new OperationResult 
            { 
                Success = false, 
                Message = message,
                Error = error,
                Timestamp = DateTime.Now
            };
        }
    }
}
