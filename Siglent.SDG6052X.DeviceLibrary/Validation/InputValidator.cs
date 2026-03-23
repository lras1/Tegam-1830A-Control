using System;
using Siglent.SDG6052X.DeviceLibrary.Models;

namespace Siglent.SDG6052X.DeviceLibrary.Validation
{
    public class InputValidator : IInputValidator
    {
        // Frequency limits in Hz
        private const double MIN_FREQUENCY = 1e-6;  // 1 µHz
        private const double MAX_FREQUENCY_SINE = 500e6;  // 500 MHz
        private const double MAX_FREQUENCY_SQUARE = 200e6;  // 200 MHz
        private const double MAX_FREQUENCY_RAMP = 50e6;  // 50 MHz
        private const double MAX_FREQUENCY_PULSE = 100e6;  // 100 MHz
        private const double MAX_FREQUENCY_ARBITRARY = 100e6;  // 100 MHz
        
        // Amplitude limits in Vpp
        private const double MIN_AMPLITUDE_50OHM = 0.001;  // 1 mVpp
        private const double MAX_AMPLITUDE_50OHM = 20.0;  // 20 Vpp
        private const double MIN_AMPLITUDE_HIGHZ = 0.002;  // 2 mVpp
        private const double MAX_AMPLITUDE_HIGHZ = 40.0;  // 40 Vpp
        
        // Offset limits in V
        private const double MIN_OFFSET_50OHM = -10.0;  // -10 V
        private const double MAX_OFFSET_50OHM = 10.0;  // +10 V
        private const double MIN_OFFSET_HIGHZ = -20.0;  // -20 V
        private const double MAX_OFFSET_HIGHZ = 20.0;  // +20 V
        
        // Phase limits in degrees
        private const double MIN_PHASE = 0.0;
        private const double MAX_PHASE = 360.0;
        
        // Duty cycle limits in percent
        private const double MIN_DUTY_CYCLE = 0.01;
        private const double MAX_DUTY_CYCLE = 99.99;
        
        // Modulation depth limits in percent
        private const double MIN_AM_DEPTH = 0.0;
        private const double MAX_AM_DEPTH = 120.0;
        private const double MIN_PWM_DEPTH = 0.0;
        private const double MAX_PWM_DEPTH = 99.0;
        
        // Modulation frequency limits in Hz
        private const double MIN_MODULATION_FREQUENCY = 0.001;  // 1 mHz
        private const double MAX_MODULATION_FREQUENCY = 1e6;  // 1 MHz
        
        // Sweep time limits in seconds
        private const double MIN_SWEEP_TIME = 0.001;  // 1 ms
        private const double MAX_SWEEP_TIME = 500.0;  // 500 s
        
        // Burst cycle limits
        private const int MIN_BURST_CYCLES = 1;
        private const int MAX_BURST_CYCLES = 1000000;
        
        // Arbitrary waveform limits
        private const int MIN_WAVEFORM_POINTS = 2;
        private const int MAX_WAVEFORM_POINTS = 16384;
        private const int MAX_WAVEFORM_NAME_LENGTH = 15;

        public ValidationResult ValidateFrequency(double frequency, WaveformType type)
        {
            if (double.IsNaN(frequency) || double.IsInfinity(frequency))
                return ValidationResult.Invalid("Frequency must be a finite number");
            
            if (frequency < MIN_FREQUENCY)
                return ValidationResult.Invalid($"Frequency must be at least {MIN_FREQUENCY * 1e6} µHz");
            
            double maxFrequency;
            string maxFreqStr;
            
            switch (type)
            {
                case WaveformType.Sine:
                    maxFrequency = MAX_FREQUENCY_SINE;
                    maxFreqStr = "500 MHz";
                    break;
                case WaveformType.Square:
                    maxFrequency = MAX_FREQUENCY_SQUARE;
                    maxFreqStr = "200 MHz";
                    break;
                case WaveformType.Ramp:
                    maxFrequency = MAX_FREQUENCY_RAMP;
                    maxFreqStr = "50 MHz";
                    break;
                case WaveformType.Pulse:
                    maxFrequency = MAX_FREQUENCY_PULSE;
                    maxFreqStr = "100 MHz";
                    break;
                case WaveformType.Arbitrary:
                    maxFrequency = MAX_FREQUENCY_ARBITRARY;
                    maxFreqStr = "100 MHz";
                    break;
                case WaveformType.Noise:
                case WaveformType.DC:
                case WaveformType.PRBS:
                case WaveformType.IQ:
                    // These waveform types don't have frequency parameters or have different constraints
                    return ValidationResult.Valid();
                default:
                    return ValidationResult.Invalid($"Unknown waveform type: {type}");
            }
            
            if (frequency > maxFrequency)
                return ValidationResult.Invalid($"Frequency for {type} waveform must not exceed {maxFreqStr}");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateAmplitude(double amplitude, LoadImpedance load)
        {
            if (load == null)
                return ValidationResult.Invalid("Load impedance must be specified");
            
            if (double.IsNaN(amplitude) || double.IsInfinity(amplitude))
                return ValidationResult.Invalid("Amplitude must be a finite number");
            
            if (amplitude < 0)
                return ValidationResult.Invalid("Amplitude must be positive");
            
            double minAmplitude, maxAmplitude;
            string loadStr;
            
            switch (load.Type)
            {
                case LoadType.FiftyOhm:
                    minAmplitude = MIN_AMPLITUDE_50OHM;
                    maxAmplitude = MAX_AMPLITUDE_50OHM;
                    loadStr = "50Ω";
                    break;
                case LoadType.HighZ:
                    minAmplitude = MIN_AMPLITUDE_HIGHZ;
                    maxAmplitude = MAX_AMPLITUDE_HIGHZ;
                    loadStr = "High-Z";
                    break;
                case LoadType.Custom:
                    // For custom loads, use High-Z limits as a conservative approach
                    minAmplitude = MIN_AMPLITUDE_HIGHZ;
                    maxAmplitude = MAX_AMPLITUDE_HIGHZ;
                    loadStr = $"{load.Value}Ω";
                    break;
                default:
                    return ValidationResult.Invalid($"Unknown load type: {load.Type}");
            }
            
            if (amplitude < minAmplitude)
                return ValidationResult.Invalid($"Amplitude for {loadStr} load must be at least {minAmplitude * 1000} mVpp");
            
            if (amplitude > maxAmplitude)
                return ValidationResult.Invalid($"Amplitude for {loadStr} load must not exceed {maxAmplitude} Vpp");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateOffset(double offset, double amplitude, LoadImpedance load)
        {
            if (load == null)
                return ValidationResult.Invalid("Load impedance must be specified");
            
            if (double.IsNaN(offset) || double.IsInfinity(offset))
                return ValidationResult.Invalid("Offset must be a finite number");
            
            if (double.IsNaN(amplitude) || double.IsInfinity(amplitude))
                return ValidationResult.Invalid("Amplitude must be a finite number");
            
            double minOffset, maxOffset;
            string loadStr;
            
            switch (load.Type)
            {
                case LoadType.FiftyOhm:
                    minOffset = MIN_OFFSET_50OHM;
                    maxOffset = MAX_OFFSET_50OHM;
                    loadStr = "50Ω";
                    break;
                case LoadType.HighZ:
                    minOffset = MIN_OFFSET_HIGHZ;
                    maxOffset = MAX_OFFSET_HIGHZ;
                    loadStr = "High-Z";
                    break;
                case LoadType.Custom:
                    // For custom loads, use High-Z limits as a conservative approach
                    minOffset = MIN_OFFSET_HIGHZ;
                    maxOffset = MAX_OFFSET_HIGHZ;
                    loadStr = $"{load.Value}Ω";
                    break;
                default:
                    return ValidationResult.Invalid($"Unknown load type: {load.Type}");
            }
            
            if (offset < minOffset)
                return ValidationResult.Invalid($"Offset for {loadStr} load must be at least {minOffset} V");
            
            if (offset > maxOffset)
                return ValidationResult.Invalid($"Offset for {loadStr} load must not exceed {maxOffset} V");
            
            // Check that |offset| + (amplitude/2) does not exceed maximum voltage
            double peakVoltage = Math.Abs(offset) + (amplitude / 2.0);
            double maxVoltage = load.Type == LoadType.FiftyOhm ? MAX_OFFSET_50OHM : MAX_OFFSET_HIGHZ;
            
            if (peakVoltage > maxVoltage)
                return ValidationResult.Invalid($"Combined offset and amplitude would exceed maximum voltage of {maxVoltage} V for {loadStr} load");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidatePhase(double phase)
        {
            if (double.IsNaN(phase) || double.IsInfinity(phase))
                return ValidationResult.Invalid("Phase must be a finite number");
            
            if (phase < MIN_PHASE || phase > MAX_PHASE)
                return ValidationResult.Invalid($"Phase must be between {MIN_PHASE}° and {MAX_PHASE}°");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateDutyCycle(double dutyCycle)
        {
            if (double.IsNaN(dutyCycle) || double.IsInfinity(dutyCycle))
                return ValidationResult.Invalid("Duty cycle must be a finite number");
            
            if (dutyCycle < MIN_DUTY_CYCLE || dutyCycle > MAX_DUTY_CYCLE)
                return ValidationResult.Invalid($"Duty cycle must be between {MIN_DUTY_CYCLE}% and {MAX_DUTY_CYCLE}%");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateModulationDepth(double depth, ModulationType type)
        {
            if (double.IsNaN(depth) || double.IsInfinity(depth))
                return ValidationResult.Invalid("Modulation depth must be a finite number");
            
            if (depth < 0)
                return ValidationResult.Invalid("Modulation depth must be positive");
            
            switch (type)
            {
                case ModulationType.AM:
                case ModulationType.ASK:
                    if (depth > MAX_AM_DEPTH)
                        return ValidationResult.Invalid($"AM/ASK modulation depth must not exceed {MAX_AM_DEPTH}%");
                    break;
                case ModulationType.PWM:
                    if (depth > MAX_PWM_DEPTH)
                        return ValidationResult.Invalid($"PWM modulation depth must not exceed {MAX_PWM_DEPTH}%");
                    break;
                case ModulationType.FM:
                case ModulationType.PM:
                case ModulationType.FSK:
                case ModulationType.PSK:
                    // These modulation types use deviation instead of depth
                    return ValidationResult.Valid();
                default:
                    return ValidationResult.Invalid($"Unknown modulation type: {type}");
            }
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateModulationFrequency(double frequency, ModulationType type)
        {
            if (double.IsNaN(frequency) || double.IsInfinity(frequency))
                return ValidationResult.Invalid("Modulation frequency must be a finite number");
            
            if (frequency < MIN_MODULATION_FREQUENCY)
                return ValidationResult.Invalid($"Modulation frequency must be at least {MIN_MODULATION_FREQUENCY * 1000} mHz");
            
            if (frequency > MAX_MODULATION_FREQUENCY)
                return ValidationResult.Invalid($"Modulation frequency must not exceed {MAX_MODULATION_FREQUENCY / 1e6} MHz");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateDeviation(double deviation, ModulationType type)
        {
            if (double.IsNaN(deviation) || double.IsInfinity(deviation))
                return ValidationResult.Invalid("Deviation must be a finite number");
            
            if (deviation < 0)
                return ValidationResult.Invalid("Deviation must be positive");
            
            // Deviation limits depend on carrier frequency and modulation type
            // For now, we perform basic validation
            // More specific validation would require carrier frequency context
            
            switch (type)
            {
                case ModulationType.FM:
                case ModulationType.FSK:
                    // Frequency deviation - should be reasonable relative to carrier
                    // Maximum deviation is typically limited by device bandwidth
                    if (deviation > MAX_FREQUENCY_SINE)
                        return ValidationResult.Invalid($"Frequency deviation must not exceed {MAX_FREQUENCY_SINE / 1e6} MHz");
                    break;
                case ModulationType.PM:
                case ModulationType.PSK:
                    // Phase deviation in degrees
                    if (deviation > MAX_PHASE)
                        return ValidationResult.Invalid($"Phase deviation must not exceed {MAX_PHASE}°");
                    break;
                case ModulationType.AM:
                case ModulationType.ASK:
                case ModulationType.PWM:
                    // These use depth instead of deviation
                    return ValidationResult.Valid();
                default:
                    return ValidationResult.Invalid($"Unknown modulation type: {type}");
            }
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateSweepRange(double startFreq, double stopFreq)
        {
            if (double.IsNaN(startFreq) || double.IsInfinity(startFreq))
                return ValidationResult.Invalid("Start frequency must be a finite number");
            
            if (double.IsNaN(stopFreq) || double.IsInfinity(stopFreq))
                return ValidationResult.Invalid("Stop frequency must be a finite number");
            
            if (startFreq < MIN_FREQUENCY)
                return ValidationResult.Invalid($"Start frequency must be at least {MIN_FREQUENCY * 1e6} µHz");
            
            if (stopFreq < MIN_FREQUENCY)
                return ValidationResult.Invalid($"Stop frequency must be at least {MIN_FREQUENCY * 1e6} µHz");
            
            if (startFreq >= stopFreq)
                return ValidationResult.Invalid("Start frequency must be less than stop frequency");
            
            // Sweep range should be within device capabilities
            // Using sine wave max frequency as upper limit
            if (stopFreq > MAX_FREQUENCY_SINE)
                return ValidationResult.Invalid($"Stop frequency must not exceed {MAX_FREQUENCY_SINE / 1e6} MHz");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateSweepTime(double time)
        {
            if (double.IsNaN(time) || double.IsInfinity(time))
                return ValidationResult.Invalid("Sweep time must be a finite number");
            
            if (time < MIN_SWEEP_TIME)
                return ValidationResult.Invalid($"Sweep time must be at least {MIN_SWEEP_TIME * 1000} ms");
            
            if (time > MAX_SWEEP_TIME)
                return ValidationResult.Invalid($"Sweep time must not exceed {MAX_SWEEP_TIME} s");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateBurstCycles(int cycles)
        {
            if (cycles < MIN_BURST_CYCLES)
                return ValidationResult.Invalid($"Burst cycles must be at least {MIN_BURST_CYCLES}");
            
            if (cycles > MAX_BURST_CYCLES)
                return ValidationResult.Invalid($"Burst cycles must not exceed {MAX_BURST_CYCLES}");
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateBurstPeriod(double period)
        {
            if (double.IsNaN(period) || double.IsInfinity(period))
                return ValidationResult.Invalid("Burst period must be a finite number");
            
            if (period <= 0)
                return ValidationResult.Invalid("Burst period must be positive");
            
            // Period should be greater than burst duration
            // This would require additional context (frequency, cycles) for precise validation
            // For now, we ensure it's a reasonable positive value
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateArbitraryWaveformPoints(double[] points)
        {
            if (points == null)
                return ValidationResult.Invalid("Waveform points array cannot be null");
            
            if (points.Length < MIN_WAVEFORM_POINTS)
                return ValidationResult.Invalid($"Waveform must have at least {MIN_WAVEFORM_POINTS} points");
            
            if (points.Length > MAX_WAVEFORM_POINTS)
                return ValidationResult.Invalid($"Waveform must not exceed {MAX_WAVEFORM_POINTS} points");
            
            // Validate that all points are finite numbers
            for (int i = 0; i < points.Length; i++)
            {
                if (double.IsNaN(points[i]) || double.IsInfinity(points[i]))
                    return ValidationResult.Invalid($"Waveform point at index {i} must be a finite number");
            }
            
            return ValidationResult.Valid();
        }

        public ValidationResult ValidateWaveformName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return ValidationResult.Invalid("Waveform name cannot be empty");
            
            if (name.Length > MAX_WAVEFORM_NAME_LENGTH)
                return ValidationResult.Invalid($"Waveform name must not exceed {MAX_WAVEFORM_NAME_LENGTH} characters");
            
            // Check for valid characters (alphanumeric and underscore)
            foreach (char c in name)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return ValidationResult.Invalid("Waveform name must contain only letters, digits, and underscores");
            }
            
            // Name should not start with a digit
            if (char.IsDigit(name[0]))
                return ValidationResult.Invalid("Waveform name must not start with a digit");
            
            return ValidationResult.Valid();
        }
    }
}
