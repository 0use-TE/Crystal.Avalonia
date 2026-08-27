using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Holds View-to-ViewModel mappings and the application <see cref="IServiceProvider"/>
    /// for a single <see cref="CrystalApplication"/> instance.
    /// </summary>
    /// <remarks>
    /// After registering View/ViewModel pairs via <see cref="MvvmServiceCollectionExtensions.AddMvvmTransient{TView, TViewModel}(IServiceCollection)"/>
    /// or <see cref="MvvmServiceCollectionExtensions.AddMvvmSingleton{TView, TViewModel}(IServiceCollection)"/>, the system will:
    /// <list type="bullet">
    ///   <item>Register the ViewModel in DI</item>
    ///   <item>Record the View ↔ ViewModel type mapping (<c>TView</c> is not registered in DI)</item>
    ///   <item>Inject the DataContext via <see cref="ViewModelLocator"/> when <c>AutoWireViewModel</c> is set</item>
    ///   <item>Instantiate the corresponding View via ViewLocator when <see cref="CrystalOptions.EnableViewLocator"/> is enabled</item>
    /// </list>
    /// </remarks>
    public sealed class MvvmManager
    {
        private readonly Dictionary<Type, Type> _viewToVm = new();
        private readonly Dictionary<Type, Type> _vmToView = new();

        /// <summary>
        /// Gets or sets the current application's service provider.
        /// Assigned by <see cref="CrystalApplication"/> after the container is built, before module initialization.
        /// </summary>
        public IServiceProvider? ServiceProvider { get; set; }

        internal void RegisterMapping(Type viewType, Type vmType)
        {
            _viewToVm[viewType] = vmType;
            _vmToView[vmType] = viewType;
        }

        /// <summary>
        /// Looks up the View type corresponding to the given ViewModel type.
        /// </summary>
        /// <param name="vmType">The ViewModel type.</param>
        /// <returns>The corresponding View type, or <c>null</c> if not found.</returns>
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        [UnconditionalSuppressMessage("Trimming", "IL2073", Justification = "View types are registered via AddMvvm* with PublicConstructors annotation.")]
        public Type? GetViewType(Type vmType) => _vmToView.GetValueOrDefault(vmType);

        /// <summary>
        /// Looks up the ViewModel type corresponding to the given View type.
        /// </summary>
        /// <param name="viewType">The View type.</param>
        /// <returns>The corresponding ViewModel type, or <c>null</c> if not found.</returns>
        public Type? GetVmType(Type viewType) => _viewToVm.GetValueOrDefault(viewType);
    }

    /// <summary>
    /// Extension methods for registering View/ViewModel pairs on <see cref="IServiceCollection"/>.
    /// </summary>
    public static class MvvmServiceCollectionExtensions
    {
        internal static MvvmManager GetOrAddMvvmManager(this IServiceCollection services)
        {
            foreach (var descriptor in services)
            {
                if (descriptor.ServiceType == typeof(MvvmManager) &&
                    descriptor.ImplementationInstance is MvvmManager existing)
                {
                    return existing;
                }
            }

            var created = new MvvmManager();
            services.AddSingleton(created);
            return created;
        }

        /// <summary>
        /// Registers a ViewModel as Transient in DI and records the View ↔ ViewModel mapping.
        /// Only <typeparamref name="TViewModel"/> is added to DI via <c>services.AddTransient&lt;TViewModel&gt;()</c>.
        /// <typeparamref name="TView"/> is used for mapping only; views are created by XAML or ViewLocator.
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
            services.AddTransient<TViewModel>();
            services.GetOrAddMvvmManager().RegisterMapping(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Registers a ViewModel as Singleton in DI and records the View ↔ ViewModel mapping.
        /// Only <typeparamref name="TViewModel"/> is added to DI via <c>services.AddSingleton&lt;TViewModel&gt;()</c>.
        /// <typeparamref name="TView"/> is used for mapping only; views are created by XAML or ViewLocator.
        /// </summary>
        /// <typeparam name="TView">The View type, must inherit from <see cref="Control"/>.</typeparam>
        /// <typeparam name="TViewModel">The ViewModel type, can be any class.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <example>
        /// <code>
        /// services.AddMvvmSingleton&lt;SettingsView, SettingsViewModel&gt;();
        /// </code>
        /// </example>
        public static void AddMvvmSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(this IServiceCollection services)
            where TView : Control where TViewModel : class
        {
            services.AddSingleton<TViewModel>();
            services.GetOrAddMvvmManager().RegisterMapping(typeof(TView), typeof(TViewModel));
        }
    }
}
