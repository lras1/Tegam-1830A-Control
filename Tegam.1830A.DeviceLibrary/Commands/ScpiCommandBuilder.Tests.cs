using System;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Commands.Tests
{
    /// <summary>
    /// Unit tests for ScpiCommandBuilder class.
    /// These tests verify that SCPI commands are built correctly for all device operations.
    /// </summary>
    public class ScpiCommandBuilderTests
    {
        private ScpiCommandBuilder _builder;

        public void Setup()
        {
            _builder = new ScpiCommandBuilder();
        }

        // Frequency Command Tests
        public void BuildFrequencyCommand_WithHz_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyCommand(100, FrequencyUnit.Hz);
            AssertEqual("FREQ 100 HZ", result);
        }

        public void BuildFrequencyCommand_WithkHz_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyCommand(2400, FrequencyUnit.kHz);
            AssertEqual("FREQ 2400 KHZ", result);
        }

        public void BuildFrequencyCommand_WithMHz_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyCommand(2.4, FrequencyUnit.MHz);
            AssertEqual("FREQ 2.4 MHZ", result);
        }

        public void BuildFrequencyCommand_WithGHz_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyCommand(2.4, FrequencyUnit.GHz);
            AssertEqual("FREQ 2.4 GHZ", result);
        }

        public void BuildFrequencyCommand_WithNegativeFrequency_ThrowsException()
        {
            try
            {
                _builder.BuildFrequencyCommand(-1, FrequencyUnit.MHz);
                throw new Exception("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        public void BuildFrequencyQueryCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyQueryCommand();
            AssertEqual("FREQ?", result);
        }

        // Power Measurement Tests
        public void BuildMeasurePowerCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildMeasurePowerCommand();
            AssertEqual("MEAS:POW", result);
        }

        public void BuildMeasurePowerQueryCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildMeasurePowerQueryCommand();
            AssertEqual("MEAS:POW?", result);
        }

        // Sensor Management Tests
        public void BuildSelectSensorCommand_WithValidSensorId_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSelectSensorCommand(1);
            AssertEqual("SENS:SEL 1", result);
        }

        public void BuildSelectSensorCommand_WithSensorId4_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSelectSensorCommand(4);
            AssertEqual("SENS:SEL 4", result);
        }

        public void BuildSelectSensorCommand_WithInvalidSensorId0_ThrowsException()
        {
            try
            {
                _builder.BuildSelectSensorCommand(0);
                throw new Exception("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        public void BuildSelectSensorCommand_WithInvalidSensorId5_ThrowsException()
        {
            try
            {
                _builder.BuildSelectSensorCommand(5);
                throw new Exception("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        public void BuildQuerySensorCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildQuerySensorCommand();
            AssertEqual("SENS:SEL?", result);
        }

        public void BuildQueryAvailableSensorsCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildQueryAvailableSensorsCommand();
            AssertEqual("SENS:LIST?", result);
        }

        // Calibration Tests
        public void BuildCalibrateCommand_WithInternalMode_ReturnsCorrectCommand()
        {
            var result = _builder.BuildCalibrateCommand(CalibrationMode.Internal);
            AssertEqual("CAL:START INT", result);
        }

        public void BuildCalibrateCommand_WithExternalMode_ReturnsCorrectCommand()
        {
            var result = _builder.BuildCalibrateCommand(CalibrationMode.External);
            AssertEqual("CAL:START EXT", result);
        }

        public void BuildQueryCalibrationStatusCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildQueryCalibrationStatusCommand();
            AssertEqual("CAL:STAT?", result);
        }

        // Data Logging Tests
        public void BuildStartLoggingCommand_WithValidFilename_ReturnsCorrectCommand()
        {
            var result = _builder.BuildStartLoggingCommand("measurements.log");
            AssertEqual("LOG:START \"measurements.log\"", result);
        }

        public void BuildStartLoggingCommand_WithNullFilename_ThrowsException()
        {
            try
            {
                _builder.BuildStartLoggingCommand(null);
                throw new Exception("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        public void BuildStartLoggingCommand_WithEmptyFilename_ThrowsException()
        {
            try
            {
                _builder.BuildStartLoggingCommand("");
                throw new Exception("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        public void BuildStopLoggingCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildStopLoggingCommand();
            AssertEqual("LOG:STOP", result);
        }

        public void BuildQueryLoggingStatusCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildQueryLoggingStatusCommand();
            AssertEqual("LOG:STAT?", result);
        }

        // System Command Tests
        public void BuildSystemCommand_WithReset_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSystemCommand("RESET");
            AssertEqual("*RST", result);
        }

        public void BuildSystemCommand_WithStatus_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSystemCommand("STATUS");
            AssertEqual("*STB?", result);
        }

        public void BuildSystemCommand_WithIdentity_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSystemCommand("IDENTITY");
            AssertEqual("*IDN?", result);
        }

        public void BuildSystemCommand_WithClear_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSystemCommand("CLEAR");
            AssertEqual("*CLS", result);
        }

        public void BuildSystemCommand_WithError_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSystemCommand("ERROR");
            AssertEqual("SYST:ERR?", result);
        }

        public void BuildSystemCommand_WithUnknownCommand_ThrowsException()
        {
            try
            {
                _builder.BuildSystemCommand("UNKNOWN");
                throw new Exception("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        // Frequency Unit Conversion Tests
        public void ConvertFrequency_FromHzToKHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(1000, FrequencyUnit.Hz, FrequencyUnit.kHz);
            AssertEqual(1.0, result);
        }

        public void ConvertFrequency_FromKHzToMHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(1000, FrequencyUnit.kHz, FrequencyUnit.MHz);
            AssertEqual(1.0, result);
        }

        public void ConvertFrequency_FromMHzToGHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(1000, FrequencyUnit.MHz, FrequencyUnit.GHz);
            AssertEqual(1.0, result);
        }

        public void ConvertFrequency_FromGHzToHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(1, FrequencyUnit.GHz, FrequencyUnit.Hz);
            AssertEqual(1000000000, result);
        }

        public void ConvertFrequency_SameUnit_ReturnsOriginalValue()
        {
            var result = _builder.ConvertFrequency(2.4, FrequencyUnit.GHz, FrequencyUnit.GHz);
            AssertEqual(2.4, result);
        }

        // Power Unit Conversion Tests
        public void ConvertPower_FromWToMilliWatts_ReturnsCorrectValue()
        {
            var result = _builder.ConvertPower(1, PowerUnit.W, PowerUnit.mW);
            AssertEqual(1000.0, result);
        }

        public void ConvertPower_FromMilliWattsToWatts_ReturnsCorrectValue()
        {
            var result = _builder.ConvertPower(1000, PowerUnit.mW, PowerUnit.W);
            AssertEqual(1.0, result);
        }

        public void ConvertPower_FromWattsToDBm_ReturnsCorrectValue()
        {
            // 1W = 30 dBm
            var result = _builder.ConvertPower(1, PowerUnit.W, PowerUnit.dBm);
            AssertEqual(30.0, result, 0.01);
        }

        public void ConvertPower_FromDBmToWatts_ReturnsCorrectValue()
        {
            // 30 dBm = 1W
            var result = _builder.ConvertPower(30, PowerUnit.dBm, PowerUnit.W);
            AssertEqual(1.0, result, 0.01);
        }

        public void ConvertPower_SameUnit_ReturnsOriginalValue()
        {
            var result = _builder.ConvertPower(12.5, PowerUnit.dBm, PowerUnit.dBm);
            AssertEqual(12.5, result);
        }

        // Helper methods for assertions
        private void AssertEqual(string expected, string actual)
        {
            if (expected != actual)
                throw new Exception(string.Format("Expected '{0}' but got '{1}'", expected, actual));
        }

        private void AssertEqual(double expected, double actual)
        {
            AssertEqual(expected, actual, 0.0001);
        }

        private void AssertEqual(double expected, double actual, double tolerance)
        {
            if (Math.Abs(expected - actual) > tolerance)
                throw new Exception(string.Format("Expected {0} but got {1}", expected, actual));
        }
    }
}
