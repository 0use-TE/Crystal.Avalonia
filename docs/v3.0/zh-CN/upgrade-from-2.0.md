# 从 2.0 升级

Crystal.Avalonia **3.0.0** 仍是仅 ViewModel 进 DI，以及 `CreateShell<TWindow, TView>()`。以下 API 有变化。

## 变更

| 2.0.1 | 3.0.0 |
|-------|-------|
| 静态 `MvvmManager` 字典和 `MvvmManager.ServiceProvider` | 实例在 `CrystalApplication.Mvvm`；`AddMvvm*` 写入该实例 |
| `CrystalOptions.EnableViewModelLocator` | **`EnableViewLocator`** — 只控制是否把 `ViewLocator` 加入 `DataTemplates`，不控制 `AutoWireViewModel` |
| `OnLoadedAsync()` / Loaded 与 Unloaded 各触发一次 | 每次 Loaded 调用 `OnLoadedAsync(bool isFirstLoad)`；每次 Unload 调用 `OnUnloaded`。`isFirstLoad` 按 ViewModel 实例 |
| `ServiceProvider` 在 `InitModules` 之后赋值 | 在 **`InitModules` 之前**赋值，模块初始化时 AutoWire 可用 |

`AddMvvmTransient` / `AddMvvmSingleton` 的调用写法不变。

## 迁移步骤

### 1. 重命名选项

```csharp
// 2.0
CrystalOptions.EnableViewModelLocator = false;

// 3.0
CrystalOptions.EnableViewLocator = false;
```

### 2. 更新 `ILifecycleAware`

```csharp
public Task OnLoadedAsync(bool isFirstLoad)
{
    if (!isFirstLoad) return Task.CompletedTask;
    return LoadDataAsync();
}

public Task OnUnloaded() => Task.CompletedTask;
```

### 3. 不要再使用静态 `MvvmManager.ServiceProvider`

使用 `CreateShell` / `InitializeModule` 传入的 `IServiceProvider`，或当前 `CrystalApplication` 的 `Mvvm.ServiceProvider`。

```csharp
var mvvm = ((CrystalApplication)Application.Current!).Mvvm;
```

### 包 / 模板

```bash
dotnet add package Crystal.Avalonia --version 3.0.0
dotnet new install CrystalTemplate::3.0.0
```

## 3.0.0 未改动部分

- 通过 `AddMvvmTransient` / `AddMvvmSingleton` 仅 ViewModel 进 DI
- Shell 用 `CreateShell<MainWindow, MainView>()`（`new`，不进 DI）
- `ViewModelLocator.AutoWireViewModel`
- `IModule`（仍无扫描，无内置导航或事件聚合器）
- AOT 注解
