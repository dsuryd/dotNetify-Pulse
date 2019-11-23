using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevApp
{
   public class TestHostedService : IHostedService
   {
      private readonly ILogger _logger;
      private IDisposable _subs;

      public TestHostedService(ILogger<TestHostedService> logger) => _logger = logger;

      public Task StartAsync(CancellationToken cancellationToken)
      {
         var random = new Random();
         _subs = Observable
            .Interval(TimeSpan.FromMilliseconds(100))
            .Subscribe(_ =>
            {
               var level = random.Next(0, 6);
               var logLevel = (LogLevel)level;
               _logger.Log(logLevel, $"Test log level {logLevel}");
            });

         return Task.CompletedTask;
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
         _subs.Dispose();
         return Task.CompletedTask;
      }
   }
}