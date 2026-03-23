using System;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Validation.Tests
{
    /// <summary>
    /// Unit tests for InputValidator class.
    /// These tests verify that input validation works correctly for all device parameters.
    /// </summary>
    public class InputValidatorTests
    {
        private IInputValidator _validator;

        public void Setup()
        {
            _validator = new InputValidator();
        }

        #region ValidateFrequency Tests

        public void ValidateFrequency_WithValidFrequencyInHz_ReturnsValid()
        {
            Setup();
            double frequency = 100_000; // 100 kHz in Hz
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.Hz);
            AssertTrue(result.IsValid, "Frequency 100 kHz should be valid");
        }

        public void ValidateFrequency_WithValidFrequencyInkHz_ReturnsValid()
        {
            Setup();
            double frequency = 100; // 100 kHz
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.kHz);
            AssertTrue(result.IsValid, "Frequency 100 kHz should be valid");
        }

        public void ValidateFrequency_WithValidFrequencyInMHz_ReturnsValid()
        {
            Setup();
            double frequency = 2.4; // 2.4 MHz
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.MHz);
            AssertTrue(result.IsValid, "Frequency 2.4 MHz should be valid");
        }

        public void ValidateFrequency_WithValidFrequencyInGHz_ReturnsValid()
        {
            Setup();
            double frequency = 2.4; // 2.4 GHz
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.GHz);
            AssertTrue(result.IsValid, "Frequency 2.4 GHz should be valid");
        }

        public void ValidateFrequency_WithMaxFrequency_ReturnsValid()
        {
            Setup();
            double frequency = 40; // 40 GHz
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.GHz);
            AssertTrue(result.IsValid, "Frequency 40 GHz should be valid");
        }

        public void ValidateFrequency_WithMinFrequency_ReturnsValid()
        {
            Setup();
            double frequency = 100; // 100 kHz
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.kHz);
            AssertTrue(result.IsValid, "Frequency 100 kHz should be valid");
        }

        public void ValidateFrequency_BelowMinimum_ReturnsInvalid()
        {
            Setup();
            double frequency = 50; // 50 kHz (below 100 kHz minimum)
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.kHz);
            AssertFalse(result.IsValid, "Frequency 50 kHz should be invalid");
            AssertTrue(result.ErrorMessage.Contains("100 kHz"), "Error message should mention 100 kHz");
        }

        public void ValidateFrequency_AboveMaximum_ReturnsInvalid()
        {
            Setup();
            double frequency = 50; // 50 GHz (above 40 GHz maximum)
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.GHz);
            AssertFalse(result.IsValid, "Frequency 50 GHz should be invalid");
            AssertTrue(result.ErrorMessage.Contains("40 GHz"), "Error message should mention 40 GHz");
        }

        public void ValidateFrequency_WithNegativeFrequency_ReturnsInvalid()
        {
            Setup();
            double frequency = -10;
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.MHz);
            AssertFalse(result.IsValid, "Negative frequency should be invalid");
            AssertTrue(result.ErrorMessage.Contains("negative"), "Error message should mention negative");
        }

        public void ValidateFrequency_WithZeroFrequency_ReturnsInvalid()
        {
            Setup();
            double frequency = 0;
            var result = _validator.ValidateFrequency(frequency, FrequencyUnit.MHz);
            AssertFalse(result.IsValid, "Zero frequency should be invalid");
        }

        #endregion

        #region ValidateSensorId Tests

        public void ValidateSensorId_WithValidId1_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateSensorId(1);
            AssertTrue(result.IsValid, "Sensor ID 1 should be valid");
        }

        public void ValidateSensorId_WithValidId2_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateSensorId(2);
            AssertTrue(result.IsValid, "Sensor ID 2 should be valid");
        }

        public void ValidateSensorId_WithValidId3_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateSensorId(3);
            AssertTrue(result.IsValid, "Sensor ID 3 should be valid");
        }

        public void ValidateSensorId_WithValidId4_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateSensorId(4);
            AssertTrue(result.IsValid, "Sensor ID 4 should be valid");
        }

        public void ValidateSensorId_WithIdBelowRange_ReturnsInvalid()
        {
            Setup();
            var result = _validator.ValidateSensorId(0);
            AssertFalse(result.IsValid, "Sensor ID 0 should be invalid");
            AssertTrue(result.ErrorMessage.Contains("1") && result.ErrorMessage.Contains("4"), 
                "Error message should mention range 1-4");
        }

        public void ValidateSensorId_WithIdAboveRange_ReturnsInvalid()
        {
            Setup();
            var result = _validator.ValidateSensorId(5);
            AssertFalse(result.IsValid, "Sensor ID 5 should be invalid");
            AssertTrue(result.ErrorMessage.Contains("1") && result.ErrorMessage.Contains("4"), 
                "Error message should mention range 1-4");
        }

        public void ValidateSensorId_WithNegativeId_ReturnsInvalid()
        {
            Setup();
            var result = _validator.ValidateSensorId(-1);
            AssertFalse(result.IsValid, "Negative sensor ID should be invalid");
        }

        #endregion

        #region ValidateCalibrationMode Tests

        public void ValidateCalibrationMode_WithInternal_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateCalibrationMode(CalibrationMode.Internal);
            AssertTrue(result.IsValid, "Internal calibration mode should be valid");
        }

        public void ValidateCalibrationMode_WithExternal_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateCalibrationMode(CalibrationMode.External);
            AssertTrue(result.IsValid, "External calibration mode should be valid");
        }

        #endregion

        #region ValidateFilename Tests

        public void ValidateFilename_WithValidFilename_ReturnsValid()
        {
            Setup();
            string filename = "measurements.csv";
            var result = _validator.ValidateFilename(filename);
            AssertTrue(result.IsValid, "Valid filename should be valid");
        }

        public void ValidateFilename_WithValidPathFilename_ReturnsValid()
        {
            Setup();
            string filename = "C:\\Logs\\measurements.csv";
            var result = _validator.ValidateFilename(filename);
            AssertTrue(result.IsValid, "Valid path filename should be valid");
        }

        public void ValidateFilename_WithEmptyString_ReturnsInvalid()
        {
            Setup();
            string filename = "";
            var result = _validator.ValidateFilename(filename);
            AssertFalse(result.IsValid, "Empty filename should be invalid");
            AssertTrue(result.ErrorMessage.Contains("empty"), "Error message should mention empty");
        }

        public void ValidateFilename_WithWhitespaceOnly_ReturnsInvalid()
        {
            Setup();
            string filename = "   ";
            var result = _validator.ValidateFilename(filename);
            AssertFalse(result.IsValid, "Whitespace-only filename should be invalid");
        }

        public void ValidateFilename_WithNull_ReturnsInvalid()
        {
            Setup();
            string filename = null;
            var result = _validator.ValidateFilename(filename);
            AssertFalse(result.IsValid, "Null filename should be invalid");
        }

        public void ValidateFilename_WithInvalidCharacters_ReturnsInvalid()
        {
            Setup();
            string filename = "file<name>.csv";
            var result = _validator.ValidateFilename(filename);
            AssertFalse(result.IsValid, "Filename with invalid characters should be invalid");
            AssertTrue(result.ErrorMessage.Contains("invalid"), "Error message should mention invalid");
        }

        #endregion

        #region ValidateMeasurementCount Tests

        public void ValidateMeasurementCount_WithPositiveCount_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateMeasurementCount(1);
            AssertTrue(result.IsValid, "Positive count should be valid");
        }

        public void ValidateMeasurementCount_WithLargePositiveCount_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateMeasurementCount(1000);
            AssertTrue(result.IsValid, "Large positive count should be valid");
        }

        public void ValidateMeasurementCount_WithZero_ReturnsInvalid()
        {
            Setup();
            var result = _validator.ValidateMeasurementCount(0);
            AssertFalse(result.IsValid, "Zero count should be invalid");
            AssertTrue(result.ErrorMessage.Contains("positive"), "Error message should mention positive");
        }

        public void ValidateMeasurementCount_WithNegativeCount_ReturnsInvalid()
        {
            Setup();
            var result = _validator.ValidateMeasurementCount(-5);
            AssertFalse(result.IsValid, "Negative count should be invalid");
            AssertTrue(result.ErrorMessage.Contains("positive"), "Error message should mention positive");
        }

        #endregion

        #region ValidateMeasurementDelay Tests

        public void ValidateMeasurementDelay_WithZeroDelay_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateMeasurementDelay(0);
            AssertTrue(result.IsValid, "Zero delay should be valid");
        }

        public void ValidateMeasurementDelay_WithPositiveDelay_ReturnsValid()
        {
            Setup();
            var result = _validator.ValidateMeasurementDelay(1000);
            AssertTrue(result.IsValid, "Positive delay should be valid");
        }

        public void ValidateMeasurementDelay_WithNegativeDelay_ReturnsInvalid()
        {
            Setup();
            var result = _validator.ValidateMeasurementDelay(-100);
            AssertFalse(result.IsValid, "Negative delay should be invalid");
            AssertTrue(result.ErrorMessage.Contains("negative"), "Error message should mention negative");
        }

        #endregion

        #region Helper Methods

        private void AssertTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception($"Assertion failed: {message}");
            }
        }

        private void AssertFalse(bool condition, string message)
        {
            if (condition)
            {
                throw new Exception($"Assertion failed: {message}");
            }
        }

        #endregion
    }
}
