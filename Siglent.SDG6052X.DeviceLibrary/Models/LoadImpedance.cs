namespace Siglent.SDG6052X.DeviceLibrary.Models
{
    public class LoadImpedance
    {
        public LoadType Type { get; set; }
        public double Value { get; set; }            // Ohms (for custom)
        
        public static LoadImpedance HighZ => new LoadImpedance { Type = LoadType.HighZ };
        public static LoadImpedance FiftyOhm => new LoadImpedance { Type = LoadType.FiftyOhm, Value = 50 };
        public static LoadImpedance Custom(double ohms) => new LoadImpedance { Type = LoadType.Custom, Value = ohms };
    }
}
