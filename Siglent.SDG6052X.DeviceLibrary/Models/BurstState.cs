namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class BurstState
    {
        public bool Enabled { get; set; }
        public BurstMode Mode { get; set; }
        public int Cycles { get; set; }
        public double Period { get; set; }
        public TriggerSource TriggerSource { get; set; }
        public TriggerEdge TriggerEdge { get; set; }
        public double StartPhase { get; set; }
    }
}
