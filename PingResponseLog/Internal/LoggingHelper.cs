using PingResponseLog.Internal.Core;

namespace PingResponseLog.Internal;

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
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
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
        return _applicationSettings.LoggingFileInterval switch
        {
            "per minute" => _applicationSettings.CurrentLoggingDateTime.Minute == DateTime.Now.Minute,
            "per hour" => _applicationSettings.CurrentLoggingDateTime.Hour == DateTime.Now.Hour,
            "per day" => _applicationSettings.CurrentLoggingDateTime.Day == DateTime.Now.Day,
            "per month" => _applicationSettings.CurrentLoggingDateTime.Month == DateTime.Now.Month,
            "per year" => _applicationSettings.CurrentLoggingDateTime.Year == DateTime.Now.Year,
            _ => true
        };
    }
}