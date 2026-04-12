using Avalonia;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystal.Avalonia
{
    public class CrystalApplication : Application
    {

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();
            ServiceCollection services = new ServiceCollection();
            RegisterServices(services);

            ModuleManager moduleManager = new ModuleManager();
            services.AddSingleton(moduleManager);
            RegisterModules(moduleManager);
            //InitModule
            moduleManager.InitService(services);

            //AddServiceProvider
            services.AddSingleton(serviceProvider => serviceProvider);

            var serviceProvider = services.BuildServiceProvider();

            if (CrystalOptions.EnableViewModelLocator)
                DataTemplates.Add(new ViewLocator());

            moduleManager.InitModules(serviceProvider);

            MvvmManager.ServiceProvider = serviceProvider;
            CreateShell(serviceProvider);

        }
        public virtual void RegisterModules(IModuleRegistrar moduleRegistrar)
        {

        }

        public virtual void RegisterServices(IServiceCollection services)
        {

        }

        public virtual void CreateShell(IServiceProvider serviceProvider)
        {

        }

    }
}
