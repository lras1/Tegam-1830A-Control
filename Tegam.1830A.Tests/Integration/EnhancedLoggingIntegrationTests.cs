using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Tegam._1830A.DeviceLibrary.Logging;
using Tegam._1830A.DeviceLibrary.Models;
using Tegam._1830A.DeviceLibrary.Services;
using Tegam._1830A.DeviceLibrary.Simulation;
using Tegam.WinFormsUI.Controllers;

namespace Tegam._1830A.Tests.Integration
{
    /// <summary>
    /// Integration tests for enhanced logging system with connection-independent logging.
    /// Tests Requirements 11.1-11.7: Logging Independent of Device Connection
    /// </summary>
    [TestFixture]
    public class EnhancedLoggingIntegrationTests
    {
        private LogManager _logManager;
        private IPowerMeterService _powerMeterService;
        private EnhancedLoggingController _controller;
        private FrequencyConfigurationController _frequencyController;
        private SensorManagementController _sensorController;
        private CalibrationController _calibrationController;
        private string _testLogFile;

        [SetUp]
        public void SetUp()
        {
            // Create test log file path
            _testLogFile = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.csv");

            // Create mock communication manager and power meter service
            var mockComm = new MockVisaCommunicationManager();
            var commandBuilder = new ScpiCommandBuilder();
            var responseParser = new ScpiResponseParser();
            var validator = new InputValidator();
            
            _powerMeterService = new PowerMeterService(mockComm, commandBuilder, responseParser, validator);

            // Create log manager
            _logManager = new LogManager();

            // Create other controllers (minimal setup)
            _frequencyController = new FrequencyConfigurationController(_powerMeterService);
            _sensorController = new SensorManagementController(_powerMeterService);
            _calibrationController = new CalibrationController(_powerMeterService);

            // Create enhanced logging controller
            _controller = new EnhancedLoggingController(
                _powerMeterService,
                _logManager,
                _frequencyController,
                _sensorController,
                _calibrationController);
        }

        [TearDown]
        public void TearDown()
        {
            // Stop logging if active
            if (_logManager.CurrentState == LoggingState.Active)
            {
                _logManager.StopLogging();
            }

            // Clean up test file
            if (File.Exists(_testLogFile))
            {
                try
                {
                    File.Delete(_testLogFile);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        /// <summary>
        /// Tests Requirement 11.1: THE Enhanced_Logger SHALL allow logging to be started when the device is not connected
        /// </summary>
        [Test]
        public void StartLogging_WithoutDeviceConnection_Succeeds()
        {
            // Arrange - device is not connected
            Assert.That(_powerMeterService.IsConnected, Is.False);

            // Act - start logging without connection
            _logManager.StartLogging(_testLogFile);

            // Assert
            Assert.That(_logManager.CurrentState, Is.EqualTo(LoggingState.Active));
            Assert.That(File.Exists(_testLogFile), Is.True);
        }

        /// <summary>
        /// Tests Requirement 11.2: WHEN logging is started without device connection, 
        /// THE Enhanced_Logger SHALL create the CSV file and write the header
        /// </summary>
        [Test]
        public void StartLogging_WithoutConnection_CreatesFileWithHeader()
        {
            // Arrange - device is not connected
            Assert.That(_powerMeterService.IsConnected, Is.False);

            // Act
            _logManager.StartLogging(_testLogFile);
            _logManager.StopLogging();

            // Assert - file exists and has header
            Assert.That(File.Exists(_testLogFile), Is.True);
            var lines = File.ReadAllLines(_testLogFile);
            Assert.That(lines.Length, Is.GreaterThan(0));
            Assert.That(lines[0], Does.Contain("Type"));
            Assert.That(lines[0], Does.Contain("Timestamp"));
        }

        /// <summary>
        /// Tests Requirements 11.3 and 11.4: Connection and disconnection events are logged as Setting entries
        /// </summary>
        [Test]
        public void ConnectionEvents_WhileLoggingActive_AreLoggedAsSettingEntries()
        {
            // Arrange - start logging before connection
            _logManager.StartLogging(_testLogFile);
            Thread.Sleep(100); // Allow initial entries to be written

            // Act - connect device
            _powerMeterService.Connect("TCPIP::192.168.1.100::INSTR");
            Thread.Sleep(100); // Allow connection entry to be written

            // Disconnect device
            _powerMeterService.Disconnect();
            Thread.Sleep(100); // Allow disconnection entry to be written

            // Stop logging
            _logManager.StopLogging();

            // Assert - check log file contains connection events
            var content = File.ReadAllText(_testLogFile);
            Assert.That(content, Does.Contain("Connection"));
            Assert.That(content, Does.Contain("Connected"));
            Assert.That(content, Does.Contain("Disconnected"));
        }

        /// <summary>
        /// Tests Requirement 11.5: THE Enhanced_Logger SHALL queue all Setting_Entry records 
        /// when logging is inactive and write them when logging starts
        /// </summary>
        [Test]
        public void SettingEntries_QueuedWhenInactive_WrittenWhenLoggingStarts()
        {
            // Arrange - logging is not active
            Assert.That(_logManager.CurrentState, Is.Not.EqualTo(LoggingState.Active));

            // Act - trigger connection event before logging starts (should be queued)
            _powerMeterService.Connect("TCPIP::192.168.1.100::INSTR");
            Thread.Sleep(100);

            // Start logging - queued entries should be written
            _logManager.StartLogging(_testLogFile);
            Thread.Sleep(100);

            // Stop logging
            _logManager.StopLogging();

            // Assert - connection event should be in the log file
            var content = File.ReadAllText(_testLogFile);
            Assert.That(content, Does.Contain("Connection"));
            Assert.That(content, Does.Contain("Connected"));
        }

        /// <summary>
        /// Tests complete workflow: Start logging → Connect → Disconnect → Stop logging
        /// Validates Requirements 11.1, 11.3, 11.4
        /// </summary>
        [Test]
        public void CompleteWorkflow_StartLogging_Connect_Disconnect_StopLogging()
        {
            // Step 1: Start logging (device not connected)
            Assert.That(_powerMeterService.IsConnected, Is.False);
            _logManager.StartLogging(_testLogFile);
            Assert.That(_logManager.CurrentState, Is.EqualTo(LoggingState.Active));
            Thread.Sleep(100);

            // Step 2: Connect device
            _powerMeterService.Connect("TCPIP::192.168.1.100::INSTR");
            Assert.That(_powerMeterService.IsConnected, Is.True);
            Thread.Sleep(100);

            // Step 3: Disconnect device
            _powerMeterService.Disconnect();
            Assert.That(_powerMeterService.IsConnected, Is.False);
            Thread.Sleep(100);

            // Step 4: Stop logging
            _logManager.StopLogging();
            Assert.That(_logManager.CurrentState, Is.EqualTo(LoggingState.Stopped));

            // Assert - verify log file contains all events in order
            var lines = File.ReadAllLines(_testLogFile);
            var content = string.Join("\n", lines);

            // Should contain: Logging Started, Connection Connected, Connection Disconnected, Logging Stopped
            Assert.That(content, Does.Contain("Logging"));
            Assert.That(content, Does.Contain("Started"));
            Assert.That(content, Does.Contain("Connection"));
            Assert.That(content, Does.Contain("Connected"));
            Assert.That(content, Does.Contain("Disconnected"));
            Assert.That(content, Does.Contain("Stopped"));

            // Verify chronological order by checking line positions
            int loggingStartLine = -1;
            int connectedLine = -1;
            int disconnectedLine = -1;
            int loggingStopLine = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Logging") && lines[i].Contains("Started"))
                    loggingStartLine = i;
                if (lines[i].Contains("Connection") && lines[i].Contains("Connected") && !lines[i].Contains("Disconnected"))
                    connectedLine = i;
                if (lines[i].Contains("Connection") && lines[i].Contains("Disconnected"))
                    disconnectedLine = i;
                if (lines[i].Contains("Logging") && lines[i].Contains("Stopped"))
                    loggingStopLine = i;
            }

            Assert.That(loggingStartLine, Is.GreaterThan(0), "Logging start entry not found");
            Assert.That(connectedLine, Is.GreaterThan(loggingStartLine), "Connected entry should come after logging start");
            Assert.That(disconnectedLine, Is.GreaterThan(connectedLine), "Disconnected entry should come after connected");
            Assert.That(loggingStopLine, Is.GreaterThan(disconnectedLine), "Logging stop entry should come last");
        }

        /// <summary>
        /// Tests Requirement 11.7: WHEN logging is active and device is not connected, 
        /// THE Enhanced_Logger SHALL only log Setting entries (no Data entries until device connects)
        /// </summary>
        [Test]
        public void ManualSample_WithoutConnection_DoesNotCrash()
        {
            // Arrange - start logging without connection
            _logManager.StartLogging(_testLogFile);
            Assert.That(_powerMeterService.IsConnected, Is.False);

            // Act - attempt manual sample (should not crash, but won't log data)
            Assert.DoesNotThrow(() => _controller.ManualSample());
            Thread.Sleep(100);

            // Stop logging
            _logManager.StopLogging();

            // Assert - no data entries should be in the log (only Setting entries)
            var content = File.ReadAllText(_testLogFile);
            var lines = File.ReadAllLines(_testLogFile);
            
            // Count data entries (lines starting with "Data,")
            int dataEntryCount = 0;
            foreach (var line in lines)
            {
                if (line.StartsWith("Data,"))
                    dataEntryCount++;
            }

            Assert.That(dataEntryCount, Is.EqualTo(0), "No data entries should be logged when device is not connected");
        }
    }
}
