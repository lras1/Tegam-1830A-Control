using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tegam._1830A.DeviceLibrary.Commands;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Parsing;
using Tegam._1830A.DeviceLibrary.Services;
using Tegam._1830A.DeviceLibrary.Simulation;
using Tegam._1830A.DeviceLibrary.Validation;

namespace Tegam._1830A.Tests.Integration
{
    [TestFixture]
    public class PowerMeterServiceIntegrationTests
    {
        private PowerMeterService _service;
        private MockVisaCommunicationManager _mockManager;

        [SetUp]
        public void Setup()
        {
            _mockManager = new MockVisaCommunicationManager();
            var commandBuilder = new ScpiCommandBuilder();
            var responseParser = new ScpiResponseParser();
            var validator = new InputValidator();
            
            _service = new PowerMeterService(_mockManager, commandBuilder, responseParser, validator);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
            _mockManager?.Dispose();
        }

        #region Connection Management Tests

        [Test]
        public async Task ConnectAsync_WithValidAddress_SetsIsConnectedTrue()
        {
            var result = await _service.ConnectAsync("192.168.1.100");
            Assert.That(_service.IsConnected, Is.True);
        }

        [Test]
        public async Task ConnectAsync_WithValidAddress_ReturnsTrue()
        {
            var result = await _service.ConnectAsync("192.168.1.100");
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ConnectAsync_PopulatesDeviceInfo()
        {
            await _service.ConnectAsync("192.168.1.100");
            Assert.That(_service.DeviceInfo, Is.Not.Null);
            Assert.That(_service.DeviceInfo.Manufacturer, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task DisconnectAsync_SetsIsConnectedFalse()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.DisconnectAsync();
            Assert.That(_service.IsConnected, Is.False);
        }

        #endregion

        #region Frequency Configuration Tests

        [Test]
        public async Task SetFrequencyAsync_WithValidFrequency_ReturnsSuccess()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.SetFrequencyAsync(2.4, FrequencyUnit.GHz);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task SetFrequencyAsync_WithValidFrequency_UpdatesDeviceState()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.SetFrequencyAsync(5.8, FrequencyUnit.GHz);
            var state = _mockManager.GetDeviceState();
            Assert.That(state.CurrentFrequency, Is.EqualTo(5.8));
        }

        [Test]
        public async Task GetFrequencyAsync_ReturnsCurrentFrequency()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.SetFrequencyAsync(2.4, FrequencyUnit.GHz);
            var result = await _service.GetFrequencyAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Frequency, Is.EqualTo(2.4));
        }

        [Test]
        public async Task SetFrequencyAsync_WithInvalidFrequency_ReturnsFalse()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.SetFrequencyAsync(50, FrequencyUnit.GHz);
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task SetFrequencyAsync_WithDifferentUnits_Works()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.SetFrequencyAsync(2400, FrequencyUnit.MHz);
            Assert.That(result.IsSuccess, Is.True);
        }

        #endregion

        #region Power Measurement Tests

        [Test]
        public async Task MeasurePowerAsync_ReturnsValidMeasurement()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.MeasurePowerAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PowerValue, Is.Not.EqualTo(0));
        }

        [Test]
        public async Task MeasurePowerAsync_WithFrequency_SetFrequencyAndMeasures()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.MeasurePowerAsync(2.4, FrequencyUnit.GHz);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Frequency, Is.EqualTo(2.4));
        }

        [Test]
        public async Task MeasureMultipleAsync_ReturnsMultipleMeasurements()
        {
            await _service.ConnectAsync("192.168.1.100");
            var results = await _service.MeasureMultipleAsync(3, 10);
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task MeasureMultipleAsync_RaisesMeasurementReceivedEvent()
        {
            await _service.ConnectAsync("192.168.1.100");
            var eventRaised = false;
            _service.MeasurementReceived += (s, e) => eventRaised = true;
            
            await _service.MeasureMultipleAsync(1, 0);
            Assert.That(eventRaised, Is.True);
        }

        #endregion

        #region Sensor Management Tests

        [Test]
        public async Task SelectSensorAsync_WithValidSensorId_ReturnsSuccess()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.SelectSensorAsync(2);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task SelectSensorAsync_UpdatesDeviceState()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.SelectSensorAsync(3);
            var state = _mockManager.GetDeviceState();
            Assert.That(state.CurrentSensorId, Is.EqualTo(3));
        }

        [Test]
        public async Task GetCurrentSensorAsync_ReturnsCurrentSensor()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.SelectSensorAsync(2);
            var result = await _service.GetCurrentSensorAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SensorId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAvailableSensorsAsync_ReturnsListOfSensors()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.GetAvailableSensorsAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task SelectSensorAsync_WithInvalidSensorId_ReturnsFalse()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.SelectSensorAsync(5);
            Assert.That(result.IsSuccess, Is.False);
        }

        #endregion

        #region Calibration Tests

        [Test]
        public async Task CalibrateAsync_WithInternalMode_ReturnsSuccess()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.CalibrateAsync(CalibrationMode.Internal);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task CalibrateAsync_WithExternalMode_ReturnsSuccess()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.CalibrateAsync(CalibrationMode.External);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task GetCalibrationStatusAsync_ReturnsCalibrationStatus()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.CalibrateAsync(CalibrationMode.Internal);
            var result = await _service.GetCalibrationStatusAsync();
            Assert.That(result, Is.Not.Null);
        }

        #endregion

        #region Data Logging Tests

        [Test]
        public async Task StartLoggingAsync_WithValidFilename_ReturnsSuccess()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.StartLoggingAsync("test_measurements.csv");
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task StopLoggingAsync_ReturnsSuccess()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.StartLoggingAsync("test_measurements.csv");
            var result = await _service.StopLoggingAsync();
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task IsLoggingAsync_ReturnsTrueWhenLogging()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.StartLoggingAsync("test_measurements.csv");
            var result = await _service.IsLoggingAsync();
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsLoggingAsync_ReturnsFalseWhenNotLogging()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.IsLoggingAsync();
            Assert.That(result, Is.False);
        }

        #endregion

        #region System Status Tests

        [Test]
        public async Task GetSystemStatusAsync_ReturnsSystemStatus()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.GetSystemStatusAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsReady, Is.True);
        }

        [Test]
        public async Task ResetDeviceAsync_ReturnsSuccess()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.ResetDeviceAsync();
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task GetErrorQueueAsync_ReturnsErrorList()
        {
            await _service.ConnectAsync("192.168.1.100");
            var result = await _service.GetErrorQueueAsync();
            Assert.That(result, Is.Not.Null);
        }

        #endregion

        #region Event Raising Tests

        [Test]
        public async Task ConnectionStateChanged_RaisedOnConnect()
        {
            var eventRaised = false;
            _service.ConnectionStateChanged += (s, e) => eventRaised = true;
            
            await _service.ConnectAsync("192.168.1.100");
            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public async Task ConnectionStateChanged_RaisedOnDisconnect()
        {
            await _service.ConnectAsync("192.168.1.100");
            var eventRaised = false;
            _service.ConnectionStateChanged += (s, e) => eventRaised = true;
            
            await _service.DisconnectAsync();
            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public async Task MeasurementReceived_RaisedOnMeasurement()
        {
            await _service.ConnectAsync("192.168.1.100");
            var eventRaised = false;
            _service.MeasurementReceived += (s, e) => eventRaised = true;
            
            await _service.MeasurePowerAsync();
            Assert.That(eventRaised, Is.True);
        }

        #endregion

        #region Error Propagation Tests

        [Test]
        public async Task SetFrequencyAsync_WithInvalidFrequency_DoesNotUpdateState()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.SetFrequencyAsync(2.4, FrequencyUnit.GHz);
            var initialState = _mockManager.GetDeviceState();
            
            await _service.SetFrequencyAsync(50, FrequencyUnit.GHz);
            var finalState = _mockManager.GetDeviceState();
            
            Assert.That(finalState.CurrentFrequency, Is.EqualTo(initialState.CurrentFrequency));
        }

        [Test]
        public async Task SelectSensorAsync_WithInvalidSensorId_DoesNotUpdateState()
        {
            await _service.ConnectAsync("192.168.1.100");
            await _service.SelectSensorAsync(1);
            var initialState = _mockManager.GetDeviceState();
            
            await _service.SelectSensorAsync(5);
            var finalState = _mockManager.GetDeviceState();
            
            Assert.That(finalState.CurrentSensorId, Is.EqualTo(initialState.CurrentSensorId));
        }

        #endregion

        #region End-to-End Workflow Tests

        [Test]
        public async Task CompleteFrequencyConfigurationWorkflow()
        {
            // Connect
            var connected = await _service.ConnectAsync("192.168.1.100");
            Assert.That(connected, Is.True);
            
            // Set frequency
            var setResult = await _service.SetFrequencyAsync(2.4, FrequencyUnit.GHz);
            Assert.That(setResult.IsSuccess, Is.True);
            
            // Query frequency
            var getResult = await _service.GetFrequencyAsync();
            Assert.That(getResult.Frequency, Is.EqualTo(2.4));
            
            // Disconnect
            await _service.DisconnectAsync();
            Assert.That(_service.IsConnected, Is.False);
        }

        [Test]
        public async Task CompletePowerMeasurementWorkflow()
        {
            // Connect
            await _service.ConnectAsync("192.168.1.100");
            
            // Set frequency
            await _service.SetFrequencyAsync(2.4, FrequencyUnit.GHz);
            
            // Select sensor
            await _service.SelectSensorAsync(1);
            
            // Measure power
            var measurement = await _service.MeasurePowerAsync();
            Assert.That(measurement, Is.Not.Null);
            Assert.That(measurement.PowerValue, Is.Not.EqualTo(0));
            
            // Disconnect
            await _service.DisconnectAsync();
        }

        [Test]
        public async Task CompleteSensorSelectionWorkflow()
        {
            // Connect
            await _service.ConnectAsync("192.168.1.100");
            
            // Get available sensors
            var sensors = await _service.GetAvailableSensorsAsync();
            Assert.That(sensors.Count, Is.GreaterThan(0));
            
            // Select a sensor
            var selectResult = await _service.SelectSensorAsync(2);
            Assert.That(selectResult.IsSuccess, Is.True);
            
            // Get current sensor
            var currentSensor = await _service.GetCurrentSensorAsync();
            Assert.That(currentSensor.SensorId, Is.EqualTo(2));
            
            // Disconnect
            await _service.DisconnectAsync();
        }

        [Test]
        public async Task CompleteCalibrationWorkflow()
        {
            // Connect
            await _service.ConnectAsync("192.168.1.100");
            
            // Start calibration
            var calibResult = await _service.CalibrateAsync(CalibrationMode.Internal);
            Assert.That(calibResult.IsSuccess, Is.True);
            
            // Get calibration status
            var status = await _service.GetCalibrationStatusAsync();
            Assert.That(status, Is.Not.Null);
            
            // Disconnect
            await _service.DisconnectAsync();
        }

        [Test]
        public async Task CompleteDataLoggingWorkflow()
        {
            // Connect
            await _service.ConnectAsync("192.168.1.100");
            
            // Start logging
            var startResult = await _service.StartLoggingAsync("measurements.csv");
            Assert.That(startResult.IsSuccess, Is.True);
            
            // Verify logging is active
            var isLogging = await _service.IsLoggingAsync();
            Assert.That(isLogging, Is.True);
            
            // Stop logging
            var stopResult = await _service.StopLoggingAsync();
            Assert.That(stopResult.IsSuccess, Is.True);
            
            // Verify logging is stopped
            isLogging = await _service.IsLoggingAsync();
            Assert.That(isLogging, Is.False);
            
            // Disconnect
            await _service.DisconnectAsync();
        }

        #endregion

        #region Async Coordination Tests

        [Test]
        public async Task MultipleAsyncOperations_ExecuteCorrectly()
        {
            await _service.ConnectAsync("192.168.1.100");
            
            var task1 = _service.SetFrequencyAsync(2.4, FrequencyUnit.GHz);
            var task2 = _service.SelectSensorAsync(1);
            var task3 = _service.MeasurePowerAsync();
            
            await Task.WhenAll(task1, task2, task3);
            
            Assert.That(task1.Result.IsSuccess, Is.True);
            Assert.That(task2.Result.IsSuccess, Is.True);
            Assert.That(task3.Result, Is.Not.Null);
        }

        #endregion
    }
}
