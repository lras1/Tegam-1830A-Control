namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class BurstParameters
    {
        public BurstMode Mode { get; set; }
        public int Cycles { get; set; }              // Number of cycles (1-1000000)
        public double Period { get; set; }           // Seconds
        public TriggerSource TriggerSource { get; set; }
        public TriggerEdge TriggerEdge { get; set; }
        public double StartPhase { get; set; }       // Degrees
        public GatePolarity GatePolarity { get; set; }
    }
}
