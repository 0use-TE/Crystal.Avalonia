using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Crystal.Avalonia
{
    internal static class ViewLifecycleBinder
    {
        public static void AttachIfNeeded(Control view, object? dataContext)
        {
            if (dataContext is not ILifecycleAware)
                return;

            view.Loaded += OnLoaded;
            view.Unloaded += OnUnloaded;
        }

        private static async void OnLoaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is not Control view)
                return;

            view.Loaded -= OnLoaded;

            if (view.DataContext is ILifecycleAware vm)
                await InvokeSafely(vm.OnLoadedAsync);
        }

        private static async void OnUnloaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is not Control view)
                return;

            view.Unloaded -= OnUnloaded;

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
