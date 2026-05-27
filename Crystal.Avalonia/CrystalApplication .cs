using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Crystal.Avalonia
{
    /// <summary>
    /// The base application class, extending Avalonia's <see cref="Application"/>.
    /// Provides the foundation for modular development and MVVM binding.
    /// </summary>
    /// <remarks>
    /// Usage steps:
    /// <list type="number">
    ///   <item>Define modules: Create classes implementing <see cref="IModule"/>.</item>
    ///   <item>Override <see cref="RegisterModules"/>: Register all functional modules.</item>
    ///   <item>Override <see cref="RegisterServices"/>: Register application-level services (optional).</item>
    ///   <item>Override <see cref="CreateShell"/>: Create the main window or main view.</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyApp : CrystalApplication
    /// {
    ///     public override void RegisterModules(IModuleRegistrar moduleRegistrar)
    ///     {
    ///         moduleRegistrar.RegisterModule&lt;MyModule&gt;();
    ///     }
    ///
    ///     public override void RegisterServices(IServiceCollection services)
    ///     {
    ///         services.AddMvvmTransient&lt;MainView, MainViewModel&gt;();
    ///         services.AddTransient&lt;MainWindow&gt;();
    ///     }
    ///
    ///     public override void CreateShell(IServiceProvider serviceProvider)
    ///     {
    ///         CreateShellFromDi&lt;MainWindow, MainView&gt;(serviceProvider);
    ///     }
    /// }
    /// </code>
    /// </example>
    public class CrystalApplication : Application
    {
        /// <inheritdoc />
        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            var services = new ServiceCollection();
            RegisterServices(services);

            var moduleManager = new ModuleManager();
            services.AddSingleton(moduleManager);
            RegisterModules(moduleManager);
            moduleManager.InitService(services);

            services.AddSingleton(serviceProvider => serviceProvider);

            var serviceProvider = services.BuildServiceProvider();

            if (CrystalOptions.EnableViewModelLocator)
                DataTemplates.Add(new ViewLocator());

            moduleManager.InitModules(serviceProvider);

            MvvmManager.ServiceProvider = serviceProvider;
            CreateShell(serviceProvider);
        }

        /// <summary>
        /// Override this method to register all functional modules for the application.
        /// </summary>
        /// <param name="moduleRegistrar">The module registrar, used to register <see cref="IModule"/> implementations.</param>
        /// <example>
        /// <code>
        /// public override void RegisterModules(IModuleRegistrar moduleRegistrar)
        /// {
        ///     moduleRegistrar.RegisterModule&lt;HomeModule&gt;();
        ///     moduleRegistrar.RegisterModule&lt;SettingsModule&gt;();
        /// }
        /// </code>
        /// </example>
        public virtual void RegisterModules(IModuleRegistrar moduleRegistrar)
        {
        }

        /// <summary>
        /// Override this method to register application-level services.
        /// It is recommended to use <see cref="MvvmManager.AddMvvmTransient{TView, TViewModel}(IServiceCollection)"/>
        /// or <see cref="MvvmManager.AddMvvmSingleton{TView, TViewModel}(IServiceCollection)"/> to register View/ViewModel pairs.
        /// </summary>
        /// <param name="services">The service collection to add application-level services to.</param>
        public virtual void RegisterServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// Override this method to create the main shell (window or view) of the application.
        /// </summary>
        /// <param name="serviceProvider">The service provider, can be used to resolve services registered in <see cref="RegisterServices"/> or modules.</param>
        public virtual void CreateShell(IServiceProvider serviceProvider)
        {
        }

        /// <summary>
        /// Resolves the shell from DI based on <see cref="ApplicationLifetime"/>.
        /// Register shell types explicitly, e.g. <c>services.AddTransient&lt;MainWindow&gt;()</c>.
        /// </summary>
        /// <typeparam name="TWindow">Desktop main window type.</typeparam>
        /// <typeparam name="TView">Single-view main view type.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        protected void CreateShellFromDi<TWindow, TView>(IServiceProvider serviceProvider)
            where TWindow : Window
            where TView : Control
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = serviceProvider.GetRequiredService<TWindow>();
            else if (ApplicationLifetime is IActivityApplicationLifetime factory)
                factory.MainViewFactory = () => serviceProvider.GetRequiredService<TView>();
            else if (ApplicationLifetime is ISingleViewApplicationLifetime single)
                single.MainView = serviceProvider.GetRequiredService<TView>();
        }
    }
}
