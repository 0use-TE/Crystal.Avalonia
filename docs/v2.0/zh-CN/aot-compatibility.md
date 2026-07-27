# AOT 兼容性

Crystal.Avalonia 完全兼容 .NET AOT（Ahead-of-Time，提前编译）与裁剪（trimming）。

## 什么是 AOT？

AOT 编译在构建时将 .NET 代码转换为原生代码，而不是在运行时通过 JIT 编译。这带来：

- 更快的启动时间
- 更小的内存占用
- 更小的部署体积（配合裁剪）
- 无 JIT 开销

## Crystal.Avalonia 的 AOT 特性

### IsAotCompatible

库已标记为 AOT 兼容：

```xml
<PropertyGroup>
    <IsAotCompatible>true</IsAotCompatible>
</PropertyGroup>
```

### DynamicallyAccessedMembers 注解

所有使用反射（如 `Activator.CreateInstance`）的泛型方法均已正确标注：

```csharp
public static void AddMvvmTransient<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>
    (this IServiceCollection services)
    where TView : Control where TViewModel : class
{
    // ...
}
```

同样，`AddMvvmSingleton` 也使用这些注解以确保 AOT 兼容性。

这会让裁剪器明确知道运行时需要的成员，避免误删。

## 使用 AOT 发布

### 桌面应用

```bash
dotnet publish -c Release -p:PublishAot=true
```

### 配合裁剪

```bash
dotnet publish -c Release -p:PublishTrimmed=true -p:PublishAot=true
```

### 显示裁剪警告

```bash
dotnet publish -c Release -p:ShowTrimmedWarnings=true
```

## 项目配置

### .csproj 示例

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>WinExe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>enable</Nullable>
        <IsAotCompatible>true</IsAotCompatible>
    </PropertyGroup>

</Project>
```

## Avalonia 集成

Crystal.Avalonia 模板默认已启用编译绑定：

```xml
<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
```

这意味着 XAML 绑定在编译期预生成，运行时不再使用反射。配合 `x:DataType`，绑定完全对 AOT 友好。

## 故障排除

### 类型被裁剪

若出现类型被裁剪的警告：

1. 在泛型方法参数上添加 `[DynamicallyAccessedMembers]`
2. 在需要的类型上使用 `Preserve<>` 特性
3. 在复杂场景下向 `rd.xml` 添加条目

### 运行时缺少类型

若运行时缺少类型：

1. 确认所有 View/ViewModel 类型均已注册
2. 确认模块程序集未被裁剪掉
3. 在需要处使用 `PreserveAll` 或 `PreserveMember`

## 延伸阅读

- [架构原理 — AOT 与裁剪](architecture.md#aot--trimming) — 编译期类型发现，无程序集扫描
- [.NET AOT 文档](https://docs.microsoft.com/dotnet/core/deploying/trimming)
- [Avalonia AOT 提示](https://docs.avaloniaui.net/)
