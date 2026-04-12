using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystal.Avalonia
{
    public interface IModule
    {
        void RegisterServices(IServiceCollection services);
        void InitializeModule(IServiceProvider serviceProvider);
    }
}
