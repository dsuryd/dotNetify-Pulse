namespace DotNetify.Pulse
{
    public class PulseConfiguration
    {
        // Absolute path to the folder containing UI static files.
        public string UIPath { get; set; }

        // Minimum interval between push updates in milliseconds.
        public int PushUpdateInterval { get; set; } = 100;

        public LogConfiguration Log { get; set; } = new LogConfiguration();
    }

    public class LogConfiguration
    {
        // How many last log items to cache.
        public int Buffer { get; set; } = 10;

        // Number of rows in the log data grid.
        public int Rows { get; set; } = 20;
    }
}