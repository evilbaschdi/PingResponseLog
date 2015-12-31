﻿namespace PingResponseLog.Core
{
    /// <summary>
    ///     Wrapper arround Default Settings.
    /// </summary>
    public class ApplicationSettings : IApplicationSettings
    {
        /// <summary>
        /// </summary>
        public string Addresses
        {
            get
            {
                return string.IsNullOrWhiteSpace(Properties.Settings.Default.Addresses)
                    ? ""
                    : Properties.Settings.Default.Addresses;
            }
            set
            {
                Properties.Settings.Default.Addresses = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// </summary>
        public string LoggingPath
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.LoggingPath)
                        ? ""
                        : Properties.Settings.Default.LoggingPath;
            }
            set
            {
                Properties.Settings.Default.LoggingPath = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// </summary>
        public string InterNetworkType
        {
            get
            {
                return string.IsNullOrWhiteSpace(Properties.Settings.Default.InterNetworkType)
                    ? "V4"
                    : Properties.Settings.Default.InterNetworkType;
            }
            set
            {
                Properties.Settings.Default.InterNetworkType = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// </summary>
        public int TimeSpanHours
        {
            get { return Properties.Settings.Default.TimeSpanHours; }
            set
            {
                Properties.Settings.Default.TimeSpanHours = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// </summary>
        public int TimeSpanMinutes
        {
            get { return Properties.Settings.Default.TimeSpanMinutes; }
            set
            {
                Properties.Settings.Default.TimeSpanMinutes = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// </summary>
        public int TimeSpanSeconds
        {
            get { return Properties.Settings.Default.TimeSpanSeconds; }
            set
            {
                Properties.Settings.Default.TimeSpanSeconds = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}