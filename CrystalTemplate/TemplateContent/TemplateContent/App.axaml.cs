using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Crystal.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TemplateContent.ViewModels;
using TemplateContent.Views;

namespace TemplateContent
{
    public partial class App : CrystalApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public override void RegisterModules(IModuleRegistrar moduleRegistrar)
        {
        }
        public override void RegisterServices(IServiceCollection services)
        {
            services.AddMvvmBindingTransient<MainView, MainViewModel>();
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