using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Crystal.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        /// <inheritdoc cref="CrystalApplication.RegisterModules"/>
        public override void RegisterModules(IModuleRegistrar moduleRegistrar)
        {
            // Example: Register business modules
            // moduleRegistrar.RegisterModule<MyModule>();
        }

        /// <inheritdoc cref="CrystalApplication.RegisterServices"/>
        public override void RegisterServices(IServiceCollection services)
        {
            // Register View and ViewModel mapping
            services.AddMvvmBindingTransient<MainView, MainViewModel>();
        }

        /// <inheritdoc cref="CrystalApplication.CreateShell"/>
        public override void CreateShell(IServiceProvider serviceProvider)
        {
            // Create the main window or main view based on application lifetime
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow();
            else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
                singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView();
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
                singleViewPlatform.MainView = new MainView();
        }
    }
}
