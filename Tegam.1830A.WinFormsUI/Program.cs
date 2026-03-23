using System;
using System.Windows.Forms;
using Tegam._1830A.DeviceLibrary.Communication;
using Tegam._1830A.DeviceLibrary.Commands;
using Tegam._1830A.DeviceLibrary.Parsing;
using Tegam._1830A.DeviceLibrary.Services;
using Tegam._1830A.DeviceLibrary.Validation;
using Tegam._1830A.DeviceLibrary.Simulation;
using Tegam.WinFormsUI.Forms;
using Tegam.WinFormsUI.Controllers;

namespace Tegam.WinFormsUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Create services manually (without DI container)
                var communicationManager = new MockVisaCommunicationManager();
                var commandBuilder = new ScpiCommandBuilder();
                var responseParser = new ScpiResponseParser();
                var validator = new InputValidator();
                var powerMeterService = new PowerMeterService(communicationManager, commandBuilder, responseParser, validator);

                // Create controllers
                var mainFormController = new MainFormController(powerMeterService);
                var powerMeasurementController = new PowerMeasurementController(powerMeterService);
                var frequencyConfigurationController = new FrequencyConfigurationController(powerMeterService);
                var sensorManagementController = new SensorManagementController(powerMeterService);
                var calibrationController = new CalibrationController(powerMeterService);
                
                // Create LogManager and EnhancedLoggingController
                var logManager = new Tegam._1830A.DeviceLibrary.Logging.LogManager();
                var enhancedLoggingController = new EnhancedLoggingController(
                    powerMeterService,
                    logManager,
                    frequencyConfigurationController,
                    sensorManagementController,
                    calibrationController,
                    powerMeasurementController);

                // Create panels
                var powerMeasurementPanel = new PowerMeasurementPanel(powerMeasurementController, enhancedLoggingController);
                var frequencyConfigurationPanel = new FrequencyConfigurationPanel(frequencyConfigurationController, enhancedLoggingController);
                var sensorManagementPanel = new SensorManagementPanel(sensorManagementController, enhancedLoggingController);
                var calibrationPanel = new CalibrationPanel(calibrationController, enhancedLoggingController);
                var enhancedLoggingPanel = new EnhancedLoggingPanel(enhancedLoggingController, logManager);

                // Create and run main form
                var mainForm = new MainForm(
                    powerMeterService,
                    mainFormController,
                    powerMeasurementPanel,
                    frequencyConfigurationPanel,
                    sensorManagementPanel,
                    calibrationPanel,
                    enhancedLoggingPanel);

                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application error: {ex.Message}\n\n{ex.StackTrace}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
