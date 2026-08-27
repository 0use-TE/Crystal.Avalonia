using Avalonia.Markup.Xaml;
using Crystal.Avalonia;
using Crystal.AvaloniaExample.ViewModels;
using Crystal.AvaloniaExample.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Crystal.AvaloniaExample;

public partial class App : CrystalApplication
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void RegisterModules(IModuleRegistrar moduleRegistrar)
    {
        moduleRegistrar.RegisterModule<DemoModule>();
    }

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddMvvmTransient<MainView, MainViewModel>();
        services.AddMvvmTransient<OuseView, OuseViewModel>();
    }

    public override void CreateShell(IServiceProvider serviceProvider)
    {
        CreateShell<MainWindow, MainView>();
    }
}
