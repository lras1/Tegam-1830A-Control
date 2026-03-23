using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using CalibrationTuning.Controllers;
using Tegam._1830A.DeviceLibrary.Services;
using Siglent.SDG6052X.DeviceLibrary.Services;

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
    }
}
