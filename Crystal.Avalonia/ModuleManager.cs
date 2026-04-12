using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Crystal.Avalonia
{
    public class ModuleManager : IModuleRegistrar
    {
        public IList<IModule> LoadModules { get; set; } = new List<IModule>();

        public void RegisterModule(IModule module)
        {
            LoadModules.Add(module);
        }
        public void RegisterModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TModule>() where TModule : IModule
        {
            var module = Activator.CreateInstance<TModule>();
            LoadModules.Add(module);
        }
        public void InitService(IServiceCollection services)
        {
            foreach (var module in LoadModules)
            {
                module.RegisterServices(services);
            }
        }
        public void InitModules(IServiceProvider serviceProvider)
        {
            foreach (var module in LoadModules)
            {
                module.InitializeModule(serviceProvider);
            }
        }


    }
}
