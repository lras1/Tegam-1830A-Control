using NUnit.Framework;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Validation;

namespace Tegam._1830A.Tests.Unit
{
    [TestFixture]
    public class InputValidatorTests
    {
        private InputValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new InputValidator();
        }

        #region Frequency Validation Tests

        [Test]
        public void ValidateFrequency_WithValidFrequencyInHz_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(100_000, FrequencyUnit.Hz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_WithValidFrequencyInkHz_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(100, FrequencyUnit.kHz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_WithValidFrequencyInMHz_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(2.4, FrequencyUnit.MHz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_WithValidFrequencyInGHz_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(5.8, FrequencyUnit.GHz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_WithMinimumFrequency_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(100, FrequencyUnit.kHz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_WithMaximumFrequency_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(40, FrequencyUnit.GHz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_BelowMinimum_ReturnsInvalid()
        {
            var result = _validator.ValidateFrequency(50, FrequencyUnit.kHz);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ValidateFrequency_AboveMaximum_ReturnsInvalid()
        {
            var result = _validator.ValidateFrequency(50, FrequencyUnit.GHz);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ValidateFrequency_WithNegativeFrequency_ReturnsInvalid()
        {
            var result = _validator.ValidateFrequency(-100, FrequencyUnit.MHz);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFrequency_WithZeroFrequency_ReturnsInvalid()
        {
            var result = _validator.ValidateFrequency(0, FrequencyUnit.MHz);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFrequency_WithDecimalFrequency_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(2.45, FrequencyUnit.GHz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_WithVerySmallFrequency_ReturnsInvalid()
        {
            var result = _validator.ValidateFrequency(0.001, FrequencyUnit.kHz);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFrequency_WithVeryLargeFrequency_ReturnsInvalid()
        {
            var result = _validator.ValidateFrequency(100, FrequencyUnit.GHz);
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region Sensor ID Validation Tests

        [Test]
        public void ValidateSensorId_WithValidId1_ReturnsValid()
        {
            var result = _validator.ValidateSensorId(1);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateSensorId_WithValidId2_ReturnsValid()
        {
            var result = _validator.ValidateSensorId(2);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateSensorId_WithValidId3_ReturnsValid()
        {
            var result = _validator.ValidateSensorId(3);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateSensorId_WithValidId4_ReturnsValid()
        {
            var result = _validator.ValidateSensorId(4);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateSensorId_WithIdZero_ReturnsInvalid()
        {
            var result = _validator.ValidateSensorId(0);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ValidateSensorId_WithIdFive_ReturnsInvalid()
        {
            var result = _validator.ValidateSensorId(5);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateSensorId_WithNegativeId_ReturnsInvalid()
        {
            var result = _validator.ValidateSensorId(-1);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateSensorId_WithLargeId_ReturnsInvalid()
        {
            var result = _validator.ValidateSensorId(100);
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region Calibration Mode Validation Tests

        [Test]
        public void ValidateCalibrationMode_WithInternal_ReturnsValid()
        {
            var result = _validator.ValidateCalibrationMode(CalibrationMode.Internal);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateCalibrationMode_WithExternal_ReturnsValid()
        {
            var result = _validator.ValidateCalibrationMode(CalibrationMode.External);
            Assert.That(result.IsValid, Is.True);
        }

        #endregion

        #region Filename Validation Tests

        [Test]
        public void ValidateFilename_WithValidFilename_ReturnsValid()
        {
            var result = _validator.ValidateFilename("measurements.csv");
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFilename_WithValidPath_ReturnsValid()
        {
            var result = _validator.ValidateFilename("C:\\Data\\measurements.csv");
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFilename_WithValidPathAndSpecialChars_ReturnsValid()
        {
            var result = _validator.ValidateFilename("C:\\Data\\test_2024-01-15.csv");
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFilename_WithEmptyString_ReturnsInvalid()
        {
            var result = _validator.ValidateFilename("");
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ValidateFilename_WithWhitespace_ReturnsInvalid()
        {
            var result = _validator.ValidateFilename("   ");
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFilename_WithInvalidCharacters_ReturnsInvalid()
        {
            var result = _validator.ValidateFilename("file<name>.csv");
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFilename_WithPipeCharacter_ReturnsInvalid()
        {
            var result = _validator.ValidateFilename("file|name.csv");
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFilename_WithAsteriskCharacter_ReturnsInvalid()
        {
            var result = _validator.ValidateFilename("file*name.csv");
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFilename_WithQuestionMarkCharacter_ReturnsInvalid()
        {
            var result = _validator.ValidateFilename("file?name.csv");
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFilename_WithDoubleQuoteCharacter_ReturnsInvalid()
        {
            var result = _validator.ValidateFilename("file\"name.csv");
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region Measurement Count Validation Tests

        [Test]
        public void ValidateMeasurementCount_WithValidCount_ReturnsValid()
        {
            var result = _validator.ValidateMeasurementCount(10);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateMeasurementCount_WithCountOne_ReturnsValid()
        {
            var result = _validator.ValidateMeasurementCount(1);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateMeasurementCount_WithLargeCount_ReturnsValid()
        {
            var result = _validator.ValidateMeasurementCount(1000);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateMeasurementCount_WithZero_ReturnsInvalid()
        {
            var result = _validator.ValidateMeasurementCount(0);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ValidateMeasurementCount_WithNegativeCount_ReturnsInvalid()
        {
            var result = _validator.ValidateMeasurementCount(-5);
            Assert.That(result.IsValid, Is.False);
        }

        #endregion

        #region Measurement Delay Validation Tests

        [Test]
        public void ValidateMeasurementDelay_WithValidDelay_ReturnsValid()
        {
            var result = _validator.ValidateMeasurementDelay(100);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateMeasurementDelay_WithZeroDelay_ReturnsValid()
        {
            var result = _validator.ValidateMeasurementDelay(0);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateMeasurementDelay_WithLargeDelay_ReturnsValid()
        {
            var result = _validator.ValidateMeasurementDelay(10000);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateMeasurementDelay_WithNegativeDelay_ReturnsInvalid()
        {
            var result = _validator.ValidateMeasurementDelay(-100);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null.And.Not.Empty);
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ValidateFrequency_WithBoundaryMinimum_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(100_000, FrequencyUnit.Hz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_WithBoundaryMaximum_ReturnsValid()
        {
            var result = _validator.ValidateFrequency(40_000_000_000, FrequencyUnit.Hz);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ValidateFrequency_JustBelowMinimum_ReturnsInvalid()
        {
            var result = _validator.ValidateFrequency(99_999, FrequencyUnit.Hz);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFrequency_JustAboveMaximum_ReturnsInvalid()
        {
            var result = _validator.ValidateFrequency(40_000_000_001, FrequencyUnit.Hz);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ValidateFilename_WithUnicodeCharacters_ReturnsValid()
        {
            var result = _validator.ValidateFilename("measurements_日本.csv");
            Assert.That(result.IsValid, Is.True);
        }

        #endregion
    }
}
