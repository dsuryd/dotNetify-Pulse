using DotNetify.Elements;
using DotNetify.Pulse.Log;
using System;

namespace DotNetify.Pulse
{
    public class PulseVM : BaseVM
    {
        public PulseVM(ILogEmitter logEmitter)
        {
            // Property names.
            string Logs;
            string SelectedLog;

            AddProperty(nameof(Logs), new LogItem[] { })
                .WithItemKey(nameof(LogItem.TimeStamp))
                .WithAttribute(
                    new DataGridAttribute
                    {
                        RowKey = nameof(LogItem.TimeStamp),
                        Columns = new DataGridColumn[]
                        {
                            new DataGridColumn(nameof(LogItem.TimeStamp), "Timestamp") { Sortable = true },
                            new DataGridColumn(nameof(LogItem.Level), "Log Level"),
                            new DataGridColumn(nameof(LogItem.Message), "Message")
                        },
                        Rows = 20
                    }
                    .CanSelect(
                        DataGridAttribute.Selection.Single,
                        AddInternalProperty(nameof(SelectedLog), string.Empty))
                );

            logEmitter.Log.Subscribe(log =>
            {
                this.AddList(nameof(Logs), log);
                PushUpdates();
            });
        }
    }
}