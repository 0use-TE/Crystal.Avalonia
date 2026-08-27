using Avalonia;
using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Crystal.Avalonia
{
    internal static class ViewLifecycleBinder
    {
        private static readonly AttachedProperty<bool> IsAttachedProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("IsAttached", typeof(ViewLifecycleBinder));

        private static readonly ConditionalWeakTable<object, StrongBox<bool>> FirstLoadFlags = new();

        public static void AttachIfNeeded(Control view, object? dataContext)
        {
            if (dataContext is not ILifecycleAware)
                return;

            if (view.GetValue(IsAttachedProperty))
                return;

            view.SetValue(IsAttachedProperty, true);
            view.Loaded += OnLoaded;
            view.Unloaded += OnUnloaded;
        }

        private static async void OnLoaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is not Control view)
                return;

            if (view.DataContext is not ILifecycleAware vm)
                return;

            var isFirstLoad = true;
            if (FirstLoadFlags.TryGetValue(vm, out var box))
            {
                isFirstLoad = !box.Value;
                box.Value = true;
            }
            else
            {
                FirstLoadFlags.Add(vm, new StrongBox<bool>(true));
            }

            await InvokeSafely(() => vm.OnLoadedAsync(isFirstLoad));
        }

        private static async void OnUnloaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is not Control view)
                return;

            if (view.DataContext is ILifecycleAware vm)
                await InvokeSafely(vm.OnUnloaded);
        }

        private static async Task InvokeSafely(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}
