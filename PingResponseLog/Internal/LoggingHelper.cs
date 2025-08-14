using PingResponseLog.Internal.Core;

namespace PingResponseLog.Internal;

/// <summary>
/// </summary>
public class LoggingHelper : ILoggingHelper
{
    private readonly IApplicationSettings _applicationSettings;

    /// <summary>
    ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="applicationSettings" /> is <see langword="null" />.</exception>
    public LoggingHelper(IApplicationSettings applicationSettings)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
    }

    /// <summary>
    /// </summary>
    public string PingResponseLogFileName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(field) && IsCurrentDateTimeValid())
            {
                return field;
            }

            _applicationSettings.CurrentLoggingDateTime = DateTime.Now;
            field = $"PingResponseLog_{_applicationSettings.CurrentLoggingDateTime:yyyy-MM-dd_HHmm}.csv";
            return field;
        }
    }

    private bool IsCurrentDateTimeValid()
    {
        var dateTimeNow = DateTime.Now;
        return _applicationSettings.LoggingFileInterval switch
        {
            "per minute" => _applicationSettings.CurrentLoggingDateTime.Minute == dateTimeNow.Minute,
            "per hour" => _applicationSettings.CurrentLoggingDateTime.Hour == dateTimeNow.Hour,
            "per day" => _applicationSettings.CurrentLoggingDateTime.Day == dateTimeNow.Day,
            "per month" => _applicationSettings.CurrentLoggingDateTime.Month == dateTimeNow.Month,
            "per year" => _applicationSettings.CurrentLoggingDateTime.Year == dateTimeNow.Year,
            _ => true
        };
    }
}