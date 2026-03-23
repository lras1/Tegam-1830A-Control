using NUnit.Framework;
using Siglent.SDG6052X.DeviceLibrary.Simulation;
using Siglent.SDG6052X.DeviceLibrary.Models;
using System;

namespace Siglent.SDG6052X.Tests.Unit
{
    [TestFixture]
    public class MockCommunicationManagerTests
    {
        private MockVisaCommunicationManager _mock;

        [SetUp]
        public void SetUp()
        {
            _mock = new MockVisaCommunicationManager();
        }

        [TearDown]
        public void TearDown()
        {
            _mock?.Dispose();
        }

        #region Connection Tests

        [Test]
        public void Connect_ValidResourceName_ReturnsTrue()
        {
            // Act
            var result = _mock.Connect("MOCK::DEVICE::INSTR");

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_mock.IsConnected, Is.True);
        }

        [Test]
        public void Connect_NullResourceName_ReturnsFalse()
        {
            // Act
            var result = _mock.Connect(null);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mock.IsConnected, Is.False);
        }

        [Test]
        public void Connect_EmptyResourceName_ReturnsFalse()
        {
            // Act
            var result = _mock.Connect("");

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_mock.IsConnected, Is.False);
        }

        [Test]
        public void Disconnect_AfterConnect_SetsIsConnectedToFalse()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.Disconnect();

            // Assert
            Assert.That(_mock.IsConnected, Is.False);
        }

        #endregion

        #region SendCommand Tests

        [Test]
        public void SendCommand_WhenConnected_ReturnsSuccess()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            var result = _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0");

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public void SendCommand_WhenNotConnected_ReturnsFailure()
        {
            // Act
            var result = _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0");

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Response, Does.Contain("Not connected"));
        }

        [Test]
        public void SendCommand_NullCommand_ReturnsFailure()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            var result = _mock.SendCommand(null);

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public void SendCommand_EmptyCommand_ReturnsFailure()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            var result = _mock.SendCommand("");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        #endregion

        #region Query Tests

        [Test]
        public void Query_IdentityQuery_ReturnsDeviceIdentity()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            var response = _mock.Query("*IDN?");

            // Assert
            Assert.That(response, Does.Contain("Siglent"));
            Assert.That(response, Does.Contain("SDG6052X"));
        }

        [Test]
        public void Query_WhenNotConnected_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _mock.Query("*IDN?"));
        }

        [Test]
        public void Query_NullQuery_ThrowsArgumentException()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _mock.Query(null));
        }

        [Test]
        public void Query_ErrorQuery_ReturnsNoError()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            var response = _mock.Query("SYST:ERR?");

            // Assert
            Assert.That(response, Does.Contain("0"));
            Assert.That(response, Does.Contain("No Error"));
        }

        #endregion

        #region State Management Tests

        [Test]
        public void SendCommand_BasicWaveform_UpdatesChannelState()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0");
            var state = _mock.GetChannelState(1);

            // Assert
            Assert.That(state, Is.Not.Null);
            Assert.That(state.WaveformType, Is.EqualTo(WaveformType.Sine));
            Assert.That(state.Frequency, Is.EqualTo(1000).Within(0.001));
            Assert.That(state.Amplitude, Is.EqualTo(5.0).Within(0.001));
            Assert.That(state.Offset, Is.EqualTo(0.0).Within(0.001));
            Assert.That(state.Phase, Is.EqualTo(0.0).Within(0.001));
        }

        [Test]
        public void SendCommand_OutputState_UpdatesOutputEnabled()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SendCommand("C1:OUTP ON");
            var state = _mock.GetChannelState(1);

            // Assert
            Assert.That(state.OutputEnabled, Is.True);
        }

        [Test]
        public void SendCommand_OutputOff_UpdatesOutputEnabled()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SendCommand("C1:OUTP OFF");
            var state = _mock.GetChannelState(1);

            // Assert
            Assert.That(state.OutputEnabled, Is.False);
        }

        [Test]
        public void SendCommand_FrequencyInKHz_ConvertsToHz()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,10KHZ,AMP,5VPP,OFST,0V,PHSE,0");
            var state = _mock.GetChannelState(1);

            // Assert
            Assert.That(state.Frequency, Is.EqualTo(10000).Within(0.001));
        }

        [Test]
        public void SendCommand_FrequencyInMHz_ConvertsToHz()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,1MHZ,AMP,5VPP,OFST,0V,PHSE,0");
            var state = _mock.GetChannelState(1);

            // Assert
            Assert.That(state.Frequency, Is.EqualTo(1000000).Within(0.001));
        }

        [Test]
        public void SendCommand_SquareWave_ParsesDutyCycle()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SendCommand("C1:BSWV WVTP,SQUARE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0,DUTY,50");
            var state = _mock.GetChannelState(1);

            // Assert
            Assert.That(state.WaveformType, Is.EqualTo(WaveformType.Square));
            Assert.That(state.DutyCycle, Is.EqualTo(50.0).Within(0.001));
        }

        #endregion

        #region Response Generation Tests

        [Test]
        public void Query_WaveformState_ReturnsFormattedResponse()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");
            _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0");

            // Act
            var response = _mock.Query("C1:BSWV?");

            // Assert
            Assert.That(response, Does.Contain("C1:BSWV"));
            Assert.That(response, Does.Contain("WVTP,SINE"));
            Assert.That(response, Does.Contain("FRQ,"));
            Assert.That(response, Does.Contain("AMP,"));
            Assert.That(response, Does.Contain("OFST,"));
            Assert.That(response, Does.Contain("PHSE,"));
        }

        [Test]
        public void Query_OutputState_ReturnsOnOrOff()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");
            _mock.SendCommand("C1:OUTP ON");

            // Act
            var response = _mock.Query("C1:OUTP?");

            // Assert
            Assert.That(response, Is.EqualTo("ON"));
        }

        [Test]
        public void Query_OutputStateOff_ReturnsOFF()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");
            _mock.SendCommand("C1:OUTP OFF");

            // Act
            var response = _mock.Query("C1:OUTP?");

            // Assert
            Assert.That(response, Is.EqualTo("OFF"));
        }

        #endregion

        #region Error Simulation Tests

        [Test]
        public void SimulateError_AddsErrorToQueue()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SimulateError(-222, "Data out of range");
            var response = _mock.Query("SYST:ERR?");

            // Assert
            Assert.That(response, Does.Contain("-222"));
            Assert.That(response, Does.Contain("Data out of range"));
        }

        [Test]
        public void SimulateError_MultipleErrors_ReturnsInOrder()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SimulateError(-100, "First error");
            _mock.SimulateError(-200, "Second error");
            
            var response1 = _mock.Query("SYST:ERR?");
            var response2 = _mock.Query("SYST:ERR?");
            var response3 = _mock.Query("SYST:ERR?");

            // Assert
            Assert.That(response1, Does.Contain("-100"));
            Assert.That(response2, Does.Contain("-200"));
            Assert.That(response3, Does.Contain("0"));
        }

        [Test]
        public void SimulateConnectionLoss_CausesNextCommandToFail()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SimulateConnectionLoss();
            var result = _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0");

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(_mock.IsConnected, Is.False);
        }

        [Test]
        public void SimulateTimeout_CausesNextCommandToTimeout()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SimulateTimeout();
            var result = _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0");

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Exception, Is.TypeOf<TimeoutException>());
        }

        #endregion

        #region Async Tests

        [Test]
        public async System.Threading.Tasks.Task SendCommandAsync_WhenConnected_ReturnsSuccess()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            var result = await _mock.SendCommandAsync("C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0");

            // Assert
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async System.Threading.Tasks.Task QueryAsync_IdentityQuery_ReturnsDeviceIdentity()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            var response = await _mock.QueryAsync("*IDN?");

            // Assert
            Assert.That(response, Does.Contain("Siglent"));
            Assert.That(response, Does.Contain("SDG6052X"));
        }

        #endregion

        #region GetDeviceIdentity Tests

        [Test]
        public void GetDeviceIdentity_ReturnsFormattedIdentity()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            var identity = _mock.GetDeviceIdentity();

            // Assert
            Assert.That(identity, Does.Contain("Siglent"));
            Assert.That(identity, Does.Contain("SDG6052X"));
        }

        #endregion

        #region Channel Isolation Tests

        [Test]
        public void SendCommand_Channel1_DoesNotAffectChannel2()
        {
            // Arrange
            _mock.Connect("MOCK::DEVICE::INSTR");

            // Act
            _mock.SendCommand("C1:BSWV WVTP,SINE,FRQ,1000HZ,AMP,5VPP,OFST,0V,PHSE,0");
            _mock.SendCommand("C2:BSWV WVTP,SQUARE,FRQ,2000HZ,AMP,3VPP,OFST,1V,PHSE,90");

            var state1 = _mock.GetChannelState(1);
            var state2 = _mock.GetChannelState(2);

            // Assert
            Assert.That(state1.WaveformType, Is.EqualTo(WaveformType.Sine));
            Assert.That(state1.Frequency, Is.EqualTo(1000).Within(0.001));
            
            Assert.That(state2.WaveformType, Is.EqualTo(WaveformType.Square));
            Assert.That(state2.Frequency, Is.EqualTo(2000).Within(0.001));
        }

        #endregion
    }
}
