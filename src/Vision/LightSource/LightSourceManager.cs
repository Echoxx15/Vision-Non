using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using Vision.Solutions.Models;
using LightControlNet;

namespace Vision.LightSource;

public sealed class LightSourceManager : IDisposable
{
    private static readonly Lazy<LightSourceManager> _instance = new(() => new LightSourceManager());

    public static LightSourceManager Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, ILightController> _controllers = new();

    private LightSourceManager()
    {
        LogHelper.Info("[LightSourceManager] 初始化完成");
    }

    /// <summary>
    /// 从方案初始化光源控制：建立控制器引用并按工位配置写入亮度
    /// </summary>
    public void InitializeFromSolution(Solution solution)
    {
        try
        {
            DisposeAllControllers();
            if (solution?.Stations == null || solution.Stations.Count == 0)
            {
                LogHelper.Info("[LightSourceManager] 方案无工位，跳过光源初始化");
                return;
            }

            foreach (var st in solution.Stations)
            {
                var lc = st?.LightControl;
                if (lc == null || !lc.EnableLightControl || string.IsNullOrWhiteSpace(lc.LightConfigName)) continue;
                var ctrl = LightFactory.Instance.GetController(lc.LightConfigName);
                if (ctrl == null) { LogHelper.Warn($"[LightSourceManager] 未找到光源控制器: {lc.LightConfigName}"); continue; }
                _controllers[lc.LightConfigName] = ctrl;
                try
                {
                    ctrl.SetBrightness(lc.Channel1, lc.Brightness1);
                    if (lc.IsMultiChannel) ctrl.SetBrightness(lc.Channel2, lc.Brightness2);
                    LogHelper.Info($"[LightSourceManager] 已写入工位[{st.Name}]亮度配置: {lc.LightConfigName}");
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"[LightSourceManager] 写入亮度失败: 工位={st.Name}, 控制器={lc.LightConfigName}");
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "[LightSourceManager] 初始化失败");
        }
    }

    /// <summary>
    /// 获取光源控制器实例
    /// </summary>
    private ILightController GetController(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        if (_controllers.TryGetValue(name, out var controller) && controller != null) return controller;
        controller = LightFactory.Instance.GetController(name);
        if (controller != null) _controllers[name] = controller;
        return controller;
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
            LogHelper.Error(ex, $"[LightSourceManager] 设置亮度失败: {lightConfigName}, CH{channel}");
            return false;
        }
    }

    /// <summary>
    /// 按工位配置开/关光源：打开为同步，关闭可在外部异步调用
    /// </summary>
    public bool ControlStationLight(StationLightControl lightControl, bool turnOn)
    {
        if (lightControl == null || !lightControl.EnableLightControl) return true;
        if (string.IsNullOrWhiteSpace(lightControl.LightConfigName)) { LogHelper.Warn("[LightSourceManager] 工位光源配置为空"); return false; }

        try
        {
            var controller = GetController(lightControl.LightConfigName);
            if (controller == null) { LogHelper.Warn($"[LightSourceManager] 未找到工位光源控制器: {lightControl.LightConfigName}"); return false; }

            if (turnOn)
            {
                try { controller.TurnOn(lightControl.Channel1); } catch (Exception ex) { LogHelper.Error(ex, $"[LightSourceManager] 打开光源失败 CH{lightControl.Channel1}"); }
                if (lightControl.IsMultiChannel)
                {
                    try { controller.TurnOn(lightControl.Channel2); } catch (Exception ex) { LogHelper.Error(ex, $"[LightSourceManager] 打开光源失败 CH{lightControl.Channel2}"); }
                }
                if (lightControl.OpenDelayMs > 0) Thread.Sleep(lightControl.OpenDelayMs);
                return true;
            }
            else
            {
                try { controller.TurnOff(lightControl.Channel1); } catch (Exception ex) { LogHelper.Error(ex, $"[LightSourceManager] 关闭光源失败 CH{lightControl.Channel1}"); }
                if (lightControl.IsMultiChannel)
                {
                    try { controller.TurnOff(lightControl.Channel2); } catch (Exception ex) { LogHelper.Error(ex, $"[LightSourceManager] 关闭光源失败 CH{lightControl.Channel2}"); }
                }
                return true;
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"[LightSourceManager] 控制工位光源失败: {lightControl.LightConfigName}");
            return false;
        }
    }

    /// <summary>
    /// 异步关闭工位光源，异常独立捕获
    /// </summary>
    public void TurnOffStationLightAsync(StationLightControl lightControl)
    {
        if (lightControl == null || !lightControl.EnableLightControl) return;
        Task.Run(() =>
        {
            try
            {
                var controller = GetController(lightControl.LightConfigName);
                if (controller == null) return;
                try { controller.TurnOff(lightControl.Channel1); } catch (Exception ex) { LogHelper.Error(ex, $"[LightSourceManager] 关闭光源失败 CH{lightControl.Channel1}"); }
                if (lightControl.IsMultiChannel) { try { controller.TurnOff(lightControl.Channel2); } catch (Exception ex) { LogHelper.Error(ex, $"[LightSourceManager] 关闭光源失败 CH{lightControl.Channel2}"); } }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "[LightSourceManager] 异步关闭光源异常");
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
                LogHelper.Info($"[LightSourceManager] 光源控制器[{kvp.Key}]已释放");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"[LightSourceManager] 释放光源控制器[{kvp.Key}]失败");
            }
        }

        _controllers.Clear();
    }

    public void Dispose()
    {
        DisposeAllControllers();
        LogHelper.Info("[LightSourceManager] 已释放所有光源控制器");
    }
}
