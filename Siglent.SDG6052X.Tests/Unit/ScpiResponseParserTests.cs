using NUnit.Framework;
using Siglent.SDG6052X.DeviceLibrary.Parsing;
using Siglent.SDG6052X.DeviceLibrary.Models;
using System;

namespace Siglent.SDG6052X.Tests.Unit
{
    [TestFixture]
    public class ScpiResponseParserTests
    {
        private ScpiResponseParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new ScpiResponseParser();
        }

        #region ParseBooleanResponse Tests

        [Test]
        public void ParseBooleanResponse_ON_ReturnsTrue()
        {
            // Act
            var result = _parser.ParseBooleanResponse("ON");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ParseBooleanResponse_OFF_ReturnsFalse()
        {
            // Act
            var result = _parser.ParseBooleanResponse("OFF");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ParseBooleanResponse_1_ReturnsTrue()
        {
            // Act
            var result = _parser.ParseBooleanResponse("1");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ParseBooleanResponse_0_ReturnsFalse()
        {
            // Act
            var result = _parser.ParseBooleanResponse("0");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ParseBooleanResponse_CaseInsensitive_Works()
        {
            // Act & Assert
            Assert.That(_parser.ParseBooleanResponse("on"), Is.True);
            Assert.That(_parser.ParseBooleanResponse("Off"), Is.False);
        }

        [Test]
        public void ParseBooleanResponse_InvalidValue_ThrowsFormatException()
        {
            // Act & Assert
            Assert.Throws<FormatException>(() => _parser.ParseBooleanResponse("INVALID"));
        }

        [Test]
        public void ParseBooleanResponse_NullOrEmpty_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.ParseBooleanResponse(null));
            Assert.Throws<ArgumentException>(() => _parser.ParseBooleanResponse(""));
        }

        #endregion

        #region ParseNumericResponse Tests

        [Test]
        public void ParseNumericResponse_PlainNumber_ParsesCorrectly()
        {
            // Act
            var result = _parser.ParseNumericResponse("123.45");

            // Assert
            Assert.That(result, Is.EqualTo(123.45).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_FrequencyInHz_ParsesCorrectly()
        {
            // Act
            var result = _parser.ParseNumericResponse("1000HZ");

            // Assert
            Assert.That(result, Is.EqualTo(1000).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_FrequencyInKHz_ParsesCorrectly()
        {
            // Act
            var result = _parser.ParseNumericResponse("10KHZ");

            // Assert
            Assert.That(result, Is.EqualTo(10000).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_FrequencyInMHz_ParsesCorrectly()
        {
            // Act
            var result = _parser.ParseNumericResponse("1MHZ");

            // Assert
            Assert.That(result, Is.EqualTo(1000000).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_VoltageInV_ParsesCorrectly()
        {
            // Act
            var result = _parser.ParseNumericResponse("5V");

            // Assert
            Assert.That(result, Is.EqualTo(5.0).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_VoltageInVPP_ParsesCorrectly()
        {
            // Act
            var result = _parser.ParseNumericResponse("5VPP");

            // Assert
            Assert.That(result, Is.EqualTo(5.0).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_VoltageInVRMS_ParsesCorrectly()
        {
            // Act
            var result = _parser.ParseNumericResponse("3.5VRMS");

            // Assert
            Assert.That(result, Is.EqualTo(3.5).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_dBm_ParsesCorrectly()
        {
            // Act
            var result = _parser.ParseNumericResponse("10DBM");

            // Assert
            Assert.That(result, Is.EqualTo(10.0).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_CaseInsensitive_Works()
        {
            // Act
            var result = _parser.ParseNumericResponse("10khz");

            // Assert
            Assert.That(result, Is.EqualTo(10000).Within(0.001));
        }

        [Test]
        public void ParseNumericResponse_InvalidValue_ThrowsFormatException()
        {
            // Act & Assert
            Assert.Throws<FormatException>(() => _parser.ParseNumericResponse("INVALID"));
        }

        #endregion

        #region ParseStringResponse Tests

        [Test]
        public void ParseStringResponse_PlainString_ReturnsString()
        {
            // Act
            var result = _parser.ParseStringResponse("TestString");

            // Assert
            Assert.That(result, Is.EqualTo("TestString"));
        }

        [Test]
        public void ParseStringResponse_QuotedString_RemovesQuotes()
        {
            // Act
            var result = _parser.ParseStringResponse("\"TestString\"");

            // Assert
            Assert.That(result, Is.EqualTo("TestString"));
        }

        [Test]
        public void ParseStringResponse_WithWhitespace_Trims()
        {
            // Act
            var result = _parser.ParseStringResponse("  TestString  ");

            // Assert
            Assert.That(result, Is.EqualTo("TestString"));
        }

        [Test]
        public void ParseStringResponse_Null_ReturnsNull()
        {
            // Act
            var result = _parser.ParseStringResponse(null);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region ParseWaveformState Tests

        [Test]
        public void ParseWaveformState_ValidSineResponse_ParsesCorrectly()
        {
            // Arrange
            var response = "C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0";

            // Act
            var state = _parser.ParseWaveformState(response);

            // Assert
            Assert.That(state.WaveformType, Is.EqualTo(WaveformType.Sine));
            Assert.That(state.Frequency, Is.EqualTo(1000).Within(0.001));
            Assert.That(state.Amplitude, Is.EqualTo(5.0).Within(0.001));
            Assert.That(state.Offset, Is.EqualTo(0.0).Within(0.001));
            Assert.That(state.Phase, Is.EqualTo(0.0).Within(0.001));
            Assert.That(state.Unit, Is.EqualTo(AmplitudeUnit.Vpp));
        }

        [Test]
        public void ParseWaveformState_SquareWithDuty_ParsesDutyCycle()
        {
            // Arrange
            var response = "C1:BSWV WVTP,SQUARE,FRQ,1KHZ,AMP,3.3VPP,OFST,0V,PHSE,0,DUTY,50";

            // Act
            var state = _parser.ParseWaveformState(response);

            // Assert
            Assert.That(state.WaveformType, Is.EqualTo(WaveformType.Square));
            Assert.That(state.DutyCycle, Is.EqualTo(50.0).Within(0.001));
        }

        [Test]
        public void ParseWaveformState_VrmsUnit_ParsesCorrectly()
        {
            // Arrange
            var response = "C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,3.5VRMS,OFST,0V,PHSE,0";

            // Act
            var state = _parser.ParseWaveformState(response);

            // Assert
            Assert.That(state.Unit, Is.EqualTo(AmplitudeUnit.Vrms));
            Assert.That(state.Amplitude, Is.EqualTo(3.5).Within(0.001));
        }

        [Test]
        public void ParseWaveformState_MalformedResponse_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.ParseWaveformState(""));
        }

        #endregion

        #region ParseModulationState Tests

        [Test]
        public void ParseModulationState_AMModulation_ParsesCorrectly()
        {
            // Arrange
            var response = "C1:MDWV STATE,ON,MDTP,AM,SRC,INT,DEPTH,50,FRQ,100HZ";

            // Act
            var state = _parser.ParseModulationState(response);

            // Assert
            Assert.That(state.Enabled, Is.True);
            Assert.That(state.Type, Is.EqualTo(ModulationType.AM));
            Assert.That(state.Source, Is.EqualTo(ModulationSource.Internal));
            Assert.That(state.Depth, Is.EqualTo(50.0).Within(0.001));
            Assert.That(state.Rate, Is.EqualTo(100.0).Within(0.001));
        }

        [Test]
        public void ParseModulationState_FMModulation_ParsesDeviation()
        {
            // Arrange
            var response = "C1:MDWV STATE,ON,MDTP,FM,SRC,EXT,DEVI,1KHZ,FRQ,100HZ";

            // Act
            var state = _parser.ParseModulationState(response);

            // Assert
            Assert.That(state.Type, Is.EqualTo(ModulationType.FM));
            Assert.That(state.Source, Is.EqualTo(ModulationSource.External));
            Assert.That(state.Deviation, Is.EqualTo(1000.0).Within(0.001));
        }

        #endregion

        #region ParseSweepState Tests

        [Test]
        public void ParseSweepState_LinearSweep_ParsesCorrectly()
        {
            // Arrange
            var response = "C1:SWV STATE,ON,SWTP,LINE,START,100HZ,STOP,10KHZ,TIME,1,DIR,UP,TRSR,INT";

            // Act
            var state = _parser.ParseSweepState(response);

            // Assert
            Assert.That(state.Enabled, Is.True);
            Assert.That(state.Type, Is.EqualTo(SweepType.Linear));
            Assert.That(state.StartFrequency, Is.EqualTo(100.0).Within(0.001));
            Assert.That(state.StopFrequency, Is.EqualTo(10000.0).Within(0.001));
            Assert.That(state.Time, Is.EqualTo(1.0).Within(0.001));
            Assert.That(state.Direction, Is.EqualTo(SweepDirection.Up));
            Assert.That(state.TriggerSource, Is.EqualTo(TriggerSource.Internal));
        }

        [Test]
        public void ParseSweepState_LogarithmicSweep_ParsesType()
        {
            // Arrange
            var response = "C1:SWV STATE,ON,SWTP,LOG,START,100HZ,STOP,10KHZ,TIME,1,DIR,UP,TRSR,INT";

            // Act
            var state = _parser.ParseSweepState(response);

            // Assert
            Assert.That(state.Type, Is.EqualTo(SweepType.Logarithmic));
        }

        #endregion

        #region ParseBurstState Tests

        [Test]
        public void ParseBurstState_NCycleBurst_ParsesCorrectly()
        {
            // Arrange
            var response = "C1:BTWV STATE,ON,MODE,NCYC,TRSR,INT,CYCLES,10,PRD,0.001,EDGE,RISE,STPS,0";

            // Act
            var state = _parser.ParseBurstState(response);

            // Assert
            Assert.That(state.Enabled, Is.True);
            Assert.That(state.Mode, Is.EqualTo(BurstMode.NCycle));
            Assert.That(state.TriggerSource, Is.EqualTo(TriggerSource.Internal));
            Assert.That(state.Cycles, Is.EqualTo(10));
            Assert.That(state.Period, Is.EqualTo(0.001).Within(0.0001));
            Assert.That(state.TriggerEdge, Is.EqualTo(TriggerEdge.Rising));
            Assert.That(state.StartPhase, Is.EqualTo(0.0).Within(0.001));
        }

        [Test]
        public void ParseBurstState_GatedBurst_ParsesMode()
        {
            // Arrange
            var response = "C1:BTWV STATE,ON,MODE,GATE,TRSR,EXT,CYCLES,5,STPS,0";

            // Act
            var state = _parser.ParseBurstState(response);

            // Assert
            Assert.That(state.Mode, Is.EqualTo(BurstMode.Gated));
            Assert.That(state.TriggerSource, Is.EqualTo(TriggerSource.External));
        }

        #endregion

        #region ParseIdentityResponse Tests

        [Test]
        public void ParseIdentityResponse_ValidResponse_ParsesCorrectly()
        {
            // Arrange
            var response = "Siglent Technologies,SDG6052X,SDG00000000001,1.01.01.32";

            // Act
            var identity = _parser.ParseIdentityResponse(response);

            // Assert
            Assert.That(identity.Manufacturer, Is.EqualTo("Siglent Technologies"));
            Assert.That(identity.Model, Is.EqualTo("SDG6052X"));
            Assert.That(identity.SerialNumber, Is.EqualTo("SDG00000000001"));
            Assert.That(identity.FirmwareVersion, Is.EqualTo("1.01.01.32"));
        }

        [Test]
        public void ParseIdentityResponse_MalformedResponse_ThrowsFormatException()
        {
            // Arrange
            var response = "Siglent Technologies,SDG6052X";

            // Act & Assert
            Assert.Throws<FormatException>(() => _parser.ParseIdentityResponse(response));
        }

        [Test]
        public void ParseIdentityResponse_EmptyResponse_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.ParseIdentityResponse(""));
        }

        #endregion

        #region ParseErrorResponse Tests

        [Test]
        public void ParseErrorResponse_NoError_ParsesCorrectly()
        {
            // Arrange
            var response = "0,No Error";

            // Act
            var error = _parser.ParseErrorResponse(response);

            // Assert
            Assert.That(error.Code, Is.EqualTo(0));
            Assert.That(error.Message, Is.EqualTo("No Error"));
        }

        [Test]
        public void ParseErrorResponse_CommandError_ParsesCorrectly()
        {
            // Arrange
            var response = "-100,Command error";

            // Act
            var error = _parser.ParseErrorResponse(response);

            // Assert
            Assert.That(error.Code, Is.EqualTo(-100));
            Assert.That(error.Message, Is.EqualTo("Command error"));
        }

        [Test]
        public void ParseErrorResponse_QuotedMessage_RemovesQuotes()
        {
            // Arrange
            var response = "-222,\"Data out of range\"";

            // Act
            var error = _parser.ParseErrorResponse(response);

            // Assert
            Assert.That(error.Code, Is.EqualTo(-222));
            Assert.That(error.Message, Is.EqualTo("Data out of range"));
        }

        [Test]
        public void ParseErrorResponse_MalformedResponse_ThrowsFormatException()
        {
            // Act & Assert
            Assert.Throws<FormatException>(() => _parser.ParseErrorResponse("InvalidFormat"));
        }

        #endregion

        #region ParseArbitraryWaveformData Tests

        [Test]
        public void ParseArbitraryWaveformData_ValidBinaryData_ParsesCorrectly()
        {
            // Arrange
            byte[] binaryData = new byte[]
            {
                0x00, 0x00,  // 0
                0x00, 0x40,  // 16384 (0.5)
                0x00, 0x80,  // -32768 (-1.0)
                0xFF, 0x7F   // 32767 (0.999...)
            };

            // Act
            var points = _parser.ParseArbitraryWaveformData(binaryData);

            // Assert
            Assert.That(points.Length, Is.EqualTo(4));
            Assert.That(points[0], Is.EqualTo(0.0).Within(0.001));
            Assert.That(points[1], Is.EqualTo(0.5).Within(0.001));
            Assert.That(points[2], Is.EqualTo(-1.0).Within(0.001));
            Assert.That(points[3], Is.EqualTo(0.999).Within(0.001));
        }

        [Test]
        public void ParseArbitraryWaveformData_NullData_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.ParseArbitraryWaveformData(null));
        }

        [Test]
        public void ParseArbitraryWaveformData_EmptyData_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.ParseArbitraryWaveformData(new byte[0]));
        }

        #endregion
    }
}
