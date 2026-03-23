using NUnit.Framework;
using System;
using System.Collections.Generic;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Parsing;

namespace Tegam._1830A.Tests.Unit
{
    [TestFixture]
    public class ScpiResponseParserTests
    {
        private ScpiResponseParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new ScpiResponseParser();
        }

        #region Boolean Response Tests

        [Test]
        public void ParseBooleanResponse_WithOne_ReturnsTrue()
        {
            var result = _parser.ParseBooleanResponse("1");
            Assert.That(result, Is.True);
        }

        [Test]
        public void ParseBooleanResponse_WithZero_ReturnsFalse()
        {
            var result = _parser.ParseBooleanResponse("0");
            Assert.That(result, Is.False);
        }

        [Test]
        public void ParseBooleanResponse_WithWhitespace_ReturnsCorrectValue()
        {
            var result = _parser.ParseBooleanResponse("  1  ");
            Assert.That(result, Is.True);
        }

        #endregion

        #region Numeric Response Tests

        [Test]
        public void ParseNumericResponse_WithInteger_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("42");
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void ParseNumericResponse_WithDecimal_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("3.14");
            Assert.That(result, Is.EqualTo(3.14));
        }

        [Test]
        public void ParseNumericResponse_WithNegativeValue_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("-10.5");
            Assert.That(result, Is.EqualTo(-10.5));
        }

        [Test]
        public void ParseNumericResponse_WithScientificNotation_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("1.23E+3");
            Assert.That(result, Is.EqualTo(1230));
        }

        [Test]
        public void ParseNumericResponse_WithWhitespace_ReturnsCorrectValue()
        {
            var result = _parser.ParseNumericResponse("  42.5  ");
            Assert.That(result, Is.EqualTo(42.5));
        }

        #endregion

        #region String Response Tests

        [Test]
        public void ParseStringResponse_WithQuotedString_ReturnsUnquotedString()
        {
            var result = _parser.ParseStringResponse("\"Hello World\"");
            Assert.That(result, Is.EqualTo("Hello World"));
        }

        [Test]
        public void ParseStringResponse_WithUnquotedString_ReturnsString()
        {
            var result = _parser.ParseStringResponse("HelloWorld");
            Assert.That(result, Is.EqualTo("HelloWorld"));
        }

        [Test]
        public void ParseStringResponse_WithWhitespace_ReturnsTrimmmedString()
        {
            var result = _parser.ParseStringResponse("  \"Test String\"  ");
            Assert.That(result, Is.EqualTo("Test String"));
        }

        #endregion

        #region Power Measurement Tests

        [Test]
        public void ParsePowerMeasurement_WithValidResponse_ReturnsPowerMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("+12.34 dBm");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PowerValue, Is.EqualTo(12.34));
            Assert.That(result.PowerUnit, Is.EqualTo(PowerUnit.dBm));
        }

        [Test]
        public void ParsePowerMeasurement_WithWatts_ReturnsPowerMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("0.5 W");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PowerValue, Is.EqualTo(0.5));
            Assert.That(result.PowerUnit, Is.EqualTo(PowerUnit.W));
        }

        [Test]
        public void ParsePowerMeasurement_WithMilliWatts_ReturnsPowerMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("500 mW");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PowerValue, Is.EqualTo(500));
            Assert.That(result.PowerUnit, Is.EqualTo(PowerUnit.mW));
        }

        [Test]
        public void ParsePowerMeasurement_WithNegativeValue_ReturnsPowerMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("-5.5 dBm");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PowerValue, Is.EqualTo(-5.5));
            Assert.That(result.PowerUnit, Is.EqualTo(PowerUnit.dBm));
        }

        [Test]
        public void ParsePowerMeasurement_WithTimestamp_IncludesTimestamp()
        {
            var result = _parser.ParsePowerMeasurement("+12.34 dBm");
            Assert.That(result.Timestamp, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void ParsePowerMeasurement_WithMalformedResponse_ThrowsException()
        {
            Assert.Throws<FormatException>(() => _parser.ParsePowerMeasurement("invalid"));
        }

        [Test]
        public void ParsePowerMeasurement_WithMissingUnit_ThrowsException()
        {
            Assert.Throws<FormatException>(() => _parser.ParsePowerMeasurement("12.34"));
        }

        #endregion

        #region Frequency Response Tests

        [Test]
        public void ParseFrequencyResponse_WithHz_ReturnsFrequencyResponse()
        {
            var result = _parser.ParseFrequencyResponse("1000 Hz");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Frequency, Is.EqualTo(1000));
            Assert.That(result.Unit, Is.EqualTo(FrequencyUnit.Hz));
        }

        [Test]
        public void ParseFrequencyResponse_WithkHz_ReturnsFrequencyResponse()
        {
            var result = _parser.ParseFrequencyResponse("100 kHz");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Frequency, Is.EqualTo(100));
            Assert.That(result.Unit, Is.EqualTo(FrequencyUnit.kHz));
        }

        [Test]
        public void ParseFrequencyResponse_WithMHz_ReturnsFrequencyResponse()
        {
            var result = _parser.ParseFrequencyResponse("2.4 MHz");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Frequency, Is.EqualTo(2.4));
            Assert.That(result.Unit, Is.EqualTo(FrequencyUnit.MHz));
        }

        [Test]
        public void ParseFrequencyResponse_WithGHz_ReturnsFrequencyResponse()
        {
            var result = _parser.ParseFrequencyResponse("5.8 GHz");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Frequency, Is.EqualTo(5.8));
            Assert.That(result.Unit, Is.EqualTo(FrequencyUnit.GHz));
        }

        [Test]
        public void ParseFrequencyResponse_WithMalformedResponse_ThrowsException()
        {
            Assert.Throws<FormatException>(() => _parser.ParseFrequencyResponse("invalid"));
        }

        #endregion

        #region Sensor Info Tests

        [Test]
        public void ParseSensorInfo_WithValidResponse_ReturnsSensorInfo()
        {
            var result = _parser.ParseSensorInfo("1,Sensor1,100kHz,40GHz,-30dBm,30dBm");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SensorId, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Sensor1"));
        }

        [Test]
        public void ParseSensorInfo_WithDifferentSensorId_ReturnsSensorInfo()
        {
            var result = _parser.ParseSensorInfo("3,Sensor3,100kHz,40GHz,-30dBm,30dBm");
            Assert.That(result.SensorId, Is.EqualTo(3));
        }

        [Test]
        public void ParseSensorInfo_WithMalformedResponse_ThrowsException()
        {
            Assert.Throws<FormatException>(() => _parser.ParseSensorInfo("invalid"));
        }

        #endregion

        #region Available Sensors Tests

        [Test]
        public void ParseAvailableSensors_WithMultipleSensors_ReturnsList()
        {
            var response = "1,Sensor1,100kHz,40GHz,-30dBm,30dBm;2,Sensor2,100kHz,40GHz,-30dBm,30dBm";
            var result = _parser.ParseAvailableSensors(response);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void ParseAvailableSensors_WithSingleSensor_ReturnsList()
        {
            var response = "1,Sensor1,100kHz,40GHz,-30dBm,30dBm";
            var result = _parser.ParseAvailableSensors(response);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void ParseAvailableSensors_WithEmptyResponse_ReturnsEmptyList()
        {
            var result = _parser.ParseAvailableSensors("");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        #endregion

        #region Calibration Status Tests

        [Test]
        public void ParseCalibrationStatus_WithCalibrating_ReturnsStatus()
        {
            var result = _parser.ParseCalibrationStatus("1,0,0,");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsCalibrating, Is.True);
            Assert.That(result.IsComplete, Is.False);
        }

        [Test]
        public void ParseCalibrationStatus_WithComplete_ReturnsStatus()
        {
            var result = _parser.ParseCalibrationStatus("0,1,1,");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsCalibrating, Is.False);
            Assert.That(result.IsComplete, Is.True);
            Assert.That(result.IsSuccessful, Is.True);
        }

        [Test]
        public void ParseCalibrationStatus_WithError_ReturnsStatus()
        {
            var result = _parser.ParseCalibrationStatus("0,1,0,Calibration failed");
            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Calibration failed"));
        }

        [Test]
        public void ParseCalibrationStatus_WithMalformedResponse_ThrowsException()
        {
            Assert.Throws<FormatException>(() => _parser.ParseCalibrationStatus("invalid"));
        }

        #endregion

        #region Device Identity Tests

        [Test]
        public void ParseIdentityResponse_WithValidResponse_ReturnsDeviceIdentity()
        {
            var result = _parser.ParseIdentityResponse("Tegam,1830A,SN12345,1.0");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Manufacturer, Is.EqualTo("Tegam"));
            Assert.That(result.Model, Is.EqualTo("1830A"));
            Assert.That(result.SerialNumber, Is.EqualTo("SN12345"));
            Assert.That(result.FirmwareVersion, Is.EqualTo("1.0"));
        }

        [Test]
        public void ParseIdentityResponse_WithMalformedResponse_ThrowsException()
        {
            Assert.Throws<FormatException>(() => _parser.ParseIdentityResponse("invalid"));
        }

        #endregion

        #region System Status Tests

        [Test]
        public void ParseSystemStatus_WithValidResponse_ReturnsSystemStatus()
        {
            var result = _parser.ParseSystemStatus("1,25.5,0");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsReady, Is.True);
            Assert.That(result.Temperature, Is.EqualTo(25.5));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
        }

        [Test]
        public void ParseSystemStatus_WithErrors_ReturnsSystemStatus()
        {
            var result = _parser.ParseSystemStatus("1,30.2,3");
            Assert.That(result.IsReady, Is.True);
            Assert.That(result.ErrorCount, Is.EqualTo(3));
        }

        [Test]
        public void ParseSystemStatus_WithMalformedResponse_ThrowsException()
        {
            Assert.Throws<FormatException>(() => _parser.ParseSystemStatus("invalid"));
        }

        #endregion

        #region Error Response Tests

        [Test]
        public void ParseErrorResponse_WithValidResponse_ReturnsDeviceError()
        {
            var result = _parser.ParseErrorResponse("100,Invalid frequency");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.EqualTo(100));
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid frequency"));
        }

        [Test]
        public void ParseErrorResponse_WithDifferentErrorCode_ReturnsDeviceError()
        {
            var result = _parser.ParseErrorResponse("200,Sensor not found");
            Assert.That(result.ErrorCode, Is.EqualTo(200));
            Assert.That(result.ErrorMessage, Is.EqualTo("Sensor not found"));
        }

        [Test]
        public void ParseErrorResponse_WithMalformedResponse_ThrowsException()
        {
            Assert.Throws<FormatException>(() => _parser.ParseErrorResponse("invalid"));
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ParsePowerMeasurement_WithVerySmallValue_ReturnsPowerMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("0.001 W");
            Assert.That(result.PowerValue, Is.EqualTo(0.001));
        }

        [Test]
        public void ParsePowerMeasurement_WithVeryLargeValue_ReturnsPowerMeasurement()
        {
            var result = _parser.ParsePowerMeasurement("100 W");
            Assert.That(result.PowerValue, Is.EqualTo(100));
        }

        [Test]
        public void ParseFrequencyResponse_WithVerySmallFrequency_ReturnsFrequencyResponse()
        {
            var result = _parser.ParseFrequencyResponse("0.1 Hz");
            Assert.That(result.Frequency, Is.EqualTo(0.1));
        }

        [Test]
        public void ParseFrequencyResponse_WithVeryLargeFrequency_ReturnsFrequencyResponse()
        {
            var result = _parser.ParseFrequencyResponse("40 GHz");
            Assert.That(result.Frequency, Is.EqualTo(40));
        }

        [Test]
        public void ParseNumericResponse_WithZero_ReturnsZero()
        {
            var result = _parser.ParseNumericResponse("0");
            Assert.That(result, Is.EqualTo(0));
        }

        #endregion
    }
}
