using System;
using System.Reactive.Subjects;

namespace DotNetify.Pulse.Log
{
    public interface ILogEmitter
    {
        IObservable<LogItem> Log { get; }

        void Emit(LogItem logItem);
    }

    public class LogEmitter : ILogEmitter
    {
        public IObservable<LogItem> Log { get; }

        public LogEmitter(PulseConfiguration pulseConfig)
        {
            Log = new ReplaySubject<LogItem>(pulseConfig.Log.Buffer);
        }

        public void Emit(LogItem logItem)
        {
            (Log as ReplaySubject<LogItem>).OnNext(logItem);
        }
    }
}