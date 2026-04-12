using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crystal.Avalonia
{
    public class ViewModelLocator
    {
        public static readonly AttachedProperty<bool> AutoWireViewModelProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("AutoWireViewModel", typeof(ViewModelLocator));

        public static bool GetAutoWireViewModel(Control element) => element.GetValue(AutoWireViewModelProperty);
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
            // 如果是设计器模式，不执行逻辑，防止预览报错
            if (Design.IsDesignMode) return;

            var viewType = view.GetType();
            var vmType = MvvmManager.GetVmType(viewType);

            if (vmType != null)
            {
                if (MvvmManager.ServiceProvider == null)
                {
                    throw new InvalidOperationException("ServiceProvider is not initialized. Make sure to set it in your DIApplication.");
                }

                // 从 DI 容器（ServiceProvider）中请求实例
                var viewModel = MvvmManager.ServiceProvider.GetService(vmType);

                view.DataContext = viewModel;
            }
            else
            {
                throw new InvalidOperationException($"No ViewModel mapping found for view type {viewType.FullName}. Make sure to register the mapping using AddMvvmBindingTransient or AddMvvmBindingSingleton.");
            }
        }
    }
}
