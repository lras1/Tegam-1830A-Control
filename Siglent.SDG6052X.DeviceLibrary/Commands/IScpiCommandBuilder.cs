using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Commands
{
    /// <summary>
    /// Interface for building SCPI commands for the Siglent SDG6052X signal generator
    /// </summary>
    public interface IScpiCommandBuilder
    {
        /// <summary>
        /// Builds a basic waveform configuration command
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="type">Waveform type</param>
        /// <param name="parameters">Waveform parameters</param>
        /// <returns>SCPI command string</returns>
        string BuildBasicWaveCommand(int channel, WaveformType type, WaveformParameters parameters);

        /// <summary>
        /// Builds an output state command (enable/disable channel output)
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="enabled">True to enable output, false to disable</param>
        /// <returns>SCPI command string</returns>
        string BuildOutputStateCommand(int channel, bool enabled);

        /// <summary>
        /// Builds a load impedance configuration command
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="load">Load impedance configuration</param>
        /// <returns>SCPI command string</returns>
        string BuildLoadCommand(int channel, LoadImpedance load);

        /// <summary>
        /// Builds a modulation configuration command
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="type">Modulation type</param>
        /// <param name="parameters">Modulation parameters</param>
        /// <returns>SCPI command string</returns>
        string BuildModulationCommand(int channel, ModulationType type, ModulationParameters parameters);

        /// <summary>
        /// Builds a modulation state command (enable/disable modulation)
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="enabled">True to enable modulation, false to disable</param>
        /// <returns>SCPI command string</returns>
        string BuildModulationStateCommand(int channel, bool enabled);

        /// <summary>
        /// Builds a sweep configuration command
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="parameters">Sweep parameters</param>
        /// <returns>SCPI command string</returns>
        string BuildSweepCommand(int channel, SweepParameters parameters);

        /// <summary>
        /// Builds a sweep state command (enable/disable sweep)
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="enabled">True to enable sweep, false to disable</param>
        /// <returns>SCPI command string</returns>
        string BuildSweepStateCommand(int channel, bool enabled);

        /// <summary>
        /// Builds a burst configuration command
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="parameters">Burst parameters</param>
        /// <returns>SCPI command string</returns>
        string BuildBurstCommand(int channel, BurstParameters parameters);

        /// <summary>
        /// Builds a burst state command (enable/disable burst)
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="enabled">True to enable burst, false to disable</param>
        /// <returns>SCPI command string</returns>
        string BuildBurstStateCommand(int channel, bool enabled);

        /// <summary>
        /// Builds a command to select an arbitrary waveform
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="waveformName">Name of the arbitrary waveform</param>
        /// <returns>SCPI command string</returns>
        string BuildArbitraryWaveCommand(int channel, string waveformName);

        /// <summary>
        /// Builds a command to store an arbitrary waveform
        /// </summary>
        /// <param name="name">Waveform name</param>
        /// <param name="points">Waveform data points</param>
        /// <returns>SCPI command string</returns>
        string BuildStoreArbitraryWaveCommand(string name, double[] points);

        /// <summary>
        /// Builds a query command
        /// </summary>
        /// <param name="channel">Channel number (1 or 2)</param>
        /// <param name="queryType">Type of query</param>
        /// <returns>SCPI query string</returns>
        string BuildQueryCommand(int channel, QueryType queryType);

        /// <summary>
        /// Builds a system command
        /// </summary>
        /// <param name="type">System command type</param>
        /// <param name="parameters">Optional parameters</param>
        /// <returns>SCPI command string</returns>
        string BuildSystemCommand(SystemCommandType type, params object[] parameters);
    }

    /// <summary>
    /// Query types for device queries
    /// </summary>
    public enum QueryType
    {
        BasicWaveform,
        OutputState,
        Load,
        Modulation,
        ModulationState,
        Sweep,
        SweepState,
        Burst,
        BurstState
    }

    /// <summary>
    /// System command types
    /// </summary>
    public enum SystemCommandType
    {
        Identity,
        Reset,
        Error,
        RecallSetup,
        SaveSetup
    }
}
