using System;
using System.Collections.Generic;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.DeviceLibrary.Parsing.Tests
{
    /// <summary>
    /// Unit tests for ScpiResponseParser class.
    /// These tests verify that SCPI responses are parsed correctly into strongly-typed objects.
    /// </summary>
    public class ScpiResponseParserTests
    {
        private ScpiResponseParser _parser;

        public void Setup()
        {
            _parser = new ScpiResponseParser();
        }

        // Boolean Response Tests
        public void ParseBooleanResponse_WithOne_ReturnsTrue()
        {
            var result = _parser.ParseBooleanResponse("1");
            AssertTrue(result);
        }

        public void ParseBooleanResponse_WithZero_ReturnsFalse()
        {
            var result = _parser.ParseBooleanResponse("0");
            AssertFalse(result);
        }

        public void ParseBooleanResponse_WithON_ReturnsTrue()
        {
            var result = _parser.ParseBooleanResponse("ON");
            AssertTrue(result);
        }

        public void ParseBooleanResponse_WithOFF_ReturnsFalse()
        {
            var result = _parser.ParseBooleanResponse("OFF");
            AssertFalse(result);
        }

        public void ParseBooleanResponse_WithTRUE_ReturnsTrue()
        {
            var result = _parser.ParseBooleanResponse("TRUE");
            AssertTrue(result);
        }

        public void ParseBooleanResponse_WithFALSE_ReturnsFalse()
        {
            var result = _parser.ParseBooleanResponse("FALSE");
            AssertFalse(result);
        }

        public void ParseBooleanResponse_WithInvalidValue_ThrowsException()
        {
            try
            {
                _parser.ParseBooleanResponse("INVALID");
                throw new Exception("Expected FormatException");
            }
            catch (FormatException)
            {
                // Expected
            }
        }

        public void ParseBooleanResponse_WithNull_ThrowsException()
        {
            try
            {
                _parser.ParseBooleanResponse(null);
                throw new Exception("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        // Numeric Response Tests
        public void ParseNumericResponse_WithInteger_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("42");
            AssertEqual(42.0, result);
        }

        public void ParseNumericResponse_WithDecimal_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("12.34");
            AssertEqual(12.34, result);
        }

        public void ParseNumericResponse_WithNegativeValue_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("-50.5");
            AssertEqual(-50.5, result);
        }

        public void ParseNumericResponse_WithScientificNotation_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("1.23e-4");
            AssertEqual(0.000123, result, 0.0000001);
        }

        public void ParseNumericResponse_WithInvalidValue_ThrowsException()
        {
            try
            {
                _parser.ParseNumericResponse("NOT_A_NUMBER");
                throw new Exception("Expected FormatException");
            }
            catch (FormatException)
            {
                // Expected
            }
        }

        // String Response Tests
        public void ParseStringResponse_WithPlainText_ReturnsText()
        {
            var result = _parser.ParseStringResponse("Hello");
            AssertEqual("Hello", result);
        }

        public void ParseStringResponse_WithQuotedText_RemovesQuotes()
        {
            var result = _parser.ParseStringResponse("\"Hello World\"");
            AssertEqual("Hello World", result);
        }

        public void ParseStringResponse_WithWhitespace_TrimsWhitespace()
        {
            var result = _parser.ParseStringResponse("  Hello  ");
            AssertEqual("Hello", result);
        }

        // Power Measurement Tests
        public void ParsePowerMeasurement_WithDBm_ReturnsCorrectMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("+12.34 dBm");
            AssertEqual(12.34, result.PowerValue);
            AssertEqual(PowerUnit.dBm, result.PowerUnit);
        }

        public void ParsePowerMeasurement_WithWatts_ReturnsCorrectMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("0.025 W");
            AssertEqual(0.025, result.PowerValue);
            AssertEqual(PowerUnit.W, result.PowerUnit);
        }

        public void ParsePowerMeasurement_WithMilliWatts_ReturnsCorrectMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("25 mW");
            AssertEqual(25.0, result.PowerValue);
            AssertEqual(PowerUnit.mW, result.PowerUnit);
        }

        public void ParsePowerMeasurement_WithNegativeDBm_ReturnsCorrectMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("-50.5 dBm");
            AssertEqual(-50.5, result.PowerValue);
            AssertEqual(PowerUnit.dBm, result.PowerUnit);
        }

        public void ParsePowerMeasurement_WithInvalidFormat_ThrowsException()
        {
            try
            {
                _parser.ParsePowerMeasurement("INVALID");
                throw new Exception("Expected FormatException");
            }
            catch (FormatException)
            {
                // Expected
            }
        }

        // Frequency Response Tests
        public void ParseFrequencyResponse_WithGHz_ReturnsCorrectResponse()
        {
            var result = _parser.ParseFrequencyResponse("2.4 GHZ");
            AssertEqual(2.4, result.Frequency);
            AssertEqual(FrequencyUnit.GHz, result.Unit);
        }

        public void ParseFrequencyResponse_WithMHz_ReturnsCorrectResponse()
        {
            var result = _parser.ParseFrequencyResponse("2400 MHZ");
            AssertEqual(2400.0, result.Frequency);
            AssertEqual(FrequencyUnit.MHz, result.Unit);
        }

        public void ParseFrequencyResponse_WithKHz_ReturnsCorrectResponse()
        {
            var result = _parser.ParseFrequencyResponse("2400000 KHZ");
            AssertEqual(2400000.0, result.Frequency);
            AssertEqual(FrequencyUnit.kHz, result.Unit);
        }

        public void ParseFrequencyResponse_WithHz_ReturnsCorrectResponse()
        {
            var result = _parser.ParseFrequencyResponse("2400000000 HZ");
            AssertEqual(2400000000.0, result.Frequency);
            AssertEqual(FrequencyUnit.Hz, result.Unit);
        }

        public void ParseFrequencyResponse_WithInvalidFormat_ThrowsException()
        {
            try
            {
                _parser.ParseFrequencyResponse("INVALID");
                throw new Exception("Expected FormatException");
            }
            catch (FormatException)
            {
                // Expected
            }
        }

        // Sensor Info Tests
        public void ParseSensorInfo_WithValidResponse_ReturnsCorrectSensorInfo()
        {
            var result = _parser.ParseSensorInfo("1,Sensor1,100KHZ,40GHZ,-50DBM,+20DBM");
            AssertEqual(1, result.SensorId);
            AssertEqual("Sensor1", result.Name);
            AssertEqual(100000, result.MinFrequency); // 100 kHz in Hz
            AssertEqual(40000000000, result.MaxFrequency); // 40 GHz in Hz
        }

        public void ParseSensorInfo_WithMultipleSensors_ParsesCorrectly()
        {
            var result = _parser.ParseSensorInfo("2,Sensor2,1MHZ,20GHZ,-40DBM,+30DBM");
            AssertEqual(2, result.SensorId);
            AssertEqual("Sensor2", result.Name);
        }

        public void ParseSensorInfo_WithInvalidFormat_ThrowsException()
        {
            try
            {
                _parser.ParseSensorInfo("INVALID");
                throw new Exception("Expected FormatException");
            }
            catch (FormatException)
            {
                // Expected
            }
        }

        // Available Sensors Tests
        public void ParseAvailableSensors_WithSingleSensor_ReturnsListWithOneSensor()
        {
            var result = _parser.ParseAvailableSensors("1,Sensor1,100KHZ,40GHZ,-50DBM,+20DBM");
            AssertEqual(1, result.Count);
            AssertEqual(1, result[0].SensorId);
        }

        public void ParseAvailableSensors_WithMultipleSensors_ReturnsListWithAllSensors()
        {
            var response = "1,Sensor1,100KHZ,40GHZ,-50DBM,+20DBM;2,Sensor2,1MHZ,20GHZ,-40DBM,+30DBM;3,Sensor3,10MHZ,10GHZ,-30DBM,+40DBM";
            var result = _parser.ParseAvailableSensors(response);
            AssertEqual(3, result.Count);
            AssertEqual(1, result[0].SensorId);
            AssertEqual(2, result[1].SensorId);
            AssertEqual(3, result[2].SensorId);
        }

        // Calibration Status Tests
        public void ParseCalibrationStatus_WithNotCalibrating_ReturnsCorrectStatus()
        {
            var result = _parser.ParseCalibrationStatus("0,1,1,OK");
            AssertFalse(result.IsCalibrating);
            AssertTrue(result.IsComplete);
            AssertTrue(result.IsSuccessful);
            AssertEqual("OK", result.ErrorMessage);
        }

        public void ParseCalibrationStatus_WithCalibrating_ReturnsCorrectStatus()
        {
            var result = _parser.ParseCalibrationStatus("1,0,0,");
            AssertTrue(result.IsCalibrating);
            AssertFalse(result.IsComplete);
            AssertFalse(result.IsSuccessful);
        }

        public void ParseCalibrationStatus_WithFailure_ReturnsCorrectStatus()
        {
            var result = _parser.ParseCalibrationStatus("0,1,0,Calibration failed");
            AssertFalse(result.IsCalibrating);
            AssertTrue(result.IsComplete);
            AssertFalse(result.IsSuccessful);
            AssertEqual("Calibration failed", result.ErrorMessage);
        }

        // Device Identity Tests
        public void ParseIdentityResponse_WithValidResponse_ReturnsCorrectIdentity()
        {
            var result = _parser.ParseIdentityResponse("Tegam,1830A,SN12345,1.0.0");
            AssertEqual("Tegam", result.Manufacturer);
            AssertEqual("1830A", result.Model);
            AssertEqual("SN12345", result.SerialNumber);
            AssertEqual("1.0.0", result.FirmwareVersion);
        }

        public void ParseIdentityResponse_WithDifferentValues_ReturnsCorrectIdentity()
        {
            var result = _parser.ParseIdentityResponse("Tegam,1830A,SN99999,2.1.5");
            AssertEqual("Tegam", result.Manufacturer);
            AssertEqual("1830A", result.Model);
            AssertEqual("SN99999", result.SerialNumber);
            AssertEqual("2.1.5", result.FirmwareVersion);
        }

        // System Status Tests
        public void ParseSystemStatus_WithValidResponse_ReturnsCorrectStatus()
        {
            var result = _parser.ParseSystemStatus("1,25.5,0");
            AssertTrue(result.IsReady);
            AssertEqual(25.5, result.Temperature);
            AssertEqual(0, result.ErrorCount);
        }

        public void ParseSystemStatus_WithErrors_ReturnsCorrectStatus()
        {
            var result = _parser.ParseSystemStatus("0,35.2,3");
            AssertFalse(result.IsReady);
            AssertEqual(35.2, result.Temperature);
            AssertEqual(3, result.ErrorCount);
        }

        public void ParseSystemStatus_WithHighTemperature_ReturnsCorrectStatus()
        {
            var result = _parser.ParseSystemStatus("1,45.0,0");
            AssertTrue(result.IsReady);
            AssertEqual(45.0, result.Temperature);
        }

        // Error Response Tests
        public void ParseErrorResponse_WithCodeAndMessage_ReturnsCorrectError()
        {
            var result = _parser.ParseErrorResponse("100,Device not ready");
            AssertEqual(100, result.ErrorCode);
            AssertEqual("Device not ready", result.ErrorMessage);
        }

        public void ParseErrorResponse_WithCodeOnly_ReturnsCorrectError()
        {
            var result = _parser.ParseErrorResponse("200");
            AssertEqual(200, result.ErrorCode);
            AssertEqual("", result.ErrorMessage);
        }

        public void ParseErrorResponse_WithDifferentCodes_ReturnsCorrectError()
        {
            var result = _parser.ParseErrorResponse("404,Not found");
            AssertEqual(404, result.ErrorCode);
            AssertEqual("Not found", result.ErrorMessage);
        }

        public void ParseErrorResponse_WithInvalidCode_ThrowsException()
        {
            try
            {
                _parser.ParseErrorResponse("NOT_A_CODE,Error message");
                throw new Exception("Expected FormatException");
            }
            catch (FormatException)
            {
                // Expected
            }
        }

        // Helper methods for assertions
        private void AssertTrue(bool value)
        {
            if (!value)
                throw new Exception("Expected true but got false");
        }

        private void AssertFalse(bool value)
        {
            if (value)
                throw new Exception("Expected false but got true");
        }

        private void AssertEqual(string expected, string actual)
        {
            if (expected != actual)
                throw new Exception(string.Format("Expected '{0}' but got '{1}'", expected, actual));
        }

        private void AssertEqual(int expected, int actual)
        {
            if (expected != actual)
                throw new Exception(string.Format("Expected {0} but got {1}", expected, actual));
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

        private void AssertEqual<T>(T expected, T actual) where T : class
        {
            if (!expected.Equals(actual))
                throw new Exception(string.Format("Expected {0} but got {1}", expected, actual));
        }
    }
}
