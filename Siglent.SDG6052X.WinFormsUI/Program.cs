using System;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Siglent.SDG6052X.DeviceLibrary.Services;
using Siglent.SDG6052X.DeviceLibrary.Commands;
using Siglent.SDG6052X.DeviceLibrary.Communication;
using Siglent.SDG6052X.DeviceLibrary.Parsing;
using Siglent.SDG6052X.DeviceLibrary.Validation;
using Siglent.SDG6052X.DeviceLibrary.Simulation;
using Siglent.SDG6052X.WinFormsUI.Forms;

namespace Siglent.SDG6052X.WinFormsUI
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
            
            // Configure dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            
            // Get MainForm from DI container
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            
            Application.Run(mainForm);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Check if simulation mode is enabled
            bool useSimulation = false;
            string simulationSetting = ConfigurationManager.AppSettings["UseSimulation"];
            if (!string.IsNullOrEmpty(simulationSetting))
            {
                bool.TryParse(simulationSetting, out useSimulation);
            }
            
            // Register Device Library components
            if (useSimulation)
            {
                services.AddSingleton<IVisaCommunicationManager, MockVisaCommunicationManager>();
            }
            else
            {
                services.AddSingleton<IVisaCommunicationManager, VisaCommunicationManager>();
            }
            
            services.AddSingleton<IScpiCommandBuilder, ScpiCommandBuilder>();
            services.AddSingleton<IScpiResponseParser, ScpiResponseParser>();
            services.AddSingleton<IInputValidator, InputValidator>();
            services.AddSingleton<ISignalGeneratorService, SignalGeneratorService>();
            
            // Register MainForm
            services.AddTransient<MainForm>();
        }
    }
}
