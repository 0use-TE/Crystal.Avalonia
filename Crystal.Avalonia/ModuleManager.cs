using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Manages and initializes application modules.
    /// Automatically created and used by the framework during <see cref="CrystalApplication.OnFrameworkInitializationCompleted"/>.
    /// </summary>
    public class ModuleManager : IModuleRegistrar
    {
        /// <summary>
        /// Gets the list of registered modules.
        /// </summary>
        public IList<IModule> LoadModules { get; set; } = new List<IModule>();

        /// <inheritdoc />
        public void RegisterModule(IModule module)
        {
            LoadModules.Add(module);
        }

        /// <inheritdoc />
        public void RegisterModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TModule>() where TModule : IModule
        {
            var module = Activator.CreateInstance<TModule>();
            LoadModules.Add(module);
        }

        /// <summary>
        /// Iterates through all registered modules and calls their <see cref="IModule.RegisterServices"/> method
        /// to register services into the container.
        /// Automatically invoked by the framework before the service provider is built.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void InitService(IServiceCollection services)
        {
            foreach (var module in LoadModules)
            {
                module.RegisterServices(services);
            }
        }

        /// <summary>
        /// Iterates through all registered modules and calls their <see cref="IModule.InitializeModule"/> method
        /// to perform module initialization.
        /// Automatically invoked by the framework after the service provider is built.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public void InitModules(IServiceProvider serviceProvider)
        {
            foreach (var module in LoadModules)
            {
                module.InitializeModule(serviceProvider);
            }
        }
    }
}
