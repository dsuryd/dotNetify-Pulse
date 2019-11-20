using Microsoft.Extensions.Logging;
using System;

namespace DotNetify.Pulse.Log
{
    public struct LogItem
    {
        public DateTimeOffset Time { get; }
        public string FormattedTime => Time.ToString("yyyy'-'MM'-'dd HH':'mm':'ss.fff");
        public string Level { get; }
        public string Message { get; }

        public LogItem(LogLevel level, string message)
        {
            Level = level.ToString();
            Message = message;
            Time = DateTimeOffset.Now;
        }
    }
}