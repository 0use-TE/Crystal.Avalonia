# Architecture

How Crystal.Avalonia works internally. For usage, see [Getting Started](getting-started.md) and [Tutorials](tutorials/mvvm-pattern.md).

<a id="bootstrap-pipeline"></a>
## Bootstrap Pipeline

`CrystalApplication.OnFrameworkInitializationCompleted()` runs this sequence:

```
CrystalApplication.Mvvm created
    ↓
services.AddSingleton(Mvvm)
RegisterServices(services)          ← App-level DI (AddMvvm writes mappings on Mvvm)
    ↓
ModuleManager created + registered
RegisterModules(moduleRegistrar)    ← App registers IModule instances
    ↓
moduleManager.InitService(services) ← Each module.RegisterServices()
    ↓
services.BuildServiceProvider()
    ↓
Mvvm.ServiceProvider = sp           ← before InitModules (ViewModelLocator can resolve)
    ↓
ViewLocator added to DataTemplates  ← if EnableViewLocator
    ↓
moduleManager.InitModules(sp)       ← Each module.InitializeModule()
    ↓
CreateShell(sp)                     ← App creates MainWindow / MainView
```

Key points:

- **App runs first**, then modules — `RegisterServices` in `App` executes before `InitService`.
- **Single `ServiceProvider`** — built once; modules initialize after it exists.
- **`CrystalApplication.Mvvm.ServiceProvider`** — set before `InitModules` and `CreateShell`, required by `ViewModelLocator`.
- **One `MvvmManager` per app** — mappings live on the application instance, not process-wide static dictionaries.

<a id="module-system"></a>
## Module System

`ModuleManager` implements `IModuleRegistrar`:

| Phase | Method | When |
|-------|--------|------|
| Register | `RegisterModule<T>()` | Before container build; `Activator.CreateInstance<T>()` |
| Services | `IModule.RegisterServices()` | During `InitService`, before `BuildServiceProvider` |
| Init | `IModule.InitializeModule()` | After container build, during `InitModules` |

Modules are plain classes — no assembly scanning. Each module is explicitly registered in `RegisterModules`. The library does not provide module dependency graphs, lazy loading, navigation, or an event aggregator.

<a id="mvvm-wiring"></a>
## MVVM Wiring

### Type Mapping (`MvvmManager`)

`AddMvvmTransient` / `AddMvvmSingleton` do two things:

1. Register **ViewModel only** in DI (`AddTransient` / `AddSingleton`)
2. Record View ↔ ViewModel mapping on the `MvvmManager` instance registered in the `IServiceCollection` (the same instance as `CrystalApplication.Mvvm`)

`TView` is never added to `IServiceCollection`.

### View-First (`ViewModelLocator`)

Attached property `AutoWireViewModel="True"` triggers on property change. This does **not** depend on `CrystalOptions.EnableViewLocator`.

```
View loaded in XAML
    ↓
ViewModelLocator reads view.GetType()
    ↓
CrystalApplication.Mvvm.GetVmType(viewType)
    ↓
ServiceProvider.GetRequiredService(vmType)
    ↓
view.DataContext = viewModel
    ↓
ViewLifecycleBinder.AttachIfNeeded()
```

Skipped in design mode (`Design.IsDesignMode`).

### ViewModel-First (`ViewLocator`)

Registered as `IDataTemplate` on `Application.DataTemplates` when `CrystalOptions.EnableViewLocator` is `true` (default):

```
ContentControl.Content = viewModel
    ↓
ViewLocator.Match(vm) — mapping exists?
    ↓
Mvvm.GetViewType(vmType)
    ↓
Activator.CreateInstance(viewType)   ← not from DI
    ↓
view.DataContext = viewModel
    ↓
ViewLifecycleBinder.AttachIfNeeded()
```

`ViewLocator` only matches types with a registered mapping.

### Lifecycle (`ViewLifecycleBinder`)

When DataContext implements `ILifecycleAware`:

- Subscribes to View `Loaded` / `Unloaded` and keeps the subscription
- Every `Loaded` calls `OnLoadedAsync(isFirstLoad)` — `isFirstLoad` is per **ViewModel instance** (`true` the first time that object is loaded)
- Every `Unloaded` calls `OnUnloaded()`
- Exceptions are caught and written to `Trace`

Used by both `ViewModelLocator` and `ViewLocator`.

## Shell Creation

Shell views (`MainWindow`, `MainView`) are **not** in DI — created with `new`:

```csharp
public override void CreateShell(IServiceProvider sp)
{
    CreateShell<MainWindow, MainView>();
}
```

`CreateShell<TWindow, TView>()` assigns by `ApplicationLifetime`:

| Lifetime | Action |
|----------|--------|
| `IClassicDesktopStyleApplicationLifetime` | `desktop.MainWindow = new TWindow()` |
| `ISingleViewApplicationLifetime` | `single.MainView = new TView()` |
| `IActivityApplicationLifetime` | `factory.MainViewFactory = () => new TView()` |

Child content ViewModels are wired when the View loads (`AutoWireViewModel="True"`).

If a shell **does** need constructor injection, override `CreateShell` and resolve manually — that is the exception, not the default.

<a id="design-decisions"></a>
## Design Decisions

### Why View is not in DI

| | View | ViewModel |
|---|------|-----------|
| Created by | XAML parser / `Activator.CreateInstance` | DI container |
| Lifetime | Tied to visual tree | Transient or Singleton |
| Constructor deps | Usually none (code-behind minimal) | Services, repositories, etc. |

Views are UI artifacts; ViewModels carry business logic and dependencies. Keeping Views out of DI avoids container-managed UI lifetimes and simplifies AOT trimming.

### Transient vs Singleton

Only two modes — no Hybrid. Choose per ViewModel:

- **Transient** — new instance each navigation (typical for pages); `OnLoadedAsync(true)` every time
- **Singleton** — shared state (settings, session); later loads pass `isFirstLoad: false`

### Version history (high level)

| From | To | Notes |
|------|----|--------|
| v1.2 | 2.0 | ViewModel-only DI; no `AddMvvmHybrid` |
| 2.0.0 | 2.0.1 | Shell via `CreateShell` / `new`; `CreateShellFromDi` removed |
| 2.0.1 | 3.0.0 | Instance `MvvmManager`; `EnableViewLocator`; `OnLoadedAsync(bool)` |

See [Upgrade Guide](upgrade.md).

<a id="aot--trimming"></a>
## AOT & Trimming

Crystal.Avalonia avoids runtime assembly scanning. All type discovery is compile-time via generics:

- `AddMvvmTransient<TView, TViewModel>` — trimmer preserves `TView` / `TViewModel` constructors via `[DynamicallyAccessedMembers(PublicConstructors)]`
- `ModuleManager.RegisterModule<TModule>()` — same annotation on module type
- `ViewLocator.CreateView` — annotated view type from mapping dictionary

Library sets `IsAotCompatible=true`. See [AOT Compatibility](aot-compatibility.md) for publish commands.

## Component Map

```
CrystalApplication
├── Mvvm (MvvmManager) ───── per-app mappings + ServiceProvider
├── ModuleManager ────────── IModule.RegisterServices / InitializeModule
├── ViewModelLocator ─────── attached property → DI resolve ViewModel
├── ViewLocator ──────────── IDataTemplate → Activator.CreateInstance View
├── ViewLifecycleBinder ──── ILifecycleAware hooks
└── CrystalOptions ───────── EnableViewLocator (default: true)
```
