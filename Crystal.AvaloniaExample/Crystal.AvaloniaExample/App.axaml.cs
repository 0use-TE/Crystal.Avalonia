using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Crystal.Avalonia;
using Crystal.AvaloniaExample.ViewModels;
using Crystal.AvaloniaExample.Views;
using Microsoft.Extensions.DependencyInjection;
using ModuleAExample;
using ModuleBExample;
using System;
using System.Linq;

namespace Crystal.AvaloniaExample
{
    public partial class App : CrystalApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public override void RegisterModules(IModuleRegistrar moduleRegistrar)
        {
            moduleRegistrar.RegisterModule<ModuleA>();
            moduleRegistrar.RegisterModule<ModuleB>();
        }
        public override void RegisterServices(IServiceCollection services)
        {
            services.AddMvvmTransient<MainView, MainViewModel>();
            services.AddMvvmTransient<OuseView, OuseViewModel>();
        }
        public override void CreateShell(IServiceProvider serviceProvider)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow();
            else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
                singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView();
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
                singleViewPlatform.MainView = new MainView();
        }
    }
}