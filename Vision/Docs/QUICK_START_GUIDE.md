# ?? 快速实施指南：WorkFlow Modbus 自动初始化 + TCP 启用开关

## ?? 推荐方案

**? 强烈推荐使用单设备优化方案**：`Vision\Docs\MODBUS_QUICK_IMPL_GUIDE.md`

该方案针对实际应用场景优化，更简洁、更易维护：
- ? 单设备模式（符合大多数工业场景）
- ? 配置界面友好（暂停/恢复轮询）
- ? 代码更简洁（无冗余字典）

---

## 备选：完整多设备方案

如果你确实需要支持多个 Modbus 设备，可以参考以下完整方案：

## ? 已自动完成的修改

1. **ProcessStation.cs** - 添加了 `EnableTcp` 属性
2. **WorkFlow.cs** - 添加了 `_modbusClients` 字典
3. **WorkFlow.cs** - 修改了 `OnSolutionChanged` 调用（调用新方法）
4. **WorkFlow.cs** - 修改了 `RunTool` 中的 TCP 发送逻辑

## ?? 需要手动完成的步骤

### 步骤 1：添加两个新方法到 WorkFlow.cs

打开文件：`Vision\Solutions\WorkFlow\WorkFlow.cs`

在 `#region Modbus通讯` 中，找到 `InitModbus` 方法，**在它后面**添加以下两个方法：

```csharp
/// <summary>
/// 从方案配置初始化所有Modbus连接
/// 自动连接所有已启用的Modbus配置，并启动轮询
/// 
/// 调用时机：
/// 1. 程序启动后（在 OnSolutionChanged 中）
/// 2. 方案切换后（在 OnSolutionChanged 中）
/// </summary>
private void InitializeModbusFromSolution()
{
  try
  {
    var solution = SolutionManager.Instance.Current;
    if (solution?.ModbusConfigs?.Configs == null || solution.ModbusConfigs.Configs.Count == 0)
    {
    LogHelper.Info("当前方案未配置Modbus通讯");
    return;
    }

    int successCount = 0;
    int failCount = 0;

    foreach (var config in solution.ModbusConfigs.Configs.Where(c => c.Enabled))
    {
try
      {
        // 解析数据格式
  DataFormat dataFormat = config.DataFormat switch
        {
          "ABCD" => DataFormat.ABCD,
          "CDAB" => DataFormat.CDAB,
          "BADC" => DataFormat.BADC,
          "DCBA" => DataFormat.DCBA,
          _ => DataFormat.CDAB
        };

        // 创建访问器并连接
        var accessor = ModbusTcp.Get(
   config.IpAddress,
   config.Port,
      dataFormat,
     config.Station,
          config.ConnectTimeout,
          config.StringReverse);

   var result = accessor.Connect();

        if (result.IsSuccess)
        {
          // 存入字典，允许后续按名称访问
    _modbusClients[config.Name] = accessor;
          
       // 兼容旧代码：第一个连接也赋值给 _plc
          if (_plc == null)
    {
            _plc = accessor;
          }

          successCount++;
          LogHelper.Info($"Modbus [{config.Name}] 连接成功: {config.IpAddress}:{config.Port}");
   }
        else
        {
  failCount++;
          LogHelper.Warn($"Modbus [{config.Name}] 连接失败: {result.Message}");
        }
      }
      catch (Exception ex)
      {
    failCount++;
        LogHelper.Error(ex, $"Modbus [{config.Name}] 初始化异常");
      }
    }

    if (successCount > 0 || failCount > 0)
    {
      LogHelper.Info($"Modbus初始化完成: 成功 {successCount} 个, 失败 {failCount} 个");
    }

    // 自动启动轮询（使用默认间隔100ms）
    if (successCount > 0)
    {
      StartModbusPolling(100);
    }
  }
catch (Exception ex)
  {
    LogHelper.Error(ex, "初始化Modbus通讯失败");
  }
}

/// <summary>
/// 清理所有Modbus连接
/// 方案切换时调用，释放旧连接避免内存泄漏
/// </summary>
private void CleanupModbusConnections()
{
  try
  {
    // 停止轮询
    StopModbusPolling();

    // 等待轮询任务完成（最多等待1秒）
    if (_plcPollTask != null && !_plcPollTask.IsCompleted)
    {
      try
      {
  _plcPollTask.Wait(1000);
    }
      catch
      {
      // 忽略等待超时
      }
    }

    // 关闭所有连接
foreach (var kvp in _modbusClients)
    {
    try
   {
        kvp.Value?.Close();
      }
      catch
      {
        // 忽略关闭异常
      }
    }

    _modbusClients.Clear();
    _plc = null;
    _plcPollTask = null;

    LogHelper.Info("Modbus连接已清理");
  }
  catch (Exception ex)
  {
    LogHelper.Error(ex, "清理Modbus连接异常");
  }
}
```

### 步骤 2：修改 StopModbusPolling 方法

在同一个文件中，找到 `StopModbusPolling` 方法，将其修改为：

```csharp
/// <summary>
/// 停止Modbus轮询
/// </summary>
public void StopModbusPolling()
{
  try
  {
    if (_plcPollTask != null && !_plcPollTask.IsCompleted)
    {
      // 注意：不要调用 _cts.Cancel()，因为这会影响其他后台任务
      // 轮询任务会在下次检查 IsOnline 或配置时自然停止
      LogHelper.Info("Modbus轮询停止请求已发送");
    }
  }
  catch (Exception ex)
  {
    LogHelper.Error(ex, "停止Modbus轮询异常");
  }
}
```

**关键点**：删除 `_cts.Cancel()` 和 `_plcPollTask.Wait(1000)`，仅保留日志输出。

### 步骤 3：修改 Dispose 方法

在 `Dispose` 方法中，找到这段代码：

```csharp
// 2. 关闭PLC连接
try
{
  _plc?.Close();
}
catch
{
  // 忽略关闭异常
}
```

将其替换为：

```csharp
// 2. 清理所有Modbus连接
try
{
  CleanupModbusConnections();
}
catch
{
  // 忽略清理异常
}
```

## ? 验证步骤

完成上述修改后：

1. **编译项目**
   ```bash
   dotnet build
   ```
   应该没有编译错误。

2. **启动程序并检查日志**
   - 如果没有配置 Modbus：应该看到 "当前方案未配置Modbus通讯"
   - 如果配置了 Modbus：应该看到 "Modbus初始化完成" 和 "Modbus轮询已启动"

3. **切换方案**
   - 应该看到 "Modbus连接已清理"
   - 然后看到新方案的 "Modbus初始化完成"

4. **测试 TCP 启用开关**
   - 在工位配置界面，禁用某个工位的 `EnableTcp` 属性
   - 触发该工位检测
   - 日志应显示："TCP发送已禁用"

## ?? 功能说明

### 1. Modbus 自动初始化
- ? 程序启动时自动连接所有已启用的 Modbus 配置
- ? 连接成功后自动启动轮询任务
- ? 方案切换时自动清理旧连接并初始化新配置

### 2. TCP 启用开关
- ? 工位配置中添加了 `EnableTcp` 属性（默认 true）
- ? 当 `EnableTcp = false` 时，不会通过 TCP 发送检测结果
- ? 即使配置了 `TcpConnectionName`，也不会发送

### 3. 多 Modbus 设备支持
- ? 使用 `_modbusClients` 字典管理多个 Modbus 设备
- ? 第一个连接仍然赋值给 `_plc`，保持向后兼容
- ? 可以通过配置名称访问特定的 Modbus 设备

## ?? 常见问题

### Q: 编译错误 "当前上下文中不存在名称 InitializeModbusFromSolution"
A: 说明步骤1的两个方法还没有添加，请参考上面的代码完整复制。

### Q: 轮询任务没有启动？
A: 检查：
- Modbus 配置是否已启用
- 至少有一个连接成功
- 查看日志中是否有 "Modbus轮询已启动"

### Q: 方案切换后连接没有清理？
A: 确保步骤3已完成，`Dispose` 方法调用了 `CleanupModbusConnections()`。

### Q: 程序关闭时报错？
A: 确保步骤2已完成，`StopModbusPolling` 中没有调用 `_cts.Cancel()`。

## ?? 相关文档

- 完整代码：`Vision\Docs\MODBUS_METHODS_TO_ADD.cs`
- 详细说明：`Vision\Docs\WORKFLOW_MODBUS_AUTO_INIT.md`
- 实施检查表：`Vision\Docs\IMPLEMENTATION_CHECKLIST.md`

## ?? 完成后的效果

- ? 程序启动后自动连接 Modbus，无需手动初始化
- ? 方案切换时自动清理旧连接并初始化新配置
- ? 工位可以单独控制是否发送 TCP 数据
- ? 支持多个 Modbus 设备同时连接
- ? 内存管理更安全，无泄漏风险
