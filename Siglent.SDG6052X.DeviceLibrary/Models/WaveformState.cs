namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class WaveformState
    {
        public WaveformType WaveformType { get; set; }
        public double Frequency { get; set; }
        public double Amplitude { get; set; }
        public double Offset { get; set; }
        public double Phase { get; set; }
        public double DutyCycle { get; set; }
        public AmplitudeUnit Unit { get; set; }
        public bool OutputEnabled { get; set; }
    }
}
