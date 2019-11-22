using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotNetify.Elements;
using DotNetify.Pulse.Log;

namespace DotNetify.Pulse
{
   public class PulseVM : BaseVM
   {
      private readonly List<Action> _disposeActions = new List<Action>();

      public PulseVM(IEnumerable<IDataSource> dataSources, PulseConfiguration pulseConfig)
      {
         string LiveUpdate;
         var options = new Dictionary<string, string> { { "on", "On" }, { "off", "Off" } };

         var liveUpdateProp = AddProperty(nameof(LiveUpdate), "on")
             .WithAttribute(new RadioGroupAttribute() { Options = options.ToArray() });

         var onPushUpdates = new List<Action>();
         foreach (var dataSource in dataSources)
         {
            dataSource.ConfigureView(this, out Action onPushUpdate, out Action onDispose);
            onPushUpdates.Add(onPushUpdate);
            _disposeActions.Add(onDispose);
         }

         // Set minimum interval to push updates.
         var intervalSubscription = Observable
            .Interval(TimeSpan.FromMilliseconds(pulseConfig.PushUpdateInterval))
            .Subscribe(_ =>
            {
               if (liveUpdateProp == "off")
                  return;

               onPushUpdates.ForEach(x => x());
               PushUpdates();
            });

         _disposeActions.Add(() => intervalSubscription.Dispose());
      }

      public override void Dispose()
      {
         _disposeActions.ForEach(x => x());
         base.Dispose();
      }
   }
}