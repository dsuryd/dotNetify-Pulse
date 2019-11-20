namespace DotNetify.Pulse.Log
{
    public interface ILogEmitter
    {
        ReactiveProperty<LogItem> Log { get; }
    }

    public class LogEmitter : ILogEmitter
    {
        public ReactiveProperty<LogItem> Log { get; } = new ReactiveProperty<LogItem>();
    }
}