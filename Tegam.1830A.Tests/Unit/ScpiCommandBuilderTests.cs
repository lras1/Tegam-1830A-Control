using NUnit.Framework;
using Tegam._1830A.DeviceLibrary.Commands;
using Tegam._1830A.DeviceLibrary.Models;

namespace Tegam._1830A.Tests.Unit
{
    [TestFixture]
    public class ScpiCommandBuilderTests
    {
        private ScpiCommandBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new ScpiCommandBuilder();
        }

        #region Frequency Command Tests

        [Test]
        public void BuildFrequencyCommand_WithHz_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyCommand(1000, FrequencyUnit.Hz);
            Assert.That(result, Is.EqualTo("FREQ 1000 Hz"));
        }

        [Test]
        public void BuildFrequencyCommand_WithkHz_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyCommand(100, FrequencyUnit.kHz);
            Assert.That(result, Is.EqualTo("FREQ 100 kHz"));
        }

        [Test]
        public void BuildFrequencyCommand_WithMHz_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyCommand(2.4, FrequencyUnit.MHz);
            Assert.That(result, Is.EqualTo("FREQ 2.4 MHz"));
        }

        [Test]
        public void BuildFrequencyCommand_WithGHz_ReturnsCorrectCommand()
        {
            var result = _builder.BuildFrequencyCommand(5.8, FrequencyUnit.GHz);
            Assert.That(result, Is.EqualTo("FREQ 5.8 GHz"));
        }

        [Test]
        public void BuildFrequencyCommand_WithZeroFrequency_ReturnsCommand()
        {
            var result = _builder.BuildFrequencyCommand(0, FrequencyUnit.MHz);
            Assert.That(result, Is.EqualTo("FREQ 0 MHz"));
        }

        [Test]
        public void BuildFrequencyCommand_WithLargeFrequency_ReturnsCommand()
        {
            var result = _builder.BuildFrequencyCommand(40, FrequencyUnit.GHz);
            Assert.That(result, Is.EqualTo("FREQ 40 GHz"));
        }

        [Test]
        public void BuildFrequencyQueryCommand_ReturnsCorrectQuery()
        {
            var result = _builder.BuildFrequencyQueryCommand();
            Assert.That(result, Is.EqualTo("FREQ?"));
        }

        #endregion

        #region Power Measurement Command Tests

        [Test]
        public void BuildMeasurePowerCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildMeasurePowerCommand();
            Assert.That(result, Is.EqualTo("MEAS:POW"));
        }

        [Test]
        public void BuildMeasurePowerQueryCommand_ReturnsCorrectQuery()
        {
            var result = _builder.BuildMeasurePowerQueryCommand();
            Assert.That(result, Is.EqualTo("MEAS:POW?"));
        }

        #endregion

        #region Sensor Command Tests

        [Test]
        public void BuildSelectSensorCommand_WithValidSensorId_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSelectSensorCommand(1);
            Assert.That(result, Is.EqualTo("SENS:SEL 1"));
        }

        [Test]
        public void BuildSelectSensorCommand_WithSensorId2_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSelectSensorCommand(2);
            Assert.That(result, Is.EqualTo("SENS:SEL 2"));
        }

        [Test]
        public void BuildSelectSensorCommand_WithSensorId4_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSelectSensorCommand(4);
            Assert.That(result, Is.EqualTo("SENS:SEL 4"));
        }

        [Test]
        public void BuildQuerySensorCommand_ReturnsCorrectQuery()
        {
            var result = _builder.BuildQuerySensorCommand();
            Assert.That(result, Is.EqualTo("SENS:SEL?"));
        }

        [Test]
        public void BuildQueryAvailableSensorsCommand_ReturnsCorrectQuery()
        {
            var result = _builder.BuildQueryAvailableSensorsCommand();
            Assert.That(result, Is.EqualTo("SENS:LIST?"));
        }

        #endregion

        #region Calibration Command Tests

        [Test]
        public void BuildCalibrateCommand_WithInternal_ReturnsCorrectCommand()
        {
            var result = _builder.BuildCalibrateCommand(CalibrationMode.Internal);
            Assert.That(result, Is.EqualTo("CAL:INT"));
        }

        [Test]
        public void BuildCalibrateCommand_WithExternal_ReturnsCorrectCommand()
        {
            var result = _builder.BuildCalibrateCommand(CalibrationMode.External);
            Assert.That(result, Is.EqualTo("CAL:EXT"));
        }

        [Test]
        public void BuildQueryCalibrationStatusCommand_ReturnsCorrectQuery()
        {
            var result = _builder.BuildQueryCalibrationStatusCommand();
            Assert.That(result, Is.EqualTo("CAL:STAT?"));
        }

        #endregion

        #region Logging Command Tests

        [Test]
        public void BuildStartLoggingCommand_WithFilename_ReturnsCorrectCommand()
        {
            var result = _builder.BuildStartLoggingCommand("measurements.csv");
            Assert.That(result, Is.EqualTo("LOG:START \"measurements.csv\""));
        }

        [Test]
        public void BuildStartLoggingCommand_WithPathFilename_ReturnsCorrectCommand()
        {
            var result = _builder.BuildStartLoggingCommand("C:\\Data\\measurements.csv");
            Assert.That(result, Is.EqualTo("LOG:START \"C:\\Data\\measurements.csv\""));
        }

        [Test]
        public void BuildStopLoggingCommand_ReturnsCorrectCommand()
        {
            var result = _builder.BuildStopLoggingCommand();
            Assert.That(result, Is.EqualTo("LOG:STOP"));
        }

        [Test]
        public void BuildQueryLoggingStatusCommand_ReturnsCorrectQuery()
        {
            var result = _builder.BuildQueryLoggingStatusCommand();
            Assert.That(result, Is.EqualTo("LOG:STAT?"));
        }

        #endregion

        #region System Command Tests

        [Test]
        public void BuildSystemCommand_WithReset_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSystemCommand("RESET");
            Assert.That(result, Is.EqualTo("*RST"));
        }

        [Test]
        public void BuildSystemCommand_WithIdentity_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSystemCommand("IDENTITY");
            Assert.That(result, Is.EqualTo("*IDN?"));
        }

        [Test]
        public void BuildSystemCommand_WithStatus_ReturnsCorrectCommand()
        {
            var result = _builder.BuildSystemCommand("STATUS");
            Assert.That(result, Is.EqualTo("SYST:STAT?"));
        }

        #endregion

        #region Frequency Unit Conversion Tests

        [Test]
        public void ConvertFrequency_FromHzToKHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(1000, FrequencyUnit.Hz, FrequencyUnit.kHz);
            Assert.That(result, Is.EqualTo(1.0));
        }

        [Test]
        public void ConvertFrequency_FromKHzToMHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(1000, FrequencyUnit.kHz, FrequencyUnit.MHz);
            Assert.That(result, Is.EqualTo(1.0));
        }

        [Test]
        public void ConvertFrequency_FromMHzToGHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(1000, FrequencyUnit.MHz, FrequencyUnit.GHz);
            Assert.That(result, Is.EqualTo(1.0));
        }

        [Test]
        public void ConvertFrequency_FromGHzToHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(1, FrequencyUnit.GHz, FrequencyUnit.Hz);
            Assert.That(result, Is.EqualTo(1_000_000_000));
        }

        [Test]
        public void ConvertFrequency_FromHzToHz_ReturnsSameValue()
        {
            var result = _builder.ConvertFrequency(2400, FrequencyUnit.Hz, FrequencyUnit.Hz);
            Assert.That(result, Is.EqualTo(2400));
        }

        [Test]
        public void ConvertFrequency_2_4GHzToMHz_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(2.4, FrequencyUnit.GHz, FrequencyUnit.MHz);
            Assert.That(result, Is.EqualTo(2400));
        }

        #endregion

        #region Power Unit Conversion Tests

        [Test]
        public void ConvertPower_FromWattsToMilliWatts_ReturnsCorrectValue()
        {
            var result = _builder.ConvertPower(1, PowerUnit.W, PowerUnit.mW);
            Assert.That(result, Is.EqualTo(1000));
        }

        [Test]
        public void ConvertPower_FromMilliWattsTodBm_ReturnsCorrectValue()
        {
            var result = _builder.ConvertPower(1, PowerUnit.mW, PowerUnit.dBm);
            Assert.That(result, Is.GreaterThan(0).And.LessThan(1));
        }

        [Test]
        public void ConvertPower_FromdBmToWatts_ReturnsCorrectValue()
        {
            var result = _builder.ConvertPower(0, PowerUnit.dBm, PowerUnit.W);
            Assert.That(result, Is.EqualTo(0.001).Within(0.0001));
        }

        [Test]
        public void ConvertPower_FromWattsToWatts_ReturnsSameValue()
        {
            var result = _builder.ConvertPower(0.5, PowerUnit.W, PowerUnit.W);
            Assert.That(result, Is.EqualTo(0.5));
        }

        [Test]
        public void ConvertPower_From10dBmToWatts_ReturnsCorrectValue()
        {
            var result = _builder.ConvertPower(10, PowerUnit.dBm, PowerUnit.W);
            Assert.That(result, Is.EqualTo(0.01).Within(0.0001));
        }

        [Test]
        public void ConvertPower_From30dBmToWatts_ReturnsCorrectValue()
        {
            var result = _builder.ConvertPower(30, PowerUnit.dBm, PowerUnit.W);
            Assert.That(result, Is.EqualTo(1.0).Within(0.01));
        }

        #endregion

        #region Edge Cases

        [Test]
        public void BuildFrequencyCommand_WithDecimalFrequency_ReturnsCommand()
        {
            var result = _builder.BuildFrequencyCommand(2.45, FrequencyUnit.GHz);
            Assert.That(result, Is.EqualTo("FREQ 2.45 GHz"));
        }

        [Test]
        public void BuildStartLoggingCommand_WithSpecialCharactersInPath_ReturnsCommand()
        {
            var result = _builder.BuildStartLoggingCommand("C:\\Data\\test_2024-01-15.csv");
            Assert.That(result, Is.EqualTo("LOG:START \"C:\\Data\\test_2024-01-15.csv\""));
        }

        [Test]
        public void ConvertFrequency_WithSmallValue_ReturnsCorrectValue()
        {
            var result = _builder.ConvertFrequency(0.001, FrequencyUnit.GHz, FrequencyUnit.MHz);
            Assert.That(result, Is.EqualTo(1.0));
        }

        [Test]
        public void ConvertPower_WithNegativedBm_ReturnsCorrectValue()
        {
            var result = _builder.ConvertPower(-10, PowerUnit.dBm, PowerUnit.W);
            Assert.That(result, Is.EqualTo(0.0001).Within(0.00001));
        }

        #endregion
    }
}
