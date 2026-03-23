using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Simulation
{
    /// <summary>
    /// Represents the simulated state of a single channel
    /// </summary>
    public class SimulatedChannelState
    {
        // Basic waveform settings
        public WaveformType WaveformType { get; set; }
        public double Frequency { get; set; }
        public double Amplitude { get; set; }
        public AmplitudeUnit AmplitudeUnit { get; set; }
        public double Offset { get; set; }
        public double Phase { get; set; }
        public double DutyCycle { get; set; }
        public double Width { get; set; }
        public double Rise { get; set; }
        public double Fall { get; set; }
        public double Delay { get; set; }

        // Output state
        public bool OutputEnabled { get; set; }

        // Load impedance
        public LoadImpedance Load { get; set; }

        // Modulation settings
        public bool ModulationEnabled { get; set; }
        public ModulationType ModulationType { get; set; }
        public ModulationSource ModulationSource { get; set; }
        public WaveformType ModulationWaveform { get; set; }
        public double ModulationDepth { get; set; }
        public double ModulationFrequency { get; set; }
        public double ModulationDeviation { get; set; }
        public double HopFrequency { get; set; }

        // Sweep settings
        public bool SweepEnabled { get; set; }
        public double SweepStartFrequency { get; set; }
        public double SweepStopFrequency { get; set; }
        public double SweepTime { get; set; }
        public SweepType SweepType { get; set; }
        public SweepDirection SweepDirection { get; set; }
        public TriggerSource SweepTriggerSource { get; set; }
        public double SweepReturnTime { get; set; }
        public double SweepHoldTime { get; set; }

        // Burst settings
        public bool BurstEnabled { get; set; }
        public BurstMode BurstMode { get; set; }
        public int BurstCycles { get; set; }
        public double BurstPeriod { get; set; }
        public TriggerSource BurstTriggerSource { get; set; }
        public TriggerEdge BurstTriggerEdge { get; set; }
        public double BurstStartPhase { get; set; }
        public GatePolarity BurstGatePolarity { get; set; }

        // Arbitrary waveform
        public string SelectedArbitraryWaveform { get; set; }

        /// <summary>
        /// Initialize with default values
        /// </summary>
        public SimulatedChannelState()
        {
            // Default basic waveform
            WaveformType = WaveformType.Sine;
            Frequency = 1000.0; // 1 kHz
            Amplitude = 5.0; // 5 Vpp
            AmplitudeUnit = AmplitudeUnit.Vpp;
            Offset = 0.0;
            Phase = 0.0;
            DutyCycle = 50.0;
            Width = 0.0001; // 100 us
            Rise = 0.00001; // 10 us
            Fall = 0.00001; // 10 us
            Delay = 0.0;

            // Default output state
            OutputEnabled = false;

            // Default load
            Load = LoadImpedance.HighZ;

            // Default modulation
            ModulationEnabled = false;
            ModulationType = ModulationType.AM;
            ModulationSource = ModulationSource.Internal;
            ModulationWaveform = WaveformType.Sine;
            ModulationDepth = 50.0;
            ModulationFrequency = 100.0;
            ModulationDeviation = 1000.0;
            HopFrequency = 2000.0;

            // Default sweep
            SweepEnabled = false;
            SweepStartFrequency = 100.0;
            SweepStopFrequency = 10000.0;
            SweepTime = 1.0;
            SweepType = SweepType.Linear;
            SweepDirection = SweepDirection.Up;
            SweepTriggerSource = TriggerSource.Internal;
            SweepReturnTime = 0.0;
            SweepHoldTime = 0.0;

            // Default burst
            BurstEnabled = false;
            BurstMode = BurstMode.NCycle;
            BurstCycles = 1;
            BurstPeriod = 0.001;
            BurstTriggerSource = TriggerSource.Internal;
            BurstTriggerEdge = TriggerEdge.Rising;
            BurstStartPhase = 0.0;
            BurstGatePolarity = GatePolarity.Positive;

            SelectedArbitraryWaveform = null;
        }
    }
}
