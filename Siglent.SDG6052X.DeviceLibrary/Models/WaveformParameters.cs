namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class WaveformParameters
    {
        public double Frequency { get; set; }        // Hz (1 µHz to 500 MHz)
        public double Amplitude { get; set; }        // Vpp or dBm
        public double Offset { get; set; }           // V DC
        public double Phase { get; set; }            // Degrees (0-360)
        public double DutyCycle { get; set; }        // Percent (for square/pulse)
        public double Width { get; set; }            // Seconds (for pulse)
        public double Rise { get; set; }             // Seconds (for pulse)
        public double Fall { get; set; }             // Seconds (for pulse)
        public double Delay { get; set; }            // Seconds
        public AmplitudeUnit Unit { get; set; }      // Vpp, Vrms, dBm
        public LoadImpedance Load { get; set; }      // Load impedance
    }
}
