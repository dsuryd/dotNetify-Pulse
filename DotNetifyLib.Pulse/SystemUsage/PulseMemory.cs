using System;
using System.Diagnostics;
using System.Reactive.Linq;
using DotNetify.Elements;

namespace DotNetify.Pulse.SystemUsage
{
   public class PulseMemory : IPulseDataSource
   {
      private readonly Process _process = Process.GetCurrentProcess();

      public IDisposable Configure(PulseVM pulseVM, out OnPushUpdate onPushUpdate)
      {
         // Property names.
         string WorkingSetMemory;

         pulseVM.AddProperty<float>(nameof(WorkingSetMemory))
            .WithAttribute(new ChartAttribute
            {
            });

         var workingSet = _process.WorkingSet64;
         var nonPagedSystemMemory = _process.NonpagedSystemMemorySize64;
         var pagedMemory = _process.PagedMemorySize64;
         var pagedSystemMemory = _process.PagedSystemMemorySize64;
         var privateMemory = _process.PrivateMemorySize64;
         var virtualMemoryMemory = _process.VirtualMemorySize64;

         var subscription = Observable
            .Interval(TimeSpan.FromMilliseconds(500))
            .Subscribe(_ =>
            {
            });

         onPushUpdate = _ => { };
         return subscription;
      }
   }
}