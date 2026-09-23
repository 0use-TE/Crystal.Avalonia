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
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void ConfigureOptions(CrystalOptions options)
        {
            options.EnableViewLocator = true; // default
        }

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddMvvmTransient<MainView, MainViewModel>();
        }

        public override void CreateShell(IServiceProvider serviceProvider)
        {
            CreateShell<MainWindow, MainView>();
        }
    }
}
