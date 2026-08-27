using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Crystal.Avalonia
{
    /// <summary>
    /// Provides automatic ViewModel binding via an attached property.
    /// Independent of <see cref="CrystalOptions.EnableViewLocator"/>, which only gates ViewModel-first view location.
    /// </summary>
    /// <remarks>
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

            if (Application.Current is not CrystalApplication app)
            {
                throw new InvalidOperationException("ServiceProvider is not initialized. Make sure to use CrystalApplication as your base class.");
            }

            var mvvm = app.Mvvm;
            var viewType = view.GetType();
            var vmType = mvvm.GetVmType(viewType);

            if (vmType != null)
            {
                if (mvvm.ServiceProvider == null)
                {
                    throw new InvalidOperationException("ServiceProvider is not initialized. Make sure to use CrystalApplication as your base class.");
                }

                var viewModel = mvvm.ServiceProvider.GetRequiredService(vmType);
                view.DataContext = viewModel;
                ViewLifecycleBinder.AttachIfNeeded(view, viewModel);
            }
            else
            {
                throw new InvalidOperationException($"No ViewModel mapping found for view type {viewType.FullName}. Register the pair with AddMvvmTransient or AddMvvmSingleton.");
            }
        }
    }
}
