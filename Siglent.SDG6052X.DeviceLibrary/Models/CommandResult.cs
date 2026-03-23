using System;

namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Response { get; set; }
        public byte[] BinaryData { get; set; }
        public Exception Exception { get; set; }
        public int ExecutionTimeMs { get; set; }
    }
}
