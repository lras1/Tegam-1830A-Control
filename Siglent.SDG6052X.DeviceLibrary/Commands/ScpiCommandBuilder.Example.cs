// This file demonstrates example usage of the ScpiCommandBuilder
// It can be removed once proper unit tests are created

using System;
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Commands
{
    /// <summary>
    /// Example usage of ScpiCommandBuilder
    /// </summary>
    public class ScpiCommandBuilderExample
    {
        public static void DemonstrateUsage()
        {
            var builder = new ScpiCommandBuilder();

            // Example 1: Basic sine wave command
            var sineParams = new WaveformParameters
            {
                Frequency = 1000,      // 1 kHz
                Amplitude = 5,         // 5 Vpp
                Offset = 0,
                Phase = 90,
                Unit = AmplitudeUnit.Vpp
            };
            string sineCommand = builder.BuildBasicWaveCommand(1, WaveformType.Sine, sineParams);
            // Expected: "C1:BSWV WVTP,SINE,FRQ,1KHZ,AMP,5VPP,OFST,0V,PHSE,90"

            // Example 2: Output state command
            string outputOn = builder.BuildOutputStateCommand(1, true);
            // Expected: "C1:OUTP ON"

            // Example 3: Load impedance command
            string loadCmd = builder.BuildLoadCommand(1, LoadImpedance.FiftyOhm);
            // Expected: "C1:OUTP LOAD,50"

            // Example 4: AM modulation command
            var amParams = new ModulationParameters
            {
                Type = ModulationType.AM,
                Source = ModulationSource.Internal,
                Depth = 50,
                Rate = 100
            };
            string amCommand = builder.BuildModulationCommand(1, ModulationType.AM, amParams);
            // Expected: "C1:MDWV AM,SRC,INT,DEPTH,50,FRQ,100HZ"

            // Example 5: Sweep command
            var sweepParams = new SweepParameters
            {
                StartFrequency = 1000,
                StopFrequency = 10000,
                Time = 1.0,
                Type = SweepType.Linear,
                Direction = SweepDirection.Up,
                TriggerSource = TriggerSource.Internal,
                ReturnTime = 0.1,
                HoldTime = 0.1
            };
            string sweepCommand = builder.BuildSweepCommand(1, sweepParams);
            // Expected: "C1:SWV TYPE,LINE,START,1KHZ,STOP,10KHZ,TIME,1,DIR,UP,TRSR,INT,RTIME,0.1,HTIME,0.1"

            // Example 6: Query command
            string queryCmd = builder.BuildQueryCommand(1, QueryType.BasicWaveform);
            // Expected: "C1:BSWV?"

            // Example 7: System command
            string identityCmd = builder.BuildSystemCommand(SystemCommandType.Identity);
            // Expected: "*IDN?"
        }
    }
}
