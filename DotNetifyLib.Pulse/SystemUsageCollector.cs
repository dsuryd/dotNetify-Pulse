using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetify.Pulse
{
    public class SystemUsageCollector : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly Process _process = Process.GetCurrentProcess();
        private DateTime _lastTimeStamp;
        private TimeSpan _lastTotalProcessorTime = TimeSpan.Zero;
        private TimeSpan _lastUserProcessorTime = TimeSpan.Zero;
        private TimeSpan _lastPrivilegedProcessorTime = TimeSpan.Zero;

        public SystemUsageCollector()
        {
            _lastTimeStamp = _process.StartTime;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CollectData, null, 1000, 5000);
            return Task.CompletedTask;
        }

        private void CollectData(object state)
        {
            double totalCpuTimeUsed = _process.TotalProcessorTime.TotalMilliseconds - _lastTotalProcessorTime.TotalMilliseconds;
            double privilegedCpuTimeUsed = _process.PrivilegedProcessorTime.TotalMilliseconds - _lastPrivilegedProcessorTime.TotalMilliseconds;
            double userCpuTimeUsed = _process.UserProcessorTime.TotalMilliseconds - _lastUserProcessorTime.TotalMilliseconds;

            _lastTotalProcessorTime = _process.TotalProcessorTime;
            _lastPrivilegedProcessorTime = _process.PrivilegedProcessorTime;
            _lastUserProcessorTime = _process.UserProcessorTime;

            double cpuTimeElapsed = (DateTime.UtcNow - _lastTimeStamp).TotalMilliseconds * Environment.ProcessorCount;
            _lastTimeStamp = DateTime.UtcNow;

            var totalCpuUsed = totalCpuTimeUsed * 100 / cpuTimeElapsed;
            var privilegedCpuUsed = privilegedCpuTimeUsed * 100 / cpuTimeElapsed;
            var userCpuUsed = userCpuTimeUsed * 100 / cpuTimeElapsed;

            var workingSet = _process.WorkingSet64;
            var nonPagedSystemMemory = _process.NonpagedSystemMemorySize64;
            var pagedMemory = _process.PagedMemorySize64;
            var pagedSystemMemory = _process.PagedSystemMemorySize64;
            var privateMemory = _process.PrivateMemorySize64;
            var virtualMemoryMemory = _process.VirtualMemorySize64;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            return Task.CompletedTask;
        }
    }
}