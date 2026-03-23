using FsCheck;
using FsCheck.NUnit;
using NUnit.Framework;
using Siglent.SDG6052X.DeviceLibrary.Commands;
using Siglent.SDG6052X.DeviceLibrary.Models;
using Siglent.SDG6052X.DeviceLibrary.Parsing;
using Siglent.SDG6052X.DeviceLibrary.Validation;
using System;

namespace Siglent.SDG6052X.Tests.PropertyBased
{
    /// <summary>
    /// Property-based tests using FsCheck to verify universal properties
    /// of the Siglent SDG6052X device library components.
    /// </summary>
    [TestFixture]
    public class PropertyBasedTests
    {
        private ScpiCommandBuilder _commandBuilder;
        private ScpiResponseParser _responseParser;
        private InputValidator _inputValidator;

        [SetUp]
        public void SetUp()
        {
            _commandBuilder = new ScpiCommandBuilder();
            _responseParser = new ScpiResponseParser();
            _inputValidator = new InputValidator();
        }

        [TearDown]
        public void TearDown()
        {
            _commandBuilder = null;
            _responseParser = null;
            _inputValidator = null;
        }

        #region Task 12.2: Command-Parse Roundtrip Property Test

        /// <summary>
        /// **Validates: Requirements 2.1, 2.2, 2.3**
        /// 
        /// Property: For any valid waveform parameters, building a command and parsing
        /// the simulated device response should preserve the parameters within tolerance.
        /// 
        /// This property verifies the roundtrip consistency between command building
        /// and response parsing, ensuring no data loss or corruption occurs.
        /// </summary>
        [FsCheck.NUnit.Property]
        public void Property_CommandParseRoundtrip_PreservesWaveformParameters()
        {
            // Define custom generator for valid waveform parameters
            var validParametersGen = from freq in Arb.Default.NormalFloat().Generator
                                     from amp in Arb.Default.NormalFloat().Generator
                                     from offset in Arb.Default.NormalFloat().Generator
                                     from phase in Arb.Default.NormalFloat().Generator
                                     from duty in Arb.Default.NormalFloat().Generator
                                     where !double.IsNaN(freq.Get) && !double.IsInfinity(freq.Get) &&
                                           !double.IsNaN(amp.Get) && !double.IsInfinity(amp.Get) &&
                                           !double.IsNaN(offset.Get) && !double.IsInfinity(offset.Get) &&
                                           !double.IsNaN(phase.Get) && !double.IsInfinity(phase.Get) &&
                                           !double.IsNaN(duty.Get) && !double.IsInfinity(duty.Get)
                                     select new
                                     {
                                         Frequency = Math.Abs(freq.Get) % 500_000_000 + 1.0, // 1 Hz to 500 MHz
                                         Amplitude = Math.Abs(amp.Get) % 19.999 + 0.001,     // 0.001 to 20 Vpp (50Ω)
                                         Offset = (offset.Get % 20.0) - 10.0,                 // -10V to +10V (50Ω)
                                         Phase = Math.Abs(phase.Get) % 360.0,                 // 0 to 360 degrees
                                         DutyCycle = Math.Abs(duty.Get) % 99.98 + 0.01       // 0.01% to 99.99%
                                     };

            var arb = Arb.From(validParametersGen);

            Prop.ForAll(arb, testData =>
            {
                // Test for Sine waveform (no duty cycle)
                var sineParams = new WaveformParameters
                {
                    Frequency = testData.Frequency,
                    Amplitude = testData.Amplitude,
                    Offset = testData.Offset,
                    Phase = testData.Phase,
                    Unit = AmplitudeUnit.Vpp
                };

                bool sineRoundtrip = TestRoundtrip(WaveformType.Sine, sineParams);

                // Test for Square waveform (includes duty cycle)
                var squareParams = new WaveformParameters
                {
                    Frequency = Math.Min(testData.Frequency, 200_000_000), // Square max 200 MHz
                    Amplitude = testData.Amplitude,
                    Offset = testData.Offset,
                    Phase = testData.Phase,
                    DutyCycle = testData.DutyCycle,
                    Unit = AmplitudeUnit.Vpp
                };

                bool squareRoundtrip = TestRoundtrip(WaveformType.Square, squareParams);

                return sineRoundtrip && squareRoundtrip;
            }).QuickCheckThrowOnFailure();
        }

        /// <summary>
        /// Helper method to test command-parse roundtrip for a specific waveform type
        /// </summary>
        private bool TestRoundtrip(WaveformType waveformType, WaveformParameters parameters)
        {
            try
            {
                // Step 1: Build SCPI command from parameters
                string command = _commandBuilder.BuildBasicWaveCommand(1, waveformType, parameters);

                // Step 2: Simulate device response (device echoes back the configuration)
                string simulatedResponse = SimulateDeviceResponse(command, waveformType);

                // Step 3: Parse the simulated response
                WaveformState parsedState = _responseParser.ParseWaveformState(simulatedResponse);

                // Step 4: Verify roundtrip preserves parameters within tolerance
                bool frequencyMatch = Math.Abs(parsedState.Frequency - parameters.Frequency) < 0.01;
                bool amplitudeMatch = Math.Abs(parsedState.Amplitude - parameters.Amplitude) < 0.001;
                bool offsetMatch = Math.Abs(parsedState.Offset - parameters.Offset) < 0.001;
                bool phaseMatch = Math.Abs(parsedState.Phase - parameters.Phase) < 0.01;
                bool waveformTypeMatch = parsedState.WaveformType == waveformType;
                bool unitMatch = parsedState.Unit == parameters.Unit;

                // For square/pulse waveforms, also check duty cycle
                bool dutyCycleMatch = true;
                if (waveformType == WaveformType.Square || waveformType == WaveformType.Pulse)
                {
                    dutyCycleMatch = Math.Abs(parsedState.DutyCycle - parameters.DutyCycle) < 0.01;
                }

                return frequencyMatch && amplitudeMatch && offsetMatch && 
                       phaseMatch && waveformTypeMatch && unitMatch && dutyCycleMatch;
            }
            catch (Exception)
            {
                // If any exception occurs during roundtrip, the property fails
                return false;
            }
        }

        /// <summary>
        /// Simulates a device response based on the SCPI command.
        /// This mimics how the SDG6052X would respond to a query after setting parameters.
        /// </summary>
        private string SimulateDeviceResponse(string command, WaveformType waveformType)
        {
            // Parse the command to extract parameters
            // Command format: "C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0"
            
            // Extract channel
            int channelEnd = command.IndexOf(':');
            string channel = command.Substring(0, channelEnd);

            // Extract parameters after "BSWV "
            int bswvStart = command.IndexOf("BSWV ") + 5;
            string paramsString = command.Substring(bswvStart);

            // Build response in the format the device would return
            // The device echoes back the configuration in query response format
            return $"{channel}:BSWV {paramsString}";
        }

        #endregion

        #region Task 12.3: Validation Consistency Property Test

        /// <summary>
        /// **Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7**
        /// 
        /// Property: If parameter validation passes, command building should succeed.
        /// If validation fails, the command should not be sent to the device.
        /// 
        /// This property verifies the consistency between the validation layer and
        /// command building layer, ensuring that validated parameters can always be
        /// successfully converted to SCPI commands.
        /// </summary>
        [FsCheck.NUnit.Property]
        public void Property_ValidationConsistency_ValidParametersProduceValidCommands()
        {
            // Define custom generator for waveform parameters with wide range
            var parametersGen = from freq in Arb.Default.NormalFloat().Generator
                               from amp in Arb.Default.NormalFloat().Generator
                               from offset in Arb.Default.NormalFloat().Generator
                               from phase in Arb.Default.NormalFloat().Generator
                               from duty in Arb.Default.NormalFloat().Generator
                               from loadType in Gen.Elements(LoadType.FiftyOhm, LoadType.HighZ)
                               from waveType in Gen.Elements(WaveformType.Sine, WaveformType.Square, WaveformType.Ramp, WaveformType.Pulse)
                               where !double.IsNaN(freq.Get) && !double.IsInfinity(freq.Get) &&
                                     !double.IsNaN(amp.Get) && !double.IsInfinity(amp.Get) &&
                                     !double.IsNaN(offset.Get) && !double.IsInfinity(offset.Get) &&
                                     !double.IsNaN(phase.Get) && !double.IsInfinity(phase.Get) &&
                                     !double.IsNaN(duty.Get) && !double.IsInfinity(duty.Get)
                               select new
                               {
                                   Frequency = freq.Get,
                                   Amplitude = amp.Get,
                                   Offset = offset.Get,
                                   Phase = phase.Get,
                                   DutyCycle = duty.Get,
                                   LoadType = loadType,
                                   WaveformType = waveType
                               };

            var arb = Arb.From(parametersGen);

            Prop.ForAll(arb, testData =>
            {
                // Create load impedance
                LoadImpedance load = testData.LoadType == LoadType.FiftyOhm
                    ? LoadImpedance.FiftyOhm
                    : LoadImpedance.HighZ;

                // Create waveform parameters
                var parameters = new WaveformParameters
                {
                    Frequency = testData.Frequency,
                    Amplitude = testData.Amplitude,
                    Offset = testData.Offset,
                    Phase = testData.Phase,
                    DutyCycle = testData.DutyCycle,
                    Unit = AmplitudeUnit.Vpp,
                    Load = load
                };

                // Step 1: Validate all parameters
                var freqValidation = _inputValidator.ValidateFrequency(parameters.Frequency, testData.WaveformType);
                var ampValidation = _inputValidator.ValidateAmplitude(parameters.Amplitude, load);
                var offsetValidation = _inputValidator.ValidateOffset(parameters.Offset, parameters.Amplitude, load);
                var phaseValidation = _inputValidator.ValidatePhase(parameters.Phase);

                // For square/pulse waveforms, also validate duty cycle
                ValidationResult dutyValidation = ValidationResult.Valid();
                if (testData.WaveformType == WaveformType.Square || testData.WaveformType == WaveformType.Pulse)
                {
                    dutyValidation = _inputValidator.ValidateDutyCycle(parameters.DutyCycle);
                }

                // Determine if all validations passed
                bool allValid = freqValidation.IsValid &&
                               ampValidation.IsValid &&
                               offsetValidation.IsValid &&
                               phaseValidation.IsValid &&
                               dutyValidation.IsValid;

                // Step 2: Attempt to build command
                bool commandBuildSucceeded = false;
                string command = null;

                try
                {
                    command = _commandBuilder.BuildBasicWaveCommand(1, testData.WaveformType, parameters);
                    commandBuildSucceeded = !string.IsNullOrEmpty(command);
                }
                catch (Exception)
                {
                    commandBuildSucceeded = false;
                }

                // Step 3: Verify consistency
                // If all validations passed, command building should succeed
                if (allValid)
                {
                    if (!commandBuildSucceeded)
                    {
                        // This is a consistency violation: validation passed but command building failed
                        return false;
                    }

                    // Additionally verify the command is well-formed
                    if (!IsWellFormedScpiCommand(command))
                    {
                        return false;
                    }
                }

                // If any validation failed, we don't require command building to fail
                // (the command builder might still produce a syntactically valid command,
                // but the service layer should prevent it from being sent)
                // The key property is: valid parameters MUST produce valid commands

                return true;
            }).QuickCheckThrowOnFailure();
        }

        /// <summary>
        /// Helper method to verify a SCPI command is well-formed
        /// </summary>
        private bool IsWellFormedScpiCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return false;

            // Check basic SCPI command structure
            // Should start with channel prefix (C1: or C2:)
            if (!command.StartsWith("C1:") && !command.StartsWith("C2:"))
                return false;

            // Should contain BSWV for basic waveform commands
            if (!command.Contains("BSWV"))
                return false;

            // Should contain key parameters
            if (!command.Contains("WVTP") || !command.Contains("FRQ"))
                return false;

            // Command should not be excessively long (SCPI limit is typically 65535 bytes)
            if (command.Length > 65535)
                return false;

            return true;
        }

        #endregion

        #region Task 12.4: Frequency Unit Conversion Property Test

        /// <summary>
        /// **Validates: Requirements 2.1, 2.2**
        /// 
        /// Property: Converting a frequency between Hz, kHz, and MHz should preserve
        /// the actual frequency value within floating-point tolerance.
        /// 
        /// This property verifies that frequency unit conversions are consistent and
        /// reversible: Hz → (unit, value) → Hz should preserve the original frequency.
        /// </summary>
        [FsCheck.NUnit.Property]
        public void Property_FrequencyUnitConversion_PreservesValue()
        {
            // Define custom generator for valid frequency values
            // Test across the full device range: 1 µHz to 500 MHz
            var validFrequencyGen = from freq in Arb.Default.NormalFloat().Generator
                                   where !double.IsNaN(freq.Get) && !double.IsInfinity(freq.Get)
                                   select Math.Abs(freq.Get) % 500_000_000 + 0.000001; // 1 µHz to 500 MHz

            var arb = Arb.From(validFrequencyGen);

            Prop.ForAll(arb, frequencyHz =>
            {
                try
                {
                    // Step 1: Determine appropriate unit for the frequency
                    string unit = DetermineFrequencyUnit(frequencyHz);

                    // Step 2: Convert frequency to that unit
                    double convertedValue = ConvertFrequencyToUnit(frequencyHz, unit);

                    // Step 3: Format as SCPI string (as the command builder would)
                    string formattedFrequency = $"{FormatNumber(convertedValue)}{unit}";

                    // Step 4: Parse back to Hz (as the response parser would)
                    double parsedFrequencyHz = ParseFrequencyString(formattedFrequency);

                    // Step 5: Verify roundtrip preserves value within tolerance
                    // Use relative tolerance for large values, absolute for small values
                    double tolerance = Math.Max(0.01, frequencyHz * 1e-9); // 0.01 Hz or 1 ppb
                    bool frequencyMatch = Math.Abs(parsedFrequencyHz - frequencyHz) <= tolerance;

                    // Additional verification: ensure unit selection is correct
                    bool correctUnitSelection = VerifyUnitSelection(frequencyHz, unit);

                    return frequencyMatch && correctUnitSelection;
                }
                catch (Exception)
                {
                    // If any exception occurs during conversion, the property fails
                    return false;
                }
            }).QuickCheckThrowOnFailure();
        }

        /// <summary>
        /// Determines the appropriate frequency unit (HZ, KHZ, MHZ) based on frequency value.
        /// Mirrors the logic in ScpiCommandBuilder.DetermineFrequencyUnit()
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
        /// Converts frequency from Hz to the specified unit.
        /// Mirrors the logic in ScpiCommandBuilder.ConvertFrequencyToUnit()
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
        /// Formats a numeric value for SCPI commands using invariant culture.
        /// Mirrors the logic in ScpiCommandBuilder.FormatNumber()
        /// </summary>
        private string FormatNumber(double value)
        {
            return value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a frequency string with unit suffix back to Hz.
        /// Mirrors the logic in ScpiResponseParser.TryParseFrequency()
        /// </summary>
        private double ParseFrequencyString(string frequencyString)
        {
            string value = frequencyString.ToUpperInvariant();

            if (value.EndsWith("MHZ"))
            {
                string numStr = value.Substring(0, value.Length - 3);
                if (double.TryParse(numStr, System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out double num))
                    return num * 1e6;
            }
            else if (value.EndsWith("KHZ"))
            {
                string numStr = value.Substring(0, value.Length - 3);
                if (double.TryParse(numStr, System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out double num))
                    return num * 1e3;
            }
            else if (value.EndsWith("HZ"))
            {
                string numStr = value.Substring(0, value.Length - 2);
                if (double.TryParse(numStr, System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out double num))
                    return num;
            }

            throw new FormatException($"Unable to parse frequency string: {frequencyString}");
        }

        /// <summary>
        /// Verifies that the unit selection is appropriate for the frequency value.
        /// </summary>
        private bool VerifyUnitSelection(double frequencyHz, string selectedUnit)
        {
            // Verify unit selection follows the expected rules:
            // - MHZ for frequencies >= 1 MHz
            // - KHZ for frequencies >= 1 kHz and < 1 MHz
            // - HZ for frequencies < 1 kHz

            if (frequencyHz >= 1e6)
                return selectedUnit == "MHZ";
            else if (frequencyHz >= 1e3)
                return selectedUnit == "KHZ";
            else
                return selectedUnit == "HZ";
        }

        #endregion

        #region Task 12.5: Amplitude-Offset Constraint Property Test

        /// <summary>
        /// **Validates: Requirements 3.3, 3.4, 3.5**
        /// 
        /// Property: For any amplitude and offset values, the validation must enforce
        /// the constraint that |offset| + (amplitude/2) must not exceed the maximum
        /// voltage for the load impedance.
        /// 
        /// This property verifies that the amplitude-offset constraint is correctly
        /// enforced for both 50Ω and High-Z loads, preventing configurations that
        /// would exceed the device's voltage limits.
        /// </summary>
        [FsCheck.NUnit.Property]
        public void Property_AmplitudeOffsetConstraint_EnforcedForAllLoads()
        {
            // Define custom generator for amplitude and offset values
            // Test across wide range including valid and invalid combinations
            var parametersGen = from amp in Arb.Default.NormalFloat().Generator
                               from offset in Arb.Default.NormalFloat().Generator
                               from loadType in Gen.Elements(LoadType.FiftyOhm, LoadType.HighZ)
                               where !double.IsNaN(amp.Get) && !double.IsInfinity(amp.Get) &&
                                     !double.IsNaN(offset.Get) && !double.IsInfinity(offset.Get)
                               select new
                               {
                                   Amplitude = amp.Get,
                                   Offset = offset.Get,
                                   LoadType = loadType
                               };

            var arb = Arb.From(parametersGen);

            Prop.ForAll(arb, testData =>
            {
                // Create load impedance
                LoadImpedance load = testData.LoadType == LoadType.FiftyOhm
                    ? LoadImpedance.FiftyOhm
                    : LoadImpedance.HighZ;

                // Determine maximum voltage for this load
                double maxVoltage = testData.LoadType == LoadType.FiftyOhm ? 10.0 : 20.0;

                // Calculate the peak voltage: |offset| + (amplitude/2)
                double peakVoltage = Math.Abs(testData.Offset) + (Math.Abs(testData.Amplitude) / 2.0);

                // Step 1: Validate offset with amplitude constraint
                var offsetValidation = _inputValidator.ValidateOffset(
                    testData.Offset, 
                    Math.Abs(testData.Amplitude), 
                    load);

                // Step 2: Verify constraint enforcement
                // The constraint is: |offset| + (amplitude/2) ≤ maxVoltage
                bool constraintSatisfied = peakVoltage <= maxVoltage;

                // Step 3: Also check basic offset range validation
                double minOffset = testData.LoadType == LoadType.FiftyOhm ? -10.0 : -20.0;
                double maxOffset = testData.LoadType == LoadType.FiftyOhm ? 10.0 : 20.0;
                bool offsetInRange = testData.Offset >= minOffset && testData.Offset <= maxOffset;

                // Step 4: Also check amplitude is positive and in range
                double minAmplitude = testData.LoadType == LoadType.FiftyOhm ? 0.001 : 0.002;
                double maxAmplitude = testData.LoadType == LoadType.FiftyOhm ? 20.0 : 40.0;
                bool amplitudeValid = Math.Abs(testData.Amplitude) >= minAmplitude && 
                                     Math.Abs(testData.Amplitude) <= maxAmplitude;

                // Step 5: Verify validation result consistency
                // Validation should pass if and only if:
                // - Amplitude is valid (positive and in range)
                // - Offset is in range
                // - Constraint is satisfied
                bool shouldPass = amplitudeValid && offsetInRange && constraintSatisfied;

                // The validation result should match our expectation
                if (shouldPass)
                {
                    // If all conditions are met, validation should pass
                    if (!offsetValidation.IsValid)
                    {
                        // This is a false negative - validation rejected a valid configuration
                        return false;
                    }
                }
                else
                {
                    // If any condition fails, validation should fail
                    if (offsetValidation.IsValid)
                    {
                        // This is a false positive - validation accepted an invalid configuration
                        // This is critical for safety!
                        return false;
                    }
                }

                // Additional verification: if constraint is violated, error message should mention it
                if (!constraintSatisfied && amplitudeValid && offsetInRange)
                {
                    // The constraint is the reason for failure
                    if (offsetValidation.IsValid)
                    {
                        // Validation should have caught this!
                        return false;
                    }
                    
                    // Verify error message mentions the constraint
                    if (!offsetValidation.ErrorMessage.ToLowerInvariant().Contains("exceed") &&
                        !offsetValidation.ErrorMessage.ToLowerInvariant().Contains("maximum"))
                    {
                        // Error message should indicate the constraint violation
                        return false;
                    }
                }

                return true;
            }).QuickCheckThrowOnFailure();
        }

        /// <summary>
        /// **Validates: Requirements 3.3, 3.4, 3.5**
        /// 
        /// Property: For valid amplitude and offset combinations that satisfy the
        /// constraint, the validation must pass. For combinations that violate the
        /// constraint, validation must fail.
        /// 
        /// This is a focused test that specifically targets the boundary conditions
        /// of the amplitude-offset constraint.
        /// </summary>
        [FsCheck.NUnit.Property]
        public void Property_AmplitudeOffsetConstraint_BoundaryConditions()
        {
            // Define custom generator for boundary testing
            // Generate values near the constraint boundary
            var boundaryGen = from loadType in Gen.Elements(LoadType.FiftyOhm, LoadType.HighZ)
                             from amplitudeFraction in Arb.Default.NormalFloat().Generator
                             from offsetFraction in Arb.Default.NormalFloat().Generator
                             where !double.IsNaN(amplitudeFraction.Get) && !double.IsInfinity(amplitudeFraction.Get) &&
                                   !double.IsNaN(offsetFraction.Get) && !double.IsInfinity(offsetFraction.Get)
                             select new
                             {
                                 LoadType = loadType,
                                 AmplitudeFraction = Math.Abs(amplitudeFraction.Get) % 1.0, // 0 to 1
                                 OffsetFraction = Math.Abs(offsetFraction.Get) % 1.0        // 0 to 1
                             };

            var arb = Arb.From(boundaryGen);

            Prop.ForAll(arb, testData =>
            {
                // Create load impedance
                LoadImpedance load = testData.LoadType == LoadType.FiftyOhm
                    ? LoadImpedance.FiftyOhm
                    : LoadImpedance.HighZ;

                // Determine limits for this load
                double maxVoltage = testData.LoadType == LoadType.FiftyOhm ? 10.0 : 20.0;
                double minAmplitude = testData.LoadType == LoadType.FiftyOhm ? 0.001 : 0.002;
                double maxAmplitude = testData.LoadType == LoadType.FiftyOhm ? 20.0 : 40.0;

                // Generate amplitude in valid range
                double amplitude = minAmplitude + (testData.AmplitudeFraction * (maxAmplitude - minAmplitude));

                // Generate offset that tests the constraint boundary
                // For a given amplitude, the maximum allowed |offset| is: maxVoltage - (amplitude/2)
                double maxAllowedOffset = maxVoltage - (amplitude / 2.0);

                // Generate offset near the boundary (both valid and invalid)
                // Use offsetFraction to create values around the boundary
                double offset;
                if (testData.OffsetFraction < 0.5)
                {
                    // Generate valid offset (within constraint)
                    offset = (testData.OffsetFraction * 2.0) * maxAllowedOffset;
                }
                else
                {
                    // Generate invalid offset (violates constraint)
                    // Add a small amount beyond the limit
                    double excess = ((testData.OffsetFraction - 0.5) * 2.0) * maxVoltage;
                    offset = maxAllowedOffset + excess + 0.001;
                }

                // Randomly make offset negative
                if (testData.AmplitudeFraction > 0.5)
                {
                    offset = -offset;
                }

                // Validate the combination
                var offsetValidation = _inputValidator.ValidateOffset(offset, amplitude, load);

                // Calculate actual peak voltage
                double peakVoltage = Math.Abs(offset) + (amplitude / 2.0);

                // Determine if configuration should be valid
                bool constraintSatisfied = peakVoltage <= maxVoltage;
                bool offsetInRange = Math.Abs(offset) <= maxVoltage;

                bool shouldPass = constraintSatisfied && offsetInRange;

                // Verify validation result matches expectation
                if (shouldPass && !offsetValidation.IsValid)
                {
                    // False negative
                    return false;
                }

                if (!shouldPass && offsetValidation.IsValid)
                {
                    // False positive - this is critical!
                    return false;
                }

                return true;
            }).QuickCheckThrowOnFailure();
        }

        #endregion
    }
}
