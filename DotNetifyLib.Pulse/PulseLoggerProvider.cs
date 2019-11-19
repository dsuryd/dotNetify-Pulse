using Microsoft.Extensions.Logging;

namespace DotNetify.Pulse
{
    public class PulseLoggerProvider : ILoggerProvider
    {
        private readonly IExternalScopeProvider _scopeProvider;

        public PulseLoggerProvider()
        {
            _scopeProvider = new LoggerExternalScopeProvider();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new PulseLogger(_scopeProvider);
        }

        public void Dispose()
        {
        }
    }
}