namespace PingResponseLog.Core
{
    /// <summary>
    ///     Wrapper arround Default Settings.
    /// </summary>
    public interface IApplicationSettings
    {
        /// <summary>
        /// </summary>
        string Addresses { get; set; }

        /// <summary>
        /// </summary>
        string LoggingPath { get; set; }

        /// <summary>
        /// </summary>
        string InterNetworkType { get; set; }

        /// <summary>
        /// </summary>
        int TimeSpanHours { get; set; }

        /// <summary>
        /// </summary>
        int TimeSpanMinutes { get; set; }

        /// <summary>
        /// </summary>
        int TimeSpanSeconds { get; set; }
    }
}