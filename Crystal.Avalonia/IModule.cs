using Microsoft.Extensions.DependencyInjection;
using System;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Defines an application module, which is a self-contained unit of functionality.
    /// Modules allow you to organize your application into independent feature areas,
    /// each managing its own services and views.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Override this method to register services that belong to this module.
        /// It is recommended to use <see cref="MvvmManager.AddMvvmBindingTransient{TView, TViewModel}(IServiceCollection)"/>
        /// or <see cref="MvvmManager.AddMvvmBindingSingleton{TView, TViewModel}(IServiceCollection)"/> to register View/ViewModel pairs.
        /// </summary>
        /// <param name="services">The service collection to register services into.</param>
        void RegisterServices(IServiceCollection services);

        /// <summary>
        /// Override this method to perform module initialization logic.
        /// At this point, all services have been registered and can be resolved from <paramref name="serviceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider, can be used to resolve services registered in this module.</param>
        void InitializeModule(IServiceProvider serviceProvider);
    }
}
