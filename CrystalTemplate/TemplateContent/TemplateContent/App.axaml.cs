using Avalonia;
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

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddMvvmTransient<MainView, MainViewModel>();
            services.AddTransient<MainWindow>();
            services.AddTransient<MainView>();
        }

        public override void CreateShell(IServiceProvider serviceProvider)
        {
            CreateShellFromDi<MainWindow, MainView>(serviceProvider);
        }
    }
}
