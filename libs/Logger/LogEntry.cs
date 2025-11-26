using NLog;
using System;
using System.ComponentModel;

namespace Logger;

internal class LogEntry
{
    [DisplayName("时间")]
    public DateTime Time { get; set; }
    [DisplayName("级别")]
    public LogLevel Level { get; set; }
    [DisplayName("来源")]
    public string Source { get; set; }
    [DisplayName("消息")]
    public string Message { get; set; }

}
