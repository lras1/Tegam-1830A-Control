using System;
using System.IO;
using CalibrationTuning;
using CalibrationTuning.Controllers;
using CalibrationTuning.Models;
using NUnit.Framework;
using Moq;

namespace CalibrationTuning.Tests.Unit
{
    /// <summary>
    /// Unit tests for MainForm configuration loading and saving.
    /// </summary>
    [TestFixture]
    public class MainFormConfigurationTests
    {
        private Mock<ITuningController> _mockTuningController;
        private Mock<IDataLoggingController> _mockDataLoggingController;
        private Mock<IConfigurationController> _mockConfigurationController;

        [SetUp]
        public void SetUp()
        {
            _mockTuningController = new Mock<ITuningController>();
            _mockDataLoggingController = new Mock<IDataLoggingController>();
            _mockConfigurationController = new Mock<IConfigurationController>();
        }

        [Test]
        public void MainForm_Load_LoadsDeviceConfiguration()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration
            {
                PowerMeterIpAddress = "192.168.1.50",
                SignalGeneratorIpAddress = "192.168.1.51"
            };

            _mockConfigurationController
                .Setup(c => c.LoadDeviceConfiguration())
                .Returns(deviceConfig);

            _mockConfigurationController
                .Setup(c => c.LoadLastParameters())
                .Returns(new TuningParameters());

            _mockConfigurationController
                .Setup(c => c.LoadLastLogPath())
                .Returns(string.Empty);

            // Act
            var form = new MainForm(
                _mockTuningController.Object,
                _mockDataLoggingController.Object,
                _mockConfigurationController.Object);

            form.Show();

            // Assert
            _mockConfigurationController.Verify(c => c.LoadDeviceConfiguration(), Times.Once);
            Assert.AreEqual("192.168.1.50", form.ConnectionPanel.PowerMeterIpAddress);
            Assert.AreEqual("192.168.1.51", form.ConnectionPanel.SignalGeneratorIpAddress);

            form.Close();
        }

        [Test]
        public void MainForm_Load_LoadsTuningParameters()
        {
            // Arrange
            var tuningParams = new TuningParameters
            {
                FrequencyHz = 5000000000,
                InitialVoltage = 1.0,
                TargetPowerDbm = -5.0,
                MaxStdDevDb = 0.3,
                VoltageStepSize = 0.1,
                MinVoltage = 0.05,
                MaxVoltage = 10.0,
                MaxIterations = 200,
                SensorId = 2
            };

            _mockConfigurationController
                .Setup(c => c.LoadDeviceConfiguration())
                .Returns(new DeviceConfiguration());

            _mockConfigurationController
                .Setup(c => c.LoadLastParameters())
                .Returns(tuningParams);

            _mockConfigurationController
                .Setup(c => c.LoadLastLogPath())
                .Returns(string.Empty);

            // Act
            var form = new MainForm(
                _mockTuningController.Object,
                _mockDataLoggingController.Object,
                _mockConfigurationController.Object);

            form.Show();

            // Assert
            _mockConfigurationController.Verify(c => c.LoadLastParameters(), Times.Once);
            var loadedParams = form.TuningPanel.GetTuningParameters();
            Assert.AreEqual(5000000000, loadedParams.FrequencyHz);
            Assert.AreEqual(1.0, loadedParams.InitialVoltage);
            Assert.AreEqual(-5.0, loadedParams.TargetPowerDbm);

            form.Close();
        }

        [Test]
        public void MainForm_FormClosing_SavesDeviceConfiguration()
        {
            // Arrange
            _mockConfigurationController
                .Setup(c => c.LoadDeviceConfiguration())
                .Returns(new DeviceConfiguration());

            _mockConfigurationController
                .Setup(c => c.LoadLastParameters())
                .Returns(new TuningParameters());

            _mockConfigurationController
                .Setup(c => c.LoadLastLogPath())
                .Returns(string.Empty);

            _mockDataLoggingController
                .Setup(c => c.CurrentLogFile)
                .Returns(string.Empty);

            var form = new MainForm(
                _mockTuningController.Object,
                _mockDataLoggingController.Object,
                _mockConfigurationController.Object);

            form.Show();

            // Act
            form.Close();

            // Assert
            _mockConfigurationController.Verify(
                c => c.SaveDeviceConfiguration(It.IsAny<DeviceConfiguration>()), 
                Times.Once);
        }

        [Test]
        public void MainForm_FormClosing_SavesTuningParameters()
        {
            // Arrange
            _mockConfigurationController
                .Setup(c => c.LoadDeviceConfiguration())
                .Returns(new DeviceConfiguration());

            _mockConfigurationController
                .Setup(c => c.LoadLastParameters())
                .Returns(new TuningParameters());

            _mockConfigurationController
                .Setup(c => c.LoadLastLogPath())
                .Returns(string.Empty);

            _mockDataLoggingController
                .Setup(c => c.CurrentLogFile)
                .Returns(string.Empty);

            var form = new MainForm(
                _mockTuningController.Object,
                _mockDataLoggingController.Object,
                _mockConfigurationController.Object);

            form.Show();

            // Act
            form.Close();

            // Assert
            _mockConfigurationController.Verify(
                c => c.SaveParameters(It.IsAny<TuningParameters>()), 
                Times.Once);
        }

        [Test]
        public void MainForm_FormClosing_SavesLogFilePath_WhenAvailable()
        {
            // Arrange
            _mockConfigurationController
                .Setup(c => c.LoadDeviceConfiguration())
                .Returns(new DeviceConfiguration());

            _mockConfigurationController
                .Setup(c => c.LoadLastParameters())
                .Returns(new TuningParameters());

            _mockConfigurationController
                .Setup(c => c.LoadLastLogPath())
                .Returns(string.Empty);

            _mockDataLoggingController
                .Setup(c => c.CurrentLogFile)
                .Returns("C:\\Logs\\test.csv");

            var form = new MainForm(
                _mockTuningController.Object,
                _mockDataLoggingController.Object,
                _mockConfigurationController.Object);

            form.Show();

            // Act
            form.Close();

            // Assert
            _mockConfigurationController.Verify(
                c => c.SaveLastLogPath("C:\\Logs\\test.csv"), 
                Times.Once);
        }

        [Test]
        public void MainForm_FormClosing_DisconnectsDevices()
        {
            // Arrange
            _mockConfigurationController
                .Setup(c => c.LoadDeviceConfiguration())
                .Returns(new DeviceConfiguration());

            _mockConfigurationController
                .Setup(c => c.LoadLastParameters())
                .Returns(new TuningParameters());

            _mockConfigurationController
                .Setup(c => c.LoadLastLogPath())
                .Returns(string.Empty);

            _mockDataLoggingController
                .Setup(c => c.CurrentLogFile)
                .Returns(string.Empty);

            var form = new MainForm(
                _mockTuningController.Object,
                _mockDataLoggingController.Object,
                _mockConfigurationController.Object);

            form.Show();

            // Act
            form.Close();

            // Assert
            _mockTuningController.Verify(c => c.DisconnectDevices(), Times.Once);
        }
    }
}
