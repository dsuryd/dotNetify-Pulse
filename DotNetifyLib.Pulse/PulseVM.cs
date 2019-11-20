using DotNetify.Elements;
using DotNetify.Pulse.Log;
using System;

namespace DotNetify.Pulse
{
    public class PulseVM : BaseVM
    {
        private IDisposable _logSubscription;

        public PulseVM(ILogEmitter logEmitter)
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
                        Rows = 20
                    }
                    .CanSelect(DataGridAttribute.Selection.Single, selectedLog)
                );

            _logSubscription = logEmitter.Log.Subscribe(log =>
            {
                this.AddList(nameof(Logs), log);
                selectedLog.Value = log.Time;
                PushUpdates();
            });
        }

        public override void Dispose()
        {
            _logSubscription.Dispose();
            base.Dispose();
        }
    }
}