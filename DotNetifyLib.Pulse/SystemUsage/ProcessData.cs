using System;
using System.Diagnostics;

namespace DotNetify.Pulse.SystemUsage
{
   public class ProcessData
   {
      private Process _process;
      private TimeSpan? _lastTotalProcessorTime;
      private DateTime? _lastTimeStamp;

      public double TotalCpu => CalculateTotalCpu();
      public double GCTotalMemory => GC.GetTotalMemory(false).ToMBytes();

      public ProcessData(Process process)
      {
         _process = process;
      }

      private double CalculateTotalCpu()
      {
         _lastTotalProcessorTime = _lastTotalProcessorTime ?? _process.TotalProcessorTime;
         _lastTimeStamp = _lastTimeStamp ?? _process.StartTime;

         double totalCpuTimeUsed = _process.TotalProcessorTime.TotalMilliseconds - _lastTotalProcessorTime.Value.TotalMilliseconds;
         double cpuTimeElapsed = (DateTime.UtcNow - _lastTimeStamp.Value).TotalMilliseconds * Environment.ProcessorCount;

         _lastTotalProcessorTime = _process.TotalProcessorTime;
         _lastTimeStamp = DateTime.UtcNow;

         return (totalCpuTimeUsed / cpuTimeElapsed).ToPercent();
      }
   }

   internal static class ProcessDataExtensions
   {
      private static readonly double MB = Math.Pow(1024, 2);

      public static double ToMBytes(this long number) => Math.Round(number / MB, 2);

      public static double ToPercent(this double number) => Math.Round(number * 100, 2);
   }
}