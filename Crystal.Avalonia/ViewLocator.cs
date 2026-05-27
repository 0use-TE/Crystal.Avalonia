using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Crystal.Avalonia
{
    internal class ViewLocator : IDataTemplate
    {
        public Control? Build(object? param)
        {
            if (param == null) return null;

            var vmType = param.GetType();
            var viewType = MvvmManager.GetViewType(vmType);

            if (viewType == null)
            {
                return new TextBlock { Text = $"No mapping registered for: {vmType.Name}" };
            }

            var view = CreateView(viewType);

            if (view != null)
            {
                view.DataContext = param;
                ViewLifecycleBinder.AttachIfNeeded(view, param);
                return view;
            }

            return new TextBlock { Text = $"Unable to create view: {viewType.Name}" };
        }

        private static Control? CreateView(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type viewType)
        {
            return Activator.CreateInstance(viewType) as Control;
        }

        public bool Match(object? data)
        {
            return data != null && MvvmManager.GetViewType(data.GetType()) != null;
        }
    }
}
