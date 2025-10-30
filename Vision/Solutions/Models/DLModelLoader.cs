using System;
using Logger;

namespace Vision.Solutions.Models;

/// <summary>
/// 深度学习模型加载器
/// 负责动态加载和卸载模型，提供即时加载功能
/// </summary>
public static class DLModelLoader
{
  /// <summary>
  /// 为指定工位加载深度学习模型
  /// </summary>
  /// <param name="station">工位对象</param>
  /// <returns>是否加载成功</returns>
  public static bool LoadModel(ProcessStation station)
  {
    if (station == null)
    {
      LogHelper.Warn("工位对象为空，无法加载模型");
      return false;
    }

    // 检查是否启用模型加载
    if (!station.bLoadModel)
    {
      LogHelper.Info($"工位[{station.Name}] 未启用模型加载");
      return false;
    }

    // 检查模型路径
    if (string.IsNullOrWhiteSpace(station.ModelPath))
    {
      LogHelper.Warn($"工位[{station.Name}] 模型路径为空");
      return false;
    }

    if (!System.IO.Directory.Exists(station.ModelPath))
    {
      LogHelper.Warn($"工位[{station.Name}] 模型路径不存在: {station.ModelPath}");
      return false;
    }

    try
    {
      // 先卸载旧模型
      UnloadModel(station);

      // 使用反射动态加载模型类型
      var modelType = Type.GetType("DnnInferenceNet.DnnBase.DnnSemanticSegmetation, DnnInferenceNet");
      if (modelType == null)
      {
        LogHelper.Warn($"工位[{station.Name}] 深度学习库未引用");
        LogHelper.Warn($"  → 请确保 DnnInferenceNet.dll 已正确引用");
        return false;
      }

      var modelInstance = Activator.CreateInstance(modelType);
      var initMethod = modelType.GetMethod("Init");
      if (initMethod == null)
      {
        LogHelper.Warn($"工位[{station.Name}] 模型类型不支持 Init 方法");
        return false;
      }

      // 准备运行时参数
      var deviceTypeEnum = Type.GetType("DnnInferenceNet.BassClass.DeviceType, DnnInferenceNet");
      var runtimeEnum = Type.GetType("DnnInferenceNet.BassClass.Runtime, DnnInferenceNet");
      int runtimeValue = (int)station.RuntimeType;

      object[] parameters =
      [
        station.ModelPath,
        deviceTypeEnum != null ? Enum.ToObject(deviceTypeEnum, 0) : 0, // DeviceType.GPU
        runtimeEnum != null ? Enum.ToObject(runtimeEnum, runtimeValue) : runtimeValue
      ];

      LogHelper.Info($"? 工位[{station.Name}] 正在加载模型...");
      LogHelper.Info($"  → 模型: {station.ModelFolderName}");
      LogHelper.Info($"  → 运行时: {GetRuntimeName(station.RuntimeType)}");

      // 调用 Init 方法
      var result = initMethod.Invoke(modelInstance, parameters);

      if (result is int retCode && retCode == 0)
      {
        station.DLModel = modelInstance;
        LogHelper.Info($"? 工位[{station.Name}] 模型加载成功");
        return true;
      }
      else
      {
        int retCodeDisplay = result is int rc ? rc : -999;
        LogHelper.Warn($"? 工位[{station.Name}] 模型初始化失败，返回码: {retCodeDisplay}");
        LogHelper.Warn($"  → 可能原因:");
        LogHelper.Warn($"     1. 模型文件损坏或不完整");
        LogHelper.Warn($"   2. 运行时类型与硬件不匹配 (当前: {station.RuntimeType})");
        LogHelper.Warn($"     3. 缺少必要的驱动或库文件");
        LogHelper.Warn($"  → 建议操作:");
        LogHelper.Warn($"     1. 检查模型文件是否完整");
        LogHelper.Warn($"     2. 尝试切换运行时类型");
        LogHelper.Warn($"     3. 查看详细错误日志");
        return false;
      }
    }
    catch (System.Reflection.TargetInvocationException tie)
    {
      var innerEx = tie.InnerException ?? tie;
      LogHelper.Warn($"? 工位[{station.Name}] 模型加载异常: {innerEx.Message}");

      // 智能错误诊断
      if (innerEx.Message.Contains("没有检测到可用设备") ||
          innerEx.Message.Contains("No available device"))
      {
        LogHelper.Warn($"  → 错误原因: 未检测到支持的硬件设备");
        LogHelper.Warn($"  → 当前运行时: {station.RuntimeType}");
        LogHelper.Warn($"  → 解决方案:");

        switch (station.RuntimeType)
        {
          case DLRuntime.TensorRT:
            LogHelper.Warn($" ? 检查是否安装了 NVIDIA GPU 及 CUDA/TensorRT");
            LogHelper.Warn($"     ? 尝试切换为 'GPU 运行时' 或 'OpenVINO 加速'");
            break;
          case DLRuntime.OpenVINO:
            LogHelper.Warn($"     ? 检查是否安装了 OpenVINO Toolkit");
            LogHelper.Warn($"     ? 尝试切换为 'GPU 运行时'");
            break;
          default:
            LogHelper.Warn($"     ? 检查显卡驱动是否正常");
            LogHelper.Warn($"     ? 确认 CUDA 环境配置");
            break;
        }
      }
      else if (innerEx.Message.Contains("未找到有效的模型文件") ||
               innerEx.Message.Contains("模型文件"))
      {
        LogHelper.Warn($"  → 错误原因: 模型文件缺失或格式错误");
        LogHelper.Warn($"  → 解决方案:");
        LogHelper.Warn($"     ? 检查路径: {station.ModelPath}");
        LogHelper.Warn($"     ? 确认存在 .hdl 和 .hdict 文件");
        LogHelper.Warn($"     ? 重新选择正确的模型文件夹");
      }
      else
      {
        LogHelper.Warn($"  → 详细错误: {innerEx.GetType().Name}");
        if (innerEx.StackTrace != null)
        {
          var firstLine = innerEx.StackTrace.Split('\n')[0];
          LogHelper.Warn($"  → 堆栈跟踪: {firstLine}");
        }
      }

      return false;
    }
    catch (Exception ex)
    {
      LogHelper.Warn($"? 工位[{station.Name}] 模型加载未知异常: {ex.GetType().Name}");
      LogHelper.Warn($"  → 错误信息: {ex.Message}");
      return false;
    }
  }

  /// <summary>
  /// 卸载工位的深度学习模型
  /// </summary>
  /// <param name="station">工位对象</param>
  public static void UnloadModel(ProcessStation station)
  {
    if (station?.DLModel == null) return;

    try
    {
      // 尝试调用 UnInit 方法释放资源
      var modelType = station.DLModel.GetType();
      var unInitMethod = modelType.GetMethod("UnInit");
      if (unInitMethod != null)
      {
        unInitMethod.Invoke(station.DLModel, null);
        LogHelper.Info($"? 工位[{station.Name}] 模型已卸载");
      }
    }
    catch (Exception ex)
    {
      LogHelper.Warn($"? 工位[{station.Name}] 模型卸载异常: {ex.Message}");
    }
    finally
    {
      station.DLModel = null;
    }
  }

  /// <summary>
  /// 重新加载模型（先卸载再加载）
  /// </summary>
  /// <param name="station">工位对象</param>
  /// <returns>是否加载成功</returns>
  public static bool ReloadModel(ProcessStation station)
  {
    LogHelper.Info($"?? 工位[{station?.Name}] 重新加载模型...");
    UnloadModel(station);
    return LoadModel(station);
  }

  /// <summary>
  /// 获取运行时友好名称
  /// </summary>
  private static string GetRuntimeName(DLRuntime runtime)
  {
    return runtime switch
    {
      DLRuntime.GC => "GPU 运行时",
      DLRuntime.OpenVINO => "OpenVINO 加速",
      DLRuntime.TensorRT => "TensorRT 加速",
      _ => runtime.ToString()
    };
  }
}