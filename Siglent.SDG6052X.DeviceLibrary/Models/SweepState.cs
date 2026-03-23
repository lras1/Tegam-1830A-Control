namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class SweepState
    {
        public bool Enabled { get; set; }
        public double StartFrequency { get; set; }
        public double StopFrequency { get; set; }
        public double Time { get; set; }
        public SweepType Type { get; set; }
        public SweepDirection Direction { get; set; }
        public TriggerSource TriggerSource { get; set; }
    }
}
