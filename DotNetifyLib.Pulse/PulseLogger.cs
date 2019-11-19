using System;
using Microsoft.Extensions.Logging;

namespace DotNetify.Pulse
{
    public class PulseLogger : ILogger
    {
        private readonly IExternalScopeProvider _scopeProvider;

        public PulseLogger(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
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
            var message = formatter(state, exception);
        }
    }
}