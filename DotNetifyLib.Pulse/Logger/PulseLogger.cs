using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using DotNetify.Elements;
using Microsoft.Extensions.Logging;

namespace DotNetify.Pulse.Log
{
   public interface IPulseLogger
   {
   }

   public class PulseLogger : ILogger, IPulseLogger, IDataSource
   {
      private readonly LoggerExternalScopeProvider _scopeProvider;
      private readonly ReplaySubject<LogItem> _logStream;
      private readonly LogConfiguration _logConfig;

      public IObservable<LogItem> LogStream => _logStream;

      public PulseLogger(LoggerExternalScopeProvider scopeProvider, PulseConfiguration pulseConfig)
      {
         _scopeProvider = scopeProvider;
         _logStream = new ReplaySubject<LogItem>(pulseConfig.Log.Buffer);
         _logConfig = pulseConfig.Log;
      }

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

      public void ConfigureView(PulseVM pulseVM, out Action onPushUpdate, out Action onDispose)
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
         var subscription = _logStream.Subscribe(log => cachedLogs.Push(log));

         onDispose = () => subscription.Dispose();
         onPushUpdate = () =>
         {
            if (cachedLogs.TryPop(out LogItem log))
            {
               pulseVM.AddList(nameof(Logs), log);
               selectedLog.Value = log.Time;
            }
         };
      }
   }
}