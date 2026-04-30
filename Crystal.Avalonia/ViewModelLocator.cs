using Avalonia;
using Avalonia.Controls;
using System;
using System.Diagnostics;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Provides automatic ViewModel binding via an attached property.
    /// Works in conjunction with <see cref="CrystalOptions.EnableViewModelLocator"/>.
    /// </summary>
    /// <remarks>
    /// Usage in XAML:
    /// <code>
    /// &lt;Window xmlns
    /// When <c>AutoWireViewModel</c> is set to <c>True</c>,
    /// the system automatically resolves the corresponding ViewModel from the DI container
    /// and assigns it to the control's <c>DataContext</c>.
    /// </remarks>
    public class ViewModelLocator
    {
        /// <summary>
        /// Identifies the <see cref="AutoWireViewModelProperty"/> attached property registration.
        /// When set to <c>True</c>, the system automatically binds the corresponding ViewModel to the control's DataContext.
        /// </summary>
        public static readonly AttachedProperty<bool> AutoWireViewModelProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("AutoWireViewModel", typeof(ViewModelLocator));

        /// <summary>
        /// Gets the AutoWireViewModel attached property value for the specified control.
        /// </summary>
        /// <param name="element">The control to query.</param>
        /// <returns><c>True</c> if the control has automatic ViewModel binding enabled.</returns>
        public static bool GetAutoWireViewModel(Control element) => element.GetValue(AutoWireViewModelProperty);

        /// <summary>
        /// Sets the AutoWireViewModel attached property value for the specified control.
        /// </summary>
        /// <param name="element">The control to set.</param>
        /// <param name="value">Whether to enable automatic binding.</param>
        public static void SetAutoWireViewModel(Control element, bool? value) => element.SetValue(AutoWireViewModelProperty, value);

        static ViewModelLocator()
        {
            AutoWireViewModelProperty.Changed.AddClassHandler<Control, bool>((view, e) =>
            {
                if (e.NewValue == true)
                {
                    AutoWireBind(view);
                }
            });
        }

        private static void AutoWireBind(Control view)
        {
            if (Design.IsDesignMode) return;

            var viewType = view.GetType();
            var vmType = MvvmManager.GetVmType(viewType);

            if (vmType != null)
            {
                if (MvvmManager.ServiceProvider == null)
                {
                    throw new InvalidOperationException("ServiceProvider is not initialized. Make sure to use CrystalApplication as your base class.");
                }

                var viewModel = MvvmManager.ServiceProvider.GetService(vmType);
                view.DataContext = viewModel;
                if (viewModel is ILifecycleAware lifecycleAware)
                {
                    view.Loaded += View_Loaded;
                    view.Unloaded += View_Unloaded;
                }
            }
            else
            {
                throw new InvalidOperationException($"No ViewModel mapping found for view type {viewType.FullName}. Make sure to register the mapping using AddMvvmTransient, AddMvvmHybrid, or AddMvvmSingleton.");
            }
        }
        private static async void View_Unloaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Control view)
            {
                view.Unloaded -= View_Unloaded;

                if (view.DataContext is ILifecycleAware vm)
                {
                    try
                    {
                        await vm.OnUnloaded();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
            }

        }

        private static async void View_Loaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Control view)
            {
                // 1. 关键：执行一次就立刻取消订阅，防止重复初始化或内存泄漏
                view.Loaded -= View_Loaded;

                if (view.DataContext is ILifecycleAware vm)
                {
                    try
                    {
                        // 2. 调用异步初始化
                        await vm.OnLoadedAsync();
                    }
                    catch (Exception ex)
                    {
                        // 3. 在这里记录日志，开发者就能在调试时看到了
                        Trace.WriteLine(ex);
                    }
                }
            }

        }
    }
}
