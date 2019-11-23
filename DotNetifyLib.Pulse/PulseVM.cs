using DotNetify.Elements;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace DotNetify.Pulse
{
   public class PulseVM : BaseVM
   {
      private readonly List<IDisposable> _disposables = new List<IDisposable>();

      public ReactiveProperty<bool> LiveUpdate { get; }

      public PulseVM(IEnumerable<IPulseDataSource> dataSources, PulseConfiguration pulseConfig)
      {
         LiveUpdate = AddProperty(nameof(LiveUpdate), true)
             .WithAttribute(new CheckboxAttribute() { Label = "Live update" });

         var onPushUpdates = ConfigureDataSource(dataSources);

         // Set minimum interval to push updates.
         Observable
            .Interval(TimeSpan.FromMilliseconds(pulseConfig.PushUpdateInterval))
            .Subscribe(_ =>
            {
               onPushUpdates.ForEach(x => x(LiveUpdate));
               if (LiveUpdate)
                  PushUpdates();
            })
            .AddTo(_disposables);
      }

      public override void Dispose()
      {
         _disposables.ForEach(x => x.Dispose());
         base.Dispose();
      }

      private List<OnPushUpdate> ConfigureDataSource(IEnumerable<IPulseDataSource> dataSources)
      {
         var onPushUpdates = new List<OnPushUpdate>();

         foreach (var dataSource in dataSources)
         {
            dataSource
               .Configure(this, out OnPushUpdate onPushUpdate)
               .AddTo(_disposables);

            if (onPushUpdate != null)
               onPushUpdates.Add(onPushUpdate);
         }

         return onPushUpdates;
      }
   }
}