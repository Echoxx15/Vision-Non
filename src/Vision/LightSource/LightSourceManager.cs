using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using Vision.Solutions.Models;

namespace Vision.LightSource;

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
        //旧的 LightConfigs 已移除，这里保留空实现以兼容调用
        DisposeAllControllers();
        LogHelper.Info("光源：未加载任何控制器（LightConfigs已移除）");
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
                    controller.TurnOn(lightControl.Channel1);

                    if (lightControl.IsMultiChannel)
                        controller.TurnOn(lightControl.Channel2);

                    if (lightControl.OpenDelayMs > 0)
                        Thread.Sleep(lightControl.OpenDelayMs);

                    LogHelper.Info($"工位光源已开: {lightControl}");
                }
                else
                {
                    controller.TurnOff(lightControl.Channel1);
                    if (lightControl.IsMultiChannel)
                        controller.TurnOff(lightControl.Channel2);

                    LogHelper.Info($"工位光源已关: {lightControl.LightConfigName}");
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
