using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Manages View-to-ViewModel mapping and MVVM binding registration.
    /// </summary>
    /// <remarks>
    /// After registering View/ViewModel pairs using this class, the system will automatically:
    /// <list type="bullet">
    ///   <item>Inject the DataContext via <see cref="ViewModelLocator"/> when <see cref="CrystalOptions.EnableViewModelLocator"/> is enabled</item>
    ///   <item>Resolve the corresponding View via <see cref="ViewLocator"/> based on the ViewModel type</item>
    /// </list>
    /// </remarks>
    public static class MvvmManager
    {
        private static readonly Dictionary<Type, Type> _viewToVm = new();
        private static readonly Dictionary<Type, Type> _vmToView = new();

        /// <summary>
        /// Gets the current application's service provider instance.
        /// Available after <see cref="CrystalApplication.OnFrameworkInitializationCompleted"/> completes.
        /// </summary>
        public static IServiceProvider? ServiceProvider { get; set; }

        private static void RegisterMapping(Type viewType, Type vmType)
        {
            _viewToVm[viewType] = vmType;
            _vmToView[vmType] = viewType;
        }

        /// <summary>
        /// Registers a View and ViewModel pair as Transient.
        /// Both the View and ViewModel are created as new instances each time they are resolved.
        /// </summary>
        /// <typeparam name="TView">The View type, must inherit from <see cref="Control"/>.</typeparam>
        /// <typeparam name="TViewModel">The ViewModel type, can be any class.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <example>
        /// <code>
        /// services.AddMvvmTransient&lt;MainView, MainViewModel&gt;();
        /// </code>
        /// </example>
        public static void AddMvvmTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(this IServiceCollection services)
            where TView : Control where TViewModel : class
        {
            services.AddTransient<TView>();
            services.AddTransient<TViewModel>();
            RegisterMapping(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Registers a View and ViewModel pair as Hybrid.
        /// The View is Transient (new instance each time), while the ViewModel is Singleton (same instance reused).
        /// Useful when you want the ViewModel to maintain state across multiple View instances.
        /// </summary>
        /// <typeparam name="TView">The View type, must inherit from <see cref="Control"/>.</typeparam>
        /// <typeparam name="TViewModel">The ViewModel type, can be any class.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <example>
        /// <code>
        /// services.AddMvvmHybrid&lt;SettingsView, SettingsViewModel&gt;();
        /// </code>
        /// </example>
        public static void AddMvvmHybrid<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(this IServiceCollection services)
            where TView : Control where TViewModel : class
        {
            services.AddTransient<TView>();
            services.AddSingleton<TViewModel>();
            RegisterMapping(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Registers a View and ViewModel pair as Singleton.
        /// Both the View and ViewModel are created once and reused for the lifetime of the application.
        /// Use this when the View and ViewModel represent a single, long-lived instance.
        /// </summary>
        /// <typeparam name="TView">The View type, must inherit from <see cref="Control"/>.</typeparam>
        /// <typeparam name="TViewModel">The ViewModel type, can be any class.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <example>
        /// <code>
        /// services.AddMvvmSingleton&lt;MainView, MainViewModel&gt;();
        /// </code>
        /// </example>
        public static void AddMvvmSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(this IServiceCollection services)
            where TView : Control where TViewModel : class
        {
            services.AddSingleton<TView>();
            services.AddSingleton<TViewModel>();
            RegisterMapping(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Looks up the View type corresponding to the given ViewModel type.
        /// </summary>
        /// <param name="vmType">The ViewModel type.</param>
        /// <returns>The corresponding View type, or <c>null</c> if not found.</returns>
        public static Type? GetViewType(Type vmType) => _vmToView.GetValueOrDefault(vmType);

        /// <summary>
        /// Looks up the ViewModel type corresponding to the given View type.
        /// </summary>
        /// <param name="viewType">The View type.</param>
        /// <returns>The corresponding ViewModel type, or <c>null</c> if not found.</returns>
        public static Type? GetVmType(Type viewType) => _viewToVm.GetValueOrDefault(viewType);
    }
}
