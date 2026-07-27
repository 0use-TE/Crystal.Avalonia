# AOT 兼容性（v1.2）

与当前版本相同 — Crystal.Avalonia 为 AOT 兼容，在 `AddMvvmTransient`、`AddMvvmHybrid` 和 `AddMvvmSingleton` 上使用 `[DynamicallyAccessedMembers]`。

完整说明请参阅 [v2.0 AOT 兼容性](../../v2.0/zh-CN/aot-compatibility.md)。

```bash
dotnet publish -c Release -p:PublishAot=true
```
