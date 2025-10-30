using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using Vision.Solutions.Models;

namespace Vision.LightSource;

/// <summary>
/// 光源管理器（单例模式）
/// 统一管理所有光源控制器
/// </summary>
public sealed class LightSourceManager : IDisposable
{
    private static readonly Lazy<LightSourceManager> _instance = new(() => new LightSourceManager());

    public static LightSourceManager Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, ILightController> _controllers = new();

    private LightSourceManager()
    {
        LogHelper.Info("光源管理器已初始化");
    }

    /// <summary>
    /// 从方案初始化光源控制器
    /// </summary>
    public void InitializeFromSolution(Solution solution)
    {
        try
        {
            // 清理所有控制器
            DisposeAllControllers();

            if (solution?.LightConfigs?.Configs == null)
            {
                LogHelper.Warn("方案中没有光源配置");
                return;
            }

            // 初始化所有启用的数字控制器
            foreach (var config in solution.LightConfigs.Configs
                         .Where(c => c is { Enabled: true, Mode: LightControllerMode.Digital }))
            {
                try
                {
                    ILightController controller = config.Type switch
                    {
                        LightControllerType.Fgen => new FgenLightController(config),
                        _ => throw new NotSupportedException($"不支持的光源类型: {config.Type}")
                    };

                    if (controller.Open())
                    {
                        _controllers[config.Name] = controller;
                        LogHelper.Info($"光源控制器[{config.Name}]初始化成功");
                    }
                    else
                    {
                        LogHelper.Warn($"光源控制器[{config.Name}]打开失败");
                        controller.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"初始化光源控制器[{config.Name}]失败");
                }
            }

            LogHelper.Info($"光源管理器初始化完成，成功加载{_controllers.Count}个控制器");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "从方案初始化光源控制器失败");
        }
    }

    /// <summary>
    /// 获取光源控制器
    /// </summary>
    public ILightController GetController(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        _controllers.TryGetValue(name, out var controller);
        return controller;
    }

    /// <summary>
    /// 打开光源
    /// </summary>
    public bool TurnOn(string lightConfigName, int channel, int brightness = 255, int delayMs = 0)
    {
        var controller = GetController(lightConfigName);
        if (controller == null)
        {
            LogHelper.Warn($"未找到光源控制器: {lightConfigName}");
            return false;
        }

        try
        {
            if (!controller.SetBrightness(channel, brightness))
                return false;

            if (!controller.TurnOn(channel))
                return false;

            if (delayMs > 0)
                Thread.Sleep(delayMs);

            return true;
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"打开光源失败: {lightConfigName}, CH{channel}");
            return false;
        }
    }

    /// <summary>
    /// 关闭光源
    /// </summary>
    public bool TurnOff(string lightConfigName, int channel)
    {
        var controller = GetController(lightConfigName);
        if (controller == null)
            return false;

        try
        {
            return controller.TurnOff(channel);
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"关闭光源失败: {lightConfigName}, CH{channel}");
            return false;
        }
    }

    /// <summary>
    /// 设置亮度
    /// </summary>
    public bool SetBrightness(string lightConfigName, int channel, int brightness)
    {
        var controller = GetController(lightConfigName);
        if (controller == null)
            return false;

        try
        {
            return controller.SetBrightness(channel, brightness);
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"设置亮度失败: {lightConfigName}, CH{channel}");
            return false;
        }
    }

    /// <summary>
    /// 控制工位光源
    /// </summary>
    public async Task<bool> ControlStationLight(StationLightControl lightControl, bool turnOn)
    {
        if (lightControl == null || !lightControl.EnableLightControl)
            return true;

        if (string.IsNullOrWhiteSpace(lightControl.LightConfigName))
        {
            LogHelper.Warn("工位光源配置名为空");
            return false;
        }

        return await Task.Run(() =>
        {
            try
            {
                var controller = GetController(lightControl.LightConfigName);
                if (controller == null)
                {
                    LogHelper.Warn($"未找到工位光源控制器: {lightControl.LightConfigName}");
                    return false;
                }

                if (turnOn)
                {
                    // 主通道
                    controller.SetBrightness(lightControl.Channel1, lightControl.Brightness1);
                    controller.TurnOn(lightControl.Channel1);

                    // 如果是多通道模式，打开副通道
                    if (lightControl.IsMultiChannel)
                    {
                        controller.SetBrightness(lightControl.Channel2, lightControl.Brightness2);
                        controller.TurnOn(lightControl.Channel2);
                    }

                    // 延时
                    if (lightControl.OpenDelayMs > 0)
                        Thread.Sleep(lightControl.OpenDelayMs);

                    LogHelper.Info($"工位光源已打开: {lightControl}");
                }
                else
                {
                    // 关闭通道
                    controller.TurnOff(lightControl.Channel1);
                    if (lightControl.IsMultiChannel)
                        controller.TurnOff(lightControl.Channel2);

                    LogHelper.Info($"工位光源已关闭: {lightControl.LightConfigName}");
                }

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"控制工位光源失败: {lightControl.LightConfigName}");
                return false;
            }
        });
    }

    /// <summary>
    /// 释放所有控制器
    /// </summary>
    private void DisposeAllControllers()
    {
        foreach (var kvp in _controllers)
        {
            try
            {
                kvp.Value?.Dispose();
                LogHelper.Info($"光源控制器[{kvp.Key}]已释放");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"释放光源控制器[{kvp.Key}]失败");
            }
        }

        _controllers.Clear();
    }

    public void Dispose()
    {
        DisposeAllControllers();
        LogHelper.Info("光源管理器已释放");
    }
}
