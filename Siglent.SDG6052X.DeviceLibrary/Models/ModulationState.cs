namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class ModulationState
    {
        public bool Enabled { get; set; }
        public ModulationType Type { get; set; }
        public ModulationSource Source { get; set; }
        public double Depth { get; set; }
        public double Deviation { get; set; }
        public double Rate { get; set; }
        public WaveformType ModulationWaveform { get; set; }
    }
}
