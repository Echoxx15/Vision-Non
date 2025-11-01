using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ToolBlock;
using Logger;
using Vision.Comm.Modbus;
using Vision.Comm.TcpManager;
using Vision.Manager.CameraManager;
using Vision.SaveImage;
using Vision.Settings;
using Vision.Solutions.Models;
// 添加TCP管理器引用

namespace Vision.Solutions.WorkFlow;

/// <summary>
/// 图像帧数据结构
/// 用于在队列中传递图像及其相关信息
/// </summary>
public sealed class ImageFrame
{
    /// <summary>
    /// 工位枚举，用于标识该图像来自哪个工位
    /// </summary>
    public EnumStation enumStation { get; set; }

    /// <summary>
    /// VisionPro图像对象，包含实际的图像数据
    /// </summary>
    public ICogImage Image { get; set; }

    /// <summary>
    /// 图像索引，用于多图像序列场景（如线扫相机）
    /// </summary>
    public int ImageIndex { get; set; }

  /// <summary>
    /// 客户端ID，用于服务器模式下标识消息来源
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
}

/// <summary>
/// 工作流核心类（单例模式）
/// 负责管理整个视觉检测流程，包括：
/// 1. 通讯管理（TCP客户端/服务器、Modbus）
/// 2. 工位管理和触发控制
/// 3. 图像采集和处理队列
/// 4. 并发控制和异步处理
/// 5. 系统在线/离线状态管理
/// </summary>
internal sealed class WorkFlow : IDisposable
{
  #region 单例模式

  /// <summary>
  /// 延迟初始化的单例实例
  /// 线程安全，第一次访问时才创建实例
  /// </summary>
  private static readonly Lazy<WorkFlow> _inst = new Lazy<WorkFlow>(() => new WorkFlow());

  /// <summary>
  /// 获取WorkFlow单例实例
  /// </summary>
  public static WorkFlow Instance => _inst.Value;

  #endregion

  #region 通讯模块

  #region 地址定义

  /// <summary>
  /// 触发相机拍照信号，1-左下相机，2-左机械手,3-右下相机，4-右机械手
  /// </summary>
  private string[] trgCameraAddressArray = ["220", "240", "320", "340"];

  /// <summary>
  /// 工位结果返回地址，工位对应触发值1-OK，2-NG
  /// </summary>
  private string[] reResultAddressArray = ["221", "241", "244", "321", "341", "344"];

  #endregion

  #endregion

  #region 工位管理

  /// <summary>
  /// 工位字典：工位名称 → 工位配置
  /// 使用ConcurrentDictionary确保线程安全
  /// 忽略大小写比较（OrdinalIgnoreCase）
  /// </summary>
  private readonly ConcurrentDictionary<string, ProcessStation>
    _stationByName = new(StringComparer.OrdinalIgnoreCase);

  /// <summary>
  /// 工位触发状态：工位名称 → 是否正在采集
  /// 用于防止同一工位重复触发导致事件堆积
  /// </summary>
  private readonly ConcurrentDictionary<string, bool>
    _stationBusy = new(StringComparer.OrdinalIgnoreCase);

  #endregion

  #region 图像处理队列

  /// <summary>
  /// 图像处理队列（线程安全）

  /// </summary>
  private readonly ConcurrentQueue<ImageFrame> _imageQueue = new();

  #endregion

  #region 线程和任务管理

  /// <summary>
  /// 取消令牌源，用于优雅关闭所有后台任务
  /// 在Dispose时调用Cancel()停止所有循环
  /// </summary>
  private readonly CancellationTokenSource _cts;

  #endregion

  #region 并发控制

  /// <summary>
  /// 工位并发控制信号量
  /// 限制同时运行的工位数量（默认最多4个）
  /// 防止系统资源耗尽（CPU、内存、相机资源）
  /// 
  /// 配置建议：
  /// - 低性能PC (4核): SemaphoreSlim(2, 2)
  /// - 中等PC (8核): SemaphoreSlim(4, 4) [当前配置]
  /// - 高性能PC (16核): SemaphoreSlim(8, 8)
  /// </summary>
  private readonly SemaphoreSlim _stationSemaphore = new SemaphoreSlim(4, 4);

  #endregion

  #region 系统状态

  /// <summary>
  /// 系统在线状态标志
  /// true: 在线，允许处理通讯触发信号
  /// false: 离线，拒绝通讯触发（仅记录日志）
  /// 
  /// 注意：手动触发（ManualRun）不受此状态影响
  /// 
  /// 默认值：true（程序启动后默认在线，可接收外部触发）
  /// </summary>
  public bool IsOnline { get; private set; } = true;

  /// <summary>
  /// 系统在线状态变化事件
  /// 订阅者：主界面（更新UI状态、启用/禁用工具栏）
  /// </summary>
  public event Action<bool> OnlineStateChanged;

  #endregion

  #region 构造函数

  /// <summary>
  /// 私有构造函数（单例模式）
  /// 初始化顺序：
  /// 1. 创建取消令牌
  /// 2. 启动图像出队循环
  /// 3. 初始化TCP客户端
  /// 4. 构建工位映射
  /// 5. 订阅方案变化事件
  /// 6. 应用相机参数
  /// </summary>
  private WorkFlow()
  {
    _cts = new CancellationTokenSource();

    // 启动图像处理后台任务（生产者-消费者模式）
    StartDequeueLoop(_cts.Token);

    // 从当前方案加载工位配置
    RebuildStationsFromSolution();

    var threadModbusPolling = new Thread(StartModbusPolling)
    {
      IsBackground = true
    };
    threadModbusPolling.Start();

    try
    {
      // 监听方案切换事件，自动重建工位映射和通讯配置
      SolutionManager.Instance.CurrentChanged += OnSolutionChanged;
    }
    catch
    {
      // 忽略订阅失败（可能方案管理器未初始化）
    }

    // 初始化时应用所有工位的相机参数
    // 注意：延迟执行，确保相机初始化
    Task.Run(async () =>
    {
      await Task.Delay(1000); // 等待1秒，确保相机初始化完成
      ApplyCameraParameters();
    });
  }

  #endregion

  #region IDisposable实现

  /// <summary>
  /// 释放资源，优雅关闭所有后台任务

  /// </summary>
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

    // 2. 清理Modbus连接（使用 ModbusManager）
    try
    {
      ModbusManager.Instance.DisposeAll();
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

  #endregion

  #region 工位管理

  /// <summary>
  /// 方案切换事件处理
  /// </summary>
  private void OnSolutionChanged()
  {
    RebuildStationsFromSolution();
    InitializeTcpFromSolution();

    // 方案切换后应用所有工位的相机参数
    ApplyCameraParameters();

    // 初始化光源控制器
    try
    {
      var solution = SolutionManager.Instance.Current;
      if (solution != null)
      {
        LightSource.LightSourceManager.Instance.InitializeFromSolution(solution);
      }
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, "初始化光源控制器失败");
    }
  }

  /// <summary>
  /// 从方案初始化TCP通讯配置
  /// </summary>
  private void InitializeTcpFromSolution()
  {
    try
    {
      var solution = SolutionManager.Instance.Current;
      if (solution == null) return;

      // 初始化TCP通讯管理器
      TcpCommManager.Instance.InitializeFromSolution(solution);

      // 订阅消息接收事件（只订阅一次，避免重复）
      TcpCommManager.Instance.MessageReceived -= OnTcpMessageReceived;
      TcpCommManager.Instance.MessageReceived += OnTcpMessageReceived;

      LogHelper.Info("TCP通讯配置已从方案加载");
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, "初始化TCP通讯配置失败");
    }
  }

  /// <summary>
  /// 从当前方案重建工位映射
  /// 当方案切换或初始化时调用
  /// 
  /// 处理逻辑：
  /// 1. 获取当前方案
  /// 2. 检查工位列表是否为空，为空则创建
  /// 3. 清空现有映射
  /// 4. 遍历工位列表，建立名称→工位的映射
  /// </summary>
  private void RebuildStationsFromSolution()
  {
    var sol = SolutionManager.Instance.Current;
    if (sol == null) return;

    var list = sol.Stations;
    if (list == null)
    {
      // 方案中没有工位列表，创建空列表
      list = [];
      try
      {
        sol.Stations = list;
      }
      catch
      {
        // 忽略赋值异常（可能方案为只读）
      }
    }

    // 清空现有映射
    _stationByName.Clear();

    // 重建映射：工位名称 → 工位对象
    foreach (var st in list.Where(st => !string.IsNullOrWhiteSpace(st?.Name)))
    {
      _stationByName[st.Name] = st;
    }
  }

  #endregion

  #region 相机操作

  /// <summary>
  /// 触发指定工位的相机拍照
  /// 
  /// 工作流程：
  /// 1. 查找工位配置
  /// 2. 查找相机
  /// </summary>
  /// <param name="stationName">工位名称</param>
  /// <param name="clientId">客户端ID（可选，用于结果回传）</param>
  private void TriggerCamera(EnumStation enumStation, string clientId = "")
  {
    var stationName = enumStation.ToString();
    // 1. 参数验证
    if (string.IsNullOrWhiteSpace(stationName)) return;

    // 2. 查找工位配置
    if (!_stationByName.TryGetValue(stationName, out var station) || station == null)
    {
      LogHelper.Warn($"工位[{stationName}]未找到");
      return;
    }

    // 3. 检查工位是否正在采集（防止重复触发）
    if (!_stationBusy.TryAdd(stationName, true))
    {
      LogHelper.Warn($"工位[{stationName}]正在采集中，忽略本次触发");
      return;
    }

    // 4. 获取相机管理器中的所有相机
    var cameras = CameraManager.Instance.GetAllCameras();
    if (cameras.Count == 0)
    {
      _stationBusy.TryRemove(stationName, out bool _); // 释放忙碌状态
      LogHelper.Warn("未发现任何相机");
      return;
    }

    // 5. 根据工位配置的SN查找相机
    var camera = cameras.FirstOrDefault(c =>
      string.Equals(c.SN, station.SN, StringComparison.OrdinalIgnoreCase));
    if (camera == null)
    {
      _stationBusy.TryRemove(stationName, out bool _); // 释放忙碌状态
      LogHelper.Warn($"工位[{stationName}]对应相机[{station.SN}]未找到");
      return;
    }

    // ? 新增：打开光源（异步，不阻塞）
    if (station.LightControl?.EnableLightControl == true)
    {
      _ = Task.Run(async () =>
      {
        try
        {
          await LightSource.LightSourceManager.Instance.ControlStationLight(
            station.LightControl, turnOn: true);
        }
        catch (Exception ex)
        {
          LogHelper.Error(ex, $"工位[{stationName}]光源控制失败");
        }
      });
    }

    // 6. 初始化图像索引计数器（本次触发从0开始）
    var imageIndex = 1; // 
    // 7. 定义采图事件处理器
    EventHandler<ICogImage> handler = null;
    handler = (_, e) =>
    {
      try
      {
        // 获取当前图像的索引，然后递增
        var currentIndex = imageIndex++;

        // 将图像包装成ImageFrame并入队
        _imageQueue.Enqueue(new ImageFrame
        {
          enumStation = enumStation,
          Image = e,
          ImageIndex = currentIndex,
          ClientId = clientId
        });

        // 软触发模式：单次采集后立即解除订阅
        // 硬触发模式：需要根据触发次数判断是否解除订阅
        if (station.CameraParams?.TriggerMode == TriggerMode.Software)
        {
          // 软触发只采集一张，立即解除订阅
          camera.FrameGrabedEvent -= handler;
          _stationBusy.TryRemove(stationName, out bool _); // ? 释放忙碌状态
        }
        else if (station.CameraParams?.TriggerMode == TriggerMode.Hardware)
        {
          // 硬触发模式：检查是否已采集足够的图像
          var requiredCount = station.CameraParams?.TriggerCount ?? 1;
          if (currentIndex + 1 >= requiredCount)
          {
            // 已采集足够的图像，解除订阅
            camera.FrameGrabedEvent -= handler;
            _stationBusy.TryRemove(stationName, out bool _); // ? 释放忙碌状态
          }
          // 否则继续等待下次硬触发回调
        }
      }
      catch (Exception ex)
      {
        LogHelper.Error(ex, $"工位[{stationName}]采集回调异常");

        // 异常时也要解除订阅，防止内存泄漏
        try
        {
          camera.FrameGrabedEvent -= handler;
          _stationBusy.TryRemove(stationName, out bool _); // ? 释放忙碌状态
        }
        catch
        {
          // 忽略解除订阅异常
        }
      }
    };

    // 8. 订阅采图完成事件
    camera.FrameGrabedEvent -= handler; // 先解除可能存在的旧订阅
    camera.FrameGrabedEvent += handler;

    try
    {
      // 9. 触发拍照
      // 注意：相机参数已在初始化或配置界面设置，此处不再设置
      if (station.CameraParams?.TriggerMode == TriggerMode.Software)
      {
        // 软触发：手动触发一次
        camera.SoftwareTriggerOnce();
      }
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"工位[{stationName}]触发失败");

      // 触发失败时解除订阅，防止内存泄漏
      try
      {
        camera.FrameGrabedEvent -= handler;
        _stationBusy.TryRemove(stationName, out bool _); // ? 释放忙碌状态
      }
      catch
      {
        // 忽略解除订阅异常
      }
    }
  }

  #endregion

  #region 手动执行

  #region 触发相机执行

  /// <summary>
  /// 手动触发相机拍照（用于调试和测试）
  /// 
  /// <param name="stationName">工位名称</param>
  public void ManualRun(string stationName)
  {
    if (string.IsNullOrWhiteSpace(stationName)) return;

    try
    {
      if (Enum.TryParse(stationName, out EnumStation enumStation))
      {
        TriggerCamera(enumStation);
      }

    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"手动触发运行失败");
    }
  }

  #endregion

  #region 加载本地图片执行

  /// <summary>
  /// 手动加载本地图片进行检测（用于离线测试）
  /// 
  /// </summary>
  /// <param name="imagePath">图片完整路径</param>
  /// <param name="stationName">工位名称</param>
  public void ManualRun(string imagePath, string stationName)
  {
    if (string.IsNullOrWhiteSpace(imagePath) || string.IsNullOrWhiteSpace(stationName))
      return;

    try
    {
      if (!Enum.TryParse(stationName, out EnumStation enumStation))
      {
        LogHelper.Warn($"[{stationName}]工位不在枚举类型中");
        return;
      }

      // 1. 加载图片文件
      using var img = Image.FromFile(imagePath);
      using var bmp = new Bitmap(img);

      // 2. 根据像素格式转换为VisionPro图像
      ICogImage cogImg = bmp.PixelFormat != PixelFormat.Format8bppIndexed
        ? new CogImage24PlanarColor(bmp) // 彩色图
        : new CogImage8Grey(bmp); // 灰度图

      // 3. 创建图像帧
      var frame = new ImageFrame
      {
        enumStation = enumStation,
        Image = cogImg,
        ImageIndex = 0, // 本地图片索引为0
        ClientId = string.Empty // 手动加载无客户端ID
      };

      // 4. 入队等待处理
      _imageQueue.Enqueue(frame);
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"手动加载图片运行失败: {imagePath}");
    }
  }

  #endregion

  #endregion

  #region 系统在线控制

  /// <summary>
  /// 设置系统在线/离线状态
  /// 
  /// </summary>
  /// <param name="online">true=在线，false=离线</param>
  public void SetOnlineState(bool online)
  {
    // 状态未变化，直接返回
    if (IsOnline == online) return;

    IsOnline = online;
    LogHelper.Info($"系统状态切换为: {(online ? "在线" : "离线")}");

    // 触发状态变化事件，通知订阅者（主界面）
    try
    {
      OnlineStateChanged?.Invoke(IsOnline);
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, "触发在线状态变化事件异常");
    }
  }

  #endregion

  #region 不同程序常需修改的方法
  #region TCP消息处理

  /// <summary>
  /// TCP消息统一处理入口
  /// 参数：(TCP名称, 客户端ID, 消息内容)
  /// </summary>
  private void OnTcpMessageReceived(string tcpName, string clientId, string msg)
  {
    if (string.IsNullOrWhiteSpace(msg)) return;

    // 系统离线检查：离线时不处理通讯触发
    if (!IsOnline)
    {
      LogHelper.Warn($"系统离线中，收到TCP[{tcpName}]消息但不执行操作");
      return;
    }

    var line = msg.Trim();
    var strTrigger = "T,";

    // 解析触发消息格式：T,<工位代码>
    if (line.StartsWith(strTrigger, StringComparison.OrdinalIgnoreCase))
    {
      //var str = line.Substring(strTrigger.Length).Trim();

    }
  }

  #endregion

  #region Modbus通讯

  /// <summary>
  /// 开启Modbus轮询任务
  /// </summary>
  private void StartModbusPolling()
  {
    var token = _cts.Token;


    while (!token.IsCancellationRequested)
    {
      try
      {
        // 获取Modbus配置（单设备模式）
        var config = ModbusManager.Instance.GetConfig();
        // 获取访问器
        var accessor = ModbusManager.Instance.GetAccessor();
        //只在连接以及系统在线时轮询
        if (!ModbusManager.Instance.IsConnected() || !IsOnline || config is not { Enabled: true })
        {
          Thread.Sleep(100);
          continue;
        }

        var val = accessor.ReadShort(trgCameraAddressArray[0]);
        if (val == 1)
        {

          var station = EnumStation.左下相机工位;
          LogHelper.Info($"收到[{nameof(station)}]请求");
          TriggerCamera(station);
          accessor.WriteShort(trgCameraAddressArray[0], 0);
        }

        val = accessor.ReadShort(trgCameraAddressArray[1]);
        if (val == 1)
        {

          var station = EnumStation.左上相机工位;
          LogHelper.Info($"收到[{station.ToString()}]请求");
          TriggerCamera(station);
          accessor.WriteShort(trgCameraAddressArray[1], 0);
        }

        val = accessor.ReadShort(trgCameraAddressArray[2]);
        if (val == 1)
        {

          var station = EnumStation.右下相机工位;
          LogHelper.Info($"收到[{station.ToString()}]请求");
          TriggerCamera(station);
          accessor.WriteShort(trgCameraAddressArray[2], 0);
        }

        val = accessor.ReadShort(trgCameraAddressArray[3]);
        if (val == 1)
        {

          var station = EnumStation.右上相机工位;
          LogHelper.Info($"收到[{station.ToString()}]请求");
          TriggerCamera(station);
          accessor.WriteShort(trgCameraAddressArray[3], 0);
        }
      }
      catch (Exception)
      {
        // 轮询健壮性：忽略单次异常，继续下次轮询
        //LogHelper.Error(ex, "Modbus轮询异常");
      }

      Thread.Sleep(50);
    }

    LogHelper.Info("Modbus轮询已停止");
  }

  #endregion

  #region 核心处理逻辑

  /// <summary>
  /// 运行检测工具（同步执行，但在独立线程中）
  /// 
  /// </summary>
  /// <param name="frame">图像帧数据</param>
  private void RunTool(ImageFrame frame)
  {
    var strStationName = frame.enumStation.ToString();

    // 1. 验证工位配置和检测工具
    if (!_stationByName.TryGetValue(strStationName, out var station) ||
        station?.DetectionTool?.ToolBlock == null)
      return;

    var tool = station.DetectionTool.ToolBlock;

    // 2. 应用全局变量到工具输入
    // 例如：将方案中的参数（阈值、ROI等）传递给工具
    station.DetectionTool.ApplyVarsToInputs();

    bool result; // 检测结果（OK/NG）
    ICogRecord record = null; // 显示记录（用于界面显示）

    try
    {
      // 3. 检查工位是否启用
      if (!station.Enable)
      {
        LogHelper.Info($"[{station.Name}]未启用");
        return;
      }

      LogHelper.Info($"[{station.Name}]开始执行检测");
      var cogImg = frame.Image;

      // 4. 执行棋盘格标定工具（如果启用）
      // 用途：畸变校正、坐标转换
      if (station.bCalibCheckboardTool)
      {
        var t = station.CheckerboardTool.ToolBlock;
        t.Inputs["Image"].Value = cogImg;
        t.Run();
        cogImg = (ICogImage)t.Outputs["Image"].Value; // 获取校正后的图像
      }

      // 5. 执行九点标定工具（如果启用）
      // 用途：坐标系转换、机器人手眼标定
      if (station.bCalibNPointTool)
      {
        var t = station.NPointTool.ToolBlock;
        t.Inputs["Image"].Value = cogImg;
        t.Run();
        cogImg = (ICogImage)t.Outputs["Image"].Value; // 获取转换后的图像
      }

      // 6. 执行检测工具 ? 核心耗时操作
      tool.Inputs["Image"].Value = cogImg;
      if (frame.enumStation == EnumStation.左上相机工位 || frame.enumStation == EnumStation.右上相机工位)
      {
        if (_stationByName.TryGetValue(strStationName, out var st))
        {
        }

        CogToolBlock tb;
        if (frame.enumStation == EnumStation.左上相机工位)
        {
          tool.Inputs["stdDownX"].Value = SolutionManager.GetGlobal("左下相机基准X");
          tool.Inputs["stdDownY"].Value = SolutionManager.GetGlobal("左下相机基准Y");
          tool.Inputs["stdDownA"].Value = SolutionManager.GetGlobal("左下相机基准角度");
          tool.Inputs["RotationX"].Value = SolutionManager.GetGlobal("左下相机旋转中心X");
          tool.Inputs["RotationY"].Value = SolutionManager.GetGlobal("左下相机旋转中心Y");

          tb = _stationByName[EnumStation.左上相机工位.ToString()].DetectionTool.ToolBlock;
        }
        else
        {
          tool.Inputs["stdDownX"].Value = SolutionManager.GetGlobal("右下相机基准X");
          tool.Inputs["stdDownY"].Value = SolutionManager.GetGlobal("右下相机基准Y");
          tool.Inputs["stdDownA"].Value = SolutionManager.GetGlobal("右下相机基准角度");
          tool.Inputs["RotationX"].Value = SolutionManager.GetGlobal("右下相机旋转中心X");
          tool.Inputs["RotationY"].Value = SolutionManager.GetGlobal("右下相机旋转中心Y");

          tb = _stationByName[EnumStation.右上相机工位.ToString()].DetectionTool.ToolBlock;
        }

        tool.Inputs["curDownX"].Value = tb.Outputs["curX"].Value;
        tool.Inputs["curDownY"].Value = tb.Outputs["curY"].Value;
        tool.Inputs["curDownA"].Value = tb.Outputs["curA"].Value;
        tool.Run();
      }
      else
      {
        tool.Run();
      }



      // 7. 获取检测结果
      result = (bool)tool.Outputs["Result"].Value; // OK/NG

      var plc = ModbusManager.Instance.GetAccessor();
      var send = result ? 1 : 2;
      plc.WriteShort(reResultAddressArray[(int)frame.enumStation], (short)send);
      LogHelper.Info($"[{station.Name}]检测结束，检测结果[{(result ? "OK" : "NG")}]");

      #region Tcp数据发送

      // 检查是否启用TCP并且配置了TCP连接名称
      if (station.EnableTcp && !string.IsNullOrWhiteSpace(station.TcpConnectionName))
      {
        try
        {
          var sendData = (string)tool.Outputs["outData"].Value; // 结果数据字符串
          // 准备发送的消息
          string messageToSend = sendData ?? string.Empty;

          if (!string.IsNullOrWhiteSpace(messageToSend))
          {
            // 获取TCP配置，检查是否需要添加结束符
            var tcpInstance = TcpCommManager.Instance.GetInstance(station.TcpConnectionName);
            if (tcpInstance?.Config != null && tcpInstance.Config.UseTerminator)
            {
              // 解析结束符（将转义字符转换为实际字符）
              var terminator = ParseTerminator(tcpInstance.Config.Terminator);

              // 检查消息是否已经有结束符
              if (!string.IsNullOrEmpty(terminator) && !messageToSend.EndsWith(terminator))
              {
                messageToSend += terminator;
              }
            }

            // 发送消息
            // - 客户端模式：发送给服务器
            // - 服务器模式：发送给触发此次检测的客户端（通过ClientId）
            bool sendSuccess = TcpCommManager.Instance.Send(
              station.TcpConnectionName,
              messageToSend,
              frame.ClientId);

            if (sendSuccess)
            {
              LogHelper.Info($"[{station.Name}]检测完成，已发送结果到[{station.TcpConnectionName}]: {sendData}");
            }
            else
            {
              LogHelper.Warn($"[{station.Name}]发送结果失败，TCP连接[{station.TcpConnectionName}]未就绪");
            }
          }
          else
          {
            LogHelper.Warn($"[{station.Name}]检测完成，但结果数据为空，未发送");
          }
        }
        catch (Exception ex)
        {
          LogHelper.Error(ex, $"[{station.Name}]发送TCP结果异常");
        }
      }

      #endregion

      #region modbus数据发送

      if (station.EnableModbus)
      {
        try
        {
          var pos = (double[])tool.Outputs["PosArray"].Value;
          var floatArray = pos.Select(d => (float)d).ToArray();
          LogHelper.Info($"[{station.Name}]坐标计算完成,坐标[{string.Join(",", floatArray)}]");
          plc.WriteFloat(reResultAddressArray[(int)frame.enumStation + 1], floatArray);
        }
        catch (Exception e)
        {
          LogHelper.Error(e, $"[{station.Name}]modbus发送数据异常");
        }
      }

      #endregion

      // 打印工具运行异常信息
      if (tool.RunStatus.Result != CogToolResultConstants.Accept)
      {
        LogHelper.Warn($"[{station.Name}]工具运行错误，错误信息:{tool.RunStatus.Message}");
      }



      // 10. 提前创建显示记录（避免在显示部分重复创建）
      // 仅当工位启用显示且配置了显示界面时才创建
      if (station.bShow && !string.IsNullOrWhiteSpace(station.DisplayName))
      {
        var index = station.RecoredIndex; // 记录索引（选择显示哪个子记录）
        try
        {
          record = tool.CreateLastRunRecord()?.SubRecords?[index];
        }
        catch (Exception ex)
        {
          LogHelper.Error(ex, $"工位[{strStationName}]创建记录失败");
        }
      }
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"工位[{strStationName}]工具运行异常");
      return;
    }

    // 11. 异步处理显示和存图，不阻塞当前工位
    // 使用Task.Run启动独立任务，立即返回释放信号量
    _ = Task.Run(() => ProcessDisplayAndSave(frame, station, result, record));
  }

  /// <summary>
  /// 异步处理显示和存图（完全独立，不阻塞工具运行）
  /// 
  /// </summary>
  /// <param name="frame">图像帧</param>
  /// <param name="station">工位配置</param>
  /// <param name="result">检测结果（OK/NG）</param>
  /// <param name="record">显示记录</param>
  private async Task ProcessDisplayAndSave(ImageFrame frame, ProcessStation station, bool result, ICogRecord record)
  {
    var stationName = station.Name;
    CogRecordDisplay recordDisplay = null;

    #region 显示

    try
    {
      // 1. 检查工位是否启用显示
      if (!station.bShow)
      {
        // 显示未启用时不记录日志，避免日志过多
      }
      else if (string.IsNullOrWhiteSpace(station.DisplayName))
      {
        LogHelper.Warn($"工位[{stationName}]未配置显示界面");
      }
      else
      {
        var disp = SolutionManager.Instance.Current?.Display;
        string key = null;

        // 2. 优先匹配 DisplayName，如果匹配不到则尝试 Key
        // 支持两种匹配方式提高兼容性
        if (disp?.Items != null)
        {
          var item = disp.Items.FirstOrDefault(i =>
            string.Equals(i.DisplayName, station.DisplayName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(i.Key, station.DisplayName, StringComparison.OrdinalIgnoreCase));

          key = item?.Key;
        }

        // 3. 如果仍然没有找到，使用默认显示
        if (string.IsNullOrWhiteSpace(key))
        {
          LogHelper.Warn($"工位[{stationName}]配置的显示界面[{station.DisplayName}]未找到，使用默认显示");
          key = disp?.Items?.FirstOrDefault()?.Key ?? "显示1";
        }

        // 4. 查找并更新显示控件
        if (SolutionManager.Instance.Current.DisplayControls.TryGetValue(key, out var ctrl) && ctrl != null)
        {
          // 优先显示记录（包含图形标注），其次显示原图
          if (record != null)
            ctrl.Record = record;
          else
            ctrl.CogImage = frame.Image;

          recordDisplay = ctrl.RecordDisplay;
          // 显示成功不记录日志，避免日志过多
        }
        else
        {
          LogHelper.Warn($"工位[{stationName}]显示控件[{key}]未找到");
        }
      }
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"工位[{stationName}]显示更新失败");
    }

    #endregion

    #region 存图

    // 异步存图，完全独立，避免阻塞
    await Task.Run(() =>
    {
      try
      {
        // 1. 保存原图（如果全局和工位都启用）
        if (FileSettingsManager.Current.SaveRawImage && station.SaveRawImage)
        {
          var req = new SaveRequest
          {
            Station = stationName,
            Result = FileSettingsManager.Current.SeparateOkNg
              ? result ? "OK" : "NG" // OK/NG分文件夹
              : "OK", // 统一文件夹
            VisionProImage = frame.Image,
            Type = FileSettingsManager.Current.RawImageType,
            ScalePercent = 100, // 原图不缩放
            IsDealImage = false,
            ImageName = $"{stationName}_{DateTime.Now:yyyyMMdd_HHmmss}"
          };
          ImageSaver.Enqueue(req); // 入队异步保存
        }

        // 2. 保存处理后的图像（如果全局和工位都启用，且有显示记录）
        if (FileSettingsManager.Current.SaveDealImage &&
            station.SaveDealImage &&
            recordDisplay != null)
        {
          try
          {
            // CreateContentBitmap 可能耗时（绘制图形标注），在独立线程中执行
            var img = recordDisplay.CreateContentBitmap(CogDisplayContentBitmapConstants.Image);
            var cogImage = new CogImage24PlanarColor(new Bitmap(img));

            var req = new SaveRequest
            {
              Station = stationName,
              Result = FileSettingsManager.Current.SeparateOkNg
                ? result ? "OK" : "NG"
                : "OK",
              VisionProImage = cogImage,
              Type = FileSettingsManager.Current.DealImageType,
              ScalePercent = 100,
              IsDealImage = true,
              ImageName = $"{stationName}_{DateTime.Now:yyyyMMdd_HHmms}"
            };
            ImageSaver.Enqueue(req); // 入队异步保存
          }
          catch (Exception ex)
          {
            LogHelper.Error(ex, $"工位[{stationName}]创建处理图失败");
          }
        }
      }
      catch (Exception ex)
      {
        LogHelper.Error(ex, $"工位[{stationName}]存图异常");
        // 不抛出异常，避免线程崩溃
      }
    }).ConfigureAwait(false);

    #endregion
  }

  /// <summary>
  /// 解析结束符字符串
  /// 将转义符号转换为实际字符
  /// </summary>
  /// <param name="terminator">配置的结束符（如 \r\n ）</param>
  /// <returns>实际的结束符字符串</returns>
  private static string ParseTerminator(string terminator)
  {
    if (string.IsNullOrEmpty(terminator))
      return string.Empty;

    // 处理常见的转义序列
    return terminator
      .Replace("\\r", "\r")
      .Replace("\\n", "\n")
      .Replace("\\t", "\t")
      .Replace("\\0", "\0");
  }

  #endregion
  #endregion


  #region 图像队列处理

  /// <summary>
  /// 图像出队处理主循环（后台长任务）
  /// 
  /// </summary>
  /// <param name="token">取消令牌（用于优雅关闭）</param>
  private void StartDequeueLoop(CancellationToken token)
  {
    Task.Factory.StartNew(async () =>
    {
      while (!token.IsCancellationRequested)
      {
        try
        {
          // 尝试从队列中取出一帧图像
          if (_imageQueue.TryDequeue(out var frame))
          {
            // 使用信号量控制并发数量，避免资源耗尽
            _ = Task.Run(async () =>
            {
              // 等待可用的并发槽位（最多等待到token取消）
              await _stationSemaphore.WaitAsync(token).ConfigureAwait(false);

              try
              {
                // 执行工具运行（同步方法，但在独立线程中）
                RunTool(frame);
              }
              catch (Exception ex)
              {
                // 单个工位错误不影响其他工位
                LogHelper.Error(ex, $"工位[{frame.enumStation}]处理异常");
              }
              finally
              {
                // 始终释放信号量，避免死锁
                _stationSemaphore.Release();
              }
            }, token);

            // 继续尝试取下一帧，尽量减少等待
            continue;
          }
        }
        catch (Exception ex)
        {
          LogHelper.Error(ex, "图像队列出队异常");
        }

        // 队列为空时等待5ms，避免CPU空转
        await Task.Delay(5, token).ConfigureAwait(false);
      }
    }, TaskCreationOptions.LongRunning); // 标记为长时间运行任务
  }

  #endregion

  #region 相机参数管理

  /// <summary>
  /// 应用工位相机参数到对应的相机
  /// 
  /// </summary>
  /// <param name="stationName">工位名称，为空时应用所有工位</param>
  public void ApplyCameraParameters(string stationName = null)
  {
    try
    {
      var cameras = CameraManager.Instance.GetAllCameras();
      if (cameras.Count == 0)
      {
        LogHelper.Warn("未发现任何相机，无法应用参数");
        return;
      }

      // 确定要处理的工位列表
      var stationsToProcess = string.IsNullOrWhiteSpace(stationName)
        ? _stationByName.Values.ToList()
        : _stationByName.TryGetValue(stationName, out var station)
          ? new List<ProcessStation> { station }
          : new List<ProcessStation>();

      if (stationsToProcess.Count == 0)
      {
        LogHelper.Warn($"工位[{stationName}]未找到");
        return;
      }

      int successCount = 0;
      int failCount = 0;

      foreach (var st in stationsToProcess)
      {
        if (st == null || string.IsNullOrWhiteSpace(st.SN)) continue;

        // 查找对应的相机
        var camera = cameras.FirstOrDefault(c =>
          string.Equals(c.SN, st.SN, StringComparison.OrdinalIgnoreCase));

        if (camera == null)
        {
          LogHelper.Warn($"工位[{st.Name}]对应相机[{st.SN}]未找到");
          failCount++;
          continue;
        }

        try
        {
          // 应用相机参数
          if (st.CameraParams != null)
          {
            camera.Parameters.ExposureTime = st.CameraParams.Exposure;
            camera.Parameters.Gain = st.CameraParams.Gain;
            camera.Parameters.Width = st.CameraParams.Width;
            camera.Parameters.Height = st.CameraParams.Height;
            // 触发模式通过相机本身的接口设置，而不是Parameters
            // camera.Parameters.TriggerMode = st.CameraParams.TriggerMode;

            successCount++;
          }
        }
        catch (Exception ex)
        {
          LogHelper.Error(ex, $"工位[{st.Name}]应用相机参数失败");
          failCount++;
        }
      }

      if (successCount > 0 || failCount > 0)
      {
        if (string.IsNullOrWhiteSpace(stationName))
        {
          LogHelper.Info($"批量应用相机参数完成: 成功 {successCount} 个, 失败 {failCount} 个");
        }
        else if (successCount > 0)
        {
          LogHelper.Info($"工位[{stationName}]相机参数应用成功");
        }
      }
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, "应用相机参数异常");
    }
  }

  #endregion
}