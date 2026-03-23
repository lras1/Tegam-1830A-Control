namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class ModulationParameters
    {
        public ModulationType Type { get; set; }
        public ModulationSource Source { get; set; }
        public double Depth { get; set; }            // Percent (AM, PWM)
        public double Deviation { get; set; }        // Hz (FM, PM)
        public double Rate { get; set; }             // Hz (modulation frequency)
        public WaveformType ModulationWaveform { get; set; }
        public double HopFrequency { get; set; }     // Hz (FSK)
        public double HopAmplitude { get; set; }     // V (ASK)
        public double HopPhase { get; set; }         // Degrees (PSK)
    }
}
