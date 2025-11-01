using Logger;
using System;
using Vision.Solutions.Models;
using DataFormat = HslCommunication.Core.DataFormat;

namespace Vision.Comm.Modbus;

/// <summary>
/// Modbus通讯管理器（单例模式 - 单设备版本）
/// 针对实际应用场景优化：一个系统只连接一个 Modbus PLC
/// 
/// 设计理念：
/// - 单设备管理：使用单个字段而非字典
/// - 配置界面友好：支持断开/重连，无冲突
/// - 简洁高效：减少不必要的复杂度
/// </summary>
public sealed class ModbusManager
{
    private static readonly Lazy<ModbusManager> _instance = new(() => new ModbusManager());

    public static ModbusManager Instance => _instance.Value;

    #region 单设备字段

    /// <summary>
    /// 当前Modbus配置
    /// </summary>
    private ModbusConfig _config;

    /// <summary>
    /// 当前Modbus访问器
    /// </summary>
    private ModbusAccessor _accessor;

    #endregion

    private ModbusManager()
    {
        LogHelper.Info("Modbus管理器已初始化（单设备模式）");
    }

    #region 初始化和释放

    /// <summary>
    /// 从方案初始化Modbus连接（单设备模式）
    /// 只处理方案中的第一个Modbus配置
    /// </summary>
    public void InitializeFromSolution(Solution solution)
    {
        try
        {
            if (solution?.ModbusConfig == null)
            {
                LogHelper.Info("方案中没有Modbus配置");
                return;
            }

            // 单设备模式：只获取第一个配置
            _config = solution.ModbusConfig;

            if (!_config.Enabled)
            {
                LogHelper.Info($"Modbus配置 [{_config.Name}] 未启用");
                return;
            }

            // 解析数据格式
            DataFormat dataFormat = _config.DataFormat switch
            {
                "ABCD" => DataFormat.ABCD,
                "CDAB" => DataFormat.CDAB,
                "BADC" => DataFormat.BADC,
                "DCBA" => DataFormat.DCBA,
                _ => DataFormat.CDAB
            };

            // 创建访问器
            _accessor = ModbusTcp.Get(
                _config.IpAddress,
                _config.Port,
                dataFormat,
                _config.Station,
                _config.ConnectTimeout,
                _config.StringReverse);

            // 尝试连接
            var result = _accessor.Connect();
            if (result.IsSuccess)
            {
                LogHelper.Info($"Modbus [{_config.Name}] 连接成功: {_config.IpAddress}:{_config.Port}");
            }
            else
            {
                LogHelper.Warn($"Modbus [{_config.Name}] 连接失败: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "从方案初始化Modbus失败");
        }
    }

    /// <summary>
    /// 释放所有连接（单设备模式）
    /// </summary>
    public void DisposeAll()
    {
        if (_accessor != null)
        {
            try
            {
                _accessor.Close();
                LogHelper.Info($"Modbus [{_config?.Name ?? "Unknown"}] 已断开");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "断开Modbus连接失败");
            }
        }

        _accessor = null;
        _config = null;
    }

    #endregion

    #region 状态查询

    /// <summary>
    /// 获取当前Modbus配置
    /// </summary>
    public ModbusConfig GetConfig()
    {
        return _config;
    }

    /// <summary>
    /// 获取当前Modbus访问器
    /// </summary>
    public ModbusAccessor GetAccessor()
    {
        return _accessor;
    }

    /// <summary>
    /// 检查当前连接是否有效
    /// </summary>
    public bool IsConnected()
    {
        return _accessor?.IsConnected == true;
    }
    /// <summary>
    /// 更新客户端的IP地址、端口、站号等配置
    /// </summary>
    public void SetClientConfig(string ip,int port)
    {
        // 创建访问器
        _accessor = ModbusTcp.Get(
            ip,
            port);
    }

    #endregion

}
