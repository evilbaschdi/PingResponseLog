using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using System.Text;
using EvilBaschdi.Core.Logging;
using PingResponseLog.Internal.Core;
using PingResponseLog.Internal.Models;

namespace PingResponseLog.Internal;

/// <summary>
/// </summary>
public class PingProcessor : IPingProcessor
{
    private readonly IAppendAllTextWithHeadline _appendAllTextWithHeadline;
    private readonly IApplicationSettings _applicationSettings;
    private readonly ILoggingHelper _loggingHelper;
    private readonly IPingHelper _pingHelper;

    /// <summary>
    ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="pingHelper" /> is <see langword="null" />.
    ///     <paramref name="loggingHelper" /> is <see langword="null" />.
    ///     <paramref name="applicationSettings" /> is <see langword="null" />.
    ///     <paramref name="appendAllTextWithHeadline" /> is <see langword="null" />.
    /// </exception>
    public PingProcessor(IPingHelper pingHelper, ILoggingHelper loggingHelper, IApplicationSettings applicationSettings, IAppendAllTextWithHeadline appendAllTextWithHeadline)
    {
        _pingHelper = pingHelper ?? throw new ArgumentNullException(nameof(pingHelper));
        _loggingHelper = loggingHelper ?? throw new ArgumentNullException(nameof(loggingHelper));
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        _appendAllTextWithHeadline = appendAllTextWithHeadline ?? throw new ArgumentNullException(nameof(appendAllTextWithHeadline));
    }

    /// <summary>
    /// </summary>
    public PingLog Value
    {
        get
        {
            var pingLogEntries = new ConcurrentBag<PingLogEntry>();
            var stringBuilder = new StringBuilder();

            Parallel.ForEach(_pingHelper.AddressList, address =>
                                                      {
                                                          var pingLogEntry = new PingLogEntry
                                                                             {
                                                                                 TimeStamp = DateTime.Now
                                                                             };
                                                          PingReply reply = null;
                                                          string response;
                                                          try
                                                          {
                                                              reply = new Ping().Send(address);
                                                              response = _pingHelper.GetResponse(reply);
                                                          }
                                                          catch (PingException)
                                                          {
                                                              response = "Destination Host or Network Unreachable";
                                                          }

                                                          var (ip, dns) = reply is { Status: IPStatus.Success }
                                                              ? _pingHelper.GetDnsName(address)
                                                              : new(address, "Error resolving IP or DNS name");

                                                          pingLogEntry.Dns = dns;
                                                          pingLogEntry.Ip = ip;
                                                          pingLogEntry.Response = response;
                                                          pingLogEntries.Add(pingLogEntry);
                                                      });

            foreach (var entry in pingLogEntries)
            {
                if (entry != null)
                {
                    stringBuilder.Append($"{entry.TimeStamp};{entry.Dns};{entry.Ip};{entry.Response};{Environment.NewLine}");
                }
            }

            _appendAllTextWithHeadline.RunFor($@"{_applicationSettings.LoggingPath}\{_loggingHelper.PingResponseLogFileName}", stringBuilder, "TimeStamp;DNS;IP;Response;");

            return new()
                   {
                       LogAsText = $"{stringBuilder}-----{Environment.NewLine}",
                       PingLogEntries = new(pingLogEntries)
                   };
        }
    }
}