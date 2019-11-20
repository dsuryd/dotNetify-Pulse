using Microsoft.Extensions.Logging;
using System;

namespace DotNetify.Pulse.Log
{
    public struct LogItem
    {
        public DateTimeOffset TimeStamp { get; }
        public string Level { get; }
        public string EventName { get; }
        public string Message { get; }

        public LogItem(LogLevel level, EventId eventId, string message)
        {
            Level = level.ToString();
            EventName = eventId.Name;
            Message = message;
            TimeStamp = DateTimeOffset.Now;
        }
    }
}