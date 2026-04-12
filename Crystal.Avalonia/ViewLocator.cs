using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Crystal.Avalonia
{
    internal class ViewLocator : IDataTemplate
    {
        public Control? Build(object? param)
        {
            if (param == null) return null;

            // 1. 这里的 param 实际上是 ViewModel 的实例
            var vmType = param.GetType();

            // 2. 查字典：VM 类型 -> View 类型
            var viewType = MvvmManager.GetViewType(vmType);

            if (viewType == null)
            {
                
                return new TextBlock { Text = $"未注册映射: {vmType.Name}" };
            }

            // 3. 从 DI 容器实例化 View
            var view = MvvmManager.ServiceProvider?.GetService(viewType) as Control;

            if (view != null)
            {
                // 4. 重要：手动把传进来的 VM 实例挂上去
                // 这样就不用管 View 里的 AutoWireViewModel="True" 了，
                view.DataContext = param;
                return view;
            }

            return new TextBlock { Text = $"无法创建 View: {viewType.Name}" };
        }

        public bool Match(object? data)
        {
            // 只有在我们字典里备过案的 ViewModel，才让这个 Locator 接手
            return data != null && MvvmManager.GetViewType(data.GetType()) != null;
        }
    }
}
