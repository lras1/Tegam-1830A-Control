namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class SweepParameters
    {
        public double StartFrequency { get; set; }   // Hz
        public double StopFrequency { get; set; }    // Hz
        public double Time { get; set; }             // Seconds
        public SweepType Type { get; set; }
        public SweepDirection Direction { get; set; }
        public TriggerSource TriggerSource { get; set; }
        public double ReturnTime { get; set; }       // Seconds
        public double HoldTime { get; set; }         // Seconds
    }
}
