using System;
using System.Collections.Generic;
using System.Linq;
using HardwareCommNet.CommTable;
using Logger;

namespace HardwareCommNet;

/// <summary>
/// TCP消息处理辅助类
/// 负责解析TCP消息、更新输入变量值、检测触发条件
/// </summary>
public static class TcpMessageHelper
{
    /// <summary>
    /// 触发模式枚举
    /// </summary>
    public enum TriggerModeEnum
    {
        /// <summary>值匹配触发（默认）：当值等于TriggerValues中的任意值时触发</summary>
        ValueMatch = 0,
        /// <summary>值变化触发：当值发生变化时触发</summary>
        ValueChanged = 1,
        /// <summary>任意非空触发：当收到任意非空消息时触发</summary>
        AnyNonEmpty = 2
    }

    /// <summary>
    /// 处理TCP消息结果
    /// </summary>
    public class TcpMessageResult
    {
        /// <summary>是否有触发</summary>
        public bool HasTrigger { get; set; }
        
        /// <summary>触发的变量名称列表</summary>
        public List<string> TriggeredInputs { get; set; } = new List<string>();
        
        /// <summary>已更新的变量及其值</summary>
        public Dictionary<string, object> UpdatedValues { get; set; } = new Dictionary<string, object>();
        
        /// <summary>触发消息的原始文本</summary>
        public string RawMessage { get; set; }
    }

    /// <summary>
    /// 处理收到的TCP消息
    /// </summary>
    /// <param name="rawMessage">原始消息文本</param>
    /// <param name="table">通讯表</param>
    /// <param name="adapterName">适配器名称（用于日志）</param>
    /// <returns>处理结果</returns>
    public static TcpMessageResult ProcessMessage(string rawMessage, CommTable.CommTable table, string adapterName)
    {
        var result = new TcpMessageResult { RawMessage = rawMessage };
        
        if (string.IsNullOrEmpty(rawMessage) || table == null)
        {
            return result;
        }

        // 精简日志：仅显示收到的消息
        LogHelper.Info($"[{adapterName}] 收到消息: \"{rawMessage.Trim()}\"");

        var inputs = table.Inputs;
        if (inputs == null || inputs.Count == 0)
        {
            return result;
        }

        // 遍历所有输入变量
        foreach (var input in inputs)
        {
            try
            {
                // 获取消息中的字段值
                string fieldValue = ExtractFieldValue(rawMessage, input);
                
                if (fieldValue == null)
                {
                    continue;
                }

                // 转换值类型
                object typedValue = ConvertToType(fieldValue, input.ValueType);
                
                // 获取旧值（用于变化检测）
                var oldValue = table.GetInputCachedValue(input.Name);
                
                // 更新缓存值
                table.UpdateInputCachedValue(input.Name, typedValue);
                result.UpdatedValues[input.Name] = typedValue;

                // 检查是否触发
                if (input.IsTrigger)
                {
                    bool triggered = CheckTrigger(input, typedValue, oldValue, fieldValue);
                    if (triggered)
                    {
                        result.HasTrigger = true;
                        result.TriggeredInputs.Add(input.Name);
                        // 精简日志：仅在触发时显示变量值和触发信息
                        LogHelper.Info($"[{adapterName}] ★ 触发: {input.Name}={typedValue}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"[{adapterName}] 处理变量 [{input.Name}] 时出错");
            }
        }

        return result;
    }

    /// <summary>
    /// 从消息中提取字段值
    /// </summary>
    private static string ExtractFieldValue(string rawMessage, CommCell input)
    {
        // 如果FieldIndex为-1，表示使用整行
        if (input.FieldIndex < 0)
        {
            return rawMessage.Trim();
        }

        // 使用分隔符拆分
        string delimiter = input.Delimiter ?? ",";
        
        // 处理特殊分隔符
        if (delimiter == "\\t") delimiter = "\t";
        if (delimiter == "\\s") delimiter = " ";
        
        var fields = rawMessage.Split(new[] { delimiter }, StringSplitOptions.None);
        
        // 检查字段索引是否有效
        if (input.FieldIndex >= fields.Length)
        {
            return null;
        }

        return fields[input.FieldIndex].Trim();
    }

    /// <summary>
    /// 将字符串值转换为指定类型
    /// </summary>
    private static object ConvertToType(string value, CommValueType valueType)
    {
        try
        {
            switch (valueType)
            {
                case CommValueType.Bool:
                    // 支持 "1", "true", "True", "TRUE", "yes", "on"
                    return value == "1" || 
                           string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(value, "on", StringComparison.OrdinalIgnoreCase);
                    
                case CommValueType.Short:
                    return short.TryParse(value, out var s) ? s : (short)0;
                    
                case CommValueType.Int:
                    return int.TryParse(value, out var i) ? i : 0;
                    
                case CommValueType.Float:
                    return float.TryParse(value, out var f) ? f : 0f;
                    
                case CommValueType.Double:
                    return double.TryParse(value, out var d) ? d : 0.0;
                    
                case CommValueType.String:
                default:
                    return value;
            }
        }
        catch
        {
            return value;
        }
    }

    /// <summary>
    /// 检查是否满足触发条件
    /// </summary>
    private static bool CheckTrigger(CommCell input, object newValue, object oldValue, string rawFieldValue)
    {
        var mode = (TriggerModeEnum)input.TriggerMode;

        switch (mode)
        {
            case TriggerModeEnum.ValueMatch:
                // 值匹配触发：检查是否等于触发值列表中的任意一个
                if (input.TriggerValues == null || input.TriggerValues.Count == 0)
                {
                    // 如果没有配置触发值，默认为任意非空触发
                    return !string.IsNullOrEmpty(rawFieldValue);
                }
                return input.TriggerValues.Any(tv => 
                    string.Equals(tv, rawFieldValue, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(tv, newValue?.ToString(), StringComparison.OrdinalIgnoreCase));

            case TriggerModeEnum.ValueChanged:
                // 值变化触发：检查新值与旧值是否不同
                if (oldValue == null) return true;
                return !Equals(newValue, oldValue) && 
                       !string.Equals(newValue?.ToString(), oldValue?.ToString(), StringComparison.OrdinalIgnoreCase);

            case TriggerModeEnum.AnyNonEmpty:
                // 任意非空触发：只要收到非空消息就触发
                return !string.IsNullOrEmpty(rawFieldValue);

            default:
                return false;
        }
    }

    /// <summary>
    /// 创建触发事件的消息负载
    /// </summary>
    /// <param name="result">TCP消息处理结果</param>
    /// <returns>可用于MessageReceived事件的负载对象</returns>
    public static object CreateTriggerPayload(TcpMessageResult result)
    {
        if (!result.HasTrigger || result.TriggeredInputs.Count == 0)
        {
            return null;
        }

        // 使用第一个触发的输入变量
        var triggerName = result.TriggeredInputs.First();
        var triggerValue = result.UpdatedValues.TryGetValue(triggerName, out var val) ? val : result.RawMessage;

        return new
        {
            Name = triggerName,
            Value = triggerValue?.ToString(),
            AllUpdates = result.UpdatedValues,
            AllTriggers = result.TriggeredInputs,
            RawMessage = result.RawMessage
        };
    }
}
