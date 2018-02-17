using System;
using System.IO;
using EvilBaschdi.CoreExtended.AppHelpers;

namespace PingResponseLog.Core
{
    /// <summary>
    ///     Wrapper around Default Settings.
    /// </summary>
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly IAppSettingsBase _appSettingsBase;

        /// <summary>
        /// </summary>
        public string Addresses
        {
            get => _appSettingsBase.Get<string>("Addresses");
            set => _appSettingsBase.Set("Addresses", value);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="appSettingsBase"></param>
        public ApplicationSettings(IAppSettingsBase appSettingsBase)
        {
            _appSettingsBase = appSettingsBase ?? throw new ArgumentNullException(nameof(appSettingsBase));
        }

        /// <summary>
        /// </summary>
        public string LoggingPath
        {
            get => _appSettingsBase.Get("LoggingPath", Path.GetTempPath());
            set => _appSettingsBase.Set("LoggingPath", value);
        }

        /// <summary>
        /// </summary>
        public DateTime CurrentLoggingDateTime
        {
            get => _appSettingsBase.Get<DateTime>("CurrentLoggingDateTime");
            set => _appSettingsBase.Set("CurrentLoggingDateTime", value);
        }

        /// <summary>
        /// </summary>
        public string LoggingFileInterval
        {
            get => _appSettingsBase.Get("LoggingFileInterval", Path.GetTempPath());
            set => _appSettingsBase.Set("LoggingFileInterval", value);
        }

        /// <summary>
        /// </summary>
        public string InterNetworkType
        {
            get => _appSettingsBase.Get("InterNetworkType", "V4");
            set => _appSettingsBase.Set("InterNetworkType", value);
        }

        /// <summary>
        /// </summary>
        public int TimeSpanHours
        {
            get => _appSettingsBase.Get<int>("TimeSpanHours");
            set => _appSettingsBase.Set("TimeSpanHours", value);
        }

        /// <summary>
        /// </summary>
        public int TimeSpanMinutes
        {
            get => _appSettingsBase.Get<int>("TimeSpanMinutes");
            set => _appSettingsBase.Set("TimeSpanMinutes", value);
        }

        /// <summary>
        /// </summary>
        public int TimeSpanSeconds
        {
            get => _appSettingsBase.Get<int>("TimeSpanSeconds");
            set => _appSettingsBase.Set("TimeSpanSeconds", value);
        }
    }
}