using System.Globalization;
using EvilBaschdi.Settings.ByMachineAndUser;
using JetBrains.Annotations;

namespace PingResponseLog.Internal.Core;

/// <summary>
///     Wrapper around Default Settings.
/// </summary>
public class ApplicationSettings : IApplicationSettings
{
    private readonly IAppSettingByKey _appSettingByKey;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="appSettingByKey"></param>
    public ApplicationSettings([NotNull] IAppSettingByKey appSettingByKey)
    {
        _appSettingByKey = appSettingByKey ?? throw new ArgumentNullException(nameof(appSettingByKey));
    }

    /// <summary>
    /// </summary>
    public string Addresses
    {
        get => _appSettingByKey.ValueFor("Addresses");
        set => _appSettingByKey.RunFor("Addresses", value);
    }

    /// <summary>
    /// </summary>
    public string LoggingPath
    {
        get => _appSettingByKey.ValueFor("LoggingPath");
        set => _appSettingByKey.RunFor("LoggingPath", value);
    }

    /// <summary>
    /// </summary>
    public DateTime CurrentLoggingDateTime
    {
        get => Convert.ToDateTime(_appSettingByKey.ValueFor("CurrentLoggingDateTime"), CultureInfo.InvariantCulture);
        set => _appSettingByKey.RunFor("CurrentLoggingDateTime", value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// </summary>
    public string LoggingFileInterval
    {
        get => _appSettingByKey.ValueFor("LoggingFileInterval");
        set => _appSettingByKey.RunFor("LoggingFileInterval", value);
    }

    /// <summary>
    /// </summary>
    public string InterNetworkType
    {
        get => _appSettingByKey.ValueFor("InterNetworkType");
        set => _appSettingByKey.RunFor("InterNetworkType", value);
    }

    /// <summary>
    /// </summary>
    public int TimeSpanHours
    {
        get => Convert.ToInt32(_appSettingByKey.ValueFor("TimeSpanHours"));
        set => _appSettingByKey.RunFor("TimeSpanHours", value.ToString());
    }

    /// <summary>
    /// </summary>
    public int TimeSpanMinutes
    {
        get => Convert.ToInt32(_appSettingByKey.ValueFor("TimeSpanMinutes"));
        set => _appSettingByKey.RunFor("TimeSpanMinutes", value.ToString());
    }

    /// <summary>
    /// </summary>
    public int TimeSpanSeconds
    {
        get => Convert.ToInt32(_appSettingByKey.ValueFor("TimeSpanSeconds"));
        set => _appSettingByKey.RunFor("TimeSpanSeconds", value.ToString());
    }
}