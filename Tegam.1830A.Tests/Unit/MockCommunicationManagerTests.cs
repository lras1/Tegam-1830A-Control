using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Simulation;

namespace Tegam._1830A.Tests.Unit
{
    [TestFixture]
    public class MockCommunicationManagerTests
    {
        private MockVisaCommunicationManager _manager;

        [SetUp]
        public void Setup()
        {
            _manager = new MockVisaCommunicationManager();
        }

        [TearDown]
        public void TearDown()
        {
            _manager?.Dispose();
        }

        #region Connection Tests

        [Test]
        public void Connect_WithValidResourceName_ReturnsTrue()
        {
            var result = _manager.Connect("TCPIP::192.168.1.100::INSTR");
            Assert.That(result, Is.True);
        }

        [Test]
        public void Connect_SetsIsConnectedToTrue()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            Assert.That(_manager.IsConnected, Is.True);
        }

        [Test]
        public void Disconnect_SetsIsConnectedToFalse()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.Disconnect();
            Assert.That(_manager.IsConnected, Is.False);
        }

        [Test]
        public void IsConnected_InitiallyFalse()
        {
            Assert.That(_manager.IsConnected, Is.False);
        }

        #endregion

        #region Frequency Command Tests

        [Test]
        public void SendCommand_WithFrequencyCommand_UpdatesState()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("FREQ 2.4 GHz");
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Query_WithFrequencyQuery_ReturnsFrequency()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("FREQ 2.4 GHz");
            var result = _manager.Query("FREQ?");
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Does.Contain("2.4"));
        }

        [Test]
        public void Query_WithFrequencyQuery_ReturnsCorrectUnit()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("FREQ 2.4 GHz");
            var result = _manager.Query("FREQ?");
            Assert.That(result, Does.Contain("GHz"));
        }

        #endregion

        #region Sensor Command Tests

        [Test]
        public void SendCommand_WithSensorSelectCommand_UpdatesState()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("SENS:SEL 2");
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Query_WithSensorQuery_ReturnsSensorId()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("SENS:SEL 3");
            var result = _manager.Query("SENS:SEL?");
            Assert.That(result, Does.Contain("3"));
        }

        [Test]
        public void Query_WithAvailableSensorsQuery_ReturnsSensorList()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.Query("SENS:LIST?");
            Assert.That(result, Is.Not.Null.And.Not.Empty);
        }

        #endregion

        #region Power Measurement Tests

        [Test]
        public void Query_WithPowerMeasurementQuery_ReturnsPowerValue()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.Query("MEAS:POW?");
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Does.Contain("dBm").Or.Contain("W").Or.Contain("mW"));
        }

        [Test]
        public void Query_WithPowerMeasurementQuery_ReturnsNumericValue()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.Query("MEAS:POW?");
            var parts = result.Split(' ');
            Assert.That(parts.Length, Is.GreaterThanOrEqualTo(2));
            Assert.That(double.TryParse(parts[0], out _), Is.True);
        }

        #endregion

        #region Calibration Command Tests

        [Test]
        public void SendCommand_WithInternalCalibrationCommand_UpdatesState()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("CAL:INT");
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void SendCommand_WithExternalCalibrationCommand_UpdatesState()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("CAL:EXT");
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Query_WithCalibrationStatusQuery_ReturnsStatus()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("CAL:INT");
            var result = _manager.Query("CAL:STAT?");
            Assert.That(result, Is.Not.Null.And.Not.Empty);
        }

        #endregion

        #region Logging Command Tests

        [Test]
        public void SendCommand_WithStartLoggingCommand_UpdatesState()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("LOG:START \"test.csv\"");
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void SendCommand_WithStopLoggingCommand_UpdatesState()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("LOG:START \"test.csv\"");
            var result = _manager.SendCommand("LOG:STOP");
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Query_WithLoggingStatusQuery_ReturnsStatus()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("LOG:START \"test.csv\"");
            var result = _manager.Query("LOG:STAT?");
            Assert.That(result, Is.Not.Null.And.Not.Empty);
        }

        #endregion

        #region System Command Tests

        [Test]
        public void Query_WithIdentityQuery_ReturnsDeviceIdentity()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.Query("*IDN?");
            Assert.That(result, Is.Not.Null.And.Not.Empty);
            Assert.That(result, Does.Contain("Tegam"));
        }

        [Test]
        public void Query_WithSystemStatusQuery_ReturnsStatus()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.Query("SYST:STAT?");
            Assert.That(result, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void SendCommand_WithResetCommand_Succeeds()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("*RST");
            Assert.That(result.IsSuccess, Is.True);
        }

        #endregion

        #region Device Identity Tests

        [Test]
        public void GetDeviceIdentity_ReturnsNonEmptyString()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.GetDeviceIdentity();
            Assert.That(result, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void GetDeviceIdentity_ContainsTegam()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.GetDeviceIdentity();
            Assert.That(result, Does.Contain("Tegam"));
        }

        #endregion

        #region Async Operation Tests

        [Test]
        public async Task SendCommandAsync_WithValidCommand_ReturnsSuccess()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = await _manager.SendCommandAsync("FREQ 2.4 GHz");
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task QueryAsync_WithValidQuery_ReturnsResponse()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = await _manager.QueryAsync("FREQ?");
            Assert.That(result, Is.Not.Null.And.Not.Empty);
        }

        #endregion

        #region State Management Tests

        [Test]
        public void GetDeviceState_ReturnsSimulatedDeviceState()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var state = _manager.GetDeviceState();
            Assert.That(state, Is.Not.Null);
        }

        [Test]
        public void GetDeviceState_ReflectsFrequencyChanges()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("FREQ 5.8 GHz");
            var state = _manager.GetDeviceState();
            Assert.That(state.CurrentFrequency, Is.EqualTo(5.8));
        }

        [Test]
        public void GetDeviceState_ReflectsSensorChanges()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("SENS:SEL 2");
            var state = _manager.GetDeviceState();
            Assert.That(state.CurrentSensorId, Is.EqualTo(2));
        }

        #endregion

        #region Error Handling Tests

        [Test]
        public void SendCommand_WithoutConnection_ReturnsFalse()
        {
            var result = _manager.SendCommand("FREQ 2.4 GHz");
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void Query_WithoutConnection_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _manager.Query("FREQ?"));
        }

        [Test]
        public void SendCommand_WithInvalidCommand_ReturnsFalse()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("INVALID COMMAND");
            Assert.That(result.IsSuccess, Is.False);
        }

        #endregion

        #region Validation Tests

        [Test]
        public void SendCommand_WithInvalidSensorId_ReturnsFalse()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("SENS:SEL 5");
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void SendCommand_WithInvalidFrequency_ReturnsFalse()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("FREQ 50 GHz");
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void SendCommand_WithNegativeFrequency_ReturnsFalse()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.SendCommand("FREQ -2.4 GHz");
            Assert.That(result.IsSuccess, Is.False);
        }

        #endregion

        #region Binary Query Tests

        [Test]
        public void QueryBinary_WithValidQuery_ReturnsBytes()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            var result = _manager.QueryBinary("MEAS:POW?");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }

        #endregion

        #region Multiple Operations Tests

        [Test]
        public void MultipleCommands_MaintainState()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("FREQ 2.4 GHz");
            _manager.SendCommand("SENS:SEL 2");
            
            var freqResult = _manager.Query("FREQ?");
            var sensorResult = _manager.Query("SENS:SEL?");
            
            Assert.That(freqResult, Does.Contain("2.4"));
            Assert.That(sensorResult, Does.Contain("2"));
        }

        [Test]
        public void RepeatedQueries_ReturnConsistentResults()
        {
            _manager.Connect("TCPIP::192.168.1.100::INSTR");
            _manager.SendCommand("FREQ 5.8 GHz");
            
            var result1 = _manager.Query("FREQ?");
            var result2 = _manager.Query("FREQ?");
            
            Assert.That(result1, Is.EqualTo(result2));
        }

        #endregion
    }
}
