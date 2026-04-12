using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Crystal.Avalonia
{
    public static class MvvmManager
    {
        // View -> VM (用于自动给 DataContext 赋值)
        private static readonly Dictionary<Type, Type> _viewToVm = new();
        // VM -> View (用于根据 VM 实例生成 UI)
        private static readonly Dictionary<Type, Type> _vmToView = new();

        public static IServiceProvider? ServiceProvider { get; set; }

        private static void RegisterMapping(Type viewType, Type vmType)
        {
            _viewToVm[viewType] = vmType;
            _vmToView[vmType] = viewType;
        }

        public static void AddMvvmBindingTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(this IServiceCollection services)
            where TView : Control where TViewModel : class
        {
            services.AddTransient<TView>();
            services.AddTransient<TViewModel>();
            RegisterMapping(typeof(TView), typeof(TViewModel));
        }

        public static void AddMvvmBindingSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(this IServiceCollection services)
            where TView : Control where TViewModel : class
        {
            services.AddTransient<TView>();
            services.AddSingleton<TViewModel>();
            RegisterMapping(typeof(TView), typeof(TViewModel));
        }
        // 根据 VM 类型获取 View 类型
        public static Type? GetViewType(Type vmType) => _vmToView.GetValueOrDefault(vmType);

        // 根据 View 类型获取 VM 类型
        public static Type? GetVmType(Type viewType) => _viewToVm.GetValueOrDefault(viewType);
    }
}
