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
    ///   <item>Register the ViewModel in DI (<see cref="AddMvvmTransient{TView, TViewModel}(IServiceCollection)"/> or <see cref="AddMvvmSingleton{TView, TViewModel}(IServiceCollection)"/>)</item>
    ///   <item>Record the View ↔ ViewModel type mapping (<typeparamref name="TView"/> is not registered in DI)</item>
    ///   <item>Inject the DataContext via <see cref="ViewModelLocator"/> when <see cref="CrystalOptions.EnableViewModelLocator"/> is enabled</item>
    ///   <item>Instantiate the corresponding View via <see cref="ViewLocator"/> when using ViewModel-first binding</item>
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
        /// Registers a ViewModel as Transient in DI and records the View ↔ ViewModel mapping.
        /// Only <typeparamref name="TViewModel"/> is added to DI via <c>services.AddTransient&lt;TViewModel&gt;()</c>.
        /// <typeparamref name="TView"/> is used for mapping only; views are created by XAML or <see cref="ViewLocator"/>.
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
            RegisterMapping(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Registers a ViewModel as Singleton in DI and records the View ↔ ViewModel mapping.
        /// Only <typeparamref name="TViewModel"/> is added to DI via <c>services.AddSingleton&lt;TViewModel&gt;()</c>.
        /// <typeparamref name="TView"/> is used for mapping only; views are created by XAML or <see cref="ViewLocator"/>.
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
            services.AddSingleton<TViewModel>();
            RegisterMapping(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Looks up the View type corresponding to the given ViewModel type.
        /// </summary>
        /// <param name="vmType">The ViewModel type.</param>
        /// <returns>The corresponding View type, or <c>null</c> if not found.</returns>
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        [UnconditionalSuppressMessage("Trimming", "IL2073", Justification = "View types are registered via AddMvvm* with PublicConstructors annotation.")]
        public static Type? GetViewType(Type vmType) => _vmToView.GetValueOrDefault(vmType);

        /// <summary>
        /// Looks up the ViewModel type corresponding to the given View type.
        /// </summary>
        /// <param name="viewType">The View type.</param>
        /// <returns>The corresponding ViewModel type, or <c>null</c> if not found.</returns>
        public static Type? GetVmType(Type viewType) => _viewToVm.GetValueOrDefault(viewType);
    }
}
