using DotNetify.Elements;
using DotNetify.Pulse.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace DotNetify.Pulse
{
    public class PulseVM : BaseVM
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public PulseVM(ILogEmitter logEmitter, PulseConfiguration pulseConfig)
        {
            var logUpdateAction = AddLogDataSource(logEmitter, pulseConfig);

            // Set minimum interval to push updates.
            Observable
               .Interval(TimeSpan.FromMilliseconds(pulseConfig.PushUpdateInterval))
               .Subscribe(_ =>
               {
                   logUpdateAction();
                   PushUpdates();
               })
               .AddTo(_subscriptions);
        }

        public override void Dispose()
        {
            _subscriptions.ForEach(x => x.Dispose());
            base.Dispose();
        }

        private Action AddLogDataSource(ILogEmitter logEmitter, PulseConfiguration pulseConfig)
        {
            // Property names.
            string Logs;
            string SelectedLog;

            var selectedLog = AddProperty<DateTimeOffset>(nameof(SelectedLog));

            AddProperty(nameof(Logs), new LogItem[] { })
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
                        Rows = pulseConfig.Log.Rows
                    }
                    .CanSelect(DataGridAttribute.Selection.Single, selectedLog)
                );

            var cachedLogs = new ConcurrentStack<LogItem>();

            logEmitter.Log
                .Subscribe(log => cachedLogs.Push(log))
                .AddTo(_subscriptions);

            return () =>
            {
                if (cachedLogs.TryPop(out LogItem log))
                {
                    this.AddList(nameof(Logs), log);
                    selectedLog.Value = log.Time;
                }
            };
        }
    }
}