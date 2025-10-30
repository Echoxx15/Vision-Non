# ?? Modbus 单设备优化 - 快速实施指南

## ?? 需要修改的文件

1. `Vision\Solutions\WorkFlow\WorkFlow.cs` - 核心修改
2. `Vision\Frm\MainForm\Frm_Main.cs` - 配置按钮事件（可选）

## ?? 实施步骤

### 步骤 1：添加暂停标志（WorkFlow.cs）

在 `#region 线程和任务管理` 中，`_plcPollTask` 字段后面添加：

```csharp
/// <summary>
/// Modbus轮询暂停标志
/// true: 暂停轮询（配置界面打开时）
/// false: 正常轮询
/// </summary>
private volatile bool _modbusPollingPaused = false;
```

### 步骤 2：添加初始化和清理方法（WorkFlow.cs）

在 `#region Modbus通讯` 中，`InitModbus` 方法后面添加：

**完整代码见**：`Vision\Docs\MODBUS_METHODS_TO_ADD.cs`

关键点：
- `InitializeModbusFromSolution()` - 单设备初始化
- `CleanupModbusConnections()` - 单设备清理

### 步骤 3：添加暂停和恢复方法（WorkFlow.cs）

在 `#region Modbus通讯` 中，`StopModbusPolling` 方法后面添加：

```csharp
/// <summary>
/// 暂停Modbus轮询
/// 用于配置界面打开时，避免与配置界面的连接冲突
/// </summary>
public void PauseModbusPolling()
{
  _modbusPollingPaused = true;
  LogHelper.Info("Modbus轮询已暂停");
}

/// <summary>
/// 恢复Modbus轮询
/// 用于配置界面关闭后，恢复正常轮询
/// </summary>
public void ResumeModbusPolling()
{
  _modbusPollingPaused = false;
  LogHelper.Info("Modbus轮询已恢复");
}
```

### 步骤 4：修改 StartModbusPolling 方法（WorkFlow.cs）

找到 `StartModbusPolling` 方法，在 `while (!token.IsCancellationRequested)` 循环的开头添加暂停检查：

```csharp
public void StartModbusPolling(int intervalMs = 100)
{
  if (_plc == null)
  {
    LogHelper.Warn("Modbus未初始化，无法启动轮询");
    return;
  }

  // 如果轮询任务已经在运行，不重复启动
  if (_plcPollTask != null && !_plcPollTask.IsCompleted)
  {
    LogHelper.Info("Modbus轮询已在运行中");
    return;
  }

  var token = _cts.Token;
  _plcPollTask = Task.Factory.StartNew(async () =>
  {
    LogHelper.Info("Modbus轮询已启动");

    while (!token.IsCancellationRequested)
    {
 try
      {
        // ? 新增：检查是否暂停（配置界面打开时）
 if (_modbusPollingPaused)
        {
          await Task.Delay(intervalMs, token).ConfigureAwait(false);
          continue;
        }

        // ? 新增：检查连接是否有效
        if (_plc == null || !_plc.IsConnected)
        {
          await Task.Delay(intervalMs, token).ConfigureAwait(false);
     continue;
        }

        // 只在系统在线时轮询
        if (!IsOnline)
        {
          await Task.Delay(intervalMs, token).ConfigureAwait(false);
     continue;
        }

        // 获取Modbus配置
 var solution = SolutionManager.Instance.Current;
     if (solution?.ModbusConfigs?.Configs == null || solution.ModbusConfigs.Configs.Count == 0)
        {
 await Task.Delay(intervalMs, token).ConfigureAwait(false);
          continue;
        }

        var config = solution.ModbusConfigs.Configs[0]; // 单设备模式
        if (!config.Enabled)
        {
      await Task.Delay(intervalMs, token).ConfigureAwait(false);
          continue;
      }

   // 遍历所有输入变量
      var inputVars = config.Variables
       .Where(v => v.Direction == ModbusDirection.Input)
 .ToList();

        foreach (var variable in inputVars)
        {
          try
          {
     // 读取变量值
   object value = variable.Type switch
  {
    ModbusDataType.Bool => _plc.ReadBool(variable.Address, 1)?[0],
        ModbusDataType.Short => _plc.ReadInt16(variable.Address),
         ModbusDataType.Float => _plc.ReadFloat(variable.Address),
              ModbusDataType.String => _plc.ReadString(variable.Address, (ushort)variable.Length),
              _ => null
   };

            if (value != null)
            {
       // 处理输入变量的值变化
              HandleInputVariable(variable.Name, value, variable.Type);
   }
 }
      catch (Exception ex)
  {
   // 单个变量读取失败不影响其他变量
     LogHelper.Error(ex, $"读取Modbus变量失败: {variable.Name}");
  }
        }
      }
      catch (Exception ex)
      {
   // 轮询健壮性：忽略单次异常，继续下次轮询
        LogHelper.Error(ex, "Modbus轮询异常");
      }

      await Task.Delay(intervalMs, token).ConfigureAwait(false);
    }

    LogHelper.Info("Modbus轮询已停止");
  }, TaskCreationOptions.LongRunning);
}
```

**关键修改**：
1. 添加重复启动检查
2. 添加暂停标志检查
3. 添加连接有效性检查

### 步骤 5：修改 StopModbusPolling 方法（WorkFlow.cs）

将现有代码替换为：

```csharp
/// <summary>
/// 停止Modbus轮询
/// 注意：不要调用 _cts.Cancel()，因为这会影响图像处理队列
/// </summary>
public void StopModbusPolling()
{
  try
  {
    // 设置暂停标志，让任务自然退出
    _modbusPollingPaused = true;
    
    LogHelper.Info("Modbus轮询停止请求已发送");
  }
  catch (Exception ex)
  {
    LogHelper.Error(ex, "停止Modbus轮询异常");
  }
}
```

### 步骤 6：修改 Dispose 方法（WorkFlow.cs）

找到这段代码：
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

替换为：
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

### 步骤 7：主窗体配置按钮（可选，Frm_Main.cs）

如果有 Modbus 配置按钮，修改其点击事件：

```csharp
private void OnModbusConfigClick(object sender, EventArgs e)
{
  try
  {
    // 暂停WorkFlow中的轮询
  WorkFlow.Instance.PauseModbusPolling();
    
    // 打开配置界面
    using var frm = new Frm_ModbusConfig();
    frm.ShowDialog(this);
    
    // 关闭配置界面后，清理旧连接并重新初始化
    WorkFlow.Instance.CleanupModbusConnections();
    WorkFlow.Instance.InitializeModbusFromSolution();
  }
  catch (Exception ex)
  {
    LogHelper.Error(ex, "打开Modbus配置失败");
    MessageBox.Show($"打开配置失败: {ex.Message}", "错误", 
      MessageBoxButtons.OK, MessageBoxIcon.Error);
  }
}
```

## ? 验证清单

完成所有步骤后，进行以下验证：

### 1. 编译验证
```bash
dotnet build
```
应该没有编译错误。

### 2. 启动验证
- [ ] 程序启动
- [ ] 日志显示："Modbus连接成功"
- [ ] 日志显示："Modbus轮询已启动"

### 3. 配置界面验证
- [ ] 打开 Modbus 配置
- [ ] 日志显示："Modbus轮询已暂停"
- [ ] 修改参数并测试连接
- [ ] 保存并关闭
- [ ] 日志显示："Modbus连接已清理"
- [ ] 日志显示："Modbus连接成功"

### 4. 方案切换验证
- [ ] 切换方案
- [ ] 日志显示："Modbus连接已清理"
- [ ] 日志显示：新方案的初始化消息

### 5. 程序关闭验证
- [ ] 关闭程序
- [ ] 日志显示："Modbus连接已清理"
- [ ] 程序正常退出，无卡死

## ?? 修改摘要

| 文件 | 修改类型 | 行数 |
|------|---------|------|
| WorkFlow.cs | 添加字段 | +3 |
| WorkFlow.cs | 添加方法（2个） | +80 |
| WorkFlow.cs | 修改方法（3个） | ~50 |
| Frm_Main.cs | 修改事件（可选） | ~15 |
| **总计** | | **~148** |

## ?? 核心改进

1. **单设备模式**
   - 移除多设备字典（可选）
   - 直接使用 `_plc` 字段
   - 简化初始化逻辑

2. **暂停/恢复机制**
   - 添加 `_modbusPollingPaused` 标志
   - 配置界面打开时暂停轮询
   - 关闭后自动重新初始化

3. **健壮性增强**
   - 检查连接有效性
   - 防止重复启动
   - 优雅退出轮询任务

## ?? 常见问题

### Q: 编译错误 "当前上下文中不存在名称 InitializeModbusFromSolution"
A: 步骤2的两个方法还没添加，参考 `MODBUS_METHODS_TO_ADD.cs`。

### Q: 配置界面打开后轮询还在继续？
A: 检查步骤3的暂停方法是否添加，步骤7的事件是否调用了 `PauseModbusPolling()`。

### Q: 程序关闭时卡死？
A: 确保步骤5修改了 `StopModbusPolling`，不要调用 `_cts.Cancel()`。

### Q: 方案切换后连接没有重新初始化？
A: 确保 `OnSolutionChanged` 方法调用了 `CleanupModbusConnections()` 和 `InitializeModbusFromSolution()`。

## ?? 相关文档

- **优化方案详解**：`Vision\Docs\MODBUS_SINGLE_DEVICE_OPTIMIZATION.md`
- **初始化方法代码**：`Vision\Docs\MODBUS_METHODS_TO_ADD.cs`
- **实施检查表**：`Vision\Docs\IMPLEMENTATION_CHECKLIST.md`

## ?? 完成！

完成所有步骤后，你将拥有一个：
- ? 简洁的单设备 Modbus 管理
- ? 友好的配置界面交互
- ? 健壮的轮询机制
- ? 优雅的资源清理

有问题随时查阅文档或提问！
