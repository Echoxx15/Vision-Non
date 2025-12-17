using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using HardwareCommNet;
using Logger;
using Vision.Solutions.Models;
using Vision.Solutions.WorkFlow;

namespace Vision.Solutions.TaskFlow;

public sealed class TaskFlowManager : IDisposable
{

    private static readonly Lazy<TaskFlowManager> _instance = new(() => new TaskFlowManager());
    public static TaskFlowManager Instance => _instance.Value;
    private readonly ConcurrentDictionary<string, TaskFlow> _taskFlows = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, (IComm device, EventHandler<object> handler)> _subscriptions = new(StringComparer.OrdinalIgnoreCase);
    private bool _initialized = false;

    private TaskFlowManager()
    {

    }

    public void InitializeFromSolution(Solution solution)
    {
        if (solution == null) { LogHelper.Warn("[TaskFlowManager]方案为空，无法初始化"); return; }

        try
        {
            DisposeAll();

            // ✅ 初始化光源管理器：读取所有工位光源配置并写入亮度值
            try
            {
                LightSource.LightSourceManager.Instance.InitializeFromSolution(solution);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "[TaskFlowManager] 初始化光源管理器失败");
            }

            foreach (var station in solution.Stations ?? new System.Collections.Generic.List<StationConfig>())
            {
                if (station == null || string.IsNullOrWhiteSpace(station.Name)) continue;
                try
                {
                    var tf = new TaskFlow(station);
                    _taskFlows[station.Name] = tf;
                    //订阅该工位的通讯触发
                    TrySubscribeStationDevice(station);
                }
                catch (Exception ex)
                { LogHelper.Error(ex, $"[TaskFlowManager] 创建工位 {station.Name}任务流失败"); }
            }
            _initialized = true;
        }
        catch (Exception ex)
        { LogHelper.Error(ex, "[TaskFlowManager] 初始化失败"); }
    }

    private void TrySubscribeStationDevice(StationConfig station)
    {
        // 清理旧订阅
        TryUnsubscribeStation(station.Name);

        if (string.IsNullOrWhiteSpace(station.CommDeviceName))
        {
            LogHelper.Info($"[TaskFlowManager] 工位[{station.Name}]未配置通讯设备，跳过触发绑定");
            return;
        }
        var dev = CommunicationFactory.Instance.GetDevice(station.CommDeviceName);
        if (dev == null)
        {
            LogHelper.Warn($"[TaskFlowManager] 工位[{station.Name}]未找到通讯设备: {station.CommDeviceName}");
            return;
        }

        EventHandler<object> handler = (_, msg) => OnDeviceMessage(station, msg);
        try
        {
            // 防重复
            dev.MessageReceived -= handler;
            dev.MessageReceived += handler;
            _subscriptions[station.Name] = (dev, handler);
            LogHelper.Info($"[TaskFlowManager] ✅ 工位[{station.Name}] 已绑定设备[{dev.Name}]消息事件，触发变量=[{station.TriggerVariableName}]，触发值=[{station.TriggerValue}]");
        }
        catch (Exception ex)
        { LogHelper.Error(ex, $"[TaskFlowManager]绑定设备消息失败: 工位={station.Name},设备={dev.Name}"); }
    }

    private void TryUnsubscribeStation(string stationName)
    {
        if (string.IsNullOrWhiteSpace(stationName)) return;
        if (!_subscriptions.TryRemove(stationName, out var sub)) return;
        try 
        { 
            sub.device.MessageReceived -= sub.handler;
        } 
        catch { }
    }

    private void OnDeviceMessage(StationConfig station, object message)
    {
        try
        {
            // ✅ 检查系统在线状态：系统离线时拒绝外部触发
            if (!SystemStateManager.Instance.CanProcessExternalTrigger())
            {
                return; // 静默忽略，不记录日志（避免日志刷屏）
            }

            //只在配置了"触发变量名"和"触发值"时判断
            var varName = (station?.TriggerVariableName ?? string.Empty).Trim();
            var targetVal = (station?.TriggerValue ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(station?.Name) || string.IsNullOrWhiteSpace(varName) || string.IsNullOrWhiteSpace(targetVal))
                return;

            //解析消息：期望匿名对象 { Name, Value, RawValue }
            string msgName = null;
            string msgValue = null;
            try
            {
                var t = message?.GetType();
                if (t != null)
                {
                    var pName = t.GetProperty("Name");
                    var pVal = t.GetProperty("Value");
                    msgName = pName?.GetValue(message)?.ToString();
                    msgValue = pVal?.GetValue(message)?.ToString();
                }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(msgName)) return;

            // 名称匹配（忽略大小写)
            if (!string.Equals(msgName.Trim(), varName, StringComparison.OrdinalIgnoreCase)) return;

            // 值匹配（字符串忽略大小写)
            var val = (msgValue ?? string.Empty).Trim();
            if (!string.Equals(val, targetVal, StringComparison.OrdinalIgnoreCase)) return;

            // 异步运行工位流程，避免阻塞轮询线程
            if (_taskFlows.TryGetValue(station.Name, out var tf))
            {
                LogHelper.Info($"触发命中: 工位=[{station.Name}], 变量=[{varName}], 值=[{val}]");
                Task.Run(() => tf.Start());
            }
            else
            {
                LogHelper.Warn($"[TaskFlowManager] 未找到工位 {station.Name} 的任务流");
            }
        }
        catch (Exception ex)
        { LogHelper.Error(ex, "[TaskFlowManager]处理设备消息异常"); }
    }

    public void ReloadStation(string stationName)
    {
        if (string.IsNullOrWhiteSpace(stationName)) return;
        try
        {
            TryUnsubscribeStation(stationName);
            if (_taskFlows.TryRemove(stationName, out var old)) { try { old.Dispose(); } catch { } }
            var solution = SolutionManager.Instance.Current;
            var station = solution?.Stations?.Find(s => string.Equals(s.Name, stationName, StringComparison.OrdinalIgnoreCase));
            if (station == null)
            { LogHelper.Warn($"[TaskFlowManager] 未找到工位 {stationName} 配置"); return; }
            var tf = new TaskFlow(station); _taskFlows[stationName] = tf; TrySubscribeStationDevice(station);
        }
        catch (Exception ex)
        { LogHelper.Error(ex, $"[TaskFlowManager] 重载工位 {stationName}失败"); }
    }

    /// <summary>
    /// 移除指定工位的TaskFlow和通讯绑定
    /// 用于删除工位或清理资源
    /// </summary>
    /// <param name="stationName">工位名称</param>
    public void RemoveStation(string stationName)
    {
        if (string.IsNullOrWhiteSpace(stationName)) return;
        try
        {
            LogHelper.Info($"[TaskFlowManager] 移除工位 {stationName}...");
            
            // 取消通讯订阅
            TryUnsubscribeStation(stationName);
            
            // 释放TaskFlow
            if (_taskFlows.TryRemove(stationName, out var old))
            {
                try { old.Dispose(); } catch { }
                LogHelper.Info($"[TaskFlowManager] 工位 {stationName} 已移除");
            }
            else
            {
                LogHelper.Warn($"[TaskFlowManager] 工位 {stationName} 不存在于TaskFlowManager中");
            }
        }
        catch (Exception ex)
        { LogHelper.Error(ex, $"[TaskFlowManager] 移除工位 {stationName}失败"); }
    }

    internal bool TryGetTaskFlow(string stationName, out TaskFlow flow) => _taskFlows.TryGetValue(stationName, out flow);

    /// <summary>
    /// 释放所有任务流和订阅（公开方法，用于切换方案时释放资源）
    /// </summary>
    public void DisposeAll()
    {
        //取消所有订阅
        foreach (var kv in _subscriptions)
        {
            try { kv.Value.device.MessageReceived -= kv.Value.handler; } catch { }
        }
        _subscriptions.Clear();
        //释放流程
        foreach (var tf in _taskFlows.Values)
        {
            try { tf.Dispose(); } catch { }
        }
        _taskFlows.Clear();
        _initialized = false;
        
        LogHelper.Info("[TaskFlowManager] DisposeAll 完成，所有任务流已释放");
    }

    public void Dispose()
    {
        try
        {
            LogHelper.Info("[TaskFlowManager]释放资源...");
            DisposeAll();
            _initialized = false;
            LogHelper.Info("[TaskFlowManager]资源已释放");
        }
        catch (Exception ex)
        { LogHelper.Error(ex, "[TaskFlowManager]释放资源异常"); }
    }
}
