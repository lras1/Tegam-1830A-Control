using System.Collections.Generic;
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Simulation
{
    /// <summary>
    /// Represents the complete simulated state of the SDG6052X device
    /// </summary>
    public class SimulatedDeviceState
    {
        /// <summary>
        /// Channel states (indexed by channel number: 1 and 2)
        /// </summary>
        public Dictionary<int, SimulatedChannelState> Channels { get; set; }

        /// <summary>
        /// Stored arbitrary waveforms (name -> data points)
        /// </summary>
        public Dictionary<string, double[]> ArbitraryWaveforms { get; set; }

        /// <summary>
        /// Error queue for simulating device errors
        /// </summary>
        public Queue<DeviceError> ErrorQueue { get; set; }

        /// <summary>
        /// Device identity information
        /// </summary>
        public DeviceIdentity Identity { get; set; }

        /// <summary>
        /// Simulate connection loss
        /// </summary>
        public bool SimulateConnectionLoss { get; set; }

        /// <summary>
        /// Simulate timeout
        /// </summary>
        public bool SimulateTimeout { get; set; }

        /// <summary>
        /// Initialize with default state
        /// </summary>
        public SimulatedDeviceState()
        {
            // Initialize channels
            Channels = new Dictionary<int, SimulatedChannelState>
            {
                { 1, new SimulatedChannelState() },
                { 2, new SimulatedChannelState() }
            };

            // Initialize arbitrary waveforms storage
            ArbitraryWaveforms = new Dictionary<string, double[]>();

            // Initialize error queue
            ErrorQueue = new Queue<DeviceError>();

            // Set device identity
            Identity = new DeviceIdentity
            {
                Manufacturer = "Siglent Technologies",
                Model = "SDG6052X",
                SerialNumber = "SDG00000000001",
                FirmwareVersion = "1.01.01.32"
            };

            SimulateConnectionLoss = false;
            SimulateTimeout = false;
        }

        /// <summary>
        /// Reset device to default state
        /// </summary>
        public void Reset()
        {
            Channels[1] = new SimulatedChannelState();
            Channels[2] = new SimulatedChannelState();
            ErrorQueue.Clear();
            SimulateConnectionLoss = false;
            SimulateTimeout = false;
        }
    }
}
