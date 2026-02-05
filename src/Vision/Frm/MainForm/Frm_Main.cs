using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Logger;
using Vision.Frm.FrmHardwareState;
using HardwareCameraNet;
using Vision.Localization;
using Vision.Auth;
using Vision.Frm.Link;
using Vision.Frm.Solution;
using Vision.Solutions.Models;
using Vision.Solutions.TaskFlow;
using Vision.Solutions.WorkFlow;
// 添加 ImageDisplay 的命名空间
using LightControlNet;
using Vision.LightSource;
using DnnInterfaceNet; // 深度学习插件接口
using Vision.Frm.DLModel;
using Vision.Frm.Station;
using HardwareCommNet;
using UserControls.Display;
using UserControls.LightTestForm; // 通讯配置接口

// 界面工厂

namespace Vision.Frm.MainForm;

/// <summary>
/// 主界面窗体
/// 
/// 核心功能：
/// 1. 用户交互界面（菜单、工具栏、状态栏）
/// 2. 显示布局管理（多窗口动态布局）
/// 3. 用户权限控制（登录、注销、权限分配）
/// 4. 系统状态监控（CPU、内存、磁盘、运行时间）
/// 5. 系统在线/离线控制（生产/调试模式切换）
/// 6. 自动注销机制（3分钟无操作自动退出登录）
/// </summary>
public partial class Frm_Main : Form, ILocalizable
{
    /// <summary>
    /// 启_animation screen form reference
    /// </summary>
    private Frm_Splash splash;

    #region 界面控件

    /// <summary>
    /// 日志窗体（显示系统运行日志）
    /// </summary>
    private Frm_Log frm_Log = new Frm_Log() { Dock = DockStyle.Fill };

    /// <summary>
    /// 硬件状态窗体（显示相机、PLC等硬件连接状态）
    /// </summary>
    private Frm_HardwareState frm_HardwareState;

    #endregion

    #region 运行状态监控

    /// <summary>
    /// 程序启动时间，用于计算运行时长
    /// </summary>
    private readonly DateTime _startTime = DateTime.Now;

    /// <summary>
    /// 状态栏更新定时器
    /// 每秒更新一次状态栏信息（CPU、内存、磁盘等）
    /// </summary>
    private Timer _statusTimer;

    /// <summary>
    /// CPU性能计数器
    /// 用于获取系统CPU占用率
    /// 注意：需要管理员权限，失败时使用进程CPU占用率代替
    /// </summary>
    private PerformanceCounter _cpuCounter;

    #endregion

    #region 全局空闲检测

    /// <summary>
    /// 空闲检测定时器
    /// 每秒检查一次用户是否有操作
    /// </summary>
    private Timer _idleTimer;

    /// <summary>
    /// 最后一次用户输入时间
    /// 包括鼠标移动、键盘按键等
    /// </summary>
    private DateTime _lastInputTime = DateTime.Now;

    /// <summary>
    /// 上次记录的鼠标位置
    /// 用于判断鼠标是否真正移动（过滤小幅度抖动）
    /// </summary>
    private Point _lastMouse;

    #endregion

    #region 构造函数和初始化

    /// <summary>
    /// 主界面构造函数
    /// 
    /// 初始化顺序：
    /// 1. InitializeComponent - Designer 生成的控件初始化
    /// 2. 启用自动本地化
    /// 3. 应用当前用户权限
    /// 4. 订阅事件：
    ///    - 方案变化事件
    ///    - 文件设置变化事件
    ///    - 用户登录状态变化事件
    ///    - 系统在线状态变化事件
    /// 5. 注册全局输入过滤器（用于空闲检测）
    /// </summary>
    public Frm_Main()
    {
        InitializeComponent();

        // 启用自动本地化（支持中英文切换）
        LocalizationManager.EnableAutoLocalization(this);

        // 应用当前用户的权限设置
        ApplyPermissionsForCurrentUser();

        // 订阅方案变化事件（切换方案时更新显示布局）
        SolutionManager.Instance.CurrentChanged += OnDisplayControlsChanged;

        // 订阅文件设置变化事件（存图路径等改变时更新状态栏）
        Settings.FileSettingsManager.SettingsChanged += UpdateStatusBar;

        // 监听用户登录状态变化（登录/注销时更新权限）
        UserManager.Instance.CurrentUserChanged += _ => ApplyPermissionsForCurrentUser();

        // ✅ 监听系统在线状态变化（在线/离线切换时更新UI）
        SystemStateManager.Instance.OnlineStateChanged += OnSystemOnlineStateChanged;

        // 注册全局输入钩子（用于3分钟无操作自动注销）
        // 监听整个应用程序的键盘鼠标消息
        Application.AddMessageFilter(new GlobalInputFilter(OnUserInput));
    }

    /// <summary>
    /// 窗体加载事件
    /// 
    /// 加载流程：
    /// 1. 显示启动画面
    /// 2. 加载相机插件（10%）
    /// 3. 加载解决方案列表（40%）
    /// 4. 初始化工作流（触发单例创建）
    /// 5. 初始化界面控件（70%）
    /// 6. 同步权限到数据库
    /// 7. 应用本地化语言
    /// 8. 应用用户权限
    /// 9. 加载显示布局
    /// 10. 初始化状态栏和空闲检测
    /// 11. 关闭启动画面，最大化主窗口（95%）
    /// 12. 触发初始在线状态显示
    /// </summary>
    private void Frm_Main_Load(object sender, EventArgs e)
    {
        // 1. 创建并显示启动画面
        splash = new Frm_Splash();
        splash.Show();
        splash.Refresh();

        // 2. 加载相机插件（支持多品牌相机）
        splash.SetProgress(10, "正在加载相机插件...");
        CameraHost.Initialize();

        // 2.5. 加载通讯插件（支持多种通讯类型）
        splash.SetProgress(20, "正在加载通讯插件...");
        CommPluginServer.Instance.LoadPlugins();
        CommunicationFactory.Instance.Initialize();

        // 2.8 加载光源插件（支持多品牌光源控制器）
        splash.SetProgress(25, "正在加载光源插件...");
        LightHost.Initialize();
        try
        {
            LightSourceManager.Instance.InitializeFromSolution(SolutionManager.Instance.Current);
        }
        catch
        {
            // ignored
        }

        // 2.9 加载深度学习模型插件
        splash.SetProgress(30, "正在加载深度学习插件...");
        try
        {
            DnnPluginServer.Instance.LoadPlugins();
            // 设置配置文件路径（使用全局配置目录）
            DnnModelFactory.Instance.SetConfigPath(null);
            DnnModelFactory.Instance.LoadConfigs();
            LogHelper.Info($"[主窗体] 深度学习插件加载完成，共 {DnnPluginServer.Instance.GetLoadedPluginTypes().Count()} 种模型类型");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "加载深度学习插件失败");
        }

        // 3. 加载所有解决方案（从配置文件读取）
        splash.SetProgress(40, "正在加载解决方案列表...");
        SolutionManager.Instance.EnsureLoaded();

        splash.SetProgress(50, "初始化界面...");
        // *** 重要：先初始化界面（包括 Frm_HardwareState），订阅事件 ***
        InitFormUI();

        // 5. 加载通讯设备配置（此时 Frm_HardwareState 已经订阅了事件）
        splash.SetProgress(55, "加载通讯设备配置...");
        try
        {
            CommunicationFactory.Instance.LoadConfigs();
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "加载通讯设备配置失败");
        }

        // 移除旧IOTable事件体系的初始化
        //仅初始化工位流程（内部会按新机制绑定设备消息）
        splash.SetProgress(65, "初始化工位流程...");
        try
        {
            var solution = SolutionManager.Instance.Current;
            if (solution != null)
            {
                TaskFlowManager.Instance.InitializeFromSolution(solution);
                LogHelper.Info("[主窗体] ✅ ProcessFlowManager 初始化完成");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "初始化ProcessFlowManager失败");
        }

        splash.SetProgress(70, "同步权限...");
        // 7. 同步主界面菜单/工具栏到权限数据库
        try
        {
            var allItems = CollectAllPermissionItems();
            PermissionManager.SyncItems(allItems, Role.Admin, "btn_User", "btn_Login");
        }
        catch
        {
        }

        // 8. 应用当前用户的权限（控制菜单/工具栏可见性）
        ApplyPermissionsForCurrentUser();

        // 9. 加载显示布局（根据方案配置）
        RefreshDisplayFromSolution();

        // 10. 更新用户信息显示（将在 ApplyLanguage 中统一处理）

        // 再次应用权限（确保正确）
        ApplyPermissionsForCurrentUser();

        // 11. 初始化状态栏定时器和空闲检测定时器
        InitStatusTimer();
        InitIdleTimer();

        splash.SetProgress(95, "完成设置...");

        // 12. 关闭启动画面，最大化显示主窗口
        ShowInTaskbar = true;
        splash.Close();
        splash = null;

        // ✅ 13. 触发初始在线状态显示（确保UI与SystemStateManager状态同步）
        // 注意：SystemStateManager默认离线状态（false），需要同步到界面
        OnSystemOnlineStateChanged(SystemStateManager.Instance.IsOnline);
        
        // ✅ 14. 应用当前语言到界面（确保所有控件文本正确显示）
        ApplyLanguage();

        LogHelper.Info($"程序启动完成，系统初始状态: {(SystemStateManager.Instance.IsOnline ? "在线" : "离线")}");

        WindowState = FormWindowState.Maximized;
    }

    /// <summary>
    /// 初始化空闲检测定时器
    /// 
    /// 功能：
    /// - 每秒检查一次用户是否有操作
    /// - 3分钟无操作自动注销（仅已登录用户）
    /// - 鼠标移动超过5像素才算有效移动
    /// </summary>
    private void InitIdleTimer()
    {
        _lastMouse = MousePosition;
        _lastInputTime = DateTime.Now;

        _idleTimer = new Timer { Interval = 1000, Enabled = true };
        _idleTimer.Tick += (_, _) => CheckIdleAndAutoLogout();
    }

    /// <summary>
    /// 用户输入回调（由全局消息过滤器调用）
    /// 记录最后一次输入时间，重置空闲计时器
    /// </summary>
    private void OnUserInput()
    {
        _lastInputTime = DateTime.Now;
    }

    /// <summary>
    /// 检查空闲并自动注销
    /// 
    /// 检测逻辑：
    /// 1. 鼠标移动超过5像素才算活动
    /// 2. 3分钟无活动且已登录 → 自动注销
    /// 
    /// 设计目的：
    /// - 防止长时间离开岗位导致权限泄露
    /// - 符合信息安全规范
    /// </summary>
    private void CheckIdleAndAutoLogout()
    {
        try
        {
            // 1. 检查鼠标移动（过滤小幅度抖动）
            var cur = MousePosition;
            var dx = Math.Abs(cur.X - _lastMouse.X);
            var dy = Math.Abs(cur.Y - _lastMouse.Y);

            if (dx > 5 || dy > 5) // 移动超过5像素才算有效
            {
                _lastMouse = cur;
                _lastInputTime = DateTime.Now;
            }

            // 2. 检查空闲时间（仅对已登录用户生效）
            if (UserManager.Instance.CurrentUser != null)
            {
                var idle = DateTime.Now - _lastInputTime;
                if (idle.TotalMinutes >= 3) // 3分钟无操作
                {
                    DoLogout(silent: true); // 静默注销（不弹确认框）
                }
            }
        }
        catch
        {
        }
    }

    #endregion

    #region 工具栏按钮事件

    /// <summary>
    /// 方案列表按钮点击事件
    /// 打开方案列表对话框，允许用户切换当前方案
    /// </summary>
    private void btn_SolutionList_ItemClick(object sender, EventArgs e)
    {
        using var frm = new Frm_SolutionList();

        // 订阅方案切换事件
        frm.OpenSolutionRequested += _ =>
        {
            // 切换后刷新显示布局和状态栏
            RefreshDisplayFromSolution();
            UpdateStatusBar();
        };

        frm.ShowDialog(this);
    }

    /// <summary>
    /// 保存方案按钮点击事件
    /// 将当前方案配置保存到文件
    /// </summary>
    private void btn_SaveSolution_ItemClick(object sender, EventArgs e)
    {
        try
        {
            // 确认保存
            if (MessageBox.Show("是否保存解决方案", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                btn_SaveSolution.Enabled = false;
                // 保存当前方案
                SolutionManager.Instance.SaveCurrent();
                btn_SaveSolution.Enabled = true;
                MessageBox.Show("保存方案成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                btn_SaveSolution.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "保存方案失败");
            MessageBox.Show("保存方案失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 全局变量按钮点击事件
    /// 打开全局变量配置对话框
    /// 
    /// 用途：
    /// - 定义跨工位共享的参数
    /// - 方便批量调整阈值等参数
    /// </summary>
    private void btn_CreateVar_ItemClick(object sender, EventArgs e)
    {
        using var frm = new Frm_GlobalVal();
        frm.ShowDialog();
    }

    /// <summary>
    /// 工位配置按钮点击事件
    /// 打开工位配置对话框
    /// 
    /// 功能：
    /// - 添加/删除工位
    /// - 配置工位参数（相机、工具、显示等）
    /// </summary>
    private void btn_Station_ItemClick(object sender, EventArgs e)
    {
        using var frm = new Frm_StationConfig();
        frm.ShowDialog();
    }

    /// <summary>
    /// 相机配置按钮点击事件
    /// 打开2D相机配置对话框
    /// 
    /// 功能：
    /// - 添加/删除相机
    /// - 配置相机参数（曝光、增益、触发模式等）
    /// - 测试相机连接
    /// </summary>
    private void btn_HardwareCamera_ItemClick(object sender, EventArgs e)
    {
        using var frm = CameraHost.ConfigForm;
        frm.ShowDialog();
    }

    /// <summary>
    /// 显示设置按钮点击事件
    /// 打开显示布局配置对话框
    /// 
    /// 功能：
    /// - 设置显示窗口数量（行列）
    /// - 配置每个窗口显示的内容
    /// </summary>
    private void btn_UI_ItemClick(object sender, EventArgs e)
    {
        using var f = new Frm_DisplaySet();

        // 保存时重建显示控件
        f.Saved += cfg => SolutionManager.Instance.RebuildDisplayControls(cfg);

        f.ShowDialog(this);
    }

    #endregion

    #region 菜单按钮事件

    /// <summary>
    /// 文件参数菜单点击事件
    /// 打开文件参数配置对话框
    /// 
    /// 功能：
    /// - 配置存图路径
    /// - 配置图像格式（BMP/JPG/PNG/TIF）
    /// - 配置存图策略（OK/NG分文件夹等）
    /// </summary>
    private void btn_File_Click(object sender, EventArgs e)
    {
        using var f = new Frm_File();
        f.ShowDialog(this);
    }

    /// <summary>
    /// 中文菜单点击事件
    /// 切换界面语言为中文
    /// </summary>
    private void btn_Chinese_Click(object sender, EventArgs e)
    {
        LanguageService.Instance.SetLanguage("zh-CN");
    }

    /// <summary>
    /// 英文菜单点击事件
    /// 切换界面语言为英文
    /// </summary>
    private void btn_English_Click(object sender, EventArgs e)
    {
        LanguageService.Instance.SetLanguage("en-US");
    }

    /// <summary>
    /// 注销按钮点击事件
    /// 退出当前用户登录
    /// </summary>
    private void btn_Logout_Click(object sender, EventArgs e)
    {
        DoLogout();
    }

    /// <summary>
    /// 执行注销操作
    /// </summary>
    /// <param name="silent">是否静默注销（不弹确认框）</param>
    private void DoLogout(bool silent = false)
    {
        // 非静默模式需要确认
        if (!silent)
        {
            if (MessageBox.Show("确认注销并退出登录?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) !=
                DialogResult.Yes)
                return;
        }

        // 执行注销
        UserManager.Instance.Logout();

        // 重新应用权限（切换为未登录权限）
        ApplyPermissionsForCurrentUser();

        LogHelper.Info($"用户[{UserManager.Instance.CurrentUser?.Username ?? "无"}]退出登录");
    }

    /// <summary>
    /// 系统在线/离线切换按钮点击事件
    /// 
    /// 功能：
    /// - 在线：允许处理外部触发（TCP/Modbus），禁用配置工具栏
    /// - 离线：拒绝外部触发，启用配置工具栏
    /// 
    /// 使用场景：
    /// - 在线：生产模式，接受外部触发信号
    /// - 离线：调试模式，手动触发测试
    /// </summary>
    private void btn_SystemOnline_Click(object sender, EventArgs e)
    {
        try
        {
            // 获取当前状态并切换
            var currentState = SystemStateManager.Instance.IsOnline;
            var newState = !currentState;

            // 执行状态切换
            SystemStateManager.Instance.SetOnlineState(newState);

            // 可选：显示状态切换提示
            // MessageBox.Show($"系统已切换为{stateText}状态", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "切换系统在线状态失败");
            MessageBox.Show($"切换系统状态失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion

    #region 状态栏更新

    /// <summary>
    /// 初始化状态栏更新定时器
    /// 
    /// 功能：
    /// - 创建CPU性能计数器（需要管理员权限）
    /// - 启动1秒间隔的定时器
    /// </summary>
    private void InitStatusTimer()
    {
        try
        {
            // 尝试创建CPU性能计数器（可能失败）
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
        }
        catch
        {
            // 失败时使用进程CPU占用率代替
            _cpuCounter = null;
        }

        _statusTimer = new Timer { Interval = 1000, Enabled = true };
        _statusTimer.Tick += (_, _) => UpdateStatusBar();
    }

    /// <summary>
    /// 更新状态栏显示内容
    /// 
    /// 显示信息：
    /// - 当前方案名称
    /// - 运行时间（天时分）
    /// - 内存占用（MB）
    /// - CPU占用率（%）
    /// - 磁盘剩余空间（%）
    /// - 系统状态（在线/离线）
    /// 
    /// 更新频率：1秒
    /// </summary>
    private void UpdateStatusBar()
    {
        try
        {
            var lang = LanguageService.Instance;
            
            // 1. 当前方案名称（从SolutionInfo列表获取，确保显示最新修改后的名称）
            var sol = SolutionManager.Instance.Current;
            var solutionName = GetCurrentSolutionDisplayName(sol);
            txt_JobName.Text = $"{lang.Get("txt_JobName")}{solutionName}";

            // 2. 运行时间（天时分格式）
            var span = DateTime.Now - _startTime;
            var dayText = lang.Get("txt_RunTime_Day");
            txt_RunTime.Text = $"{lang.Get("txt_RunTime")}{(int)span.TotalDays}{dayText}{span.Hours:D2}:{span.Minutes:D2}";

            // 3. 内存占用（进程工作集，单位MB）
            using (var p = Process.GetCurrentProcess())
            {
                var memMb = p.WorkingSet64 / (1024.0 * 1024.0);
                txt_Memory.Text = $"{lang.Get("txt_Memory")}{memMb:F1}MB";
            }

            // 4. CPU占用率
            // 优先使用系统CPU计数器，失败时使用进程CPU
            string cpuText = null;
            if (_cpuCounter != null)
            {
                try
                {
                    cpuText = $"{lang.Get("txt_CPU")}{_cpuCounter.NextValue():F1}%";
                }
                catch
                {
                }
            }

            if (string.IsNullOrEmpty(cpuText))
            {
                // 退化方案：基于进程TotalProcessorTime增量估算
                cpuText = GetProcessCpuUsageFallback();
            }

            txt_CPU.Text = cpuText;

            // 5. 磁盘剩余空间百分比
            // 仅检查存图路径所在磁盘
            try
            {
                var rootPath = Settings.FileSettingsManager.Current.SavePath;
                if (string.IsNullOrWhiteSpace(rootPath))
                    rootPath = AppDomain.CurrentDomain.BaseDirectory;

                var root = Path.GetPathRoot(rootPath);
                var drive = DriveInfo.GetDrives().FirstOrDefault(d =>
                    d.IsReady && string.Equals(d.Name, root, StringComparison.OrdinalIgnoreCase));

                if (drive != null)
                {
                    var percentFree = drive.TotalSize > 0
                        ? (drive.TotalFreeSpace * 100.0 / drive.TotalSize)
                        : 0;
                    txt_Disk.Text = $"{drive.Name}:{percentFree:F0}%";
                }
            }
            catch
            {
            }

            // 6. 更新系统在线状态显示
            UpdateSystemStateDisplay();
        }
        catch
        {
        }
    }

    /// <summary>
    /// 获取当前方案的显示名称
    /// 从 SolutionInfo 列表中匹配当前方案，返回列表中的名称（可能已被修改）
    /// 如果没有匹配到，则返回 Solution.Name，最后返回"无"
    /// </summary>
    private string GetCurrentSolutionDisplayName(Solutions.Models.Solution sol)
    {
        if (sol == null) return UITranslationExtensions.TC("NoSolution", "无");
        
        try
        {
            // 根据当前方案的 FilePath 从 SolutionInfo 列表中查找对应的记录
            var filePath = sol.FilePath;
            if (!string.IsNullOrEmpty(filePath))
            {
                var info = SolutionManager.Instance.Solutions?.FirstOrDefault(s =>
                {
                    var uvPath = SolutionManager.GetUvPath(s);
                    return !string.IsNullOrEmpty(uvPath) &&
                           string.Equals(Path.GetFullPath(uvPath), Path.GetFullPath(filePath),
                               StringComparison.OrdinalIgnoreCase);
                });
                if (info != null && !string.IsNullOrWhiteSpace(info.Name))
                    return info.Name;
            }
        }
        catch { }
        
        // 如果没有匹配到 SolutionInfo，返回 Solution.Name
        return sol.Name ?? "无";
    }

    /// <summary>
    /// 更新系统在线状态显示
    /// 
    /// 显示内容：
    /// - 在线：绿色 "系统:在线"
    /// - 离线：红色 "系统:离线"
    /// </summary>
    private void UpdateSystemStateDisplay()
    {
        try
        {
            var lang = LanguageService.Instance;
            // 获取系统在线状态
            var isOnline = SystemStateManager.Instance.IsOnline;
            if (tsl_SystemState != null)
            {
                tsl_SystemState.Text = isOnline ? lang.Get("tsl_SystemState_Online") : lang.Get("tsl_SystemState_Offline");
                tsl_SystemState.ForeColor = isOnline ? Color.Green : Color.Red;
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// 系统在线状态变化回调
    /// 
    /// 触发时机：
    /// SystemStateManager.SetOnlineState() 被调用
    /// 
    /// 响应动作：
    /// 1. 更新工具栏启用状态（在线时禁用）
    /// 2. 更新状态栏显示（颜色和文本）
    /// 3. 更新菜单项文本（显示相反状态）
    /// 
    /// 注意：
    /// - 需要线程安全调用（Invoke）
    /// - 系统在线时 toolMain.Enabled = false，防止误操作
    /// </summary>
    private void OnSystemOnlineStateChanged(bool isOnline)
    {
        try
        {
            // 确保在UI线程执行
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(OnSystemOnlineStateChanged), isOnline);
                return;
            }
            
            var lang = LanguageService.Instance;

            // 1. 更新工具栏启用状态
            // 在线时禁用工具栏，防止误操作修改配置
            toolMain.Enabled = !isOnline;

            // 2. 更新状态栏显示
            UpdateSystemStateDisplay();

            // 3. 更新菜单项文本（显示相反状态，点击后切换）
            if (tsm_SystemState != null)
            {
                tsm_SystemState.Text = isOnline ? lang.Get("tsm_SystemState_Offline") : lang.Get("tsm_SystemState_Online");
            }

            LogHelper.Info($"[主界面] 响应系统状态变化: {(isOnline ? "在线" : "离线")}");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "[主界面] 更新系统在线状态显示异常");
        }
    }

    /// <summary>
    /// CPU占用率计算（退化方案）
    /// 
    /// 计算原理：
    /// - 基于进程TotalProcessorTime的增量
    /// - delta处理器时间 / delta真实时间 = CPU占用率
    /// - 除以CPU核心数得到百分比
    /// 
    /// 注意：
    /// - 仅作为备用方案，精度不如PerformanceCounter
    /// - 第一次调用返回0%（需要两次采样）
    /// </summary>
    private double _lastCpuTotalMs;

    private DateTime _lastCpuSampleTime = DateTime.Now;

    private string GetProcessCpuUsageFallback()
    {
        try
        {
            var now = DateTime.Now;
            var proc = Process.GetCurrentProcess();

            // 进程总CPU时间（所有核心累加）
            double totalMs = proc.TotalProcessorTime.TotalMilliseconds;

            // 计算增量
            double deltaProc = totalMs - _lastCpuTotalMs;
            double deltaTime = (now - _lastCpuSampleTime).TotalMilliseconds;

            // 记录本次采样
            _lastCpuTotalMs = totalMs;
            _lastCpuSampleTime = now;

            if (deltaTime <= 0) return "CPU:0%";

            // CPU占用率 = delta处理器时间 / delta真实时间 / 核心数
            int cpuCount = Environment.ProcessorCount;
            double usage = Math.Max(0, Math.Min(100.0, (deltaProc / deltaTime) * 100.0 / cpuCount));

            return $"CPU:{usage:F1}%";
        }
        catch
        {
            return "CPU:-";
        }
    }

    #endregion

    #region 界面布局初始化

    /// <summary>
    /// 控件界面初始化
    /// 将子窗体嵌入到主界面的容器中
    /// 
    /// 功能：
    /// 1. 创建日志窗体并嵌入到日志容器
    /// 2. 创建硬件状态窗体并嵌入到状态容器
    /// 3. 启用自动本地化
    /// 4. 添加语言切换菜单
    /// </summary>
    private void InitFormUI()
    {
        try
        {
            // 1. 创建日志窗体
            //frm_Log = new Frm_Log { Dock = DockStyle.Fill };

            if (grb_Log != null)
            {
                grb_Log.Controls.Add(frm_Log);
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "初始化日志窗体失败");
        }

        try
        {
            // 2. 创建硬件状态窗体
            if (frm_HardwareState == null)
            {
                frm_HardwareState = new Frm_HardwareState { Dock = DockStyle.Fill };
            }

            if (grb_State != null)
            {
                grb_State.Controls.Add(frm_HardwareState);
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "初始化硬件状态窗体失败");
        }

        try
        {
            // 3. 添加语言切换菜单到主菜单
            var languageMenu = new LanguageMenuItem();
            menuMain.Items.Add(languageMenu);
            
            // 注册语言变更后刷新界面
            LanguageService.Instance.LanguageChanged += (_, _) =>
            {
                if (InvokeRequired)
                    BeginInvoke(new Action(RefreshUIOnLanguageChanged));
                else
                    RefreshUIOnLanguageChanged();
            };
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "初始化语言菜单失败");
        }
    }

    /// <summary>
    /// 语言切换后刷新界面
    /// </summary>
    private void RefreshUIOnLanguageChanged()
    {
        try
        {
            // 直接调用ApplyLanguage
            ApplyLanguage();
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"[Frm_Main] 刷新语言失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 更新状态栏标签文本
    /// </summary>
    private void UpdateStatusBarLabels()
    {
        var lang = LanguageService.Instance;
        
        // 用户状态
        var currentUser = UserManager.Instance.CurrentUser;
        barStaticItem1.Text = currentUser != null 
            ? $"{lang.Get("barStaticItem1_User")}{currentUser.Username}" 
            : lang.Get("barStaticItem1_NoUser");
    }

    #region 显示窗口布局

    /// <summary>
    /// 刷新显示区内容（当前方案切换后调用）
    /// 
    /// 触发时机：
    /// - 打开方案时
    /// - 切换方案时
    /// - 修改显示配置后
    /// </summary>
    private void RefreshDisplayFromSolution()
    {
        OnDisplayControlsChanged();
    }

    /// <summary>
    /// 重建显示控件布局
    /// 
    /// 功能：
    /// 1. 读取当前方案的显示配置（行列数）
    /// 2. 清空现有布局
    /// 3. 重建TableLayoutPanel（行列样式）
    /// 4. 创建显示控件并添加到对应位置
    /// 
    /// 布局算法：
    /// - 使用TableLayoutPanel动态布局
    /// - 行列数从方案配置读取
    /// - 每个单元格平均分配空间（百分比布局）
    /// - 显示控件不足时用空Panel占位
    /// 
    /// 示例：
    /// 2行2列配置 → 4个显示窗口
    /// ┌─────────┬─────────┐
    /// │ 显示1   │ 显示2   │
    /// ├─────────┼─────────┤
    /// │ 显示3   │ 显示4   │
    /// └─────────┴─────────┘
    /// </summary>
    private void OnDisplayControlsChanged()
    {
        var sol = SolutionManager.Instance.Current;
        if (sol == null || tlp_Display == null) return;

        // 1. 获取显示配置（默认2行2列）
        var cfg = sol.Display ?? new DisplayConfig { Rows = 2, Cols = 2 };

        // 2. 计算所需的显示项数量
        int total = Math.Max(1, cfg.Rows) * Math.Max(1, cfg.Cols);
        cfg.Items ??= new List<DisplayItem>();

        // 3. 确保Items数量与行列配置匹配，不足则补充默认项
        // 注意：保留已存在的DisplayName，只对新增项使用默认名
        while (cfg.Items.Count < total)
        {
            int index = cfg.Items.Count + 1;
            var key = $"显示{index}";
            cfg.Items.Add(new DisplayItem { Key = key, DisplayName = key });
        }

        // 4. 移除多余的项（如果行列数减少了）
        if (cfg.Items.Count > total)
        {
            cfg.Items = cfg.Items.Take(total).ToList();
        }

        // 5. 更新方案配置（确保修改被持久化）
        sol.Display = cfg;

        // 6. 暂停布局更新（提高性能）
        tlp_Display.SuspendLayout();

        // 7. 清空现有内容
        tlp_Display.Controls.Clear();
        tlp_Display.ColumnStyles.Clear();
        tlp_Display.RowStyles.Clear();

        // 8. 设置行列数
        tlp_Display.ColumnCount = Math.Max(1, cfg.Cols);
        tlp_Display.RowCount = Math.Max(1, cfg.Rows);

        // 9. 添加列样式（平均分配宽度）
        for (int c = 0; c < tlp_Display.ColumnCount; c++)
        {
            tlp_Display.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / tlp_Display.ColumnCount));
        }

        // 10. 添加行样式（平均分配高度）
        for (int r = 0; r < tlp_Display.RowCount; r++)
        {
            tlp_Display.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / tlp_Display.RowCount));
        }

        var items = cfg.Items;

        // 11. 填充显示控件
        for (int i = 0; i < total; i++)
        {
            // 获取显示项配置（现在Items数量已经足够，直接使用）
            var di = items[i];

            // 从方案中获取显示控件（可能不存在）
            sol.DisplayControls.TryGetValue(di.Key, out var ctrl);

            // 准备添加的控件（优先使用真实控件，否则用占位Panel）
            Control add = ctrl as Control ?? new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.FromArgb(45, 45, 45), // 深灰色背景
                BorderStyle = BorderStyle.FixedSingle
            };

            // 如果是占位Panel，添加标题标签（使用DisplayName）
            if (add is Panel p)
            {
                p.Controls.Clear();
                p.Controls.Add(new Label
                {
                    Dock = DockStyle.Top,
                    Text = di.DisplayName ?? di.Key,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(64, 64, 64),
                    Height = 22,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(4, 2, 0, 0)
                });
            }
            else if (add is ImageDisplay imgDisplay)
            {
                // 如果是ImageDisplay控件，更新其DisplayName
                imgDisplay.DisplayName = di.DisplayName ?? di.Key;
            }

            // 计算行列位置（从左到右、从上到下）
            int row = i / tlp_Display.ColumnCount;
            int col = i % tlp_Display.ColumnCount;

            // 添加到TableLayoutPanel
            tlp_Display.Controls.Add(add, col, row);
        }

        // 12. 恢复布局更新
        tlp_Display.ResumeLayout(true);
    }

    #endregion

    #endregion

    #region 窗体关闭

    /// <summary>
    /// 窗体关闭中事件
    /// 
    /// 功能：
    /// - 弹出确认对话框
    /// - 用户选择"否"时取消关闭
    /// </summary>
    private void Frm_Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (MessageBox.Show("是否确定退出软件？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
        {
            e.Cancel = true;
            return;
        }
    }

    /// <summary>
    /// 窗体已关闭事件
    /// 
    /// 功能：
    /// 1. 释放相机资源
    /// 2. 释放光源控制器资源
    /// 3. 释放定时器资源
    /// 4. 释放性能计数器资源
    /// </summary>
    private void Frm_Main_FormClosed(object sender, FormClosedEventArgs e)
    {
        // 1. 释放所有相机
        CameraHost.Dispose();

        // 2. 释放所有光源控制器（先关闭所有通道）
        try
        {
            LightFactory.Instance.Dispose();
            LogHelper.Info("光源控制器资源已释放");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "释放光源控制器失败");
        }

        // 3. 释放状态栏定时器
        try
        {
            _statusTimer?.Stop();
            _statusTimer?.Dispose();
        }
        catch
        {
        }

        // 4. 释放CPU计数器
        try
        {
            _cpuCounter?.Dispose();
        }
        catch
        {
        }

        // 5. 释放空闲检测定时器
        try
        {
            _idleTimer?.Stop();
            _idleTimer?.Dispose();
        }
        catch
        {
        }
    }

    #endregion

    #region 用户权限

    /// <summary>
    /// 登录按钮点击事件
    /// 
    /// 功能：
    /// - 打开登录对话框
    /// - 登录成功后更新用户显示和权限
    /// </summary>
    private void btn_Login_Click(object sender, EventArgs e)
    {
        using var f = new User.Frm_Login();

        if (f.ShowDialog(this) != DialogResult.OK)
            return;

        // 更新用户显示
        barStaticItem1.Text = $"用户:{UserManager.Instance.CurrentUser?.Username ?? "无"}";

        // 应用新用户的权限
        ApplyPermissionsForCurrentUser();
    }

    /// <summary>
    /// 注册按钮点击事件
    /// 
    /// 功能：
    /// - 打开用户注册对话框
    /// - 创建新用户账户
    /// </summary>
    private void btn_Register_Click(object sender, EventArgs e)
    {
        using var f = new User.Frm_Register();
        f.ShowDialog(this);
    }

    /// <summary>
    /// 权限管理按钮点击事件
    /// 
    /// 功能：
    /// - 打开权限管理对话框
    /// - 配置不同角色的功能权限
    /// - 保存后重新应用权限
    /// </summary>
    private void btn_Permission_Click(object sender, EventArgs e)
    {
        // 收集所有需要权限控制的菜单/工具栏项
        var items = CollectAllPermissionItems();

        using var f = new User.Frm_Permission(items);
        f.ShowDialog(this);

        // 权限修改后重新应用
        ApplyPermissionsForCurrentUser();
    }

    /// <summary>
    /// 收集所有需要权限控制的界面元素
    /// 
    /// 功能：
    /// 1. 遍历主菜单所有项（递归）
    /// 2. 遍历工具栏所有项
    /// 3. 跳过不受控项（用户菜单、登录按钮）
    /// 
    /// 返回值：
    /// 所有ToolStripItem的列表（菜单项+工具栏按钮）
    /// 
    /// 跳过项：
    /// - btn_User: 用户菜单（始终可见）
    /// - btn_Login: 登录按钮（始终可见）
    /// </summary>
    private IEnumerable<ToolStripItem> CollectAllPermissionItems()
    {
        var list = new List<ToolStripItem>();

        // 定义跳过规则
        bool Skip(string n) => string.Equals(n, "btn_User", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(n, "btn_Login", StringComparison.OrdinalIgnoreCase);

        // 1. 遍历主菜单
        foreach (ToolStripMenuItem top in menuMain.Items)
        {
            // 顶级菜单本身是否受控（如果是跳过项则不加入）
            if (!Skip(top.Name))
                list.Add(top);

            // 递归收集子菜单项
            CollectItemsRecursive(top, list, Skip);
        }

        // 2. 遍历工具栏（过滤跳过项）
        list.AddRange(toolMain.Items.Cast<ToolStripItem>().Where(it => !Skip(it.Name)));

        // 去重并返回
        return list.Distinct().ToList();
    }

    /// <summary>
    /// 递归收集菜单项（包含子菜单）
    /// </summary>
    /// <param name="parent">父菜单项</param>
    /// <param name="list">收集列表</param>
    /// <param name="skip">跳过判断函数</param>
    private void CollectItemsRecursive(ToolStripMenuItem parent, List<ToolStripItem> list, Func<string, bool> skip)
    {
        foreach (ToolStripItem it in parent.DropDownItems)
        {
            // 非跳过项才加入列表
            if (!skip(it.Name))
                list.Add(it);

            // 如果有子菜单，递归处理
            if (it is ToolStripMenuItem { HasDropDownItems: true } mi)
            {
                CollectItemsRecursive(mi, list, skip);
            }
        }
    }

    /// <summary>
    /// 应用当前用户的权限
    /// 
    /// 功能：
    /// 1. 收集所有需要权限控制的界面元素
    /// 2. 根据当前用户角色设置可见性和启用状态
    /// 3. 更新状态栏用户显示
    /// 
    /// 调用时机：
    /// - 用户登录后
    /// - 用户注销后
    /// - 权限配置修改后
    /// - 窗体加载时
    /// </summary>
    private void ApplyPermissionsForCurrentUser()
    {
        // 1. 收集所有权限控制项
        var items = CollectAllPermissionItems();

        // 2. 应用权限（根据用户角色设置Visible/Enabled）
        PermissionManager.ApplyPermissions(UserManager.Instance.CurrentUser, items);

        // 3. 更新状态栏用户显示
        barStaticItem1.Text = UserManager.Instance.CurrentUser != null
            ? $"用户:{UserManager.Instance.CurrentUser.Username}"
            : "用户:无";
    }

    #endregion

    private void tsm_LightControl_Click(object sender, EventArgs e)
    {
        using var frm = LightHost.ConfigForm;
        frm.ShowDialog(this);
    }

    /// <summary>
    /// 深度学习配置按钮点击事件
    /// 打开深度学习模型配置界面
    /// </summary>
    private void tsm_DLModel_Click(object sender, EventArgs e)
    {
        using var frm = new Frm_DLModelConfig();
        frm.ShowDialog(this);
    }

    private void tsm_Comm_Click(object sender, EventArgs e)
    {
        // 使用新的通讯配置界面（在独立窗口中显示）
        using var frm = new Form();
        frm.Text = "通讯配置";
        frm.Size = new Size(1200, 900);
        frm.StartPosition = FormStartPosition.CenterParent;

        // 获取通讯配置控件
        var configControl = CommConfigHost.Instance.ConfigControl;
        configControl.Dock = DockStyle.Fill;
        frm.Controls.Add(configControl);
        
        // 使用局部事件处理器，避免重复订阅
        EventHandler<IComm> openCommTableHandler = (_, device) =>
        {
            CommConfigHost.Instance.OpenCommTable(device);
        };
        
        CommConfigHost.Instance.OpenCommTableRequested += openCommTableHandler;
        
        try
        {
            frm.ShowDialog(this);
        }
        finally
        {
            // 窗口关闭后取消订阅，防止重复触发
            CommConfigHost.Instance.OpenCommTableRequested -= openCommTableHandler;
        }
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
        using var frm = new FugenPinshanLight();
        frm.ShowDialog();
    }

    public void ApplyLanguage()
    {
        try
        {
            var lang = LanguageService.Instance;
            
            // 菜单项
            btn_User.Text = lang.Get("btn_User");
            btn_Login.Text = lang.Get("btn_Login");
            btn_Register.Text = lang.Get("btn_Register");
            btn_Permission.Text = lang.Get("btn_Permission");
            btn_Logout.Text = lang.Get("btn_Logout");
            btn_System.Text = lang.Get("btn_System");
            btn_SystemParam.Text = lang.Get("btn_SystemParam");
            btn_File.Text = lang.Get("btn_File");
            
            // 工具栏按钮
            btn_SolutionList.Text = lang.Get("btn_SolutionList");
            btn_SaveSolution.Text = lang.Get("btn_SaveSolution");
            btn_CreateVar.Text = lang.Get("btn_CreateVar");
            btn_Station.Text = lang.Get("btn_Station");
            btn_HardwareCamera.Text = lang.Get("btn_HardwareCamera");
            btn_HardwareComm.Text = lang.Get("btn_HardwareComm");
            btn_LightConfig.Text = lang.Get("btn_LightConfig");
            btn_DnnModel.Text = lang.Get("btn_DnnModel");
            btn_UI.Text = lang.Get("btn_UI");
            btn_TestStrobe.Text = lang.Get("btn_TestStrobe");
            
            // 面板标题
            grb_State.Text = lang.Get("grb_State");
            grb_Log.Text = lang.Get("grb_Log");
            
            // 更新状态栏
            UpdateSystemStateDisplay();
            UpdateStatusBarLabels();
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"[Frm_Main] ApplyLanguage失败: {ex.Message}");
        }
    }
}

/// <summary>
/// 全局输入过滤器
/// 
/// 功能：
/// - 监听整个应用程序的键盘鼠标消息
/// - 检测用户活动（按键、点击、鼠标移动）
/// - 用于实现自动注销功能
/// 
/// 实现原理：
/// - 实现IMessageFilter接口
/// - 通过Application.AddMessageFilter注册
/// - PreFilterMessage方法拦截所有窗口消息
/// 
/// 监听消息：
/// - WM_KEYDOWN: 锁定按键
/// - WM_SYSKEYDOWN: 系统键按下（Alt+组合键）
/// - WM_LBUTTONDOWN: 左键按下
/// - WM_RBUTTONDOWN: 右键按下
/// - WM_MBUTTONDOWN: 中键按下
/// - WM_MOUSEMOVE: 鼠标移动（过滤小幅度抖动）
/// </summary>
internal sealed class GlobalInputFilter : IMessageFilter
{
  /// <summary>
  /// 用户输入回调
  /// </summary>
  private readonly Action _onInput;

  /// <summary>
  /// 上次记录的鼠标位置（用于过滤抖动）
  /// </summary>
  private Point _lastMouse;

  /// <summary>
  /// 构造函数
  /// </summary>
  /// <param name="onInput">用户输入时的回调</param>
  public GlobalInputFilter(Action onInput)
  {
    _onInput = onInput ?? (() => { });
    _lastMouse = Control.MousePosition;
  }

  /// <summary>
  /// 消息预过滤（拦截所有窗口消息）
  /// 
  /// 返回值：
  /// - false: 不拦截消息，继续传递
  /// - true: 拦截消息，停止传递（本实现中始终不拦截）
  /// </summary>
  public bool PreFilterMessage(ref Message m)
  {
    // Windows消息常量
    const int WM_KEYDOWN = 0x0100; // 锁定按键
    const int WM_SYSKEYDOWN = 0x0104; // 系统键按下
    const int WM_LBUTTONDOWN = 0x0201; // 左键按下
    const int WM_RBUTTONDOWN = 0x0204; // 右键按下
    const int WM_MBUTTONDOWN = 0x0207; // 中键按下
    const int WM_MOUSEMOVE = 0x0200; // 鼠标移动

    switch (m.Msg)
    {
      // 键盘和鼠标点击消息 → 直接触发回调
      case WM_KEYDOWN:
      case WM_SYSKEYDOWN:
      case WM_LBUTTONDOWN:
      case WM_RBUTTONDOWN:
      case WM_MBUTTONDOWN:
        _onInput();
        break;

      // 鼠标移动消息 → 过滤小幅度抖动（5像素阈值）
      case WM_MOUSEMOVE:
      {
        var cur = Control.MousePosition;
        if (Math.Abs(cur.X - _lastMouse.X) > 5 || Math.Abs(cur.Y - _lastMouse.Y) > 5)
        {
          _lastMouse = cur;
          _onInput();
        }

        break;
      }
    }

    return false; // 不拦截消息
  }
}
