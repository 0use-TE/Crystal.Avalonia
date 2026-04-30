using Crystal.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModuleAExample
{
    public class ModuleA : IModule
    {
        public void InitializeModule(IServiceProvider serviceProvider)
        {
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddMvvmTransient<ModuleAView, ModuleAViewModel>();
        }
    }
}
