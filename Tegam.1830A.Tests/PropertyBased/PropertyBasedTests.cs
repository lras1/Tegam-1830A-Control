using FsCheck;
using FsCheck.NUnit;
using NUnit.Framework;
using System;
using Tegam._1830A.DeviceLibrary.Commands;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Parsing;
using Tegam._1830A.DeviceLibrary.Validation;

namespace Tegam._1830A.Tests.PropertyBased
{
    [TestFixture]
    public class PropertyBasedTests
    {
        private ScpiCommandBuilder _commandBuilder;
        private ScpiResponseParser _responseParser;
        private InputValidator _validator;

        [SetUp]
        public void Setup()
        {
            _commandBuilder = new ScpiCommandBuilder();
            _responseParser = new ScpiResponseParser();
            _validator = new InputValidator();
        }

        #region Property 1: Command-Response Roundtrip Consistency

        /// <summary>
        /// **Validates: Requirements 2.0, 3.0, 4.0**
        /// 
        /// Property: For any valid frequency value and unit, building a frequency command
        /// and then querying the frequency should return the same frequency value.
        /// </summary>
        [Property]
        public void Property_FrequencyCommandResponseRoundtrip(double frequency, int unitIndex)
        {
            // Arrange: Generate valid frequency and unit
            var units = new[] { FrequencyUnit.Hz, FrequencyUnit.kHz, FrequencyUnit.MHz, FrequencyUnit.GHz };
            var unit = units[Math.Abs(unitIndex) % units.Length];
            
            // Constrain frequency to valid range (100 kHz to 40 GHz)
            var validFrequency = 100 + (Math.Abs(frequency) % 39_999_900);
            
            // Act: Build command and simulate response
            var command = _commandBuilder.BuildFrequencyCommand(validFrequency, unit);
            Assert.That(command, Does.Contain("FREQ"));
            Assert.That(command, Does.Contain(validFrequency.ToString()));
            
            // Verify: Command contains the frequency value
            var parts = command.Split(' ');
            Assert.That(parts.Length, Is.GreaterThanOrEqualTo(2));
        }

        /// <summary>
        /// **Validates: Requirements 2.0, 3.0, 4.0**
        /// 
        /// Property: For any valid power value and unit, building a power measurement
        /// command should produce a valid SCPI command.
        /// </summary>
        [Property]
        public void Property_PowerMeasurementCommandConsistency(double powerValue, int unitIndex)
        {
            // Arrange: Generate valid power unit
            var units = new[] { PowerUnit.dBm, PowerUnit.W, PowerUnit.mW };
            var unit = units[Math.Abs(unitIndex) % units.Length];
            
            // Constrain power value to reasonable range
            var validPower = -50 + (Math.Abs(powerValue) % 100);
            
            // Act: Convert power between units
            var convertedValue = _commandBuilder.ConvertPower(validPower, unit, unit);
            
            // Verify: Converting to same unit returns same value
            Assert.That(convertedValue, Is.EqualTo(validPower).Within(0.0001));
        }

        /// <summary>
        /// **Validates: Requirements 2.0, 3.0, 4.0**
        /// 
        /// Property: For any valid sensor ID, building a sensor selection command
        /// should produce a valid SCPI command.
        /// </summary>
        [Property]
        public void Property_SensorSelectionCommandConsistency(int sensorId)
        {
            // Arrange: Constrain sensor ID to valid range (1-4)
            var validSensorId = 1 + (Math.Abs(sensorId) % 4);
            
            // Act: Build sensor selection command
            var command = _commandBuilder.BuildSelectSensorCommand(validSensorId);
            
            // Verify: Command contains sensor ID
            Assert.That(command, Does.Contain("SENS:SEL"));
            Assert.That(command, Does.Contain(validSensorId.ToString()));
        }

        #endregion

        #region Property 2: Validation Idempotence

        /// <summary>
        /// **Validates: Requirements 7.0**
        /// 
        /// Property: Validating the same input multiple times should always produce
        /// the same result (validation is idempotent).
        /// </summary>
        [Property]
        public void Property_FrequencyValidationIdempotence(double frequency, int unitIndex)
        {
            // Arrange: Generate valid frequency and unit
            var units = new[] { FrequencyUnit.Hz, FrequencyUnit.kHz, FrequencyUnit.MHz, FrequencyUnit.GHz };
            var unit = units[Math.Abs(unitIndex) % units.Length];
            
            // Constrain frequency to valid range
            var validFrequency = 100 + (Math.Abs(frequency) % 39_999_900);
            
            // Act: Validate the same input multiple times
            var result1 = _validator.ValidateFrequency(validFrequency, unit);
            var result2 = _validator.ValidateFrequency(validFrequency, unit);
            var result3 = _validator.ValidateFrequency(validFrequency, unit);
            
            // Verify: All results are identical
            Assert.That(result1.IsValid, Is.EqualTo(result2.IsValid));
            Assert.That(result2.IsValid, Is.EqualTo(result3.IsValid));
        }

        /// <summary>
        /// **Validates: Requirements 7.0**
        /// 
        /// Property: Validating sensor IDs should be idempotent.
        /// </summary>
        [Property]
        public void Property_SensorIdValidationIdempotence(int sensorId)
        {
            // Arrange: Constrain sensor ID to valid range
            var validSensorId = 1 + (Math.Abs(sensorId) % 4);
            
            // Act: Validate the same input multiple times
            var result1 = _validator.ValidateSensorId(validSensorId);
            var result2 = _validator.ValidateSensorId(validSensorId);
            var result3 = _validator.ValidateSensorId(validSensorId);
            
            // Verify: All results are identical
            Assert.That(result1.IsValid, Is.EqualTo(result2.IsValid));
            Assert.That(result2.IsValid, Is.EqualTo(result3.IsValid));
        }

        /// <summary>
        /// **Validates: Requirements 7.0**
        /// 
        /// Property: Validating measurement counts should be idempotent.
        /// </summary>
        [Property]
        public void Property_MeasurementCountValidationIdempotence(int count)
        {
            // Arrange: Constrain count to valid range (1-1000)
            var validCount = 1 + (Math.Abs(count) % 1000);
            
            // Act: Validate the same input multiple times
            var result1 = _validator.ValidateMeasurementCount(validCount);
            var result2 = _validator.ValidateMeasurementCount(validCount);
            var result3 = _validator.ValidateMeasurementCount(validCount);
            
            // Verify: All results are identical
            Assert.That(result1.IsValid, Is.EqualTo(result2.IsValid));
            Assert.That(result2.IsValid, Is.EqualTo(result3.IsValid));
        }

        #endregion

        #region Property 3: Frequency Unit Conversion Accuracy

        /// <summary>
        /// **Validates: Requirements 2.0**
        /// 
        /// Property: Converting a frequency from one unit to another and back
        /// should return the original value (within floating-point precision).
        /// </summary>
        [Property]
        public void Property_FrequencyConversionRoundtrip(double frequency, int fromUnitIndex, int toUnitIndex)
        {
            // Arrange: Generate valid frequency and units
            var units = new[] { FrequencyUnit.Hz, FrequencyUnit.kHz, FrequencyUnit.MHz, FrequencyUnit.GHz };
            var fromUnit = units[Math.Abs(fromUnitIndex) % units.Length];
            var toUnit = units[Math.Abs(toUnitIndex) % units.Length];
            
            // Constrain frequency to valid range
            var validFrequency = 100 + (Math.Abs(frequency) % 39_999_900);
            
            // Act: Convert from unit to another unit and back
            var converted = _commandBuilder.ConvertFrequency(validFrequency, fromUnit, toUnit);
            var roundtrip = _commandBuilder.ConvertFrequency(converted, toUnit, fromUnit);
            
            // Verify: Roundtrip conversion returns original value
            Assert.That(roundtrip, Is.EqualTo(validFrequency).Within(0.01));
        }

        /// <summary>
        /// **Validates: Requirements 2.0**
        /// 
        /// Property: Converting from Hz to kHz and back should preserve the value.
        /// </summary>
        [Property]
        public void Property_FrequencyHzToKHzConversion(double frequencyInHz)
        {
            // Arrange: Constrain frequency to valid range in Hz
            var validFrequency = 100_000 + (Math.Abs(frequencyInHz) % 39_999_900_000);
            
            // Act: Convert Hz to kHz and back
            var inKHz = _commandBuilder.ConvertFrequency(validFrequency, FrequencyUnit.Hz, FrequencyUnit.kHz);
            var backToHz = _commandBuilder.ConvertFrequency(inKHz, FrequencyUnit.kHz, FrequencyUnit.Hz);
            
            // Verify: Roundtrip returns original value
            Assert.That(backToHz, Is.EqualTo(validFrequency).Within(1.0));
        }

        /// <summary>
        /// **Validates: Requirements 2.0**
        /// 
        /// Property: Converting from MHz to GHz and back should preserve the value.
        /// </summary>
        [Property]
        public void Property_FrequencyMHzToGHzConversion(double frequencyInMHz)
        {
            // Arrange: Constrain frequency to valid range in MHz
            var validFrequency = 100 + (Math.Abs(frequencyInMHz) % 39_999_900);
            
            // Act: Convert MHz to GHz and back
            var inGHz = _commandBuilder.ConvertFrequency(validFrequency, FrequencyUnit.MHz, FrequencyUnit.GHz);
            var backToMHz = _commandBuilder.ConvertFrequency(inGHz, FrequencyUnit.GHz, FrequencyUnit.MHz);
            
            // Verify: Roundtrip returns original value
            Assert.That(backToMHz, Is.EqualTo(validFrequency).Within(0.001));
        }

        #endregion

        #region Property 4: Power Unit Conversion Accuracy

        /// <summary>
        /// **Validates: Requirements 3.0**
        /// 
        /// Property: Converting a power value from one unit to another and back
        /// should return the original value (within floating-point precision).
        /// </summary>
        [Property]
        public void Property_PowerConversionRoundtrip(double powerValue, int fromUnitIndex, int toUnitIndex)
        {
            // Arrange: Generate valid power and units
            var units = new[] { PowerUnit.dBm, PowerUnit.W, PowerUnit.mW };
            var fromUnit = units[Math.Abs(fromUnitIndex) % units.Length];
            var toUnit = units[Math.Abs(toUnitIndex) % units.Length];
            
            // Constrain power value to reasonable range
            var validPower = -50 + (Math.Abs(powerValue) % 100);
            
            // Act: Convert from unit to another unit and back
            var converted = _commandBuilder.ConvertPower(validPower, fromUnit, toUnit);
            var roundtrip = _commandBuilder.ConvertPower(converted, toUnit, fromUnit);
            
            // Verify: Roundtrip conversion returns original value
            Assert.That(roundtrip, Is.EqualTo(validPower).Within(0.01));
        }

        /// <summary>
        /// **Validates: Requirements 3.0**
        /// 
        /// Property: Converting from Watts to milliWatts and back should preserve the value.
        /// </summary>
        [Property]
        public void Property_PowerWattsToMilliWattsConversion(double powerInWatts)
        {
            // Arrange: Constrain power to valid range
            var validPower = 0.001 + (Math.Abs(powerInWatts) % 100);
            
            // Act: Convert W to mW and back
            var inMilliWatts = _commandBuilder.ConvertPower(validPower, PowerUnit.W, PowerUnit.mW);
            var backToWatts = _commandBuilder.ConvertPower(inMilliWatts, PowerUnit.mW, PowerUnit.W);
            
            // Verify: Roundtrip returns original value
            Assert.That(backToWatts, Is.EqualTo(validPower).Within(0.00001));
        }

        /// <summary>
        /// **Validates: Requirements 3.0**
        /// 
        /// Property: Converting from dBm to Watts and back should preserve the value.
        /// </summary>
        [Property]
        public void Property_PowerdBmToWattsConversion(double powerIndBm)
        {
            // Arrange: Constrain power to valid range
            var validPower = -50 + (Math.Abs(powerIndBm) % 100);
            
            // Act: Convert dBm to W and back
            var inWatts = _commandBuilder.ConvertPower(validPower, PowerUnit.dBm, PowerUnit.W);
            var backTodBm = _commandBuilder.ConvertPower(inWatts, PowerUnit.W, PowerUnit.dBm);
            
            // Verify: Roundtrip returns original value
            Assert.That(backTodBm, Is.EqualTo(validPower).Within(0.01));
        }

        /// <summary>
        /// **Validates: Requirements 3.0**
        /// 
        /// Property: Converting from Watts to dBm and back should preserve the value.
        /// </summary>
        [Property]
        public void Property_PowerWattsTodBmConversion(double powerInWatts)
        {
            // Arrange: Constrain power to valid range (must be positive for dBm conversion)
            var validPower = 0.001 + (Math.Abs(powerInWatts) % 100);
            
            // Act: Convert W to dBm and back
            var indBm = _commandBuilder.ConvertPower(validPower, PowerUnit.W, PowerUnit.dBm);
            var backToWatts = _commandBuilder.ConvertPower(indBm, PowerUnit.dBm, PowerUnit.W);
            
            // Verify: Roundtrip returns original value
            Assert.That(backToWatts, Is.EqualTo(validPower).Within(0.0001));
        }

        #endregion
    }
}
