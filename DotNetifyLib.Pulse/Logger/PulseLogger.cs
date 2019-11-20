using System;
using Microsoft.Extensions.Logging;

namespace DotNetify.Pulse.Log
{
    public class PulseLogger : ILogger
    {
        private readonly IExternalScopeProvider _scopeProvider;
        private readonly ILogEmitter _logEmitter;

        public PulseLogger(IExternalScopeProvider scopeProvider, ILogEmitter logEmitter)
        {
            _scopeProvider = scopeProvider;
            _logEmitter = logEmitter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _scopeProvider.Push(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.None)
                return false;

            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logEmitter.Log.OnNext(new LogItem(logLevel, eventId, formatter(state, exception)));
        }
    }
}