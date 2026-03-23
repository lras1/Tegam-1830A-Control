using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using CalibrationTuning.Controllers;
using Tegam._1830A.DeviceLibrary.Services;
using Tegam._1830A.DeviceLibrary.Communication;
using Tegam._1830A.DeviceLibrary.Commands;
using Tegam._1830A.DeviceLibrary.Parsing;
using Tegam._1830A.DeviceLibrary.Validation;
using Siglent.SDG6052X.DeviceLibrary.Services;
using SiglentComm = Siglent.SDG6052X.DeviceLibrary.Communication;
using SiglentCmd = Siglent.SDG6052X.DeviceLibrary.Commands;
using SiglentParse = Siglent.SDG6052X.DeviceLibrary.Parsing;
using SiglentVal = Siglent.SDG6052X.DeviceLibrary.Validation;

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
            // Register Tegam 1830A device library dependencies
            services.AddSingleton<Tegam._1830A.DeviceLibrary.Communication.IVisaCommunicationManager, 
                Tegam._1830A.DeviceLibrary.Communication.VisaCommunicationManager>();
            services.AddSingleton<IScpiCommandBuilder, ScpiCommandBuilder>();
            services.AddSingleton<IScpiResponseParser, ScpiResponseParser>();
            services.AddSingleton<IInputValidator, InputValidator>();
            
            // Register Siglent SDG6052X device library dependencies
            services.AddSingleton<SiglentComm.IVisaCommunicationManager, 
                SiglentComm.VisaCommunicationManager>();
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
    }
}
