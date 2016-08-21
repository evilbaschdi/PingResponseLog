using System;
using PingResponseLog.Core;

namespace PingResponseLog.Internal
{
    /// <summary>
    /// </summary>
    public class LoggingHelper : ILoggingHelper
    {
        private readonly IApplicationSettings _applicationSettings;
        private string _fileName;

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="applicationSettings" /> is <see langword="null" />.</exception>
        public LoggingHelper(IApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

        /// <summary>
        /// </summary>
        public string PingResponseLogFileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_fileName) && IsCurrentDateTimeValid())
                {
                    return _fileName;
                }
                _applicationSettings.CurrentLoggingDateTime = DateTime.Now;
                _fileName = $"PingResponseLog_{_applicationSettings.CurrentLoggingDateTime:yyyy-MM-dd_HHmm}.csv";
                return _fileName;
            }
        }

        private bool IsCurrentDateTimeValid()
        {
            switch (_applicationSettings.LoggingFileInterval)
            {
                case "per minute":
                    return _applicationSettings.CurrentLoggingDateTime.Minute == DateTime.Now.Minute;
                case "per hour":
                    return _applicationSettings.CurrentLoggingDateTime.Hour == DateTime.Now.Hour;
                case "per day":
                    return _applicationSettings.CurrentLoggingDateTime.Day == DateTime.Now.Day;
                case "per month":
                    return _applicationSettings.CurrentLoggingDateTime.Month == DateTime.Now.Month;
                case "per year":
                    return _applicationSettings.CurrentLoggingDateTime.Year == DateTime.Now.Year;
                //case "per application instance":
                default:
                    return true;
            }
        }
    }
}