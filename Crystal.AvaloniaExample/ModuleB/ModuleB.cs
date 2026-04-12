using Crystal.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModuleBExample
{
    public class ModuleB : IModule
    {
        public void InitializeModule(IServiceProvider serviceProvider)
        {

        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddMvvmBindingTransient<ModuleBView, ModuleBViewModel>();
        }
    }
}
