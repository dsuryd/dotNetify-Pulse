/*
Copyright 2019 Dicky Suryadi
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Subjects;
using DotNetify.Elements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DotNetify.Pulse.Log
{
   public interface IPulseLogger { }

   public class LogDataProvider : ILogger, IPulseLogger, IPulseDataProvider
   {
      private readonly LoggerExternalScopeProvider _scopeProvider;
      private readonly ReplaySubject<LogItem> _logStream;
      private readonly LogConfiguration _logConfig;

      public LogDataProvider(LoggerExternalScopeProvider scopeProvider, PulseConfiguration pulseConfig)
      {
         _scopeProvider = scopeProvider;
         _logConfig = pulseConfig.Log;
         _logStream = new ReplaySubject<LogItem>(pulseConfig.Log.Buffer);
      }

      #region ILogger Methods

      public IDisposable BeginScope<TState>(TState state)
      {
         return _scopeProvider.Push(state);
      }

      public bool IsEnabled(LogLevel logLevel)
      {
         if (logLevel == LogLevel.None)
            return false;

         return true;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
         _logStream.OnNext(new LogItem(logLevel, formatter(state, exception)));
      }

      #endregion ILogger Methods

      public IDisposable Configure(PulseVM pulseVM, out OnPushUpdate onPushUpdate)
      {
         // Property names.
         string Logs;
         string SelectedLog;

         var selectedLog = pulseVM.AddProperty<DateTimeOffset>(nameof(SelectedLog));

         pulseVM.AddProperty(nameof(Logs), new LogItem[] { })
             .WithItemKey(nameof(LogItem.Time))
             .WithAttribute(
                 new DataGridAttribute
                 {
                    RowKey = nameof(LogItem.Time),
                    Columns = new DataGridColumn[]
                     {
                        new DataGridColumn(nameof(LogItem.FormattedTime), "Time") { Sortable = true },
                        new DataGridColumn(nameof(LogItem.Level), "Level") { Sortable = true },
                        new DataGridColumn(nameof(LogItem.Message), "Message")
                     },
                    Rows = _logConfig.Rows
                 }
                 .CanSelect(DataGridAttribute.Selection.Single, selectedLog)
             );

         var cachedLogs = new ConcurrentStack<LogItem>();

         onPushUpdate = liveUpdate =>
         {
            if (liveUpdate)
            {
               if (cachedLogs.TryPop(out LogItem log))
               {
                  pulseVM.AddList(nameof(Logs), log);
                  selectedLog.Value = log.Time;
               }
            }
            else
               cachedLogs.Clear();
         };

         return _logStream.Subscribe(log => cachedLogs.Push(log));
      }
   }

   public static class PulseLoggerExtensions
   {
      public static IServiceCollection AddPulseLogger(this IServiceCollection services)
      {
         services.TryAddTransient<LoggerExternalScopeProvider>();
         services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, PulseLoggerProvider>());
         services.TryAddEnumerable(ServiceDescriptor.Singleton<IPulseDataProvider, LogDataProvider>());
         services.TryAddSingleton(provider => (IPulseLogger) provider.GetServices<IPulseDataProvider>().First(x => x is LogDataProvider));
         return services;
      }
   }
}