using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
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
               // Simulate logging.
               if (random.Next(0, 10) < 5)
               {
                  var level = random.Next(0, 6);
                  var logLevel = (LogLevel) level;
                  _logger.Log(logLevel, $"Test log level {logLevel}");
               }
            });

         // Simulate CPU load.
         Task.Run(() =>
         {
            int percentage = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
               if (watch.ElapsedMilliseconds > percentage)
               {
                  Thread.Sleep(100 - percentage);
                  percentage = random.Next(30, 90);
                  watch.Reset();
                  watch.Start();
               }
            }
         });

         return Task.CompletedTask;
      }

      public Task StopAsync(CancellationToken cancellationToken)
      {
         _subs?.Dispose();
         return Task.CompletedTask;
      }
   }
}