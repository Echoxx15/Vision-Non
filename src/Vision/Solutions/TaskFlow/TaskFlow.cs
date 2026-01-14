using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Logger;
using Vision.LightSource;
using HardwareCameraNet;
using Vision.SaveImage;
using Vision.Settings;
using Vision.Solutions.Models;

namespace Vision.Solutions.TaskFlow;

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
    private string Code = "";
    private bool _disposed;
    private string StationName => _station.Name;

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

            Code = DateTime.Now.ToString("HH-mm-ss");
            TriggerCamera(clientId);
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]启动失败");
        }
    }

    public void StartFromImage(string imagePath)
    {
        StartFromImage(imagePath, 1);
    }

    /// <summary>
    /// 从图片启动流程（指定ImageIndex，用于飞拍模拟）
    /// </summary>
    /// <param name="imagePath">图片路径</param>
    /// <param name="imageIndex">图片序号（从1开始）</param>
    public void StartFromImage(string imagePath, int imageIndex)
    {
        if (_disposed) return;
        if (string.IsNullOrWhiteSpace(imagePath)) return;
        try
        {
            using var img = Image.FromFile(imagePath);
            using var bmp = new Bitmap(img);
            ICogImage cogImg = bmp.PixelFormat != PixelFormat.Format8bppIndexed
                ? new CogImage8Grey(bmp)
                : new CogImage24PlanarColor(bmp);
            _imageQueue.Enqueue(new ImageFrame
                { StationName = StationName, Image = cogImg, ImageIndex = imageIndex, ClientId = string.Empty });
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]加载图片失败: {imagePath}");
        }
    }

    private void TriggerCamera(string clientId)
    {
        var cameras = CameraFactory.Instance.GetAllCameras();
        var camera = cameras.FirstOrDefault(c => string.Equals(c.SN, _station.SN, StringComparison.OrdinalIgnoreCase));
        if (camera == null)
        {
            LogHelper.Warn($"[TaskFlow] 工位[{StationName}]对应相机[{_station.SN}]未找到");
            return;
        }

        int imageIndex = 0;
        int required = _station.CameraParams?.TriggerCount ?? 1;

        // 使用 Action 回调方式
        camera.OnFrameGrabed = (img) =>
        {
            try
            {
                var currentIndex = ++imageIndex;
                _imageQueue.Enqueue(new ImageFrame
                    { StationName = StationName, Image = img, ImageIndex = currentIndex, ClientId = clientId });
                LogHelper.Info($"[{StationName}]拍照完成次数: {currentIndex}");

                // ✅ 图像回调完成后异步关闭光源（异常单独捕获，不影响图像处理）
                if (_station.LightControl is { EnableLightControl: true })
                {
                    try
                    {
                        LightSourceManager.Instance.TurnOffStationLightAsync(_station.LightControl);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]关闭光源异常");
                    }
                }

                // 软件触发一次即清空回调；硬件触发按计数
                if (_station.CameraParams?.TriggerMode == TriggerMode.软触发)
                {
                    camera.OnFrameGrabed = null;
                }
                else if (_station.CameraParams?.TriggerMode == TriggerMode.硬触发)
                {
                    if (currentIndex >= required) camera.OnFrameGrabed = null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]采集回调异常");
                camera.OnFrameGrabed = null;
            }
        };

        try
        {
            Stopwatch stp = new Stopwatch();
            stp.Start();
            // 设置相机参数
            if (_station.CameraParams != null)
            {
                camera.Parameters.ExposureTime = _station.CameraParams.Exposure;
                camera.Parameters.Gain = _station.CameraParams.Gain;
            }

            LogHelper.Info($"工位[{StationName}]设置相机参数耗时:{stp.ElapsedMilliseconds}");
            stp.Restart();
            // ✅ 软触发前打开光源（同步，异常单独捕获）
            if (_station.CameraParams?.TriggerMode == TriggerMode.软触发)
            {
                // 打开光源并等待延时
                if (_station.LightControl is { EnableLightControl: true })
                {
                    try
                    {
                        var success =
                            LightSourceManager.Instance.ControlStationLight(_station.LightControl, turnOn: true);
                        LogHelper.Info($"工位[{StationName}]打开光源耗时:{stp.ElapsedMilliseconds}");
                        if (!success)
                        {
                            LogHelper.Warn($"工位[{StationName}]打开光源失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, $"工位[{StationName}]打开光源异常");
                    }
                }

                Thread.Sleep(_station.TrgDelay);
                // 执行软触发
                camera.SoftwareTriggerOnce();
                LogHelper.Info($"工位[{StationName}]执行软触发");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"工位[{StationName}]触发失败");
            camera.OnFrameGrabed = null;
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
                            LogHelper.Error(ex, $"工位[{StationName}]处理异常");
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
                _station.CheckerboardTool.ApplyVarsToInputs();
                t.Inputs["Image"].Value = cogImg;
                t.Run();
                cogImg = (ICogImage)t.Outputs["Image"].Value;
            }

            if (_station.bCalibNPointTool && _station.NPointTool?.ToolBlock != null)
            {
                var t = _station.NPointTool.ToolBlock;
                _station.NPointTool.ApplyVarsToInputs();
                t.Inputs["Image"].Value = cogImg;
                t.Run();
                cogImg = (ICogImage)t.Outputs["Image"].Value;
            }

            _station.DetectionTool?.ApplyVarsToInputs();


            tool.Inputs["Image"].Value = cogImg;
            tool.Inputs["Index"].Value = frame.ImageIndex;
            tool.Run();
            result = (bool)tool.Outputs["Result"].Value;
            LogHelper.Info($"[{StationName}]检测完成，结果[{(result ? "OK" : "NG")}]");

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

            // ✅ 步骤3：根据每个映射的 SendEveryTime 配置决定是否写入输出到通讯设备
            HandleResultSend(frame.ImageIndex);

            // 保存工具输出到 LastOutputs
            try
            {
                var sol = SolutionManager.Instance.Current;
                if (sol != null)
                {
                    sol.LastOutputs ??=
                        new Dictionary<string,
                            Dictionary<string, object>>(StringComparer
                            .OrdinalIgnoreCase);
                    if (!sol.LastOutputs.TryGetValue(StationName, out var dict) || dict == null)
                    {
                        dict = new Dictionary<string, object>(StringComparer
                            .OrdinalIgnoreCase);
                        sol.LastOutputs[StationName] = dict;
                    }

                    for (int i = 0; i < tool.Outputs.Count; i++)
                    {
                        var term = tool.Outputs[i];
                        try
                        {
                            dict[term.Name] = term.Value;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"工位[{StationName}]工具运行异常");
            return;
        }

        _ = Task.Run(() => ProcessDisplayAndSave(frame, _station, result, record));
    }

    /// <summary>
    /// 根据 ToolType 获取对应的工具
    /// </summary>
    private Cognex.VisionPro.ToolBlock.CogToolBlock GetToolByType(string toolType)
    {
        switch (toolType)
        {
            case "Detection":
                return _station.DetectionTool?.ToolBlock;
            case "Checkerboard":
                return _station.CheckerboardTool?.ToolBlock;
            case "NPoint":
                return _station.NPointTool?.ToolBlock;
            default:
                return _station.DetectionTool?.ToolBlock; // 默认使用检测工具（兼容旧配置）
        }
    }

    /// <summary>
    /// 根据每个映射的配置决定是否发送
    /// </summary>
    /// <param name="imageIndex">当前拍照序号</param>
    private void HandleResultSend(int imageIndex)
    {
        // 获取触发次数配置
        int triggerCount = _station.CameraParams?.TriggerCount ?? 1;
        bool isLastShot = (imageIndex >= triggerCount);

        // 调用带条件的写入方法
        WriteToolOutputsToComm(isLastShot);
    }

    /// <summary>
    /// 将工具输出端子的值写入通讯设备的输出表
    /// 根据每个映射的 SendEveryTime 属性决定是否发送
    /// </summary>
    /// <param name="isLastShot">是否为最后一次拍照</param>
    private void WriteToolOutputsToComm(bool isLastShot)
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
                // ✅ 根据 SendEveryTime 属性决定是否发送
                // SendEveryTime = true: 每次拍照都发送
                // SendEveryTime = false: 仅在最后一次拍照（ImageIndex == TriggerCount）时发送
                if (!mapping.SendEveryTime && !isLastShot)
                {
                    // 不是最后一次拍照，且该映射配置为仅最后发送，跳过
                    continue;
                }

                // 0. 根据 ToolType 获取对应的工具
                var tool = GetToolByType(mapping.ToolType);
                if (tool == null)
                {
                    LogHelper.Warn($"[{StationName}] 工具类型[{mapping.ToolType}]对应的工具未加载，跳过");
                    continue;
                }

                // 1. 从工具输出端子获取值
                if (!tool.Outputs.Contains(mapping.ToolOutputName))
                {
                    LogHelper.Warn($"[{StationName}] 工具[{mapping.ToolType}]输出端子[{mapping.ToolOutputName}]不存在，跳过");
                    continue;
                }

                var toolOutput = tool.Outputs[mapping.ToolOutputName];
                if (toolOutput == null)
                {
                    LogHelper.Warn($"[{StationName}] 工具[{mapping.ToolType}]输出端子[{mapping.ToolOutputName}]为null，跳过");
                    continue;
                }

                var value = toolOutput.Value;
                if (value == null)
                {
                    LogHelper.Warn($"[{StationName}] 工具[{mapping.ToolType}]输出端子[{mapping.ToolOutputName}]值为null，跳过");
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

                string sendMode = mapping.SendEveryTime ? "每次" : "最后";
                LogHelper.Info(
                    $"[{StationName}] 输出映射成功({sendMode}): [{mapping.ToolType}]{mapping.ToolOutputName}({value.GetType().Name}={value}) => {mapping.CommOutputName}({commOutput.ValueType}@{commOutput.Address})");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex,
                    $"[{StationName}] 输出映射失败: [{mapping.ToolType}]{mapping.ToolOutputName} => {mapping.CommOutputName}");
            }
        }
    }

    /// <summary>
    /// 将工具输出值转换为通讯输出所需的类型
    /// 优先直接使用端子实际值，仅在类型不兼容时才进行转换
    /// 通讯适配器会根据实际值类型自行处理写入
    /// </summary>
    private object ConvertValueForCommOutput(object value, HardwareCommNet.CommTable.CommValueType targetType,
        string toolOutputName, string commOutputName)
    {
        try
        {
            var sourceType = value.GetType();

            // 相同类型或兼容类型直接返回，让通讯适配器处理
            if (IsCompatibleType(sourceType, targetType))
            {
                return value;
            }

            // 常见兼容类型直接返回（通讯适配器支持这些类型）
            // 例如: double可以直接发给Float目标（适配器会转换）
            //       int可以直接发给Short目标（适配器会转换）
            if (IsCommonNumericType(sourceType) && IsNumericTargetType(targetType))
            {
                // 数值类型直接返回，让通讯适配器处理实际写入
                return value;
            }

            // 数组类型：如果源是数组且目标也是数组类型，直接返回
            if (sourceType.IsArray && IsArrayTargetType(targetType))
            {
                return value;
            }

            // 字符串类型直接返回
            if (targetType == HardwareCommNet.CommTable.CommValueType.String)
            {
                return value.ToString();
            }

            // 其他情况直接返回原值，交给通讯适配器处理
            return value;
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex,
                $"[{StationName}] 输出映射类型转换失败: {toolOutputName}({value.GetType().Name}) => {commOutputName}({targetType})");
            return null;
        }
    }

    /// <summary>
    /// 判断是否为常见数值类型
    /// </summary>
    private bool IsCommonNumericType(Type type)
    {
        return type == typeof(bool) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(decimal) || type == typeof(byte) || type == typeof(sbyte);
    }

    /// <summary>
    /// 判断目标类型是否为数值类型
    /// </summary>
    private bool IsNumericTargetType(HardwareCommNet.CommTable.CommValueType targetType)
    {
        return targetType == HardwareCommNet.CommTable.CommValueType.Bool ||
               targetType == HardwareCommNet.CommTable.CommValueType.Short ||
               targetType == HardwareCommNet.CommTable.CommValueType.Int ||
               targetType == HardwareCommNet.CommTable.CommValueType.Float ||
               targetType == HardwareCommNet.CommTable.CommValueType.Double;
    }

    /// <summary>
    /// 判断目标类型是否为数组类型
    /// </summary>
    private bool IsArrayTargetType(HardwareCommNet.CommTable.CommValueType targetType)
    {
        return targetType == HardwareCommNet.CommTable.CommValueType.BoolArray ||
               targetType == HardwareCommNet.CommTable.CommValueType.ShortArray ||
               targetType == HardwareCommNet.CommTable.CommValueType.IntArray ||
               targetType == HardwareCommNet.CommTable.CommValueType.FloatArray ||
               targetType == HardwareCommNet.CommTable.CommValueType.DoubleArray ||
               targetType == HardwareCommNet.CommTable.CommValueType.StringArray;
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
        // 计算本次流程的Code（码）与分组逻辑
        try
        {
            // 1. 从通讯输入获取Code（字符串）
            if (!string.IsNullOrWhiteSpace(station.CommDeviceName) &&
                !string.IsNullOrWhiteSpace(station.CodeInputName))
            {
                try
                {
                    var device = HardwareCommNet.CommunicationFactory.Instance.GetDevice(station.CommDeviceName);
                    var val = device?.Table?.GetInputCachedValue(station.CodeInputName);
                    if (val != null)
                    {
                        Code = val.ToString();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"[TaskFlow] 工位[{StationName}]读取码输入[{station.CodeInputName}]失败");
                }
            }
        }
        catch { }

        int triggerCount = station.CameraParams?.TriggerCount ?? 1;
        bool isLastShot = frame.ImageIndex >= triggerCount;

        CogRecordDisplay recordDisplay = null;
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
                        // 第一张图片时清空列表
                        if (frame.ImageIndex == 1)
                        {
                            ctrl.Clear();
                        }
                        
                        if (record != null)
                        {
                            ctrl.AddRecord(record);
                        }
                        else
                        {
                            ctrl.CogImage = frame.Image;
                        }
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
                var cfg = FileSettingsManager.Current;
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                var resultFolder = cfg.SeparateOkNg ? (result ? "OK" : "NG") : "OK";
                
                // 文件名：工位名_图像下标_码.后缀
                string baseFileName = $"{StationName}_{frame.ImageIndex}_{Code}";
                
                string savedRawPath = null;

                if (cfg.SaveRawImage && station.SaveRawImage)
                {
                    var rawDir = System.IO.Path.Combine(cfg.SavePath, date, "Raw", StationName, Code, resultFolder);
                    System.IO.Directory.CreateDirectory(rawDir);
                    
                    var rawFileName = $"{baseFileName}.{cfg.RawImageType}";
                    savedRawPath = System.IO.Path.Combine(rawDir, rawFileName);
                    
                    var req = new SaveRequest
                    {
                        FullPath = savedRawPath,
                        VisionProImage = frame.Image,
                        Type = cfg.RawImageType,
                        ScalePercent = 100,
                        IsDealImage = false
                    };
                    ImageSaver.Enqueue(req);
                }

                if (cfg.SaveDealImage && station.SaveDealImage && recordDisplay != null)
                {
                    try
                    {
                        var img = recordDisplay.CreateContentBitmap(CogDisplayContentBitmapConstants.Custom);
                        if (img != null)
                        {
                            var dealDir = System.IO.Path.Combine(cfg.SavePath, date, "Deal", StationName, Code,
                                resultFolder);
                            System.IO.Directory.CreateDirectory(dealDir);

                            var dealFileName = $"{baseFileName}.{cfg.DealImageType}";
                            var savedDealPath = System.IO.Path.Combine(dealDir, dealFileName);

                            var cogImage = new CogImage24PlanarColor(new Bitmap(img));
                            var req = new SaveRequest
                            {
                                FullPath = savedDealPath,
                                VisionProImage = cogImage,
                                Type = cfg.DealImageType,
                                ScalePercent = 100,
                                IsDealImage = true
                            };
                            ImageSaver.Enqueue(req);

                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, $"工位[{StationName}]创建处理图失败");
                    }
                }

                // 写入路径到 txt 文件
                try
                {
                    if (!string.IsNullOrEmpty(savedRawPath))
                    {
                        var codeFolder = System.IO.Path.Combine(cfg.SavePath, date, "Raw", StationName, Code);
                        var txtPath = System.IO.Path.Combine(codeFolder, "paths.txt");
                        System.IO.Directory.CreateDirectory(codeFolder);
                        System.IO.File.AppendAllLines(txtPath, [savedRawPath]);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"工位[{StationName}]写入路径txt失败");
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
