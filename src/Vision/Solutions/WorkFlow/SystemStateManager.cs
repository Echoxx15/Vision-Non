using System;
using Logger;

namespace Vision.Solutions.WorkFlow;

/// <summary>
/// 系统状态管理器（单例）
/// 
/// 功能：
/// - 管理系统在线/离线状态
/// - 提供状态切换接口
/// - 发布状态变化事件
/// 
/// 设计理念：
/// - 在线状态：生产模式，允许接收外部通讯触发信号（TCP/Modbus）
/// - 离线状态：调试模式，拒绝外部触发，仅允许手动触发
/// 
/// 默认状态：离线（false）
/// 原因：系统启动时应处于安全的调试状态，避免意外触发生产流程
/// </summary>
public sealed class SystemStateManager
{
    #region 单例模式

    /// <summary>
    /// 延迟初始化的单例实例
    /// 线程安全且仅在首次访问时创建实例
    /// </summary>
    private static readonly Lazy<SystemStateManager> _instance = 
        new Lazy<SystemStateManager>(() => new SystemStateManager());

    /// <summary>
    /// 获取SystemStateManager单例实例
    /// </summary>
    public static SystemStateManager Instance => _instance.Value;

    #endregion

    #region 系统状态

    /// <summary>
    /// 系统在线状态标志
    /// true: 在线，接受通讯触发信号
    /// false: 离线，拒绝通讯触发（仅记录日志）
    /// 
    /// 注意：手动触发（ManualRun）不受此状态影响
    /// 
    /// 默认值：false（离线），系统启动时处于安全的调试状态
    /// </summary>
    public bool IsOnline { get; private set; } = false;

    /// <summary>
    /// 系统在线状态变化事件
    /// 订阅者：主界面（更新UI状态栏、工具栏启用状态）
    /// </summary>
    public event Action<bool> OnlineStateChanged;

    #endregion

    #region 构造函数

    /// <summary>
    /// 私有构造函数（单例模式）
    /// 初始化系统状态管理器，记录启动日志
    /// </summary>
    private SystemStateManager()
    {
        LogHelper.Info("[SystemStateManager] 已创建，初始状态: 离线");
    }

    #endregion

  #region 状态控制

    /// <summary>
    /// 设置系统在线/离线状态
    /// 
    /// 功能：
    /// 1. 检查状态是否发生变化
    /// 2. 更新内部状态标志
    /// 3. 记录状态变化日志
    /// 4. 触发状态变化事件通知订阅者
    /// 
    /// 使用场景：
    /// - 在线：生产模式，允许处理外部触发（TCP/Modbus）
    /// - 离线：调试模式，拒绝外部触发，启用配置工具栏
    /// </summary>
    /// <param name="online">true=在线，false=离线</param>
    public void SetOnlineState(bool online)
    {
   // 状态未变化，直接返回
        if (IsOnline == online)
        {
            LogHelper.Warn($"[SystemStateManager] 系统状态未变化，当前已为: {(online ? "在线" : "离线")}");
   return;
        }

  // 更新状态
        IsOnline = online;
  LogHelper.Info($"[SystemStateManager] ? 系统状态切换为: {(online ? "在线" : "离线")}");

        // 触发状态变化事件（通知UI、通讯模块等）
        try
     {
  OnlineStateChanged?.Invoke(IsOnline);
        }
        catch (Exception ex)
     {
            LogHelper.Error(ex, "[SystemStateManager] 触发状态变化事件异常");
   }
    }

    /// <summary>
    /// 切换系统在线/离线状态（取反）
    /// 
    /// 便捷方法：在线→离线，离线→在线
    /// </summary>
    public void ToggleOnlineState()
 {
        SetOnlineState(!IsOnline);
    }

    #endregion

    #region 状态查询

    /// <summary>
    /// 检查是否允许处理外部通讯触发
    /// 
    /// 返回值：
    /// - true: 系统在线，允许处理外部触发
    /// - false: 系统离线，拒绝外部触发
    /// 
    /// 用途：
  /// - 通讯模块在接收到触发信号时调用此方法判断是否执行
    /// - TaskFlowManager在处理设备消息时检查系统状态
    /// </summary>
    public bool CanProcessExternalTrigger()
    {
        if (!IsOnline)
        {
       LogHelper.Warn("[SystemStateManager] 系统离线，拒绝处理外部触发信号");
        }
      return IsOnline;
    }

    #endregion
}
