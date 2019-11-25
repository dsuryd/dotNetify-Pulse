using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DotNetify.Elements;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.Pulse.SystemUsage
{
   public class MemoryUsageProvider : IPulseDataProvider
   {
      private readonly ReplaySubject<ProcessData> _dataStream;
      private readonly ProcessData _process = new ProcessData(Process.GetCurrentProcess());
      private readonly int _interval = 1000;

      public MemoryUsageProvider(PulseConfiguration pulseConfig)
      {
         _dataStream = new ReplaySubject<ProcessData>();
      }

      public IDisposable Configure(PulseVM pulseVM, out OnPushUpdate onPushUpdate)
      {
         // Property names.
         string GCTotalMemory;
         string GCTotalMemoryTrend;

         int tick = 0;

         pulseVM.AddProperty<double>(nameof(GCTotalMemory))
            .WithAttribute(new { Label = "GC Total Memory", Unit = "MB" })
            .SubscribeTo(_dataStream.Select(x => x.GCTotalMemory))
            .SubscribedBy(value => pulseVM.AddList(nameof(GCTotalMemoryTrend), ToChartData(++tick, value)), out IDisposable totalMemoryTrendSubs);

         pulseVM.AddProperty(nameof(GCTotalMemoryTrend), new string[][] { ToChartData(tick, _process.GCTotalMemory) })
            .WithAttribute(new ChartAttribute
            {
               Title = "GC Total Memory",
               XAxisLabel = "Time",
               YAxisLabel = "MBytes",
               MaxDataSize = 60,
               YAxisMin = 0,
               YAxisMax = 100
            });

         onPushUpdate = _ =>
         {
         };

         var intervalSubs = Observable
            .Interval(TimeSpan.FromMilliseconds(_interval))
            .Subscribe(_ => _dataStream.OnNext(_process));

         return new Disposable(totalMemoryTrendSubs, intervalSubs);
      }

      private string[] ToChartData(int tick, double value) => new string[] { $"{tick}", $"{value}" };
   }

   public static class PulseMemoryUsageExtensions
   {
      public static IServiceCollection AddPulseMemoryUsage(this IServiceCollection services)
      {
         return services.AddSingleton<IPulseDataProvider, MemoryUsageProvider>();
      }
   }
}