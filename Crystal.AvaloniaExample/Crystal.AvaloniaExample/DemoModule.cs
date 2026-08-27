using Crystal.Avalonia;
using Crystal.AvaloniaExample.ViewModels;
using Crystal.AvaloniaExample.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Crystal.AvaloniaExample;

public class DemoModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<ModuleAView, ModuleAViewModel>();
        services.AddMvvmTransient<ModuleBView, ModuleBViewModel>();
    }

    public void InitializeModule(IServiceProvider serviceProvider)
    {
    }
}
