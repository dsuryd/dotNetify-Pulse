using Microsoft.Extensions.Logging;

namespace DotNetify.Pulse.Log
{
   public class PulseLoggerProvider : ILoggerProvider
   {
      private readonly ILogger _logger;

      public PulseLoggerProvider(IPulseLogger logger)
      {
         _logger = logger as ILogger;
      }

      public ILogger CreateLogger(string categoryName)
      {
         return _logger;
      }

      public void Dispose()
      {
      }
   }
}