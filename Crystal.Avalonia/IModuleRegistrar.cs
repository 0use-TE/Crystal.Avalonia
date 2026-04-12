using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Crystal.Avalonia
{
    public interface IModuleRegistrar
    {
        void RegisterModule(IModule module);
        void RegisterModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TModule>() where TModule : IModule;
    }
}
