using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DotNetify.Elements;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetify.Pulse.SystemUsage
{
   public class CpuUsageProvider : IPulseDataProvider
   {
      private readonly ReplaySubject<ProcessData> _dataStream;
      private readonly ProcessData _process = new ProcessData(Process.GetCurrentProcess());
      private readonly int _interval = 1000;

      public CpuUsageProvider(PulseConfiguration pulseConfig)
      {
         _dataStream = new ReplaySubject<ProcessData>();
      }

      public IDisposable Configure(PulseVM pulseVM, out OnPushUpdate onPushUpdate)
      {
         // Property names.
         string TotalCpu;
         string TotalCpuTrend;

         int tick = 0;

         pulseVM.AddProperty<double>(nameof(TotalCpu))
            .WithAttribute(new { Label = "Total CPU", Unit = "%" })
            .SubscribeTo(_dataStream.Select(x => x.TotalCpu))
            .SubscribedBy(value => pulseVM.AddList(nameof(TotalCpuTrend), ToChartData(++tick, value)), out IDisposable totalCpuTrendSubs);

         pulseVM.AddProperty(nameof(TotalCpuTrend), new string[][] { ToChartData(tick, _process.TotalCpu) })
            .WithAttribute(new ChartAttribute
            {
               Title = "Total CPU",
               XAxisLabel = "Time",
               YAxisLabel = "%",
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

         return new Disposable(totalCpuTrendSubs, intervalSubs);
      }

      private string[] ToChartData(int tick, double value) => new string[] { $"{tick}", $"{value}" };
   }

   public static class PulseCpuUsageExtensions
   {
      public static IServiceCollection AddPulseCpuUsage(this IServiceCollection services)
      {
         return services.AddSingleton<IPulseDataProvider, CpuUsageProvider>();
      }
   }
}