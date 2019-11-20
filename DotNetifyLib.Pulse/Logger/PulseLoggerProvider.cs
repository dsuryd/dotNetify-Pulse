using Microsoft.Extensions.Logging;

namespace DotNetify.Pulse.Log
{
    public class PulseLoggerProvider : ILoggerProvider
    {
        private readonly IExternalScopeProvider _scopeProvider;
        private readonly ILogEmitter _logEmitter;

        public PulseLoggerProvider(ILogEmitter logEmitter)
        {
            _scopeProvider = new LoggerExternalScopeProvider();
            _logEmitter = logEmitter;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new PulseLogger(_scopeProvider, _logEmitter);
        }

        public void Dispose()
        {
        }
    }
}