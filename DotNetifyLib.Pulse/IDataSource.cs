using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetify.Pulse
{
   public interface IDataSource
   {
      void ConfigureView(PulseVM pulseVM, out Action onPushUpdate, out Action onDispose);
   }
}