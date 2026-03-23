using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Services
{
    /// <summary>
    /// Event arguments for device errors
    /// </summary>
    public class DeviceErrorEventArgs : EventArgs
    {
        public DeviceError Error { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Event arguments for connection state changes
    /// </summary>
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// High-level service interface for controlling the SDG6052X signal generator
    /// </summary>
    public interface ISignalGeneratorService
    {
        // Connection management
        Task<bool> ConnectAsync(string ipAddress);
        Task DisconnectAsync();
        bool IsConnected { get; }
        DeviceIdentity DeviceInfo { get; }

        // Basic waveform control
        Task<OperationResult> SetBasicWaveformAsync(int channel, WaveformType type, WaveformParameters parameters);
        Task<WaveformState> GetWaveformStateAsync(int channel);
        Task<OperationResult> SetOutputStateAsync(int channel, bool enabled);
        Task<OperationResult> SetLoadImpedanceAsync(int channel, LoadImpedance load);

        // Modulation control
        Task<OperationResult> ConfigureModulationAsync(int channel, ModulationType type, ModulationParameters parameters);
        Task<OperationResult> SetModulationStateAsync(int channel, bool enabled);
        Task<ModulationState> GetModulationStateAsync(int channel);

        // Sweep control
        Task<OperationResult> ConfigureSweepAsync(int channel, SweepParameters parameters);
        Task<OperationResult> SetSweepStateAsync(int channel, bool enabled);
        Task<SweepState> GetSweepStateAsync(int channel);

        // Burst control
        Task<OperationResult> ConfigureBurstAsync(int channel, BurstParameters parameters);
        Task<OperationResult> SetBurstStateAsync(int channel, bool enabled);
        Task<BurstState> GetBurstStateAsync(int channel);

        // Arbitrary waveform management
        Task<OperationResult> UploadArbitraryWaveformAsync(string name, double[] points);
        Task<OperationResult> SelectArbitraryWaveformAsync(int channel, string name);
        Task<List<string>> GetArbitraryWaveformListAsync();
        Task<OperationResult> DeleteArbitraryWaveformAsync(string name);

        // System operations
        Task<OperationResult> RecallSetupAsync(int setupNumber);
        Task<OperationResult> SaveSetupAsync(int setupNumber);
        Task<OperationResult> ResetDeviceAsync();
        Task<DeviceError> GetLastDeviceErrorAsync();

        // Events
        event EventHandler<DeviceErrorEventArgs> DeviceError;
        event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
    }
}
