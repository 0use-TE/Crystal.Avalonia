# Architecture

How Crystal.Avalonia works internally. For usage, see [Getting Started](getting-started.md) and [Tutorials](tutorials/mvvm-pattern.md).

## Bootstrap Pipeline

`CrystalApplication.OnFrameworkInitializationCompleted()` runs this sequence:

```
RegisterServices(services)          ← App-level DI
    ↓
ModuleManager created + registered
RegisterModules(moduleRegistrar)    ← App registers IModule instances
    ↓
moduleManager.InitService(services) ← Each module.RegisterServices()
    ↓
services.BuildServiceProvider()
    ↓
ViewLocator added to DataTemplates  ← if EnableViewModelLocator
    ↓
moduleManager.InitModules(sp)       ← Each module.InitializeModule()
    ↓
MvvmManager.ServiceProvider = sp
    ↓
CreateShell(sp)                     ← App creates MainWindow / MainView
```

Key points:

- **App runs first**, then modules — `RegisterServices` in `App` executes before `InitService`.
- **Single `ServiceProvider`** — built once; modules initialize after it exists.
- **`MvvmManager.ServiceProvider`** — set before `CreateShell`, required by `ViewModelLocator`.

## Module System

`ModuleManager` implements `IModuleRegistrar`:

| Phase | Method | When |
|-------|--------|------|
| Register | `RegisterModule<T>()` | Before container build; `Activator.CreateInstance<T>()` |
| Services | `IModule.RegisterServices()` | During `InitService`, before `BuildServiceProvider` |
| Init | `IModule.InitializeModule()` | After container build, during `InitModules` |

Modules are plain classes — no assembly scanning. Each module is explicitly registered in `RegisterModules`.

## MVVM Wiring

### Type Mapping (`MvvmManager`)

`AddMvvmTransient` / `AddMvvmSingleton` do two things:

1. Register **ViewModel only** in DI (`AddTransient` / `AddSingleton`)
2. Record View ↔ ViewModel mapping in static dictionaries (`_viewToVm`, `_vmToView`)

`TView` is never added to `IServiceCollection`.

### View-First (`ViewModelLocator`)

Attached property `AutoWireViewModel="True"` triggers on property change:

```
View loaded in XAML
    ↓
ViewModelLocator reads view.GetType()
    ↓
MvvmManager.GetVmType(viewType)
    ↓
ServiceProvider.GetRequiredService(vmType)
    ↓
view.DataContext = viewModel
    ↓
ViewLifecycleBinder.AttachIfNeeded()
```

Skipped in design mode (`Design.IsDesignMode`).

### ViewModel-First (`ViewLocator`)

Registered as `IDataTemplate` on `Application.DataTemplates`:

```
ContentControl.Content = viewModel
    ↓
ViewLocator.Match(vm) — mapping exists?
    ↓
MvvmManager.GetViewType(vmType)
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

- Subscribes to View `Loaded` → calls `OnLoadedAsync()` once, then unsubscribes
- Subscribes to View `Unloaded` → calls `OnUnloaded()` once, then unsubscribes
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

- **Transient** — new instance each navigation (typical for pages)
- **Singleton** — shared state (settings, session)

### v1.2 → v2.0 Changes

| v1.2 | v2.0 |
|------|------|
| View + ViewModel in DI | ViewModel only in DI |
| `AddMvvmHybrid` | Removed |
| ViewLocator resolves View from DI | `Activator.CreateInstance` |
| `new MainWindow()` in CreateShell | `CreateShell<MainWindow, MainView>()` (same idea, helper method) |

See [Upgrade Guide](upgrade.md).

## AOT & Trimming

Crystal.Avalonia avoids runtime assembly scanning. All type discovery is compile-time via generics:

- `AddMvvmTransient<TView, TViewModel>` — trimmer preserves `TView` / `TViewModel` constructors via `[DynamicallyAccessedMembers(PublicConstructors)]`
- `ModuleManager.RegisterModule<TModule>()` — same annotation on module type
- `ViewLocator.CreateView` — annotated view type from mapping dictionary

Library sets `IsAotCompatible=true`. See [AOT Compatibility](aot-compatibility.md) for publish commands.

## Component Map

```
CrystalApplication
├── ModuleManager ────────── IModule.RegisterServices / InitializeModule
├── MvvmManager ──────────── mapping dict + AddMvvm* extensions
├── ViewModelLocator ─────── attached property → DI resolve ViewModel
├── ViewLocator ──────────── IDataTemplate → Activator.CreateInstance View
├── ViewLifecycleBinder ──── ILifecycleAware hooks
└── CrystalOptions ───────── EnableViewModelLocator (default: true)
```
