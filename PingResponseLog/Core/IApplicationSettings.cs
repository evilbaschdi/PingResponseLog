namespace PingResponseLog.Core
{
    public interface IApplicationSettings
    {
        string Addresses { get; set; }
        string LoggingPath { get; set; }

        string InterNetworkType { get; set; }
        int TimeSpanHours { get; set; }
        int TimeSpanMinutes { get; set; }
        int TimeSpanSeconds { get; set; }
    }
}