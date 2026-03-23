using Siglent.SDG6052X.DeviceLibrary.Models;
using System.Collections.Generic;

namespace Siglent.SDG6052X.DeviceLibrary.Parsing
{
    /// <summary>
    /// Interface for parsing SCPI responses from the SDG6052X into strongly-typed objects
    /// </summary>
    public interface IScpiResponseParser
    {
        /// <summary>
        /// Parse a boolean response (ON/OFF, 1/0)
        /// </summary>
        bool ParseBooleanResponse(string response);

        /// <summary>
        /// Parse a numeric response with optional units
        /// </summary>
        double ParseNumericResponse(string response);

        /// <summary>
        /// Parse a string response, removing quotes if present
        /// </summary>
        string ParseStringResponse(string response);

        /// <summary>
        /// Parse waveform state from BSWV query response
        /// </summary>
        WaveformState ParseWaveformState(string response);

        /// <summary>
        /// Parse modulation state from modulation query response
        /// </summary>
        ModulationState ParseModulationState(string response);

        /// <summary>
        /// Parse sweep state from sweep query response
        /// </summary>
        SweepState ParseSweepState(string response);

        /// <summary>
        /// Parse burst state from burst query response
        /// </summary>
        BurstState ParseBurstState(string response);

        /// <summary>
        /// Parse device identity from *IDN? response
        /// </summary>
        DeviceIdentity ParseIdentityResponse(string response);

        /// <summary>
        /// Parse arbitrary waveform data from binary response
        /// </summary>
        double[] ParseArbitraryWaveformData(byte[] binaryData);

        /// <summary>
        /// Parse error response from SYST:ERR? query
        /// </summary>
        DeviceError ParseErrorResponse(string response);
    }
}
