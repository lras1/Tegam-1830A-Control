using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Validation
{
    public interface IInputValidator
    {
        // Waveform parameter validation
        ValidationResult ValidateFrequency(double frequency, WaveformType type);
        ValidationResult ValidateAmplitude(double amplitude, LoadImpedance load);
        ValidationResult ValidateOffset(double offset, double amplitude, LoadImpedance load);
        ValidationResult ValidatePhase(double phase);
        ValidationResult ValidateDutyCycle(double dutyCycle);
        
        // Modulation parameter validation
        ValidationResult ValidateModulationDepth(double depth, ModulationType type);
        ValidationResult ValidateModulationFrequency(double frequency, ModulationType type);
        ValidationResult ValidateDeviation(double deviation, ModulationType type);
        
        // Sweep parameter validation
        ValidationResult ValidateSweepRange(double startFreq, double stopFreq);
        ValidationResult ValidateSweepTime(double time);
        
        // Burst parameter validation
        ValidationResult ValidateBurstCycles(int cycles);
        ValidationResult ValidateBurstPeriod(double period);
        
        // Arbitrary waveform validation
        ValidationResult ValidateArbitraryWaveformPoints(double[] points);
        ValidationResult ValidateWaveformName(string name);
    }
}
