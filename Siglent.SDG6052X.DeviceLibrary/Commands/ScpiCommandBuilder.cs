using System;
using System.Globalization;
using System.Text;
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Commands
{
    /// <summary>
    /// Builds SCPI commands for the Siglent SDG6052X signal generator
    /// </summary>
    public class ScpiCommandBuilder : IScpiCommandBuilder
    {
        public string BuildBasicWaveCommand(int channel, WaveformType type, WaveformParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            StringBuilder command = new StringBuilder($"C{channel}:BSWV ");

            // Add waveform type
            command.Append($"WVTP,{MapWaveformTypeToScpi(type)}");

            // Add frequency with appropriate unit
            string freqUnit = DetermineFrequencyUnit(parameters.Frequency);
            double freqValue = ConvertFrequencyToUnit(parameters.Frequency, freqUnit);
            command.Append($",FRQ,{FormatNumber(freqValue)}{freqUnit}");

            // Add amplitude with unit
            string ampUnit = MapAmplitudeUnitToScpi(parameters.Unit);
            command.Append($",AMP,{FormatNumber(parameters.Amplitude)}{ampUnit}");

            // Add offset
            command.Append($",OFST,{FormatNumber(parameters.Offset)}V");

            // Add phase
            command.Append($",PHSE,{FormatNumber(parameters.Phase)}");

            // Add waveform-specific parameters
            if (type == WaveformType.Square || type == WaveformType.Pulse)
            {
                command.Append($",DUTY,{FormatNumber(parameters.DutyCycle)}");
            }

            if (type == WaveformType.Pulse)
            {
                command.Append($",WIDTH,{FormatNumber(parameters.Width)}");
                command.Append($",RISE,{FormatNumber(parameters.Rise)}");
                command.Append($",FALL,{FormatNumber(parameters.Fall)}");
            }

            return command.ToString();
        }

        public string BuildOutputStateCommand(int channel, bool enabled)
        {
            return $"C{channel}:OUTP {(enabled ? "ON" : "OFF")}";
        }

        public string BuildLoadCommand(int channel, LoadImpedance load)
        {
            if (load == null)
                throw new ArgumentNullException(nameof(load));

            string loadValue;
            switch (load.Type)
            {
                case LoadType.HighZ:
                    loadValue = "HZ";
                    break;
                case LoadType.FiftyOhm:
                    loadValue = "50";
                    break;
                case LoadType.Custom:
                    loadValue = FormatNumber(load.Value);
                    break;
                default:
                    throw new ArgumentException($"Unknown load type: {load.Type}");
            }

            return $"C{channel}:OUTP LOAD,{loadValue}";
        }

        public string BuildModulationCommand(int channel, ModulationType type, ModulationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            StringBuilder command = new StringBuilder($"C{channel}:");

            switch (type)
            {
                case ModulationType.AM:
                    command.Append("MDWV AM");
                    command.Append($",SRC,{MapModulationSourceToScpi(parameters.Source)}");
                    command.Append($",DEPTH,{FormatNumber(parameters.Depth)}");
                    command.Append($",FRQ,{FormatFrequency(parameters.Rate)}");
                    break;

                case ModulationType.FM:
                    command.Append("MDWV FM");
                    command.Append($",SRC,{MapModulationSourceToScpi(parameters.Source)}");
                    command.Append($",DEVI,{FormatFrequency(parameters.Deviation)}");
                    command.Append($",FRQ,{FormatFrequency(parameters.Rate)}");
                    break;

                case ModulationType.PM:
                    command.Append("MDWV PM");
                    command.Append($",SRC,{MapModulationSourceToScpi(parameters.Source)}");
                    command.Append($",DEVI,{FormatNumber(parameters.Deviation)}");
                    command.Append($",FRQ,{FormatFrequency(parameters.Rate)}");
                    break;

                case ModulationType.PWM:
                    command.Append("MDWV PWM");
                    command.Append($",SRC,{MapModulationSourceToScpi(parameters.Source)}");
                    command.Append($",DEVI,{FormatNumber(parameters.Depth)}");
                    command.Append($",FRQ,{FormatFrequency(parameters.Rate)}");
                    break;

                case ModulationType.FSK:
                    command.Append("MDWV FSK");
                    command.Append($",SRC,{MapModulationSourceToScpi(parameters.Source)}");
                    command.Append($",HOP_FRQ,{FormatFrequency(parameters.HopFrequency)}");
                    command.Append($",FRQ,{FormatFrequency(parameters.Rate)}");
                    break;

                case ModulationType.ASK:
                    command.Append("MDWV ASK");
                    command.Append($",SRC,{MapModulationSourceToScpi(parameters.Source)}");
                    command.Append($",HOP_AMP,{FormatNumber(parameters.HopAmplitude)}V");
                    command.Append($",FRQ,{FormatFrequency(parameters.Rate)}");
                    break;

                case ModulationType.PSK:
                    command.Append("MDWV PSK");
                    command.Append($",SRC,{MapModulationSourceToScpi(parameters.Source)}");
                    command.Append($",HOP_PHSE,{FormatNumber(parameters.HopPhase)}");
                    command.Append($",FRQ,{FormatFrequency(parameters.Rate)}");
                    break;

                default:
                    throw new ArgumentException($"Unknown modulation type: {type}");
            }

            return command.ToString();
        }

        public string BuildModulationStateCommand(int channel, bool enabled)
        {
            return $"C{channel}:MDWV STATE,{(enabled ? "ON" : "OFF")}";
        }

        public string BuildSweepCommand(int channel, SweepParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            StringBuilder command = new StringBuilder($"C{channel}:SWV ");

            // Add sweep type
            command.Append($"TYPE,{MapSweepTypeToScpi(parameters.Type)}");

            // Add start and stop frequencies
            command.Append($",START,{FormatFrequency(parameters.StartFrequency)}");
            command.Append($",STOP,{FormatFrequency(parameters.StopFrequency)}");

            // Add sweep time
            command.Append($",TIME,{FormatNumber(parameters.Time)}");

            // Add direction
            command.Append($",DIR,{MapSweepDirectionToScpi(parameters.Direction)}");

            // Add trigger source
            command.Append($",TRSR,{MapTriggerSourceToScpi(parameters.TriggerSource)}");

            // Add return time and hold time
            command.Append($",RTIME,{FormatNumber(parameters.ReturnTime)}");
            command.Append($",HTIME,{FormatNumber(parameters.HoldTime)}");

            return command.ToString();
        }

        public string BuildSweepStateCommand(int channel, bool enabled)
        {
            return $"C{channel}:SWV STATE,{(enabled ? "ON" : "OFF")}";
        }

        public string BuildBurstCommand(int channel, BurstParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            StringBuilder command = new StringBuilder($"C{channel}:BTWV ");

            // Add burst mode
            command.Append($"STATE,{MapBurstModeToScpi(parameters.Mode)}");

            // Add trigger source
            command.Append($",TRSR,{MapTriggerSourceToScpi(parameters.TriggerSource)}");

            // Add mode-specific parameters
            if (parameters.Mode == BurstMode.NCycle)
            {
                command.Append($",TIME,{parameters.Cycles}");
                command.Append($",PRD,{FormatNumber(parameters.Period)}");
                command.Append($",EDGE,{MapTriggerEdgeToScpi(parameters.TriggerEdge)}");
            }
            else if (parameters.Mode == BurstMode.Gated)
            {
                command.Append($",GATE_NCYC,{parameters.Cycles}");
                command.Append($",PLRT,{MapGatePolarityToScpi(parameters.GatePolarity)}");
            }

            // Add start phase
            command.Append($",STPS,{FormatNumber(parameters.StartPhase)}");

            return command.ToString();
        }

        public string BuildBurstStateCommand(int channel, bool enabled)
        {
            return $"C{channel}:BTWV STATE,{(enabled ? "ON" : "OFF")}";
        }

        public string BuildArbitraryWaveCommand(int channel, string waveformName)
        {
            if (string.IsNullOrWhiteSpace(waveformName))
                throw new ArgumentException("Waveform name cannot be null or empty", nameof(waveformName));

            return $"C{channel}:ARWV NAME,{waveformName}";
        }

        public string BuildStoreArbitraryWaveCommand(string name, double[] points)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Waveform name cannot be null or empty", nameof(name));
            if (points == null || points.Length == 0)
                throw new ArgumentException("Points array cannot be null or empty", nameof(points));

            StringBuilder command = new StringBuilder($"STL {name},");
            
            // Add number of points
            command.Append($"{points.Length},");
            
            // Add waveform data points (comma-separated)
            for (int i = 0; i < points.Length; i++)
            {
                command.Append(FormatNumber(points[i]));
                if (i < points.Length - 1)
                    command.Append(",");
            }

            return command.ToString();
        }

        public string BuildQueryCommand(int channel, QueryType queryType)
        {
            switch (queryType)
            {
                case QueryType.BasicWaveform:
                    return $"C{channel}:BSWV?";
                case QueryType.OutputState:
                    return $"C{channel}:OUTP?";
                case QueryType.Load:
                    return $"C{channel}:OUTP? LOAD";
                case QueryType.Modulation:
                    return $"C{channel}:MDWV?";
                case QueryType.ModulationState:
                    return $"C{channel}:MDWV? STATE";
                case QueryType.Sweep:
                    return $"C{channel}:SWV?";
                case QueryType.SweepState:
                    return $"C{channel}:SWV? STATE";
                case QueryType.Burst:
                    return $"C{channel}:BTWV?";
                case QueryType.BurstState:
                    return $"C{channel}:BTWV? STATE";
                default:
                    throw new ArgumentException($"Unknown query type: {queryType}");
            }
        }

        public string BuildSystemCommand(SystemCommandType type, params object[] parameters)
        {
            switch (type)
            {
                case SystemCommandType.Identity:
                    return "*IDN?";
                case SystemCommandType.Reset:
                    return "*RST";
                case SystemCommandType.Error:
                    return "SYST:ERR?";
                case SystemCommandType.RecallSetup:
                    if (parameters == null || parameters.Length == 0)
                        throw new ArgumentException("RecallSetup requires a setup number parameter");
                    return $"*RCL {parameters[0]}";
                case SystemCommandType.SaveSetup:
                    if (parameters == null || parameters.Length == 0)
                        throw new ArgumentException("SaveSetup requires a setup number parameter");
                    return $"*SAV {parameters[0]}";
                default:
                    throw new ArgumentException($"Unknown system command type: {type}");
            }
        }

        #region Helper Methods - Unit Conversion

        /// <summary>
        /// Determines the appropriate frequency unit (HZ, KHZ, MHZ) based on the frequency value
        /// </summary>
        private string DetermineFrequencyUnit(double frequencyHz)
        {
            if (frequencyHz >= 1e6)
                return "MHZ";
            else if (frequencyHz >= 1e3)
                return "KHZ";
            else
                return "HZ";
        }

        /// <summary>
        /// Converts frequency from Hz to the specified unit
        /// </summary>
        private double ConvertFrequencyToUnit(double frequencyHz, string unit)
        {
            switch (unit.ToUpperInvariant())
            {
                case "MHZ":
                    return frequencyHz / 1e6;
                case "KHZ":
                    return frequencyHz / 1e3;
                case "HZ":
                default:
                    return frequencyHz;
            }
        }

        /// <summary>
        /// Formats a frequency value with appropriate unit
        /// </summary>
        private string FormatFrequency(double frequencyHz)
        {
            string unit = DetermineFrequencyUnit(frequencyHz);
            double value = ConvertFrequencyToUnit(frequencyHz, unit);
            return $"{FormatNumber(value)}{unit}";
        }

        /// <summary>
        /// Formats a numeric value for SCPI commands using invariant culture
        /// </summary>
        private string FormatNumber(double value)
        {
            return value.ToString("G", CultureInfo.InvariantCulture);
        }

        #endregion

        #region Helper Methods - SCPI Enum Mapping

        /// <summary>
        /// Maps WaveformType enum to SCPI string
        /// </summary>
        private string MapWaveformTypeToScpi(WaveformType type)
        {
            switch (type)
            {
                case WaveformType.Sine:
                    return "SINE";
                case WaveformType.Square:
                    return "SQUARE";
                case WaveformType.Ramp:
                    return "RAMP";
                case WaveformType.Pulse:
                    return "PULSE";
                case WaveformType.Noise:
                    return "NOISE";
                case WaveformType.Arbitrary:
                    return "ARB";
                case WaveformType.DC:
                    return "DC";
                case WaveformType.PRBS:
                    return "PRBS";
                case WaveformType.IQ:
                    return "IQ";
                default:
                    throw new ArgumentException($"Unknown waveform type: {type}");
            }
        }

        /// <summary>
        /// Maps AmplitudeUnit enum to SCPI string
        /// </summary>
        private string MapAmplitudeUnitToScpi(AmplitudeUnit unit)
        {
            switch (unit)
            {
                case AmplitudeUnit.Vpp:
                    return "VPP";
                case AmplitudeUnit.Vrms:
                    return "VRMS";
                case AmplitudeUnit.dBm:
                    return "DBM";
                default:
                    throw new ArgumentException($"Unknown amplitude unit: {unit}");
            }
        }

        /// <summary>
        /// Maps ModulationSource enum to SCPI string
        /// </summary>
        private string MapModulationSourceToScpi(ModulationSource source)
        {
            switch (source)
            {
                case ModulationSource.Internal:
                    return "INT";
                case ModulationSource.External:
                    return "EXT";
                case ModulationSource.Channel1:
                    return "CH1";
                case ModulationSource.Channel2:
                    return "CH2";
                default:
                    throw new ArgumentException($"Unknown modulation source: {source}");
            }
        }

        /// <summary>
        /// Maps SweepType enum to SCPI string
        /// </summary>
        private string MapSweepTypeToScpi(SweepType type)
        {
            switch (type)
            {
                case SweepType.Linear:
                    return "LINE";
                case SweepType.Logarithmic:
                    return "LOG";
                default:
                    throw new ArgumentException($"Unknown sweep type: {type}");
            }
        }

        /// <summary>
        /// Maps SweepDirection enum to SCPI string
        /// </summary>
        private string MapSweepDirectionToScpi(SweepDirection direction)
        {
            switch (direction)
            {
                case SweepDirection.Up:
                    return "UP";
                case SweepDirection.Down:
                    return "DOWN";
                case SweepDirection.UpDown:
                    return "UPDOWN";
                default:
                    throw new ArgumentException($"Unknown sweep direction: {direction}");
            }
        }

        /// <summary>
        /// Maps TriggerSource enum to SCPI string
        /// </summary>
        private string MapTriggerSourceToScpi(TriggerSource source)
        {
            switch (source)
            {
                case TriggerSource.Internal:
                    return "INT";
                case TriggerSource.External:
                    return "EXT";
                case TriggerSource.Manual:
                    return "MAN";
                default:
                    throw new ArgumentException($"Unknown trigger source: {source}");
            }
        }

        /// <summary>
        /// Maps BurstMode enum to SCPI string
        /// </summary>
        private string MapBurstModeToScpi(BurstMode mode)
        {
            switch (mode)
            {
                case BurstMode.NCycle:
                    return "NCYC";
                case BurstMode.Gated:
                    return "GATE";
                default:
                    throw new ArgumentException($"Unknown burst mode: {mode}");
            }
        }

        /// <summary>
        /// Maps TriggerEdge enum to SCPI string
        /// </summary>
        private string MapTriggerEdgeToScpi(TriggerEdge edge)
        {
            switch (edge)
            {
                case TriggerEdge.Rising:
                    return "RISE";
                case TriggerEdge.Falling:
                    return "FALL";
                default:
                    throw new ArgumentException($"Unknown trigger edge: {edge}");
            }
        }

        /// <summary>
        /// Maps GatePolarity enum to SCPI string
        /// </summary>
        private string MapGatePolarityToScpi(GatePolarity polarity)
        {
            switch (polarity)
            {
                case GatePolarity.Positive:
                    return "POS";
                case GatePolarity.Negative:
                    return "NEG";
                default:
                    throw new ArgumentException($"Unknown gate polarity: {polarity}");
            }
        }

        #endregion
    }
}
