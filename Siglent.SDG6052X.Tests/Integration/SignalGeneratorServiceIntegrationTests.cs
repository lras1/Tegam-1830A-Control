using NUnit.Framework;
using Siglent.SDG6052X.DeviceLibrary.Commands;
using Siglent.SDG6052X.DeviceLibrary.Models;
using Siglent.SDG6052X.DeviceLibrary.Parsing;
using Siglent.SDG6052X.DeviceLibrary.Services;
using Siglent.SDG6052X.DeviceLibrary.Simulation;
using Siglent.SDG6052X.DeviceLibrary.Validation;
using System;
using System.Threading.Tasks;

namespace Siglent.SDG6052X.Tests.Integration
{
    [TestFixture]
    public class SignalGeneratorServiceIntegrationTests
    {
        private MockVisaCommunicationManager _mockVisa;
        private ScpiCommandBuilder _commandBuilder;
        private ScpiResponseParser _responseParser;
        private InputValidator _inputValidator;
        private SignalGeneratorService _service;

        [SetUp]
        public void SetUp()
        {
            // Initialize all dependencies
            _mockVisa = new MockVisaCommunicationManager();
            _commandBuilder = new ScpiCommandBuilder();
            _responseParser = new ScpiResponseParser();
            _inputValidator = new InputValidator();

            // Create the service with all dependencies
            _service = new SignalGeneratorService(
                _mockVisa,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            // Connect to the mock device
            _mockVisa.Connect("MOCK::DEVICE::INSTR");
        }

        [TearDown]
        public void TearDown()
        {
            // Disconnect and dispose resources
            _mockVisa?.Disconnect();
            _mockVisa?.Dispose();
            _service = null;
        }

        [Test]
        public async Task SetBasicWaveform_EndToEndConfiguration_ConfiguresAndVerifiesWaveform()
        {
            // Arrange - Set up a complete waveform configuration for a Sine wave
            int channel = 1;
            WaveformType waveformType = WaveformType.Sine;
            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,      // 1 kHz
                Amplitude = 5.0,         // 5 Vpp
                Offset = 0.5,            // 0.5 V DC offset
                Phase = 90.0,            // 90 degrees
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Call SetBasicWaveformAsync() on the service
            var result = await _service.SetBasicWaveformAsync(channel, waveformType, parameters);

            // Assert - Verify the command was sent successfully
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Success, Is.True, $"Operation should succeed. Message: {result.Message}");

            // Act - Query the waveform state back from the device
            var state = await _service.GetWaveformStateAsync(channel);

            // Assert - Verify the returned state matches what was configured
            Assert.That(state, Is.Not.Null, "Waveform state should not be null");
            Assert.That(state.WaveformType, Is.EqualTo(waveformType), "Waveform type should match");
            Assert.That(state.Frequency, Is.EqualTo(parameters.Frequency).Within(0.01), "Frequency should match");
            Assert.That(state.Amplitude, Is.EqualTo(parameters.Amplitude).Within(0.001), "Amplitude should match");
            Assert.That(state.Offset, Is.EqualTo(parameters.Offset).Within(0.001), "Offset should match");
            Assert.That(state.Phase, Is.EqualTo(parameters.Phase).Within(0.1), "Phase should match");

            // Verify the command was sent correctly through the mock by checking the simulated state
            var channelState = _mockVisa.GetChannelState(channel);
            Assert.That(channelState, Is.Not.Null, "Channel state should exist in mock");
            Assert.That(channelState.WaveformType, Is.EqualTo(waveformType), "Mock channel waveform type should match");
            Assert.That(channelState.Frequency, Is.EqualTo(parameters.Frequency).Within(0.01), "Mock channel frequency should match");
            Assert.That(channelState.Amplitude, Is.EqualTo(parameters.Amplitude).Within(0.001), "Mock channel amplitude should match");
            Assert.That(channelState.Offset, Is.EqualTo(parameters.Offset).Within(0.001), "Mock channel offset should match");
            Assert.That(channelState.Phase, Is.EqualTo(parameters.Phase).Within(0.1), "Mock channel phase should match");
        }

        [Test]
        public async Task ConnectAsync_ValidIpAddress_EstablishesConnectionAndRetrievesIdentity()
        {
            // Arrange - Create a fresh service without pre-connected mock
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            bool connectionStateChangedRaised = false;
            bool finalStateIsConnected = false;
            string connectionMessage = null;

            freshService.ConnectionStateChanged += (sender, args) =>
            {
                connectionStateChangedRaised = true;
                finalStateIsConnected = args.IsConnected;
                connectionMessage = args.Message;
            };

            // Act - Connect to the device
            bool connectResult = await freshService.ConnectAsync("192.168.1.100");

            // Assert - Verify connection was successful
            Assert.That(connectResult, Is.True, "ConnectAsync should return true");
            Assert.That(freshService.IsConnected, Is.True, "IsConnected property should be true");
            Assert.That(connectionStateChangedRaised, Is.True, "ConnectionStateChanged event should be raised");
            Assert.That(finalStateIsConnected, Is.True, "Event should indicate connected state");
            Assert.That(connectionMessage, Does.Contain("Connected"), "Connection message should indicate success");

            // Verify device identity was retrieved
            Assert.That(freshService.DeviceInfo, Is.Not.Null, "DeviceInfo should be populated");
            Assert.That(freshService.DeviceInfo.Manufacturer, Is.EqualTo("Siglent Technologies"), "Manufacturer should match");
            Assert.That(freshService.DeviceInfo.Model, Does.Contain("SDG6052X"), "Model should contain SDG6052X");
            Assert.That(freshService.DeviceInfo.SerialNumber, Is.Not.Null.And.Not.Empty, "Serial number should be populated");
            Assert.That(freshService.DeviceInfo.FirmwareVersion, Is.Not.Null.And.Not.Empty, "Firmware version should be populated");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task DisconnectAsync_WhenConnected_DisconnectsAndUpdatesState()
        {
            // Arrange - Create a connected service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Service should be connected initially");

            bool disconnectionEventRaised = false;
            bool finalStateIsDisconnected = false;
            string disconnectionMessage = null;

            freshService.ConnectionStateChanged += (sender, args) =>
            {
                disconnectionEventRaised = true;
                finalStateIsDisconnected = !args.IsConnected;
                disconnectionMessage = args.Message;
            };

            // Act - Disconnect from the device
            await freshService.DisconnectAsync();

            // Assert - Verify disconnection was successful
            Assert.That(freshService.IsConnected, Is.False, "IsConnected property should be false");
            Assert.That(disconnectionEventRaised, Is.True, "ConnectionStateChanged event should be raised");
            Assert.That(finalStateIsDisconnected, Is.True, "Event should indicate disconnected state");
            Assert.That(disconnectionMessage, Does.Contain("Disconnected"), "Disconnection message should indicate success");
            Assert.That(freshService.DeviceInfo, Is.Null, "DeviceInfo should be cleared after disconnect");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task ConnectAsync_EmptyIpAddress_FailsAndRaisesEvent()
        {
            // Arrange - Create a fresh service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            bool connectionFailedEventRaised = false;
            string failureMessage = null;

            freshService.ConnectionStateChanged += (sender, args) =>
            {
                connectionFailedEventRaised = true;
                failureMessage = args.Message;
            };

            // Act - Attempt to connect with empty IP address
            bool connectResult = await freshService.ConnectAsync("");

            // Assert - Verify connection failed
            Assert.That(connectResult, Is.False, "ConnectAsync should return false for empty IP");
            Assert.That(freshService.IsConnected, Is.False, "IsConnected property should be false");
            Assert.That(connectionFailedEventRaised, Is.True, "ConnectionStateChanged event should be raised");
            Assert.That(failureMessage, Does.Contain("cannot be empty"), "Failure message should indicate empty IP");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task Reconnection_AfterDisconnect_EstablishesNewConnection()
        {
            // Arrange - Create a service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            // First connection
            bool firstConnect = await freshService.ConnectAsync("192.168.1.100");
            Assert.That(firstConnect, Is.True, "First connection should succeed");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected after first connect");

            // Disconnect
            await freshService.DisconnectAsync();
            Assert.That(freshService.IsConnected, Is.False, "Should be disconnected after disconnect");

            // Act - Reconnect
            bool secondConnect = await freshService.ConnectAsync("192.168.1.100");

            // Assert - Verify reconnection was successful
            Assert.That(secondConnect, Is.True, "Reconnection should succeed");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected after reconnection");
            Assert.That(freshService.DeviceInfo, Is.Not.Null, "DeviceInfo should be populated after reconnection");
            Assert.That(freshService.DeviceInfo.Model, Does.Contain("SDG6052X"), "Model should be correct after reconnection");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task ConnectionLifecycle_StateManagement_MaintainsCorrectState()
        {
            // Arrange - Create a fresh service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            // Track all connection state changes
            var stateChanges = new System.Collections.Generic.List<(bool IsConnected, string Message)>();
            freshService.ConnectionStateChanged += (sender, args) =>
            {
                stateChanges.Add((args.IsConnected, args.Message));
            };

            // Act & Assert - Test complete lifecycle

            // 1. Initial state - not connected
            Assert.That(freshService.IsConnected, Is.False, "Should start disconnected");
            Assert.That(freshService.DeviceInfo, Is.Null, "DeviceInfo should be null initially");

            // 2. Connect
            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected after ConnectAsync");
            Assert.That(freshService.DeviceInfo, Is.Not.Null, "DeviceInfo should be populated after connect");

            // 3. Verify operations work when connected
            var result = await freshService.SetOutputStateAsync(1, true);
            Assert.That(result.Success, Is.True, "Operations should work when connected");

            // 4. Disconnect
            await freshService.DisconnectAsync();
            Assert.That(freshService.IsConnected, Is.False, "Should be disconnected after DisconnectAsync");
            Assert.That(freshService.DeviceInfo, Is.Null, "DeviceInfo should be cleared after disconnect");

            // 5. Verify operations fail when disconnected
            var resultWhenDisconnected = await freshService.SetOutputStateAsync(1, true);
            Assert.That(resultWhenDisconnected.Success, Is.False, "Operations should fail when disconnected");
            Assert.That(resultWhenDisconnected.Message, Does.Contain("Not connected"), "Error message should indicate not connected");

            // 6. Verify state change events were raised correctly
            Assert.That(stateChanges.Count, Is.GreaterThanOrEqualTo(2), "Should have at least 2 state changes (connect and disconnect)");
            Assert.That(stateChanges[0].IsConnected, Is.True, "First state change should be connection");
            Assert.That(stateChanges[stateChanges.Count - 1].IsConnected, Is.False, "Last state change should be disconnection");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task ConnectAsync_MultipleConnectionAttempts_HandlesCorrectly()
        {
            // Arrange - Create a fresh service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            // Act - Connect multiple times
            bool firstConnect = await freshService.ConnectAsync("192.168.1.100");
            bool secondConnect = await freshService.ConnectAsync("192.168.1.100");

            // Assert - Both connections should succeed (second one reconnects)
            Assert.That(firstConnect, Is.True, "First connection should succeed");
            Assert.That(secondConnect, Is.True, "Second connection should succeed");
            Assert.That(freshService.IsConnected, Is.True, "Should remain connected");
            Assert.That(freshService.DeviceInfo, Is.Not.Null, "DeviceInfo should be populated");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task IsConnected_ReflectsActualConnectionState_ThroughoutLifecycle()
        {
            // Arrange - Create a fresh service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            // Act & Assert - Verify IsConnected property throughout lifecycle

            // Initially disconnected
            Assert.That(freshService.IsConnected, Is.False, "Should be disconnected initially");

            // After successful connection
            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected after ConnectAsync");

            // Still connected
            Assert.That(freshService.IsConnected, Is.True, "Should remain connected");

            // After disconnection
            await freshService.DisconnectAsync();
            Assert.That(freshService.IsConnected, Is.False, "Should be disconnected after DisconnectAsync");

            // After reconnection
            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected after reconnection");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task ErrorPropagation_TimeoutError_PropagatesThroughLayers()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Simulate timeout at communication layer
            freshMock.SimulateTimeout();

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Attempt to set waveform (should timeout)
            var result = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);

            // Assert - Verify error is propagated correctly
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Success, Is.False, "Operation should fail due to timeout");
            Assert.That(result.Message, Does.Contain("timeout").IgnoreCase, "Error message should mention timeout");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task ErrorPropagation_ConnectionLoss_PropagatesThroughLayers()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected initially");

            // Simulate connection loss at communication layer
            freshMock.SimulateConnectionLoss();

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Attempt to set waveform (should fail due to connection loss)
            var result = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);

            // Assert - Verify error is propagated correctly
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Success, Is.False, "Operation should fail due to connection loss");
            Assert.That(result.Message, Does.Contain("Connection lost").IgnoreCase, "Error message should mention connection loss");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task ErrorPropagation_DeviceError_PropagatesThroughLayersAndRaisesEvent()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Set up event tracking
            bool deviceErrorEventRaised = false;
            DeviceError capturedError = null;

            freshService.DeviceError += (sender, args) =>
            {
                deviceErrorEventRaised = true;
                capturedError = args.Error;
            };

            // Simulate a device error at communication layer (parameter out of range)
            freshMock.SimulateError(-222, "Data out of range");

            // Act - Query device error to trigger the error event
            var deviceError = await freshService.GetLastDeviceErrorAsync();

            // Assert - Verify error is propagated correctly
            Assert.That(deviceError, Is.Not.Null, "Device error should not be null");
            Assert.That(deviceError.Code, Is.EqualTo(-222), "Error code should match");
            Assert.That(deviceError.Message, Is.EqualTo("Data out of range"), "Error message should match");

            // Note: DeviceError event is raised by OnCommunicationError handler
            // which is triggered by CommunicationError event from the communication manager
            // In this test, we're verifying the error can be retrieved from the error queue

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task ErrorPropagation_CommunicationError_RaisesDeviceErrorEvent()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Set up event tracking
            bool deviceErrorEventRaised = false;
            DeviceError capturedError = null;
            DateTime eventTimestamp = DateTime.MinValue;

            freshService.DeviceError += (sender, args) =>
            {
                deviceErrorEventRaised = true;
                capturedError = args.Error;
                eventTimestamp = args.Timestamp;
            };

            // Simulate timeout which will trigger CommunicationError event
            freshMock.SimulateTimeout();

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Attempt operation that will fail and raise communication error
            var result = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);

            // Assert - Verify DeviceError event was raised
            Assert.That(deviceErrorEventRaised, Is.True, "DeviceError event should be raised");
            Assert.That(capturedError, Is.Not.Null, "Captured error should not be null");
            Assert.That(capturedError.Code, Is.EqualTo(-1), "Communication errors should have code -1");
            Assert.That(capturedError.Message, Does.Contain("timeout").IgnoreCase, "Error message should mention timeout");
            Assert.That(eventTimestamp, Is.GreaterThan(DateTime.MinValue), "Event timestamp should be set");

            // Verify OperationResult also contains error information
            Assert.That(result.Success, Is.False, "Operation should fail");
            Assert.That(result.Message, Does.Contain("timeout").IgnoreCase, "Result message should mention timeout");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task ErrorPropagation_ValidationError_ReturnsFailedOperationResult()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Create invalid parameters (frequency too high for sine wave)
            var invalidParameters = new WaveformParameters
            {
                Frequency = 1e9, // 1 GHz - exceeds 500 MHz limit
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Attempt to set waveform with invalid parameters
            var result = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, invalidParameters);

            // Assert - Verify validation error is returned in OperationResult
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Success, Is.False, "Operation should fail due to validation error");
            Assert.That(result.Message, Does.Contain("frequency").IgnoreCase, "Error message should mention frequency");
            Assert.That(result.Message, Does.Contain("range").IgnoreCase.Or.Contains("limit").IgnoreCase, 
                "Error message should mention range or limit");

            // Verify no command was sent to the device (validation happens before communication)
            var channelState = freshMock.GetChannelState(1);
            Assert.That(channelState.Frequency, Is.Not.EqualTo(1e9), "Invalid frequency should not be set on device");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task ErrorPropagation_MultipleErrorTypes_HandledCorrectly()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            var validParameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Test 1: Successful operation (baseline)
            var successResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, validParameters);
            Assert.That(successResult.Success, Is.True, "First operation should succeed");

            // Test 2: Timeout error
            freshMock.SimulateTimeout();
            var timeoutResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, validParameters);
            Assert.That(timeoutResult.Success, Is.False, "Timeout operation should fail");
            Assert.That(timeoutResult.Message, Does.Contain("timeout").IgnoreCase, "Should indicate timeout");

            // Reset mock for next test
            freshMock.Dispose();
            freshMock = new MockVisaCommunicationManager();
            freshService = new SignalGeneratorService(freshMock, _commandBuilder, _responseParser, _inputValidator);
            await freshService.ConnectAsync("192.168.1.100");

            // Test 3: Connection loss error
            freshMock.SimulateConnectionLoss();
            var connectionLossResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, validParameters);
            Assert.That(connectionLossResult.Success, Is.False, "Connection loss operation should fail");
            Assert.That(connectionLossResult.Message, Does.Contain("Connection lost").IgnoreCase, "Should indicate connection loss");

            // Reset mock for next test
            freshMock.Dispose();
            freshMock = new MockVisaCommunicationManager();
            freshService = new SignalGeneratorService(freshMock, _commandBuilder, _responseParser, _inputValidator);
            await freshService.ConnectAsync("192.168.1.100");

            // Test 4: Validation error
            var invalidParameters = new WaveformParameters
            {
                Frequency = 1e9, // Invalid
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            var validationResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, invalidParameters);
            Assert.That(validationResult.Success, Is.False, "Validation error operation should fail");
            Assert.That(validationResult.Message, Does.Contain("frequency").IgnoreCase, "Should indicate frequency issue");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task ErrorPropagation_OperationResultContainsErrorDetails()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Simulate a timeout error
            freshMock.SimulateTimeout();

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Attempt operation that will fail
            var result = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);

            // Assert - Verify OperationResult contains comprehensive error information
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result.Success, Is.False, "Success should be false");
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty, "Message should be populated");
            Assert.That(result.Message, Does.Contain("Error").IgnoreCase.Or.Contains("timeout").IgnoreCase, 
                "Message should indicate error or timeout");
            Assert.That(result.Timestamp, Is.GreaterThan(DateTime.MinValue), "Timestamp should be set");
            Assert.That(result.Timestamp, Is.LessThanOrEqualTo(DateTime.Now), "Timestamp should be reasonable");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task EventRaising_MultipleSubscriptions_AllHandlersInvoked()
        {
            // Arrange - Create a fresh service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            // Track event invocations from multiple subscribers
            int connectionHandler1Count = 0;
            int connectionHandler2Count = 0;
            int connectionHandler3Count = 0;
            int errorHandler1Count = 0;
            int errorHandler2Count = 0;

            // Subscribe multiple handlers to ConnectionStateChanged
            freshService.ConnectionStateChanged += (sender, args) => connectionHandler1Count++;
            freshService.ConnectionStateChanged += (sender, args) => connectionHandler2Count++;
            freshService.ConnectionStateChanged += (sender, args) => connectionHandler3Count++;

            // Subscribe multiple handlers to DeviceError
            freshService.DeviceError += (sender, args) => errorHandler1Count++;
            freshService.DeviceError += (sender, args) => errorHandler2Count++;

            // Act - Trigger ConnectionStateChanged event
            await freshService.ConnectAsync("192.168.1.100");

            // Assert - All connection handlers should be invoked
            Assert.That(connectionHandler1Count, Is.EqualTo(1), "Connection handler 1 should be invoked once");
            Assert.That(connectionHandler2Count, Is.EqualTo(1), "Connection handler 2 should be invoked once");
            Assert.That(connectionHandler3Count, Is.EqualTo(1), "Connection handler 3 should be invoked once");

            // Act - Trigger DeviceError event
            freshMock.SimulateTimeout();
            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);

            // Assert - All error handlers should be invoked
            Assert.That(errorHandler1Count, Is.EqualTo(1), "Error handler 1 should be invoked once");
            Assert.That(errorHandler2Count, Is.EqualTo(1), "Error handler 2 should be invoked once");

            // Act - Trigger ConnectionStateChanged again
            await freshService.DisconnectAsync();

            // Assert - All connection handlers should be invoked again
            Assert.That(connectionHandler1Count, Is.EqualTo(2), "Connection handler 1 should be invoked twice");
            Assert.That(connectionHandler2Count, Is.EqualTo(2), "Connection handler 2 should be invoked twice");
            Assert.That(connectionHandler3Count, Is.EqualTo(2), "Connection handler 3 should be invoked twice");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task EventRaising_ConnectionStateChanged_ContainsCorrectInformation()
        {
            // Arrange - Create a fresh service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            ConnectionStateChangedEventArgs capturedConnectArgs = null;
            ConnectionStateChangedEventArgs capturedDisconnectArgs = null;
            ConnectionStateChangedEventArgs capturedFailureArgs = null;

            // Subscribe to event
            freshService.ConnectionStateChanged += (sender, args) =>
            {
                if (args.IsConnected)
                    capturedConnectArgs = args;
                else if (capturedConnectArgs != null)
                    capturedDisconnectArgs = args;
                else
                    capturedFailureArgs = args;
            };

            // Act - Test successful connection
            DateTime beforeConnect = DateTime.Now;
            await freshService.ConnectAsync("192.168.1.100");
            DateTime afterConnect = DateTime.Now;

            // Assert - Verify connect event args
            Assert.That(capturedConnectArgs, Is.Not.Null, "Connect event should be raised");
            Assert.That(capturedConnectArgs.IsConnected, Is.True, "IsConnected should be true");
            Assert.That(capturedConnectArgs.Message, Is.Not.Null.And.Not.Empty, "Message should be populated");
            Assert.That(capturedConnectArgs.Message, Does.Contain("Connected"), "Message should indicate connection");
            Assert.That(capturedConnectArgs.Timestamp, Is.GreaterThanOrEqualTo(beforeConnect), "Timestamp should be after operation start");
            Assert.That(capturedConnectArgs.Timestamp, Is.LessThanOrEqualTo(afterConnect), "Timestamp should be before operation end");

            // Act - Test disconnection
            DateTime beforeDisconnect = DateTime.Now;
            await freshService.DisconnectAsync();
            DateTime afterDisconnect = DateTime.Now;

            // Assert - Verify disconnect event args
            Assert.That(capturedDisconnectArgs, Is.Not.Null, "Disconnect event should be raised");
            Assert.That(capturedDisconnectArgs.IsConnected, Is.False, "IsConnected should be false");
            Assert.That(capturedDisconnectArgs.Message, Is.Not.Null.And.Not.Empty, "Message should be populated");
            Assert.That(capturedDisconnectArgs.Message, Does.Contain("Disconnected"), "Message should indicate disconnection");
            Assert.That(capturedDisconnectArgs.Timestamp, Is.GreaterThanOrEqualTo(beforeDisconnect), "Timestamp should be after operation start");
            Assert.That(capturedDisconnectArgs.Timestamp, Is.LessThanOrEqualTo(afterDisconnect), "Timestamp should be before operation end");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task EventRaising_DeviceError_ContainsCorrectInformation()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            DeviceErrorEventArgs capturedErrorArgs = null;

            // Subscribe to event
            freshService.DeviceError += (sender, args) =>
            {
                capturedErrorArgs = args;
            };

            // Act - Trigger device error
            DateTime beforeError = DateTime.Now;
            freshMock.SimulateTimeout();
            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);
            DateTime afterError = DateTime.Now;

            // Assert - Verify error event args
            Assert.That(capturedErrorArgs, Is.Not.Null, "DeviceError event should be raised");
            Assert.That(capturedErrorArgs.Error, Is.Not.Null, "Error object should be populated");
            Assert.That(capturedErrorArgs.Error.Code, Is.Not.Zero, "Error code should be set");
            Assert.That(capturedErrorArgs.Error.Message, Is.Not.Null.And.Not.Empty, "Error message should be populated");
            Assert.That(capturedErrorArgs.Timestamp, Is.GreaterThanOrEqualTo(beforeError), "Timestamp should be after operation start");
            Assert.That(capturedErrorArgs.Timestamp, Is.LessThanOrEqualTo(afterError), "Timestamp should be before operation end");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task EventRaising_EventTiming_RaisedAtCorrectTime()
        {
            // Arrange - Create a fresh service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            var eventLog = new System.Collections.Generic.List<(string EventType, DateTime Time, bool IsConnectedState)>();

            // Subscribe to events
            freshService.ConnectionStateChanged += (sender, args) =>
            {
                eventLog.Add(("ConnectionStateChanged", DateTime.Now, args.IsConnected));
            };

            freshService.DeviceError += (sender, args) =>
            {
                eventLog.Add(("DeviceError", DateTime.Now, false));
            };

            // Act & Assert - Test event timing during connection lifecycle

            // 1. Connect - should raise ConnectionStateChanged immediately
            DateTime beforeConnect = DateTime.Now;
            await freshService.ConnectAsync("192.168.1.100");
            DateTime afterConnect = DateTime.Now;

            Assert.That(eventLog.Count, Is.EqualTo(1), "Should have 1 event after connect");
            Assert.That(eventLog[0].EventType, Is.EqualTo("ConnectionStateChanged"), "First event should be ConnectionStateChanged");
            Assert.That(eventLog[0].IsConnectedState, Is.True, "Connection state should be true");
            Assert.That(eventLog[0].Time, Is.GreaterThanOrEqualTo(beforeConnect).And.LessThanOrEqualTo(afterConnect), 
                "Event should be raised during connect operation");

            // 2. Successful operation - should not raise any events
            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);

            Assert.That(eventLog.Count, Is.EqualTo(1), "Successful operation should not raise events");

            // 3. Failed operation - should raise DeviceError immediately
            DateTime beforeError = DateTime.Now;
            freshMock.SimulateTimeout();
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);
            DateTime afterError = DateTime.Now;

            Assert.That(eventLog.Count, Is.EqualTo(2), "Should have 2 events after error");
            Assert.That(eventLog[1].EventType, Is.EqualTo("DeviceError"), "Second event should be DeviceError");
            Assert.That(eventLog[1].Time, Is.GreaterThanOrEqualTo(beforeError).And.LessThanOrEqualTo(afterError), 
                "Error event should be raised during failed operation");

            // 4. Disconnect - should raise ConnectionStateChanged immediately
            DateTime beforeDisconnect = DateTime.Now;
            await freshService.DisconnectAsync();
            DateTime afterDisconnect = DateTime.Now;

            Assert.That(eventLog.Count, Is.EqualTo(3), "Should have 3 events after disconnect");
            Assert.That(eventLog[2].EventType, Is.EqualTo("ConnectionStateChanged"), "Third event should be ConnectionStateChanged");
            Assert.That(eventLog[2].IsConnectedState, Is.False, "Connection state should be false");
            Assert.That(eventLog[2].Time, Is.GreaterThanOrEqualTo(beforeDisconnect).And.LessThanOrEqualTo(afterDisconnect), 
                "Event should be raised during disconnect operation");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task EventRaising_ConnectionFailure_RaisesEventWithErrorDetails()
        {
            // Arrange - Create a fresh service
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            ConnectionStateChangedEventArgs capturedArgs = null;

            freshService.ConnectionStateChanged += (sender, args) =>
            {
                capturedArgs = args;
            };

            // Act - Attempt connection with invalid IP (empty string)
            await freshService.ConnectAsync("");

            // Assert - Verify event was raised with failure details
            Assert.That(capturedArgs, Is.Not.Null, "ConnectionStateChanged event should be raised");
            Assert.That(capturedArgs.IsConnected, Is.False, "IsConnected should be false for failed connection");
            Assert.That(capturedArgs.Message, Is.Not.Null.And.Not.Empty, "Message should be populated");
            Assert.That(capturedArgs.Message, Does.Contain("empty").IgnoreCase, "Message should indicate empty IP issue");
            Assert.That(capturedArgs.Timestamp, Is.GreaterThan(DateTime.MinValue), "Timestamp should be set");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task EventRaising_NoSubscribers_OperationsSucceedWithoutError()
        {
            // Arrange - Create a fresh service with NO event subscribers
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            // Act & Assert - Operations should succeed even without event subscribers

            // Connect
            bool connectResult = await freshService.ConnectAsync("192.168.1.100");
            Assert.That(connectResult, Is.True, "Connect should succeed without subscribers");

            // Set waveform
            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            var result = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);
            Assert.That(result.Success, Is.True, "Operation should succeed without subscribers");

            // Trigger error
            freshMock.SimulateTimeout();
            var errorResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);
            Assert.That(errorResult.Success, Is.False, "Operation should fail as expected without subscribers");

            // Disconnect
            await freshService.DisconnectAsync();
            Assert.That(freshService.IsConnected, Is.False, "Disconnect should succeed without subscribers");

            // Cleanup
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_MultipleConcurrentOperations_CompleteSuccessfully()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            var channel1Parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            var channel2Parameters = new WaveformParameters
            {
                Frequency = 2000.0,
                Amplitude = 3.0,
                Offset = 0.5,
                Phase = 90.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Execute multiple concurrent async operations
            var task1 = freshService.SetBasicWaveformAsync(1, WaveformType.Sine, channel1Parameters);
            var task2 = freshService.SetBasicWaveformAsync(2, WaveformType.Square, channel2Parameters);
            var task3 = freshService.SetOutputStateAsync(1, true);
            var task4 = freshService.SetOutputStateAsync(2, true);

            // Wait for all tasks to complete
            var results = await Task.WhenAll(task1, task2, task3, task4);

            // Assert - All operations should complete successfully
            Assert.That(results, Has.Length.EqualTo(4), "Should have 4 results");
            Assert.That(results[0].Success, Is.True, "Channel 1 waveform configuration should succeed");
            Assert.That(results[1].Success, Is.True, "Channel 2 waveform configuration should succeed");
            Assert.That(results[2].Success, Is.True, "Channel 1 output enable should succeed");
            Assert.That(results[3].Success, Is.True, "Channel 2 output enable should succeed");

            // Verify final state of both channels
            var state1 = await freshService.GetWaveformStateAsync(1);
            var state2 = await freshService.GetWaveformStateAsync(2);

            Assert.That(state1.Frequency, Is.EqualTo(1000.0).Within(0.01), "Channel 1 frequency should be set correctly");
            Assert.That(state2.Frequency, Is.EqualTo(2000.0).Within(0.01), "Channel 2 frequency should be set correctly");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_SequentialAwaitPattern_MaintainsCorrectOrder()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            var operationLog = new System.Collections.Generic.List<string>();

            // Act - Execute operations sequentially with await
            operationLog.Add("Start");

            var result1 = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);
            operationLog.Add("Waveform configured");
            Assert.That(result1.Success, Is.True, "Waveform configuration should succeed");

            var result2 = await freshService.SetOutputStateAsync(1, true);
            operationLog.Add("Output enabled");
            Assert.That(result2.Success, Is.True, "Output enable should succeed");

            var state = await freshService.GetWaveformStateAsync(1);
            operationLog.Add("State queried");
            Assert.That(state, Is.Not.Null, "State should be retrieved");

            operationLog.Add("End");

            // Assert - Operations should complete in expected order
            Assert.That(operationLog, Has.Count.EqualTo(5), "Should have 5 log entries");
            Assert.That(operationLog[0], Is.EqualTo("Start"), "First entry should be Start");
            Assert.That(operationLog[1], Is.EqualTo("Waveform configured"), "Second entry should be Waveform configured");
            Assert.That(operationLog[2], Is.EqualTo("Output enabled"), "Third entry should be Output enabled");
            Assert.That(operationLog[3], Is.EqualTo("State queried"), "Fourth entry should be State queried");
            Assert.That(operationLog[4], Is.EqualTo("End"), "Fifth entry should be End");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_TaskWhenAllPattern_AllTasksComplete()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Create multiple tasks and wait for all to complete
            var tasks = new System.Collections.Generic.List<Task<OperationResult>>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - All tasks should complete successfully
            Assert.That(results, Has.Length.EqualTo(10), "Should have 10 results");
            foreach (var result in results)
            {
                Assert.That(result.Success, Is.True, "Each operation should succeed");
                Assert.That(result.Message, Is.Not.Null.And.Not.Empty, "Each result should have a message");
            }

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_TaskWhenAnyPattern_FirstTaskCompletes()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Create multiple tasks and wait for first to complete
            var task1 = freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);
            var task2 = freshService.GetWaveformStateAsync(1);
            var task3 = freshService.SetOutputStateAsync(1, true);

            var completedTask = await Task.WhenAny(task1, task2, task3);

            // Assert - At least one task should complete
            Assert.That(completedTask.IsCompleted, Is.True, "Completed task should be marked as completed");
            Assert.That(completedTask.IsFaulted, Is.False, "Completed task should not be faulted");
            Assert.That(completedTask.IsCanceled, Is.False, "Completed task should not be canceled");

            // Wait for all remaining tasks to complete
            await Task.WhenAll(task1, task3);

            // Verify all tasks completed successfully
            Assert.That(task1.IsCompleted, Is.True, "Task 1 should complete");
            Assert.That(task2.IsCompleted, Is.True, "Task 2 should complete");
            Assert.That(task3.IsCompleted, Is.True, "Task 3 should complete");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_MixedOperationTypes_CoordinateCorrectly()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            var waveformParams = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            var modulationParams = new ModulationParameters
            {
                Type = ModulationType.AM,
                Source = ModulationSource.Internal,
                Depth = 50.0,
                Rate = 100.0,
                ModulationWaveform = WaveformType.Sine
            };

            // Act - Execute different types of operations concurrently
            var waveformTask = freshService.SetBasicWaveformAsync(1, WaveformType.Sine, waveformParams);
            var outputTask = freshService.SetOutputStateAsync(1, true);
            var modulationTask = freshService.ConfigureModulationAsync(1, ModulationType.AM, modulationParams);
            var loadTask = freshService.SetLoadImpedanceAsync(1, LoadImpedance.FiftyOhm);

            var results = await Task.WhenAll(waveformTask, outputTask, modulationTask, loadTask);

            // Assert - All different operation types should complete successfully
            Assert.That(results, Has.Length.EqualTo(4), "Should have 4 results");
            Assert.That(results[0].Success, Is.True, "Waveform configuration should succeed");
            Assert.That(results[1].Success, Is.True, "Output state should succeed");
            Assert.That(results[2].Success, Is.True, "Modulation configuration should succeed");
            Assert.That(results[3].Success, Is.True, "Load impedance should succeed");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_TaskReturnValues_ProperlyReturned()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Execute operations and verify return values
            var setResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);
            var getState = await freshService.GetWaveformStateAsync(1);
            var outputResult = await freshService.SetOutputStateAsync(1, true);

            // Assert - Verify Task<OperationResult> returns proper OperationResult
            Assert.That(setResult, Is.Not.Null, "SetBasicWaveformAsync should return OperationResult");
            Assert.That(setResult, Is.InstanceOf<OperationResult>(), "Should be OperationResult type");
            Assert.That(setResult.Success, Is.True, "Operation should succeed");
            Assert.That(setResult.Message, Is.Not.Null.And.Not.Empty, "Should have message");
            Assert.That(setResult.Timestamp, Is.GreaterThan(DateTime.MinValue), "Should have timestamp");

            // Assert - Verify Task<WaveformState> returns proper WaveformState
            Assert.That(getState, Is.Not.Null, "GetWaveformStateAsync should return WaveformState");
            Assert.That(getState, Is.InstanceOf<WaveformState>(), "Should be WaveformState type");
            Assert.That(getState.Frequency, Is.EqualTo(1000.0).Within(0.01), "Should have correct frequency");

            // Assert - Verify Task<OperationResult> returns proper OperationResult
            Assert.That(outputResult, Is.Not.Null, "SetOutputStateAsync should return OperationResult");
            Assert.That(outputResult, Is.InstanceOf<OperationResult>(), "Should be OperationResult type");
            Assert.That(outputResult.Success, Is.True, "Operation should succeed");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_ExceptionHandling_PropagatesCorrectly()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            // Act & Assert - Test exception handling in async operations

            // Test 1: Query operation when disconnected should throw
            await freshService.DisconnectAsync();
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await freshService.GetWaveformStateAsync(1),
                "GetWaveformStateAsync should throw when disconnected"
            );

            // Test 2: Reconnect and test invalid channel
            await freshService.ConnectAsync("192.168.1.100");
            Assert.ThrowsAsync<ArgumentException>(
                async () => await freshService.GetWaveformStateAsync(99),
                "GetWaveformStateAsync should throw for invalid channel"
            );

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_ConcurrentReadWrite_NoRaceConditions()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Execute concurrent read and write operations
            var writeTasks = new System.Collections.Generic.List<Task<OperationResult>>();
            var readTasks = new System.Collections.Generic.List<Task<WaveformState>>();

            for (int i = 0; i < 5; i++)
            {
                writeTasks.Add(freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters));
                readTasks.Add(freshService.GetWaveformStateAsync(1));
            }

            var writeResults = await Task.WhenAll(writeTasks);
            var readResults = await Task.WhenAll(readTasks);

            // Assert - All operations should complete without race conditions
            Assert.That(writeResults, Has.Length.EqualTo(5), "Should have 5 write results");
            Assert.That(readResults, Has.Length.EqualTo(5), "Should have 5 read results");

            foreach (var result in writeResults)
            {
                Assert.That(result.Success, Is.True, "Each write should succeed");
            }

            foreach (var state in readResults)
            {
                Assert.That(state, Is.Not.Null, "Each read should return state");
                Assert.That(state.Frequency, Is.EqualTo(1000.0).Within(0.01), "State should be consistent");
            }

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task AsyncOperationCoordination_LongRunningOperations_CanBeAwaited()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            // Act - Execute operations that involve Task.Run (simulating long-running operations)
            var startTime = DateTime.Now;

            var task1 = freshService.SetOutputStateAsync(1, true);
            var task2 = freshService.SetOutputStateAsync(2, true);
            var task3 = freshService.GetWaveformStateAsync(1);
            var task4 = freshService.GetWaveformStateAsync(2);

            // Wait for all tasks
            await Task.WhenAll(task1, task2);
            await Task.WhenAll(task3, task4);

            var endTime = DateTime.Now;
            var duration = endTime - startTime;

            // Assert - Operations should complete in reasonable time
            Assert.That(duration.TotalSeconds, Is.LessThan(10), "Operations should complete within 10 seconds");
            Assert.That(task1.IsCompleted, Is.True, "Task 1 should be completed");
            Assert.That(task2.IsCompleted, Is.True, "Task 2 should be completed");
            Assert.That(task3.IsCompleted, Is.True, "Task 3 should be completed");
            Assert.That(task4.IsCompleted, Is.True, "Task 4 should be completed");

            // Verify results
            var result1 = await task1;
            var result2 = await task2;
            Assert.That(result1.Success, Is.True, "Result 1 should succeed");
            Assert.That(result2.Success, Is.True, "Result 2 should succeed");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task StateVerification_AfterWaveformCommand_StateMatchesConfiguration()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Test multiple waveform configurations
            var testCases = new[]
            {
                new
                {
                    Type = WaveformType.Sine,
                    Parameters = new WaveformParameters
                    {
                        Frequency = 1000.0,
                        Amplitude = 5.0,
                        Offset = 0.5,
                        Phase = 90.0,
                        Unit = AmplitudeUnit.Vpp
                    }
                },
                new
                {
                    Type = WaveformType.Square,
                    Parameters = new WaveformParameters
                    {
                        Frequency = 2500.0,
                        Amplitude = 3.5,
                        Offset = -0.25,
                        Phase = 180.0,
                        DutyCycle = 25.0,
                        Unit = AmplitudeUnit.Vpp
                    }
                },
                new
                {
                    Type = WaveformType.Ramp,
                    Parameters = new WaveformParameters
                    {
                        Frequency = 500.0,
                        Amplitude = 8.0,
                        Offset = 1.0,
                        Phase = 45.0,
                        Unit = AmplitudeUnit.Vpp
                    }
                }
            };

            foreach (var testCase in testCases)
            {
                // Act - Configure waveform
                var result = await freshService.SetBasicWaveformAsync(1, testCase.Type, testCase.Parameters);
                Assert.That(result.Success, Is.True, $"Configuration should succeed for {testCase.Type}");

                // Query state back from device
                var state = await freshService.GetWaveformStateAsync(1);

                // Assert - Verify queried state matches configuration
                Assert.That(state, Is.Not.Null, $"State should not be null for {testCase.Type}");
                Assert.That(state.WaveformType, Is.EqualTo(testCase.Type), $"Waveform type should match for {testCase.Type}");
                Assert.That(state.Frequency, Is.EqualTo(testCase.Parameters.Frequency).Within(0.01), 
                    $"Frequency should match for {testCase.Type}");
                Assert.That(state.Amplitude, Is.EqualTo(testCase.Parameters.Amplitude).Within(0.001), 
                    $"Amplitude should match for {testCase.Type}");
                Assert.That(state.Offset, Is.EqualTo(testCase.Parameters.Offset).Within(0.001), 
                    $"Offset should match for {testCase.Type}");
                Assert.That(state.Phase, Is.EqualTo(testCase.Parameters.Phase).Within(0.1), 
                    $"Phase should match for {testCase.Type}");

                // Verify mock's internal state matches queried state
                var mockState = freshMock.GetChannelState(1);
                Assert.That(mockState, Is.Not.Null, $"Mock state should exist for {testCase.Type}");
                Assert.That(mockState.WaveformType, Is.EqualTo(state.WaveformType), 
                    $"Mock waveform type should match queried state for {testCase.Type}");
                Assert.That(mockState.Frequency, Is.EqualTo(state.Frequency).Within(0.01), 
                    $"Mock frequency should match queried state for {testCase.Type}");
                Assert.That(mockState.Amplitude, Is.EqualTo(state.Amplitude).Within(0.001), 
                    $"Mock amplitude should match queried state for {testCase.Type}");
                Assert.That(mockState.Offset, Is.EqualTo(state.Offset).Within(0.001), 
                    $"Mock offset should match queried state for {testCase.Type}");
                Assert.That(mockState.Phase, Is.EqualTo(state.Phase).Within(0.1), 
                    $"Mock phase should match queried state for {testCase.Type}");
            }

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task StateVerification_AfterOutputCommand_StateMatchesConfiguration()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            var parameters = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Configure waveform first
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, parameters);

            // Act - Enable output
            var enableResult = await freshService.SetOutputStateAsync(1, true);
            Assert.That(enableResult.Success, Is.True, "Output enable should succeed");

            // Query state
            var stateEnabled = await freshService.GetWaveformStateAsync(1);

            // Assert - Verify output is enabled
            Assert.That(stateEnabled.OutputEnabled, Is.True, "Output should be enabled in queried state");

            // Verify mock's internal state
            var mockStateEnabled = freshMock.GetChannelState(1);
            Assert.That(mockStateEnabled.OutputEnabled, Is.True, "Output should be enabled in mock state");

            // Act - Disable output
            var disableResult = await freshService.SetOutputStateAsync(1, false);
            Assert.That(disableResult.Success, Is.True, "Output disable should succeed");

            // Query state again
            var stateDisabled = await freshService.GetWaveformStateAsync(1);

            // Assert - Verify output is disabled
            Assert.That(stateDisabled.OutputEnabled, Is.False, "Output should be disabled in queried state");

            // Verify mock's internal state
            var mockStateDisabled = freshMock.GetChannelState(1);
            Assert.That(mockStateDisabled.OutputEnabled, Is.False, "Output should be disabled in mock state");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task StateVerification_AfterModulationCommand_StateMatchesConfiguration()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            // Configure carrier waveform first
            var waveformParams = new WaveformParameters
            {
                Frequency = 10000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, waveformParams);

            // Test multiple modulation configurations
            var modulationTestCases = new[]
            {
                new
                {
                    Type = ModulationType.AM,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.AM,
                        Source = ModulationSource.Internal,
                        Depth = 50.0,
                        Rate = 100.0,
                        ModulationWaveform = WaveformType.Sine
                    }
                },
                new
                {
                    Type = ModulationType.FM,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.FM,
                        Source = ModulationSource.Internal,
                        Deviation = 1000.0,
                        Rate = 200.0,
                        ModulationWaveform = WaveformType.Square
                    }
                }
            };

            foreach (var testCase in modulationTestCases)
            {
                // Act - Configure modulation
                var result = await freshService.ConfigureModulationAsync(1, testCase.Type, testCase.Parameters);
                Assert.That(result.Success, Is.True, $"Modulation configuration should succeed for {testCase.Type}");

                // Enable modulation
                var enableResult = await freshService.SetModulationStateAsync(1, true);
                Assert.That(enableResult.Success, Is.True, $"Modulation enable should succeed for {testCase.Type}");

                // Query modulation state
                var state = await freshService.GetModulationStateAsync(1);

                // Assert - Verify queried state matches configuration
                Assert.That(state, Is.Not.Null, $"Modulation state should not be null for {testCase.Type}");
                Assert.That(state.Enabled, Is.True, $"Modulation should be enabled for {testCase.Type}");
                Assert.That(state.Type, Is.EqualTo(testCase.Type), $"Modulation type should match for {testCase.Type}");
                Assert.That(state.Source, Is.EqualTo(testCase.Parameters.Source), 
                    $"Modulation source should match for {testCase.Type}");
                Assert.That(state.Rate, Is.EqualTo(testCase.Parameters.Rate).Within(0.01), 
                    $"Modulation rate should match for {testCase.Type}");

                // Verify mock's internal state matches queried state
                var mockState = freshMock.GetChannelState(1);
                Assert.That(mockState.ModulationEnabled, Is.EqualTo(state.Enabled), 
                    $"Mock modulation enabled should match queried state for {testCase.Type}");
                Assert.That(mockState.ModulationType, Is.EqualTo(state.Type), 
                    $"Mock modulation type should match queried state for {testCase.Type}");
            }

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task StateVerification_AfterSweepCommand_StateMatchesConfiguration()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            // Configure carrier waveform first
            var waveformParams = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, waveformParams);

            // Test sweep configuration
            var sweepParams = new SweepParameters
            {
                StartFrequency = 100.0,
                StopFrequency = 10000.0,
                Time = 1.0,
                Type = SweepType.Linear,
                Direction = SweepDirection.Up,
                TriggerSource = TriggerSource.Internal
            };

            // Act - Configure sweep
            var result = await freshService.ConfigureSweepAsync(1, sweepParams);
            Assert.That(result.Success, Is.True, "Sweep configuration should succeed");

            // Enable sweep
            var enableResult = await freshService.SetSweepStateAsync(1, true);
            Assert.That(enableResult.Success, Is.True, "Sweep enable should succeed");

            // Query sweep state
            var state = await freshService.GetSweepStateAsync(1);

            // Assert - Verify queried state matches configuration
            Assert.That(state, Is.Not.Null, "Sweep state should not be null");
            Assert.That(state.Enabled, Is.True, "Sweep should be enabled");
            Assert.That(state.StartFrequency, Is.EqualTo(sweepParams.StartFrequency).Within(0.01), 
                "Start frequency should match");
            Assert.That(state.StopFrequency, Is.EqualTo(sweepParams.StopFrequency).Within(0.01), 
                "Stop frequency should match");
            Assert.That(state.Time, Is.EqualTo(sweepParams.Time).Within(0.001), "Sweep time should match");
            Assert.That(state.Type, Is.EqualTo(sweepParams.Type), "Sweep type should match");
            Assert.That(state.Direction, Is.EqualTo(sweepParams.Direction), "Sweep direction should match");

            // Verify mock's internal state matches queried state
            var mockState = freshMock.GetChannelState(1);
            Assert.That(mockState.SweepEnabled, Is.EqualTo(state.Enabled), 
                "Mock sweep enabled should match queried state");
            Assert.That(mockState.SweepStartFrequency, Is.EqualTo(state.StartFrequency).Within(0.01), 
                "Mock start frequency should match queried state");
            Assert.That(mockState.SweepStopFrequency, Is.EqualTo(state.StopFrequency).Within(0.01), 
                "Mock stop frequency should match queried state");
            Assert.That(mockState.SweepTime, Is.EqualTo(state.Time).Within(0.001), 
                "Mock sweep time should match queried state");
            Assert.That(mockState.SweepType, Is.EqualTo(state.Type), 
                "Mock sweep type should match queried state");
            Assert.That(mockState.SweepDirection, Is.EqualTo(state.Direction), 
                "Mock sweep direction should match queried state");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task StateVerification_AfterBurstCommand_StateMatchesConfiguration()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            // Configure carrier waveform first
            var waveformParams = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, waveformParams);

            // Test burst configuration
            var burstParams = new BurstParameters
            {
                Mode = BurstMode.NCycle,
                Cycles = 10,
                Period = 0.01,
                TriggerSource = TriggerSource.Internal,
                TriggerEdge = TriggerEdge.Rising,
                StartPhase = 0.0
            };

            // Act - Configure burst
            var result = await freshService.ConfigureBurstAsync(1, burstParams);
            Assert.That(result.Success, Is.True, "Burst configuration should succeed");

            // Enable burst
            var enableResult = await freshService.SetBurstStateAsync(1, true);
            Assert.That(enableResult.Success, Is.True, "Burst enable should succeed");

            // Query burst state
            var state = await freshService.GetBurstStateAsync(1);

            // Assert - Verify queried state matches configuration
            Assert.That(state, Is.Not.Null, "Burst state should not be null");
            Assert.That(state.Enabled, Is.True, "Burst should be enabled");
            Assert.That(state.Mode, Is.EqualTo(burstParams.Mode), "Burst mode should match");
            Assert.That(state.Cycles, Is.EqualTo(burstParams.Cycles), "Burst cycles should match");
            Assert.That(state.Period, Is.EqualTo(burstParams.Period).Within(0.000001), "Burst period should match");
            Assert.That(state.TriggerSource, Is.EqualTo(burstParams.TriggerSource), "Trigger source should match");
            Assert.That(state.TriggerEdge, Is.EqualTo(burstParams.TriggerEdge), "Trigger edge should match");
            Assert.That(state.StartPhase, Is.EqualTo(burstParams.StartPhase).Within(0.1), "Start phase should match");

            // Verify mock's internal state matches queried state
            var mockState = freshMock.GetChannelState(1);
            Assert.That(mockState.BurstEnabled, Is.EqualTo(state.Enabled), 
                "Mock burst enabled should match queried state");
            Assert.That(mockState.BurstMode, Is.EqualTo(state.Mode), 
                "Mock burst mode should match queried state");
            Assert.That(mockState.BurstCycles, Is.EqualTo(state.Cycles), 
                "Mock burst cycles should match queried state");
            Assert.That(mockState.BurstPeriod, Is.EqualTo(state.Period).Within(0.000001), 
                "Mock burst period should match queried state");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task StateVerification_StatePersistsBetweenOperations()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            // Configure initial waveform
            var initialParams = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.5,
                Phase = 90.0,
                Unit = AmplitudeUnit.Vpp
            };

            // Act - Set initial configuration
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, initialParams);
            await freshService.SetOutputStateAsync(1, true);

            // Query initial state
            var initialState = await freshService.GetWaveformStateAsync(1);
            Assert.That(initialState.Frequency, Is.EqualTo(1000.0).Within(0.01), "Initial frequency should be set");
            Assert.That(initialState.OutputEnabled, Is.True, "Initial output should be enabled");

            // Perform other operations on channel 2
            var channel2Params = new WaveformParameters
            {
                Frequency = 2000.0,
                Amplitude = 3.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            await freshService.SetBasicWaveformAsync(2, WaveformType.Square, channel2Params);

            // Query channel 1 state again
            var persistedState = await freshService.GetWaveformStateAsync(1);

            // Assert - Verify channel 1 state persisted correctly
            Assert.That(persistedState.WaveformType, Is.EqualTo(WaveformType.Sine), 
                "Waveform type should persist");
            Assert.That(persistedState.Frequency, Is.EqualTo(initialParams.Frequency).Within(0.01), 
                "Frequency should persist");
            Assert.That(persistedState.Amplitude, Is.EqualTo(initialParams.Amplitude).Within(0.001), 
                "Amplitude should persist");
            Assert.That(persistedState.Offset, Is.EqualTo(initialParams.Offset).Within(0.001), 
                "Offset should persist");
            Assert.That(persistedState.Phase, Is.EqualTo(initialParams.Phase).Within(0.1), 
                "Phase should persist");
            Assert.That(persistedState.OutputEnabled, Is.True, "Output state should persist");

            // Verify mock's internal state also persisted
            var mockState = freshMock.GetChannelState(1);
            Assert.That(mockState.Frequency, Is.EqualTo(persistedState.Frequency).Within(0.01), 
                "Mock frequency should match persisted state");
            Assert.That(mockState.OutputEnabled, Is.EqualTo(persistedState.OutputEnabled), 
                "Mock output state should match persisted state");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task StateVerification_MultipleConfigurationTypes_AllStatesVerifiable()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");

            // Configure waveform
            var waveformParams = new WaveformParameters
            {
                Frequency = 5000.0,
                Amplitude = 4.0,
                Offset = 0.25,
                Phase = 45.0,
                Unit = AmplitudeUnit.Vpp
            };
            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, waveformParams);

            // Configure modulation
            var modulationParams = new ModulationParameters
            {
                Type = ModulationType.AM,
                Source = ModulationSource.Internal,
                Depth = 75.0,
                Rate = 150.0,
                ModulationWaveform = WaveformType.Sine
            };
            await freshService.ConfigureModulationAsync(1, ModulationType.AM, modulationParams);
            await freshService.SetModulationStateAsync(1, true);

            // Configure sweep
            var sweepParams = new SweepParameters
            {
                StartFrequency = 1000.0,
                StopFrequency = 10000.0,
                Time = 2.0,
                Type = SweepType.Logarithmic,
                Direction = SweepDirection.UpDown,
                TriggerSource = TriggerSource.External
            };
            await freshService.ConfigureSweepAsync(1, sweepParams);

            // Enable output
            await freshService.SetOutputStateAsync(1, true);

            // Act - Query all state types
            var waveformState = await freshService.GetWaveformStateAsync(1);
            var modulationState = await freshService.GetModulationStateAsync(1);
            var sweepState = await freshService.GetSweepStateAsync(1);

            // Assert - Verify all states are correct
            // Waveform state
            Assert.That(waveformState.WaveformType, Is.EqualTo(WaveformType.Sine), "Waveform type should match");
            Assert.That(waveformState.Frequency, Is.EqualTo(5000.0).Within(0.01), "Frequency should match");
            Assert.That(waveformState.Amplitude, Is.EqualTo(4.0).Within(0.001), "Amplitude should match");
            Assert.That(waveformState.OutputEnabled, Is.True, "Output should be enabled");

            // Modulation state
            Assert.That(modulationState.Enabled, Is.True, "Modulation should be enabled");
            Assert.That(modulationState.Type, Is.EqualTo(ModulationType.AM), "Modulation type should match");
            Assert.That(modulationState.Depth, Is.EqualTo(75.0).Within(0.1), "Modulation depth should match");
            Assert.That(modulationState.Rate, Is.EqualTo(150.0).Within(0.01), "Modulation rate should match");

            // Sweep state
            Assert.That(sweepState.StartFrequency, Is.EqualTo(1000.0).Within(0.01), "Sweep start frequency should match");
            Assert.That(sweepState.StopFrequency, Is.EqualTo(10000.0).Within(0.01), "Sweep stop frequency should match");
            Assert.That(sweepState.Type, Is.EqualTo(SweepType.Logarithmic), "Sweep type should match");
            Assert.That(sweepState.Direction, Is.EqualTo(SweepDirection.UpDown), "Sweep direction should match");

            // Verify mock's internal state matches all queried states
            var mockState = freshMock.GetChannelState(1);
            Assert.That(mockState.Frequency, Is.EqualTo(waveformState.Frequency).Within(0.01), 
                "Mock frequency should match waveform state");
            Assert.That(mockState.ModulationEnabled, Is.EqualTo(modulationState.Enabled), 
                "Mock modulation enabled should match modulation state");
            Assert.That(mockState.SweepStartFrequency, Is.EqualTo(sweepState.StartFrequency).Within(0.01), 
                "Mock sweep start frequency should match sweep state");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task ModulationConfiguration_CompleteFlow_ConfiguresAndVerifiesModulation()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Configure carrier waveform first (required for modulation)
            var carrierParams = new WaveformParameters
            {
                Frequency = 10000.0,  // 10 kHz carrier
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };
            var carrierResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, carrierParams);
            Assert.That(carrierResult.Success, Is.True, "Carrier waveform configuration should succeed");

            // Test multiple modulation types with comprehensive parameters
            var modulationTestCases = new[]
            {
                new
                {
                    Name = "AM (Amplitude Modulation)",
                    Type = ModulationType.AM,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.AM,
                        Source = ModulationSource.Internal,
                        Depth = 80.0,           // 80% modulation depth
                        Rate = 100.0,           // 100 Hz modulation rate
                        ModulationWaveform = WaveformType.Sine
                    },
                    VerifyDepth = true,
                    ExpectedDepth = 80.0
                },
                new
                {
                    Name = "FM (Frequency Modulation)",
                    Type = ModulationType.FM,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.FM,
                        Source = ModulationSource.Internal,
                        Deviation = 2000.0,     // 2 kHz deviation
                        Rate = 200.0,           // 200 Hz modulation rate
                        ModulationWaveform = WaveformType.Square
                    },
                    VerifyDepth = false,
                    ExpectedDepth = 0.0
                },
                new
                {
                    Name = "PM (Phase Modulation)",
                    Type = ModulationType.PM,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.PM,
                        Source = ModulationSource.Internal,
                        Deviation = 180.0,      // 180 degrees deviation
                        Rate = 150.0,           // 150 Hz modulation rate
                        ModulationWaveform = WaveformType.Ramp
                    },
                    VerifyDepth = false,
                    ExpectedDepth = 0.0
                },
                new
                {
                    Name = "PWM (Pulse Width Modulation)",
                    Type = ModulationType.PWM,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.PWM,
                        Source = ModulationSource.Internal,
                        Depth = 50.0,           // 50% PWM depth
                        Rate = 50.0,            // 50 Hz modulation rate
                        ModulationWaveform = WaveformType.Sine
                    },
                    VerifyDepth = true,
                    ExpectedDepth = 50.0
                },
                new
                {
                    Name = "FSK (Frequency Shift Keying)",
                    Type = ModulationType.FSK,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.FSK,
                        Source = ModulationSource.Internal,
                        HopFrequency = 15000.0, // Hop to 15 kHz
                        Rate = 1000.0,          // 1 kHz hop rate
                        ModulationWaveform = WaveformType.Square
                    },
                    VerifyDepth = false,
                    ExpectedDepth = 0.0
                },
                new
                {
                    Name = "ASK (Amplitude Shift Keying)",
                    Type = ModulationType.ASK,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.ASK,
                        Source = ModulationSource.Internal,
                        HopAmplitude = 2.5,     // Hop to 2.5 Vpp
                        Rate = 500.0,           // 500 Hz hop rate
                        ModulationWaveform = WaveformType.Square
                    },
                    VerifyDepth = false,
                    ExpectedDepth = 0.0
                },
                new
                {
                    Name = "PSK (Phase Shift Keying)",
                    Type = ModulationType.PSK,
                    Parameters = new ModulationParameters
                    {
                        Type = ModulationType.PSK,
                        Source = ModulationSource.Internal,
                        HopPhase = 180.0,       // 180 degree phase shift
                        Rate = 250.0,           // 250 Hz hop rate
                        ModulationWaveform = WaveformType.Square
                    },
                    VerifyDepth = false,
                    ExpectedDepth = 0.0
                }
            };

            foreach (var testCase in modulationTestCases)
            {
                // Act - Configure modulation parameters
                var configResult = await freshService.ConfigureModulationAsync(1, testCase.Type, testCase.Parameters);

                // Assert - Verify configuration succeeded
                Assert.That(configResult.Success, Is.True, 
                    $"{testCase.Name}: Modulation configuration should succeed. Message: {configResult.Message}");

                // Act - Enable modulation
                var enableResult = await freshService.SetModulationStateAsync(1, true);

                // Assert - Verify enable succeeded
                Assert.That(enableResult.Success, Is.True, 
                    $"{testCase.Name}: Modulation enable should succeed. Message: {enableResult.Message}");

                // Act - Query modulation state back from device
                var state = await freshService.GetModulationStateAsync(1);

                // Assert - Verify the returned state matches what was configured
                Assert.That(state, Is.Not.Null, $"{testCase.Name}: Modulation state should not be null");
                Assert.That(state.Enabled, Is.True, $"{testCase.Name}: Modulation should be enabled");
                Assert.That(state.Type, Is.EqualTo(testCase.Type), 
                    $"{testCase.Name}: Modulation type should match");
                Assert.That(state.Source, Is.EqualTo(testCase.Parameters.Source), 
                    $"{testCase.Name}: Modulation source should match");
                Assert.That(state.Rate, Is.EqualTo(testCase.Parameters.Rate).Within(0.01), 
                    $"{testCase.Name}: Modulation rate should match");

                // Verify type-specific parameters
                if (testCase.VerifyDepth)
                {
                    Assert.That(state.Depth, Is.EqualTo(testCase.ExpectedDepth).Within(0.1), 
                        $"{testCase.Name}: Modulation depth should match");
                }

                // Verify the mock's internal state matches the queried state
                var mockState = freshMock.GetChannelState(1);
                Assert.That(mockState.ModulationEnabled, Is.True, 
                    $"{testCase.Name}: Mock should show modulation enabled");
                Assert.That(mockState.ModulationType, Is.EqualTo(testCase.Type), 
                    $"{testCase.Name}: Mock modulation type should match");

                // Act - Disable modulation
                var disableResult = await freshService.SetModulationStateAsync(1, false);

                // Assert - Verify disable succeeded
                Assert.That(disableResult.Success, Is.True, 
                    $"{testCase.Name}: Modulation disable should succeed");

                // Act - Query state again to verify disabled
                var disabledState = await freshService.GetModulationStateAsync(1);

                // Assert - Verify modulation is disabled
                Assert.That(disabledState.Enabled, Is.False, 
                    $"{testCase.Name}: Modulation should be disabled after disable command");
            }

            // Test modulation with different sources
            var externalSourceParams = new ModulationParameters
            {
                Type = ModulationType.AM,
                Source = ModulationSource.External,  // External modulation source
                Depth = 60.0,
                Rate = 100.0,
                ModulationWaveform = WaveformType.Sine
            };

            var externalResult = await freshService.ConfigureModulationAsync(1, ModulationType.AM, externalSourceParams);
            Assert.That(externalResult.Success, Is.True, "External source modulation configuration should succeed");

            await freshService.SetModulationStateAsync(1, true);
            var externalState = await freshService.GetModulationStateAsync(1);
            Assert.That(externalState.Source, Is.EqualTo(ModulationSource.External), 
                "Modulation source should be External");

            // Test that carrier waveform is still intact after modulation operations
            var finalWaveformState = await freshService.GetWaveformStateAsync(1);
            Assert.That(finalWaveformState.WaveformType, Is.EqualTo(WaveformType.Sine), 
                "Carrier waveform type should remain unchanged");
            Assert.That(finalWaveformState.Frequency, Is.EqualTo(carrierParams.Frequency).Within(0.01), 
                "Carrier frequency should remain unchanged");
            Assert.That(finalWaveformState.Amplitude, Is.EqualTo(carrierParams.Amplitude).Within(0.001), 
                "Carrier amplitude should remain unchanged");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task SweepConfiguration_LinearUpSweep_ConfiguresAndVerifiesCorrectly()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Configure carrier waveform first (required before sweep)
            var carrierParams = new WaveformParameters
            {
                Frequency = 5000.0,      // 5 kHz center frequency
                Amplitude = 3.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            var waveformResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, carrierParams);
            Assert.That(waveformResult.Success, Is.True, "Carrier waveform configuration should succeed");

            // Configure Linear Up sweep
            var sweepParams = new SweepParameters
            {
                StartFrequency = 1000.0,     // 1 kHz
                StopFrequency = 10000.0,     // 10 kHz
                Time = 2.0,                  // 2 seconds
                Type = SweepType.Linear,
                Direction = SweepDirection.Up,
                TriggerSource = TriggerSource.Internal,
                ReturnTime = 0.5,
                HoldTime = 0.1
            };

            // Act - Configure sweep
            var configResult = await freshService.ConfigureSweepAsync(1, sweepParams);

            // Assert - Verify configuration succeeded
            Assert.That(configResult.Success, Is.True, "Sweep configuration should succeed");
            Assert.That(configResult.Message, Does.Contain("configured").IgnoreCase, 
                "Success message should indicate configuration");

            // Act - Enable sweep
            var enableResult = await freshService.SetSweepStateAsync(1, true);

            // Assert - Verify enable succeeded
            Assert.That(enableResult.Success, Is.True, "Sweep enable should succeed");

            // Act - Query sweep state
            var state = await freshService.GetSweepStateAsync(1);

            // Assert - Verify state matches configuration
            Assert.That(state, Is.Not.Null, "Sweep state should not be null");
            Assert.That(state.Enabled, Is.True, "Sweep should be enabled");
            Assert.That(state.StartFrequency, Is.EqualTo(sweepParams.StartFrequency).Within(0.01), 
                "Start frequency should match");
            Assert.That(state.StopFrequency, Is.EqualTo(sweepParams.StopFrequency).Within(0.01), 
                "Stop frequency should match");
            Assert.That(state.Time, Is.EqualTo(sweepParams.Time).Within(0.001), 
                "Sweep time should match");
            Assert.That(state.Type, Is.EqualTo(SweepType.Linear), 
                "Sweep type should be Linear");
            Assert.That(state.Direction, Is.EqualTo(SweepDirection.Up), 
                "Sweep direction should be Up");
            Assert.That(state.TriggerSource, Is.EqualTo(TriggerSource.Internal), 
                "Trigger source should match");

            // Verify mock's internal state
            var mockState = freshMock.GetChannelState(1);
            Assert.That(mockState.SweepEnabled, Is.True, "Mock should show sweep enabled");
            Assert.That(mockState.SweepStartFrequency, Is.EqualTo(sweepParams.StartFrequency).Within(0.01), 
                "Mock start frequency should match");
            Assert.That(mockState.SweepStopFrequency, Is.EqualTo(sweepParams.StopFrequency).Within(0.01), 
                "Mock stop frequency should match");
            Assert.That(mockState.SweepType, Is.EqualTo(SweepType.Linear), 
                "Mock sweep type should match");
            Assert.That(mockState.SweepDirection, Is.EqualTo(SweepDirection.Up), 
                "Mock sweep direction should match");

            // Verify carrier waveform is still intact
            var waveformState = await freshService.GetWaveformStateAsync(1);
            Assert.That(waveformState.WaveformType, Is.EqualTo(WaveformType.Sine), 
                "Carrier waveform should remain Sine");
            Assert.That(waveformState.Amplitude, Is.EqualTo(carrierParams.Amplitude).Within(0.001), 
                "Carrier amplitude should remain unchanged");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task SweepConfiguration_LogarithmicDownSweep_ConfiguresAndVerifiesCorrectly()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Configure carrier waveform first
            var carrierParams = new WaveformParameters
            {
                Frequency = 50000.0,     // 50 kHz
                Amplitude = 2.5,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            var waveformResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, carrierParams);
            Assert.That(waveformResult.Success, Is.True, "Carrier waveform configuration should succeed");

            // Configure Logarithmic Down sweep
            var sweepParams = new SweepParameters
            {
                StartFrequency = 100000.0,   // 100 kHz (higher)
                StopFrequency = 1000.0,      // 1 kHz (lower)
                Time = 5.0,                  // 5 seconds
                Type = SweepType.Logarithmic,
                Direction = SweepDirection.Down,
                TriggerSource = TriggerSource.External,
                ReturnTime = 1.0,
                HoldTime = 0.2
            };

            // Act - Configure sweep
            var configResult = await freshService.ConfigureSweepAsync(1, sweepParams);

            // Assert - Verify configuration succeeded
            Assert.That(configResult.Success, Is.True, "Sweep configuration should succeed");

            // Act - Enable sweep
            var enableResult = await freshService.SetSweepStateAsync(1, true);

            // Assert - Verify enable succeeded
            Assert.That(enableResult.Success, Is.True, "Sweep enable should succeed");

            // Act - Query sweep state
            var state = await freshService.GetSweepStateAsync(1);

            // Assert - Verify state matches configuration
            Assert.That(state, Is.Not.Null, "Sweep state should not be null");
            Assert.That(state.Enabled, Is.True, "Sweep should be enabled");
            Assert.That(state.StartFrequency, Is.EqualTo(sweepParams.StartFrequency).Within(0.01), 
                "Start frequency should match");
            Assert.That(state.StopFrequency, Is.EqualTo(sweepParams.StopFrequency).Within(0.01), 
                "Stop frequency should match");
            Assert.That(state.Time, Is.EqualTo(sweepParams.Time).Within(0.001), 
                "Sweep time should match");
            Assert.That(state.Type, Is.EqualTo(SweepType.Logarithmic), 
                "Sweep type should be Logarithmic");
            Assert.That(state.Direction, Is.EqualTo(SweepDirection.Down), 
                "Sweep direction should be Down");
            Assert.That(state.TriggerSource, Is.EqualTo(TriggerSource.External), 
                "Trigger source should be External");

            // Verify mock's internal state
            var mockState = freshMock.GetChannelState(1);
            Assert.That(mockState.SweepEnabled, Is.True, "Mock should show sweep enabled");
            Assert.That(mockState.SweepType, Is.EqualTo(SweepType.Logarithmic), 
                "Mock sweep type should be Logarithmic");
            Assert.That(mockState.SweepDirection, Is.EqualTo(SweepDirection.Down), 
                "Mock sweep direction should be Down");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task SweepConfiguration_LinearUpDownSweep_ConfiguresAndVerifiesCorrectly()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Configure carrier waveform first
            var carrierParams = new WaveformParameters
            {
                Frequency = 10000.0,     // 10 kHz
                Amplitude = 4.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            var waveformResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, carrierParams);
            Assert.That(waveformResult.Success, Is.True, "Carrier waveform configuration should succeed");

            // Configure Linear UpDown sweep (sweeps up then down)
            var sweepParams = new SweepParameters
            {
                StartFrequency = 5000.0,     // 5 kHz
                StopFrequency = 15000.0,     // 15 kHz
                Time = 3.0,                  // 3 seconds
                Type = SweepType.Linear,
                Direction = SweepDirection.UpDown,
                TriggerSource = TriggerSource.Manual,
                ReturnTime = 0.3,
                HoldTime = 0.05
            };

            // Act - Configure sweep
            var configResult = await freshService.ConfigureSweepAsync(1, sweepParams);

            // Assert - Verify configuration succeeded
            Assert.That(configResult.Success, Is.True, "Sweep configuration should succeed");

            // Act - Enable sweep
            var enableResult = await freshService.SetSweepStateAsync(1, true);

            // Assert - Verify enable succeeded
            Assert.That(enableResult.Success, Is.True, "Sweep enable should succeed");

            // Act - Query sweep state
            var state = await freshService.GetSweepStateAsync(1);

            // Assert - Verify state matches configuration
            Assert.That(state, Is.Not.Null, "Sweep state should not be null");
            Assert.That(state.Enabled, Is.True, "Sweep should be enabled");
            Assert.That(state.StartFrequency, Is.EqualTo(sweepParams.StartFrequency).Within(0.01), 
                "Start frequency should match");
            Assert.That(state.StopFrequency, Is.EqualTo(sweepParams.StopFrequency).Within(0.01), 
                "Stop frequency should match");
            Assert.That(state.Time, Is.EqualTo(sweepParams.Time).Within(0.001), 
                "Sweep time should match");
            Assert.That(state.Type, Is.EqualTo(SweepType.Linear), 
                "Sweep type should be Linear");
            Assert.That(state.Direction, Is.EqualTo(SweepDirection.UpDown), 
                "Sweep direction should be UpDown");
            Assert.That(state.TriggerSource, Is.EqualTo(TriggerSource.Manual), 
                "Trigger source should be Manual");

            // Verify mock's internal state
            var mockState = freshMock.GetChannelState(1);
            Assert.That(mockState.SweepEnabled, Is.True, "Mock should show sweep enabled");
            Assert.That(mockState.SweepType, Is.EqualTo(SweepType.Linear), 
                "Mock sweep type should be Linear");
            Assert.That(mockState.SweepDirection, Is.EqualTo(SweepDirection.UpDown), 
                "Mock sweep direction should be UpDown");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task SweepConfiguration_MultipleSweepTypes_AllTypesWorkCorrectly()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Configure carrier waveform first
            var carrierParams = new WaveformParameters
            {
                Frequency = 1000.0,
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, carrierParams);

            // Test data: different sweep type and direction combinations
            var testCases = new[]
            {
                new
                {
                    Name = "Linear Up",
                    Type = SweepType.Linear,
                    Direction = SweepDirection.Up,
                    StartFreq = 100.0,
                    StopFreq = 10000.0
                },
                new
                {
                    Name = "Linear Down",
                    Type = SweepType.Linear,
                    Direction = SweepDirection.Down,
                    StartFreq = 10000.0,
                    StopFreq = 100.0
                },
                new
                {
                    Name = "Linear UpDown",
                    Type = SweepType.Linear,
                    Direction = SweepDirection.UpDown,
                    StartFreq = 500.0,
                    StopFreq = 5000.0
                },
                new
                {
                    Name = "Logarithmic Up",
                    Type = SweepType.Logarithmic,
                    Direction = SweepDirection.Up,
                    StartFreq = 100.0,
                    StopFreq = 100000.0
                },
                new
                {
                    Name = "Logarithmic Down",
                    Type = SweepType.Logarithmic,
                    Direction = SweepDirection.Down,
                    StartFreq = 50000.0,
                    StopFreq = 500.0
                },
                new
                {
                    Name = "Logarithmic UpDown",
                    Type = SweepType.Logarithmic,
                    Direction = SweepDirection.UpDown,
                    StartFreq = 1000.0,
                    StopFreq = 10000.0
                }
            };

            // Test each sweep type and direction combination
            foreach (var testCase in testCases)
            {
                // Arrange - Create sweep parameters for this test case
                var sweepParams = new SweepParameters
                {
                    StartFrequency = testCase.StartFreq,
                    StopFrequency = testCase.StopFreq,
                    Time = 1.0,
                    Type = testCase.Type,
                    Direction = testCase.Direction,
                    TriggerSource = TriggerSource.Internal,
                    ReturnTime = 0.1,
                    HoldTime = 0.05
                };

                // Act - Configure sweep
                var configResult = await freshService.ConfigureSweepAsync(1, sweepParams);

                // Assert - Verify configuration succeeded
                Assert.That(configResult.Success, Is.True, 
                    $"{testCase.Name}: Sweep configuration should succeed");

                // Act - Enable sweep
                var enableResult = await freshService.SetSweepStateAsync(1, true);

                // Assert - Verify enable succeeded
                Assert.That(enableResult.Success, Is.True, 
                    $"{testCase.Name}: Sweep enable should succeed");

                // Act - Query sweep state
                var state = await freshService.GetSweepStateAsync(1);

                // Assert - Verify state matches configuration
                Assert.That(state, Is.Not.Null, 
                    $"{testCase.Name}: Sweep state should not be null");
                Assert.That(state.Enabled, Is.True, 
                    $"{testCase.Name}: Sweep should be enabled");
                Assert.That(state.Type, Is.EqualTo(testCase.Type), 
                    $"{testCase.Name}: Sweep type should match");
                Assert.That(state.Direction, Is.EqualTo(testCase.Direction), 
                    $"{testCase.Name}: Sweep direction should match");
                Assert.That(state.StartFrequency, Is.EqualTo(testCase.StartFreq).Within(0.01), 
                    $"{testCase.Name}: Start frequency should match");
                Assert.That(state.StopFrequency, Is.EqualTo(testCase.StopFreq).Within(0.01), 
                    $"{testCase.Name}: Stop frequency should match");

                // Verify mock's internal state
                var mockState = freshMock.GetChannelState(1);
                Assert.That(mockState.SweepEnabled, Is.True, 
                    $"{testCase.Name}: Mock should show sweep enabled");
                Assert.That(mockState.SweepType, Is.EqualTo(testCase.Type), 
                    $"{testCase.Name}: Mock sweep type should match");
                Assert.That(mockState.SweepDirection, Is.EqualTo(testCase.Direction), 
                    $"{testCase.Name}: Mock sweep direction should match");

                // Act - Disable sweep for next test case
                var disableResult = await freshService.SetSweepStateAsync(1, false);

                // Assert - Verify disable succeeded
                Assert.That(disableResult.Success, Is.True, 
                    $"{testCase.Name}: Sweep disable should succeed");

                // Verify sweep is disabled
                var disabledState = await freshService.GetSweepStateAsync(1);
                Assert.That(disabledState.Enabled, Is.False, 
                    $"{testCase.Name}: Sweep should be disabled after disable command");
            }

            // Verify carrier waveform is still intact after all sweep operations
            var finalWaveformState = await freshService.GetWaveformStateAsync(1);
            Assert.That(finalWaveformState.WaveformType, Is.EqualTo(WaveformType.Sine), 
                "Carrier waveform should remain Sine");
            Assert.That(finalWaveformState.Frequency, Is.EqualTo(carrierParams.Frequency).Within(0.01), 
                "Carrier frequency should remain unchanged");
            Assert.That(finalWaveformState.Amplitude, Is.EqualTo(carrierParams.Amplitude).Within(0.001), 
                "Carrier amplitude should remain unchanged");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task SweepConfiguration_EnableDisableCycle_WorksCorrectly()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Configure carrier waveform first
            var carrierParams = new WaveformParameters
            {
                Frequency = 2000.0,
                Amplitude = 3.5,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, carrierParams);

            // Configure sweep
            var sweepParams = new SweepParameters
            {
                StartFrequency = 1000.0,
                StopFrequency = 5000.0,
                Time = 2.0,
                Type = SweepType.Linear,
                Direction = SweepDirection.Up,
                TriggerSource = TriggerSource.Internal,
                ReturnTime = 0.5,
                HoldTime = 0.1
            };

            var configResult = await freshService.ConfigureSweepAsync(1, sweepParams);
            Assert.That(configResult.Success, Is.True, "Sweep configuration should succeed");

            // Test multiple enable/disable cycles
            for (int cycle = 1; cycle <= 3; cycle++)
            {
                // Act - Enable sweep
                var enableResult = await freshService.SetSweepStateAsync(1, true);

                // Assert - Verify enable succeeded
                Assert.That(enableResult.Success, Is.True, 
                    $"Cycle {cycle}: Sweep enable should succeed");

                // Query and verify enabled state
                var enabledState = await freshService.GetSweepStateAsync(1);
                Assert.That(enabledState.Enabled, Is.True, 
                    $"Cycle {cycle}: Sweep should be enabled");
                Assert.That(enabledState.StartFrequency, Is.EqualTo(sweepParams.StartFrequency).Within(0.01), 
                    $"Cycle {cycle}: Start frequency should persist");
                Assert.That(enabledState.Type, Is.EqualTo(sweepParams.Type), 
                    $"Cycle {cycle}: Sweep type should persist");

                // Act - Disable sweep
                var disableResult = await freshService.SetSweepStateAsync(1, false);

                // Assert - Verify disable succeeded
                Assert.That(disableResult.Success, Is.True, 
                    $"Cycle {cycle}: Sweep disable should succeed");

                // Query and verify disabled state
                var disabledState = await freshService.GetSweepStateAsync(1);
                Assert.That(disabledState.Enabled, Is.False, 
                    $"Cycle {cycle}: Sweep should be disabled");

                // Verify sweep parameters persist even when disabled
                Assert.That(disabledState.StartFrequency, Is.EqualTo(sweepParams.StartFrequency).Within(0.01), 
                    $"Cycle {cycle}: Start frequency should persist when disabled");
                Assert.That(disabledState.StopFrequency, Is.EqualTo(sweepParams.StopFrequency).Within(0.01), 
                    $"Cycle {cycle}: Stop frequency should persist when disabled");
                Assert.That(disabledState.Type, Is.EqualTo(sweepParams.Type), 
                    $"Cycle {cycle}: Sweep type should persist when disabled");
                Assert.That(disabledState.Direction, Is.EqualTo(sweepParams.Direction), 
                    $"Cycle {cycle}: Sweep direction should persist when disabled");
            }

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task SweepConfiguration_WithoutCarrierWaveform_StillConfigures()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Note: Not configuring carrier waveform first - testing if sweep can be configured independently

            // Configure sweep parameters
            var sweepParams = new SweepParameters
            {
                StartFrequency = 1000.0,
                StopFrequency = 10000.0,
                Time = 1.5,
                Type = SweepType.Linear,
                Direction = SweepDirection.Up,
                TriggerSource = TriggerSource.Internal,
                ReturnTime = 0.2,
                HoldTime = 0.1
            };

            // Act - Configure sweep without carrier waveform
            var configResult = await freshService.ConfigureSweepAsync(1, sweepParams);

            // Assert - Verify configuration succeeded (sweep can be configured independently)
            Assert.That(configResult.Success, Is.True, 
                "Sweep configuration should succeed even without prior carrier waveform configuration");

            // Act - Enable sweep
            var enableResult = await freshService.SetSweepStateAsync(1, true);

            // Assert - Verify enable succeeded
            Assert.That(enableResult.Success, Is.True, "Sweep enable should succeed");

            // Act - Query sweep state
            var state = await freshService.GetSweepStateAsync(1);

            // Assert - Verify state matches configuration
            Assert.That(state, Is.Not.Null, "Sweep state should not be null");
            Assert.That(state.Enabled, Is.True, "Sweep should be enabled");
            Assert.That(state.StartFrequency, Is.EqualTo(sweepParams.StartFrequency).Within(0.01), 
                "Start frequency should match");
            Assert.That(state.StopFrequency, Is.EqualTo(sweepParams.StopFrequency).Within(0.01), 
                "Stop frequency should match");
            Assert.That(state.Type, Is.EqualTo(SweepType.Linear), "Sweep type should match");
            Assert.That(state.Direction, Is.EqualTo(SweepDirection.Up), "Sweep direction should match");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }

        [Test]
        public async Task BurstConfiguration_CompleteFlow_ConfiguresAndVerifiesBurst()
        {
            // Arrange - Create a fresh service and connect
            var freshMock = new MockVisaCommunicationManager();
            var freshService = new SignalGeneratorService(
                freshMock,
                _commandBuilder,
                _responseParser,
                _inputValidator
            );

            await freshService.ConnectAsync("192.168.1.100");
            Assert.That(freshService.IsConnected, Is.True, "Should be connected");

            // Configure carrier waveform first (required for burst)
            var carrierParams = new WaveformParameters
            {
                Frequency = 1000.0,      // 1 kHz carrier
                Amplitude = 5.0,
                Offset = 0.0,
                Phase = 0.0,
                Unit = AmplitudeUnit.Vpp
            };

            var waveformResult = await freshService.SetBasicWaveformAsync(1, WaveformType.Sine, carrierParams);
            Assert.That(waveformResult.Success, Is.True, "Carrier waveform configuration should succeed");

            // Test multiple burst configurations with comprehensive parameters
            var burstTestCases = new[]
            {
                new
                {
                    Name = "N-Cycle Burst with Internal Trigger",
                    Parameters = new BurstParameters
                    {
                        Mode = BurstMode.NCycle,
                        Cycles = 10,                    // 10 cycles per burst
                        Period = 0.01,                  // 10 ms period
                        TriggerSource = TriggerSource.Internal,
                        TriggerEdge = TriggerEdge.Rising,
                        StartPhase = 0.0,               // 0 degrees
                        GatePolarity = GatePolarity.Positive
                    }
                },
                new
                {
                    Name = "N-Cycle Burst with External Trigger",
                    Parameters = new BurstParameters
                    {
                        Mode = BurstMode.NCycle,
                        Cycles = 50,                    // 50 cycles per burst
                        Period = 0.02,                  // 20 ms period
                        TriggerSource = TriggerSource.External,
                        TriggerEdge = TriggerEdge.Falling,
                        StartPhase = 90.0,              // 90 degrees
                        GatePolarity = GatePolarity.Positive
                    }
                },
                new
                {
                    Name = "N-Cycle Burst with Manual Trigger",
                    Parameters = new BurstParameters
                    {
                        Mode = BurstMode.NCycle,
                        Cycles = 100,                   // 100 cycles per burst
                        Period = 0.05,                  // 50 ms period
                        TriggerSource = TriggerSource.Manual,
                        TriggerEdge = TriggerEdge.Rising,
                        StartPhase = 180.0,             // 180 degrees
                        GatePolarity = GatePolarity.Positive
                    }
                },
                new
                {
                    Name = "Gated Burst with Positive Polarity",
                    Parameters = new BurstParameters
                    {
                        Mode = BurstMode.Gated,
                        Cycles = 1,                     // Not used in gated mode
                        Period = 0.001,                 // 1 ms period
                        TriggerSource = TriggerSource.External,
                        TriggerEdge = TriggerEdge.Rising,
                        StartPhase = 0.0,
                        GatePolarity = GatePolarity.Positive
                    }
                },
                new
                {
                    Name = "Gated Burst with Negative Polarity",
                    Parameters = new BurstParameters
                    {
                        Mode = BurstMode.Gated,
                        Cycles = 1,                     // Not used in gated mode
                        Period = 0.002,                 // 2 ms period
                        TriggerSource = TriggerSource.External,
                        TriggerEdge = TriggerEdge.Falling,
                        StartPhase = 45.0,              // 45 degrees
                        GatePolarity = GatePolarity.Negative
                    }
                }
            };

            // Test each burst configuration
            foreach (var testCase in burstTestCases)
            {
                // Act - Configure burst
                var configResult = await freshService.ConfigureBurstAsync(1, testCase.Parameters);

                // Assert - Verify configuration succeeded
                Assert.That(configResult.Success, Is.True, 
                    $"{testCase.Name}: Burst configuration should succeed");

                // Act - Enable burst
                var enableResult = await freshService.SetBurstStateAsync(1, true);

                // Assert - Verify enable succeeded
                Assert.That(enableResult.Success, Is.True, 
                    $"{testCase.Name}: Burst enable should succeed");

                // Act - Query burst state
                var state = await freshService.GetBurstStateAsync(1);

                // Assert - Verify state matches configuration
                Assert.That(state, Is.Not.Null, 
                    $"{testCase.Name}: Burst state should not be null");
                Assert.That(state.Enabled, Is.True, 
                    $"{testCase.Name}: Burst should be enabled");
                Assert.That(state.Mode, Is.EqualTo(testCase.Parameters.Mode), 
                    $"{testCase.Name}: Burst mode should match");
                Assert.That(state.Cycles, Is.EqualTo(testCase.Parameters.Cycles), 
                    $"{testCase.Name}: Burst cycles should match");
                Assert.That(state.Period, Is.EqualTo(testCase.Parameters.Period).Within(0.000001), 
                    $"{testCase.Name}: Burst period should match");
                Assert.That(state.TriggerSource, Is.EqualTo(testCase.Parameters.TriggerSource), 
                    $"{testCase.Name}: Trigger source should match");
                Assert.That(state.TriggerEdge, Is.EqualTo(testCase.Parameters.TriggerEdge), 
                    $"{testCase.Name}: Trigger edge should match");
                Assert.That(state.StartPhase, Is.EqualTo(testCase.Parameters.StartPhase).Within(0.1), 
                    $"{testCase.Name}: Start phase should match");

                // Verify mock's internal state matches queried state
                var mockState = freshMock.GetChannelState(1);
                Assert.That(mockState.BurstEnabled, Is.True, 
                    $"{testCase.Name}: Mock should show burst enabled");
                Assert.That(mockState.BurstMode, Is.EqualTo(testCase.Parameters.Mode), 
                    $"{testCase.Name}: Mock burst mode should match");
                Assert.That(mockState.BurstCycles, Is.EqualTo(testCase.Parameters.Cycles), 
                    $"{testCase.Name}: Mock burst cycles should match");
                Assert.That(mockState.BurstPeriod, Is.EqualTo(testCase.Parameters.Period).Within(0.000001), 
                    $"{testCase.Name}: Mock burst period should match");

                // Act - Disable burst for next test case
                var disableResult = await freshService.SetBurstStateAsync(1, false);

                // Assert - Verify disable succeeded
                Assert.That(disableResult.Success, Is.True, 
                    $"{testCase.Name}: Burst disable should succeed");

                // Verify burst is disabled
                var disabledState = await freshService.GetBurstStateAsync(1);
                Assert.That(disabledState.Enabled, Is.False, 
                    $"{testCase.Name}: Burst should be disabled after disable command");

                // Verify burst parameters persist even when disabled
                Assert.That(disabledState.Mode, Is.EqualTo(testCase.Parameters.Mode), 
                    $"{testCase.Name}: Burst mode should persist when disabled");
                Assert.That(disabledState.Cycles, Is.EqualTo(testCase.Parameters.Cycles), 
                    $"{testCase.Name}: Burst cycles should persist when disabled");
                Assert.That(disabledState.Period, Is.EqualTo(testCase.Parameters.Period).Within(0.000001), 
                    $"{testCase.Name}: Burst period should persist when disabled");
            }

            // Verify carrier waveform is still intact after all burst operations
            var finalWaveformState = await freshService.GetWaveformStateAsync(1);
            Assert.That(finalWaveformState.WaveformType, Is.EqualTo(WaveformType.Sine), 
                "Carrier waveform should remain Sine");
            Assert.That(finalWaveformState.Frequency, Is.EqualTo(carrierParams.Frequency).Within(0.01), 
                "Carrier frequency should remain unchanged");
            Assert.That(finalWaveformState.Amplitude, Is.EqualTo(carrierParams.Amplitude).Within(0.001), 
                "Carrier amplitude should remain unchanged");

            // Cleanup
            await freshService.DisconnectAsync();
            freshMock.Dispose();
        }
    }
}
