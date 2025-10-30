# ?? Modbus 单设备模式优化方案

## ?? 优化目标

针对实际应用场景优化 Modbus 管理：
- ? 单设备模式：通常一个系统只连接一个 Modbus PLC
- ? 配置界面友好：打开配置界面时断开连接，允许修改参数
- ? 自动重连：关闭配置界面后自动重新连接
- ? 简化轮询：针对单设备优化轮询逻辑

## ?? 架构设计

### 当前问题分析

1. **多设备字典冗余**：实际只需要一个设备，但代码中有 `_modbusClients` 字典
2. **配置界面冲突**：配置界面和 WorkFlow 可能同时持有连接
3. **轮询复杂性**：轮询代码假设多设备，但实际只需处理一个

### 优化后的架构

```
┌─────────────────────────────────────────┐
│         WorkFlow（单设备模式）           │
├─────────────────────────────────────────┤
│  private ModbusAccessor _plc;           │  ← 单个设备实例
│  private Task _plcPollTask;      │  ← 单个轮询任务
│  private CancellationTokenSource _cts;  │  ← 统一取消令牌
├─────────────────────────────────────────┤
│  InitializeModbusFromSolution()         │  ← 单设备初始化
│  CleanupModbusConnections()       │  ← 单设备清理
│  StartModbusPolling()       │  ← 单设备轮询
│  PauseModbusPolling()    │  ← 新增：暂停轮询
│  ResumeModbusPolling()    │  ← 新增：恢复轮询
└─────────────────────────────────────────┘
          ↑  ↑
          │  │
   自动初始化     配置界面控制
```

## ?? 实施步骤

### 步骤 1：简化 WorkFlow.cs 的 Modbus 管理

#### 1.1 移除多设备字典（可选）

在 `#region 通讯模块` 中，可以移除这行：
```csharp
// 不需要了（单设备模式）
// private readonly ConcurrentDictionary<string, ModbusAccessor> _modbusClients = ...
```

保留原有的单设备字段即可：
```csharp
private ModbusAccessor _plc;
```

#### 1.2 添加轮询暂停标志

在 `#region 线程和任务管理` 中添加：
```csharp
/// <summary>
/// Modbus轮询暂停标志
/// true: 暂停轮询（配置界面打开时）
/// false: 正常轮询
/// </summary>
private volatile bool _modbusPollingPaused = false;
```

### 步骤 2：优化初始化和清理方法

使用 `Vision\Docs\MODBUS_METHODS_TO_ADD.cs` 中已经优化的代码，关键改进：

1. **InitializeModbusFromSolution()**
   - 只处理第一个配置（单设备模式）
   - 直接赋值给 `_plc`
   - 连接成功后自动启动轮询

2. **CleanupModbusConnections()**
   - 简化清理逻辑
   - 不需要遍历字典
   - 直接关闭 `_plc`

### 步骤 3：优化轮询方法

#### 3.1 修改 StartModbusPolling 方法

找到 `StartModbusPolling` 方法，修改为：

```csharp
/// <summary>
/// 开启Modbus轮询任务（单设备模式）
/// 持续读取Modbus触发寄存器，检测到触发信号时调用TriggerCamera
/// 
/// 设计说明：
/// - 单设备模式：只处理方案中的第一个Modbus配置
/// - 支持暂停/恢复：配置界面打开时暂停轮询
/// - 健壮性：单次异常不影响后续轮询
/// </summary>
/// <param name="intervalMs">轮询间隔（毫秒）</param>
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
        // 检查是否暂停（配置界面打开时）
    if (_modbusPollingPaused)
        {
          await Task.Delay(intervalMs, token).ConfigureAwait(false);
        continue;
    }

        // 检查系统在线状态
      if (!IsOnline)
    {
          await Task.Delay(intervalMs, token).ConfigureAwait(false);
          continue;
}

        // 检查连接是否有效
        if (_plc == null || !_plc.IsConnected)
        {
          await Task.Delay(intervalMs, token).ConfigureAwait(false);
 continue;
        }

        // 获取Modbus配置（单设备模式：第一个配置）
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

#### 3.2 添加暂停和恢复方法

在 `#region Modbus通讯` 中添加：

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

### 步骤 4：修改配置界面集成

#### 4.1 打开配置界面时暂停轮询

在主窗体的配置按钮点击事件中：

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

#### 4.2 配置界面无需调用 WorkFlow

`Frm_ModbusConfig.cs` 中的连接/断开操作保持不变，使用自己的 `_accessor` 实例。

### 步骤 5：修改 StopModbusPolling 方法

```csharp
/// <summary>
/// 停止Modbus轮询
/// 注意：不要调用 _cts.Cancel()，因为这会影响图像处理队列
/// 改为设置暂停标志，让任务自然退出
/// </summary>
public void StopModbusPolling()
{
  try
  {
    // 设置暂停标志
    _modbusPollingPaused = true;
    
    LogHelper.Info("Modbus轮询停止请求已发送");
  }
  catch (Exception ex)
  {
    LogHelper.Error(ex, "停止Modbus轮询异常");
  }
}
```

### 步骤 6：修改 Dispose 方法

```csharp
public void Dispose()
{
  // 1. 取消所有后台循环任务
  try
  {
    _cts.Cancel();
  }
  catch
  {
    // 忽略取消异常
  }

  // 2. 清理Modbus连接（替换原来的 _plc?.Close()）
  try
  {
    CleanupModbusConnections();
  }
  catch
  {
    // 忽略清理异常
  }

  // 3. 停止所有TCP连接
  try
  {
    TcpCommManager.Instance.DisposeAll();
  }
  catch
  {
    // 忽略停止异常
}

  // 3.5 释放所有光源控制器
  try
  {
    LightSource.LightSourceManager.Instance.Dispose();
}
  catch
  {
    // 忽略释放异常
  }

  // 4. 释放并发控制信号量
  try
  {
    _stationSemaphore?.Dispose();
  }
  catch
  {
    // 忽略释放异常
  }
}
```

## ? 验证步骤

### 1. 程序启动验证
- 启动程序
- 查看日志：应显示 "Modbus连接成功" 和 "Modbus轮询已启动"

### 2. 配置界面验证
- 点击 Modbus 配置按钮
- 查看日志：应显示 "Modbus轮询已暂停"
- 在配置界面修改参数并保存
- 关闭配置界面
- 查看日志：应显示 "Modbus连接已清理" 和 "Modbus连接成功"

### 3. 方案切换验证
- 切换方案
- 查看日志：应显示清理和重新初始化的消息

### 4. 程序关闭验证
- 关闭程序
- 查看日志：应显示 "Modbus连接已清理"
- 不应有异常或卡死

## ?? 优化效果对比

| 功能 | 优化前 | 优化后 |
|------|--------|--------|
| 设备管理 | 字典管理多设备 | 单字段管理单设备 |
| 代码复杂度 | 高（遍历字典） | 低（直接访问） |
| 配置界面 | 可能冲突 | 暂停轮询，无冲突 |
| 内存占用 | 字典 + 多实例 | 单实例 |
| 轮询逻辑 | 检查多配置 | 只检查一个配置 |
| 启动时间 | 遍历所有配置 | 只初始化一个 |

## ?? 常见问题

### Q: 如果真的需要多设备怎么办？
A: 可以保留 `_modbusClients` 字典，但大多数工业视觉项目只需一个 PLC。

### Q: 配置界面和轮询冲突怎么办？
A: 使用 `PauseModbusPolling()` 暂停轮询，配置界面使用独立的连接实例。

### Q: 轮询任务如何优雅退出？
A: 不调用 `_cts.Cancel()`，而是设置 `_modbusPollingPaused` 标志，让任务检查 `_plc == null` 后自然退出。

## ?? 相关文件

- **优化后的初始化方法**：`Vision\Docs\MODBUS_METHODS_TO_ADD.cs`
- **快速开始指南**：`Vision\Docs\QUICK_START_GUIDE.md`
- **详细实施说明**：`Vision\Docs\IMPLEMENTATION_CHECKLIST.md`

## ?? 总结

优化后的方案更符合实际应用场景：
- ? 代码更简洁，易于维护
- ? 单设备模式，性能更好
- ? 配置界面友好，无冲突
- ? 轮询逻辑清晰，支持暂停/恢复
