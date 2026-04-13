using System;
using System.Diagnostics.CodeAnalysis;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Provides module registration functionality.
    /// Used to register all functional modules during application startup.
    /// </summary>
    public interface IModuleRegistrar
    {
        /// <summary>
        /// Registers an already instantiated module.
        /// </summary>
        /// <param name="module">The module instance to register.</param>
        void RegisterModule(IModule module);

        /// <summary>
        /// Registers a module type. The system will automatically instantiate the module.
        /// </summary>
        /// <typeparam name="TModule">The module type, must implement <see cref="IModule"/> and have a public parameterless constructor.</typeparam>
        void RegisterModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TModule>() where TModule : IModule;
    }
}
