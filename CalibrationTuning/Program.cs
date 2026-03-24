using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using CalibrationTuning.Controllers;
using Tegam._1830A.DeviceLibrary.Services;
using Tegam._1830A.DeviceLibrary.Communication;
using Tegam._1830A.DeviceLibrary.Commands;
using Tegam._1830A.DeviceLibrary.Parsing;
using Tegam._1830A.DeviceLibrary.Validation;
using Tegam._1830A.DeviceLibrary.Simulation;
using Siglent.SDG6052X.DeviceLibrary.Services;
using SiglentComm = Siglent.SDG6052X.DeviceLibrary.Communication;
using SiglentCmd = Siglent.SDG6052X.DeviceLibrary.Commands;
using SiglentParse = Siglent.SDG6052X.DeviceLibrary.Parsing;
using SiglentVal = Siglent.SDG6052X.DeviceLibrary.Validation;
using SiglentSim = Siglent.SDG6052X.DeviceLibrary.Simulation;

namespace CalibrationTuning
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Configure dependency injection container
            var services = new ServiceCollection();
            ConfigureServices(services);
            
            // Build service provider
            var serviceProvider = services.BuildServiceProvider();
            
            // Resolve and run MainForm
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        /// <summary>
        /// Configures the dependency injection container with all required services.
        /// </summary>
        private static void ConfigureServices(IServiceCollection services)
        {
            // Check for simulation mode (can be set via command line argument or environment variable)
            bool useSimulation = CheckSimulationMode();

            if (useSimulation)
            {
                // Register mock/simulated device library dependencies
                services.AddSingleton<Tegam._1830A.DeviceLibrary.Communication.IVisaCommunicationManager, 
                    Tegam._1830A.DeviceLibrary.Simulation.MockVisaCommunicationManager>();
                services.AddSingleton<SiglentComm.IVisaCommunicationManager, 
                    SiglentSim.MockVisaCommunicationManager>();
            }
            else
            {
                // Register real device library dependencies
                services.AddSingleton<Tegam._1830A.DeviceLibrary.Communication.IVisaCommunicationManager, 
                    Tegam._1830A.DeviceLibrary.Communication.VisaCommunicationManager>();
                services.AddSingleton<SiglentComm.IVisaCommunicationManager, 
                    SiglentComm.VisaCommunicationManager>();
            }

            // Register common device library dependencies (same for both real and simulated)
            services.AddSingleton<IScpiCommandBuilder, ScpiCommandBuilder>();
            services.AddSingleton<IScpiResponseParser, ScpiResponseParser>();
            services.AddSingleton<IInputValidator, InputValidator>();
            services.AddSingleton<SiglentCmd.IScpiCommandBuilder, SiglentCmd.ScpiCommandBuilder>();
            services.AddSingleton<SiglentParse.IScpiResponseParser, SiglentParse.ScpiResponseParser>();
            services.AddSingleton<SiglentVal.IInputValidator, SiglentVal.InputValidator>();
            
            // Register device services
            services.AddSingleton<IPowerMeterService, PowerMeterService>();
            services.AddSingleton<ISignalGeneratorService, SignalGeneratorService>();
            
            // Register controllers
            services.AddSingleton<ITuningController, TuningController>();
            services.AddSingleton<IDataLoggingController, DataLoggingController>();
            services.AddSingleton<IConfigurationController, ConfigurationController>();
            
            // Register MainForm with injected dependencies
            services.AddTransient<MainForm>();
        }

        /// <summary>
        /// Checks if simulation mode should be enabled.
        /// Simulation mode can be enabled by:
        /// 1. Command line argument: --simulate or /simulate
        /// 2. Environment variable: CALIBRATION_SIMULATE=true
        /// </summary>
        private static bool CheckSimulationMode()
        {
            // Check command line arguments
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.Equals("--simulate", StringComparison.OrdinalIgnoreCase) ||
                    arg.Equals("/simulate", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Running in SIMULATION MODE\n\n" +
                        "Mock devices will be used instead of real hardware.",
                        "Simulation Mode",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return true;
                }
            }

            // Check environment variable
            string envVar = Environment.GetEnvironmentVariable("CALIBRATION_SIMULATE");
            if (!string.IsNullOrEmpty(envVar) && 
                (envVar.Equals("true", StringComparison.OrdinalIgnoreCase) || envVar == "1"))
            {
                MessageBox.Show(
                    "Running in SIMULATION MODE\n\n" +
                    "Mock devices will be used instead of real hardware.\n" +
                    "(Set via CALIBRATION_SIMULATE environment variable)",
                    "Simulation Mode",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return true;
            }

            return false;
        }
    }
}
