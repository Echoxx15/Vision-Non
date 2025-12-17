using System;
using Logger;

namespace Vision.Solutions.WorkFlow;

/// <summary>
/// 系统状态管理器（单例）
/// 
/// 功能：
/// - 管理系统在线/离线状态
/// - 提供状态切换接口
/// - 广播状态变化事件
/// 
/// 约束：
/// - 在线状态下允许外部通讯触发（TCP/Modbus）
/// - 离线状态下拒绝外部触发，仅允许手动运行
/// 
/// 默认状态为离线（false），启动时先进入安全状态，避免误触发
/// </summary>
public sealed class SystemStateManager
{
    #region 单例模式

    /// <summary>
    /// 延迟初始化的单例实例，线程安全
    /// </summary>
    private static readonly Lazy<SystemStateManager> _instance = 
        new Lazy<SystemStateManager>(() => new SystemStateManager());

    /// <summary>
    /// 获取 SystemStateManager 单例
    /// </summary>
    public static SystemStateManager Instance => _instance.Value;

    #endregion

    #region 系统状态

    /// <summary>
    /// 系统在线状态标记
    /// true: 在线，允许外部通讯触发
    /// false: 离线，拒绝外部触发（但允许手动）
    /// 默认值为 false（离线）
    /// </summary>
    public bool IsOnline { get; private set; } = true;

    /// <summary>
    /// 系统在线状态变化事件（用于刷新UI与通讯模块）
    /// </summary>
    public event Action<bool> OnlineStateChanged;

    #endregion

    #region 构造函数

    /// <summary>
    /// 私有构造函数（单例）
    /// </summary>
    private SystemStateManager()
    {
        LogHelper.Info("[SystemStateManager] 已创建，初始状态: 离线");
    }

    #endregion

  #region 状态管理

    /// <summary>
    /// 设置系统在线/离线状态
    /// </summary>
    /// <param name="online">true=在线; false=离线</param>
    public void SetOnlineState(bool online)
    {
   // 状态未变化，直接返回
        if (IsOnline == online)
        {
            LogHelper.Warn($"[SystemStateManager] 状态未变化，当前为: {(online ? "在线" : "离线")}");
   return;
        }

  // 更新状态
        IsOnline = online;
  LogHelper.Info($"[SystemStateManager] 状态切换为: {(online ? "在线" : "离线")}");

        // 触发状态变化事件（通知UI和通讯模块）
        try
     {
  OnlineStateChanged?.Invoke(IsOnline);
        }
        catch (Exception ex)
     {
            LogHelper.Error(ex, "[SystemStateManager] 状态变化事件异常");
   }
    }

    /// <summary>
    /// 切换系统在线/离线状态的便捷方法
    /// </summary>
    public void ToggleOnlineState()
 {
        SetOnlineState(!IsOnline);
    }

    #endregion

    #region 状态查询

    /// <summary>
    /// 是否允许处理外部通讯触发
    /// </summary>
    public bool CanProcessExternalTrigger()
    {
        if (!IsOnline)
        {
       LogHelper.Warn("[SystemStateManager] 离线，拒绝外部触发信号");
        }
      return IsOnline;
    }

    #endregion
}
