using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Logger;
using Vision.Manager.CameraManager;
using Vision.SaveImage;
using Vision.Settings;
using Vision.Solutions.Models;
using Vision.LightSource;

namespace Vision.Solutions.WorkFlow;

public sealed class ImageFrame
{
 public string StationName { get; set; }
 public ICogImage Image { get; set; }
 public int ImageIndex { get; set; }
 public string ClientId { get; set; } = string.Empty;
}

internal sealed class TaskFlow : IDisposable
{
 private readonly StationConfig _station;
 private readonly CancellationTokenSource _cts = new();
 private readonly ConcurrentQueue<ImageFrame> _imageQueue = new();
 private readonly SemaphoreSlim _runSemaphore = new(1, 1);
 private bool _disposed;
 public string StationName => _station.Name;

 public TaskFlow(StationConfig station)
 {
  _station = station ?? throw new ArgumentNullException(nameof(station));
  LogHelper.Info($"[TaskFlow] 工位[{StationName}]实例已创建");
  StartDequeueLoop(_cts.Token);
 }

 public void Start(string clientId = "")
 {
  if (_disposed) return;
  try
  {
   if (string.IsNullOrWhiteSpace(_station.SN))
   {
    LogHelper.Warn($"[TaskFlow] 工位[{StationName}]未配置相机SN，直接跳过");
    return;
   }

   TriggerCamera(clientId);
  }
  catch (Exception ex)
  {
   LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]启动失败");
  }
 }

 public void StartFromImage(string imagePath)
 {
  if (_disposed) return;
  if (string.IsNullOrWhiteSpace(imagePath)) return;
  try
  {
   using var img = Image.FromFile(imagePath);
   using var bmp = new Bitmap(img);
   ICogImage cogImg = bmp.PixelFormat != PixelFormat.Format8bppIndexed
    ? new CogImage24PlanarColor(bmp)
    : new CogImage8Grey(bmp);
   _imageQueue.Enqueue(new ImageFrame
    { StationName = StationName, Image = cogImg, ImageIndex = 0, ClientId = string.Empty });
  }
  catch (Exception ex)
  {
   LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]加载图片失败: {imagePath}");
  }
 }

 private void TriggerCamera(string clientId)
 {
  var cameras = CameraManager.Instance.GetAllCameras();
  var camera = cameras.FirstOrDefault(c => string.Equals(c.SN, _station.SN, StringComparison.OrdinalIgnoreCase));
  if (camera == null)
  {
   LogHelper.Warn($"[TaskFlow] 工位[{StationName}]对应相机[{_station.SN}]未找到");
   return;
  }

  int imageIndex = 1;
  EventHandler<ICogImage> handler = null;
  handler = (_, img) =>
  {
   try
   {
    var currentIndex = imageIndex++;
    _imageQueue.Enqueue(new ImageFrame
     { StationName = StationName, Image = img, ImageIndex = currentIndex, ClientId = clientId });
    // 软件触发一次即移除；硬件触发按计数
    if (_station.CameraParams?.TriggerMode == TriggerMode.软触发)
    {
     camera.FrameGrabedEvent -= handler;
    }
    else if (_station.CameraParams?.TriggerMode == TriggerMode.硬触发)
    {
     var required = _station.CameraParams?.TriggerCount ?? 1;
     if (currentIndex >= required) camera.FrameGrabedEvent -= handler;
    }
   }
   catch (Exception ex)
   {
    LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]采集回调异常");
    try
    {
     camera.FrameGrabedEvent -= handler;
    }
    catch
    {
    }
   }
  };
  camera.FrameGrabedEvent -= handler;
  camera.FrameGrabedEvent += handler;
  try
  {
   if (_station.CameraParams != null)
   {
    camera.Parameters.ExposureTime = _station.CameraParams.Exposure;
    camera.Parameters.Gain = _station.CameraParams.Gain;
   }

   // 可选：打开光源
   _ = LightSourceManager.Instance.ControlStationLight(_station.LightControl, true);
   if (_station.CameraParams?.TriggerMode == TriggerMode.软触发)
   {
    camera.SoftwareTriggerOnce();
   }
  }
  catch (Exception ex)
  {
   LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]触发失败");
   try
   {
    camera.FrameGrabedEvent -= handler;
   }
   catch
   {
   }
  }
 }

 private void StartDequeueLoop(CancellationToken token)
 {
  Task.Factory.StartNew(async () =>
  {
   while (!token.IsCancellationRequested)
   {
    try
    {
     if (_imageQueue.TryDequeue(out var frame))
     {
      await _runSemaphore.WaitAsync(token).ConfigureAwait(false);
      try
      {
       RunTool(frame);
      }
      catch (Exception ex)
      {
       LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]处理异常");
      }
      finally
      {
       _runSemaphore.Release();
      }

      continue;
     }
    }
    catch (Exception ex)
    {
     LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]图像出队异常");
    }

    await Task.Delay(5, token).ConfigureAwait(false);
   }
  }, TaskCreationOptions.LongRunning);
 }

 private void RunTool(ImageFrame frame)
 {
  if (_station.DetectionTool?.ToolBlock == null) return;
  var tool = _station.DetectionTool.ToolBlock;
  _station.DetectionTool.ApplyVarsToInputs();
  bool result;
  ICogRecord record = null;
  try
  {
   if (!_station.Enable)
   {
    LogHelper.Info($"[{StationName}]未启用");
    return;
   }

   LogHelper.Info($"[{StationName}]开始执行检测");
   var cogImg = frame.Image;
   // 可选：按配置执行标定工具
   if (_station.bCalibCheckboardTool && _station.CheckerboardTool?.ToolBlock != null)
   {
    var t = _station.CheckerboardTool.ToolBlock;
    t.Inputs["Image"].Value = cogImg;
    t.Run();
    cogImg = (ICogImage)t.Outputs["Image"].Value;
   }

   if (_station.bCalibNPointTool && _station.NPointTool?.ToolBlock != null)
   {
    var t = _station.NPointTool.ToolBlock;
    t.Inputs["Image"].Value = cogImg;
    t.Run();
    cogImg = (ICogImage)t.Outputs["Image"].Value;
   }

   tool.Inputs["Image"].Value = cogImg;
   tool.Run();
   result = (bool)tool.Outputs["Result"].Value;
   LogHelper.Info($"[{StationName}]检测完成，结果【{(result ? "OK" : "NG")}】");
   if (tool.RunStatus.Result != CogToolResultConstants.Accept)
   {
    LogHelper.Warn($"[{StationName}]工具运行错误:{tool.RunStatus.Message}");
   }

   if (_station.bShow && !string.IsNullOrWhiteSpace(_station.DisplayName))
   {
    var index = _station.RecoredIndex;
    try
    {
     record = tool.CreateLastRunRecord()?.SubRecords?[index];
    }
    catch (Exception ex)
    {
     LogHelper.Error(ex, $"工位[{StationName}]创建记录失败");
    }
   }

   // ✅ 步骤3：工具执行完成后写入输出到通讯设备
   WriteToolOutputsToComm(tool);
  }
  catch (Exception ex)
  {
   LogHelper.Error(ex, $"工位[{StationName}]工具运行异常");
   return;
  }

  _ = Task.Run(() => ProcessDisplayAndSave(frame, _station, result, record));
 }

 /// <summary>
 /// 将工具输出端子的值写入通讯设备的输出表
 /// 支持类型自动转换（如 int -> short）
 /// </summary>
 private void WriteToolOutputsToComm(Cognex.VisionPro.ToolBlock.CogToolBlock tool)
 {
  // 检查前置条件
  if (_station.OutputMappings == null || _station.OutputMappings.Count == 0)
  {
   return; // 没有配置映射，跳过
  }

  if (string.IsNullOrWhiteSpace(_station.CommDeviceName))
  {
   LogHelper.Warn($"[{StationName}] 输出映射已配置但未指定通讯设备，跳过输出写入");
   return;
  }

  var device = HardwareCommNet.CommunicationFactory.Instance.GetDevice(_station.CommDeviceName);
  if (device == null)
  {
   LogHelper.Warn($"[{StationName}] 通讯设备[{_station.CommDeviceName}]未找到，跳过输出写入");
   return;
  }

  // 遍历所有映射配置
  foreach (var mapping in _station.OutputMappings)
  {
   try
   {
    // 1. 从工具输出端子获取值
    if (!tool.Outputs.Contains(mapping.ToolOutputName))
    {
     LogHelper.Warn($"[{StationName}] 工具输出端子[{mapping.ToolOutputName}]不存在，跳过");
     continue;
    }

    var toolOutput = tool.Outputs[mapping.ToolOutputName];
    if (toolOutput == null)
    {
     LogHelper.Warn($"[{StationName}] 工具输出端子[{mapping.ToolOutputName}]为null，跳过");
     continue;
    }

    var value = toolOutput.Value;
    if (value == null)
    {
     LogHelper.Warn($"[{StationName}] 工具输出端子[{mapping.ToolOutputName}]值为null，跳过");
     continue;
    }

    // 2. 找到对应的通讯输出变量
    var commOutput = device.Table.Outputs.FirstOrDefault(o =>
     string.Equals(o.Name, mapping.CommOutputName, StringComparison.OrdinalIgnoreCase));

    if (commOutput == null)
    {
     LogHelper.Warn($"[{StationName}] 通讯输出变量[{mapping.CommOutputName}]未找到，跳过");
     continue;
    }

    // 3. 类型转换并写入
    var convertedValue = ConvertValueForCommOutput(value, commOutput.ValueType, mapping.ToolOutputName,
     mapping.CommOutputName);

    if (convertedValue == null)
    {
     continue; // 转换失败，已记录日志
    }

    // 4. 写入通讯设备
    device.Write(commOutput.Address, convertedValue);

    LogHelper.Info(
     $"[{StationName}] 输出映射成功: {mapping.ToolOutputName}({value.GetType().Name}={value}) => {mapping.CommOutputName}({commOutput.ValueType}@{commOutput.Address})");
   }
   catch (Exception ex)
   {
    LogHelper.Error(ex,
     $"[{StationName}] 输出映射失败: {mapping.ToolOutputName} => {mapping.CommOutputName}");
   }
  }
 }

 /// <summary>
 /// 将工具输出值转换为通讯输出所需的类型
 /// 支持常见的数值类型转换（如 int -> short, double -> float）
 /// </summary>
 private object ConvertValueForCommOutput(object value, HardwareCommNet.CommTable.CommValueType targetType,
  string toolOutputName, string commOutputName)
 {
  try
  {
   var sourceType = value.GetType();

   // 相同类型直接返回
   if (IsCompatibleType(sourceType, targetType))
   {
    return value;
   }

   // 数值类型转换
   switch (targetType)
   {
    case HardwareCommNet.CommTable.CommValueType.Bool:
     return Convert.ToBoolean(value);

    case HardwareCommNet.CommTable.CommValueType.Short:
     // int -> short 需要检查范围
     if (sourceType == typeof(int))
     {
      var intValue = (int)value;
      if (intValue < short.MinValue || intValue > short.MaxValue)
      {
       LogHelper.Warn(
        $"[{StationName}] 输出映射: {toolOutputName}({intValue})超出short范围，将截断");
      }

      return (short)intValue;
     }

     return Convert.ToInt16(value);

    case HardwareCommNet.CommTable.CommValueType.Int:
     return Convert.ToInt32(value);

    case HardwareCommNet.CommTable.CommValueType.Float:
     // double -> float
     if (sourceType == typeof(double))
     {
      var doubleValue = (double)value;
      if (Math.Abs(doubleValue) > float.MaxValue)
      {
       LogHelper.Warn(
        $"[{StationName}] 输出映射: {toolOutputName}({doubleValue})超出float范围，将截断");
      }

      return (float)doubleValue;
     }

     return Convert.ToSingle(value);

    case HardwareCommNet.CommTable.CommValueType.Double:
     return Convert.ToDouble(value);

    case HardwareCommNet.CommTable.CommValueType.String:
     return value.ToString();

    // 数组类型转换（简单处理，实际可能需要更复杂的逻辑）
    case HardwareCommNet.CommTable.CommValueType.BoolArray:
    case HardwareCommNet.CommTable.CommValueType.ShortArray:
    case HardwareCommNet.CommTable.CommValueType.IntArray:
    case HardwareCommNet.CommTable.CommValueType.FloatArray:
    case HardwareCommNet.CommTable.CommValueType.DoubleArray:
    case HardwareCommNet.CommTable.CommValueType.StringArray:
     LogHelper.Warn(
      $"[{StationName}] 输出映射: 数组类型转换暂不支持 {toolOutputName} => {commOutputName}");
     return null;

    default:
     LogHelper.Warn(
      $"[{StationName}] 输出映射: 未知目标类型 {targetType} for {toolOutputName} => {commOutputName}");
     return null;
   }
  }
  catch (Exception ex)
  {
   LogHelper.Error(ex,
    $"[{StationName}] 输出映射类型转换失败: {toolOutputName}({value.GetType().Name}) => {commOutputName}({targetType})");
   return null;
  }
 }

 /// <summary>
 /// 检查源类型是否与目标CommValueType兼容（无需转换）
 /// </summary>
 private bool IsCompatibleType(Type sourceType, HardwareCommNet.CommTable.CommValueType targetType)
 {
  return targetType switch
  {
   HardwareCommNet.CommTable.CommValueType.Bool => sourceType == typeof(bool),
   HardwareCommNet.CommTable.CommValueType.Short => sourceType == typeof(short),
   HardwareCommNet.CommTable.CommValueType.Int => sourceType == typeof(int),
   HardwareCommNet.CommTable.CommValueType.Float => sourceType == typeof(float),
   HardwareCommNet.CommTable.CommValueType.Double => sourceType == typeof(double),
   HardwareCommNet.CommTable.CommValueType.String => sourceType == typeof(string),
   HardwareCommNet.CommTable.CommValueType.BoolArray => sourceType == typeof(bool[]),
   HardwareCommNet.CommTable.CommValueType.ShortArray => sourceType == typeof(short[]),
   HardwareCommNet.CommTable.CommValueType.IntArray => sourceType == typeof(int[]),
   HardwareCommNet.CommTable.CommValueType.FloatArray => sourceType == typeof(float[]),
   HardwareCommNet.CommTable.CommValueType.DoubleArray => sourceType == typeof(double[]),
   HardwareCommNet.CommTable.CommValueType.StringArray => sourceType == typeof(string[]),
   _ => false
  };
 }

 private async Task ProcessDisplayAndSave(ImageFrame frame, StationConfig station, bool result, ICogRecord record)
 {
  object recordDisplay = null;
  try
  {
   if (station.bShow)
   {
    if (string.IsNullOrWhiteSpace(station.DisplayName))
    {
     LogHelper.Warn($"工位[{StationName}]未配置显示界面");
    }
    else
    {
     var disp = SolutionManager.Instance.Current?.Display;
     string key = null;
     if (disp?.Items != null)
     {
      var item = disp.Items.FirstOrDefault(i =>
       string.Equals(i.DisplayName, station.DisplayName, StringComparison.OrdinalIgnoreCase) ||
       string.Equals(i.Key, station.DisplayName, StringComparison.OrdinalIgnoreCase));
      key = item?.Key;
     }

     if (string.IsNullOrWhiteSpace(key)) key = disp?.Items?.FirstOrDefault()?.Key ?? "显示1";
     if (SolutionManager.Instance.Current.DisplayControls.TryGetValue(key, out var ctrl) && ctrl != null)
     {
      if (record != null) ctrl.Record = record;
      else ctrl.CogImage = frame.Image;
      recordDisplay = ctrl.RecordDisplay;
     }
     else
     {
      LogHelper.Warn($"工位[{StationName}]显示控件[{key}]未找到");
     }
    }
   }
  }
  catch (Exception ex)
  {
   LogHelper.Error(ex, $"工位[{StationName}]显示更新失败");
  }

  await Task.Run(() =>
  {
   try
   {
    if (FileSettingsManager.Current.SaveRawImage && station.SaveRawImage)
    {
     var req = new SaveRequest
     {
      Station = StationName, Result = FileSettingsManager.Current.SeparateOkNg ? (result ? "OK" : "NG") : "OK",
      VisionProImage = frame.Image, Type = FileSettingsManager.Current.RawImageType, ScalePercent = 100,
      IsDealImage = false, ImageName = $"{StationName}_{DateTime.Now:yyyyMMdd_HHmmss}"
     };
     ImageSaver.Enqueue(req);
    }

    if (FileSettingsManager.Current.SaveDealImage && station.SaveDealImage && recordDisplay != null)
    {
     try
     {
      var method = recordDisplay.GetType().GetMethod("CreateContentBitmap", [typeof(int)]);
      if (method != null)
      {
       //13 is CogDisplayContentBitmapConstants.Image in most versions, avoid hard reference
       var imgObj = method.Invoke(recordDisplay, [13]);
       if (imgObj is Bitmap img)
       {
        var cogImage = new CogImage24PlanarColor(new Bitmap(img));
        var req = new SaveRequest
        {
         Station = StationName, Result = FileSettingsManager.Current.SeparateOkNg ? (result ? "OK" : "NG") : "OK",
         VisionProImage = cogImage, Type = FileSettingsManager.Current.DealImageType, ScalePercent = 100,
         IsDealImage = true, ImageName = $"{StationName}_{DateTime.Now:yyyyMMdd_HHmms}"
        };
        ImageSaver.Enqueue(req);
       }
      }
     }
     catch (Exception ex)
     {
      LogHelper.Error(ex, $"工位[{StationName}]创建处理图失败");
     }
    }
   }
   catch (Exception ex)
   {
    LogHelper.Error(ex, $"工位[{StationName}]存图异常");
   }
  }).ConfigureAwait(false);
 }

 public void Dispose()
 {
  if (_disposed) return;
  _disposed = true;
  try
  {
   _cts.Cancel();
  }
  catch
  {
  }

  try
  {
   _runSemaphore?.Dispose();
  }
  catch
  {
  }
 }
}
