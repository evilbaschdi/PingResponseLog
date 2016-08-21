using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using EvilBaschdi.Core.Logging;
using PingResponseLog.Core;
using PingResponseLog.Models;

namespace PingResponseLog.Internal
{
    /// <summary>
    /// </summary>
    public class PingProcessor : IPingProcessor
    {
        private readonly IPingHelper _pingHelper;
        private readonly ILoggingHelper _loggingHelper;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IAppendAllTextWithHeadline _appendAllTextWithHeadline;

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
            if (pingHelper == null)
            {
                throw new ArgumentNullException(nameof(pingHelper));
            }
            if (loggingHelper == null)
            {
                throw new ArgumentNullException(nameof(loggingHelper));
            }
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            if (appendAllTextWithHeadline == null)
            {
                throw new ArgumentNullException(nameof(appendAllTextWithHeadline));
            }

            _pingHelper = pingHelper;
            _loggingHelper = loggingHelper;
            _applicationSettings = applicationSettings;
            _appendAllTextWithHeadline = appendAllTextWithHeadline;
        }

        /// <summary>
        /// </summary>
        public PingLog Value
        {
            get
            {
                var pingLogEntries = new ObservableCollection<PingLogEntry>();
                var stringBuilder = new StringBuilder();

                Parallel.ForEach(_pingHelper.AddressList, address =>
                                                          {
                                                              var pingLogEntry = new PingLogEntry();
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

                                                              var dnsName = reply != null && reply.Status == IPStatus.Success
                                                                  ? _pingHelper.GetDnsName(address)
                                                                  : new KeyValuePair<string, string>(address, "Error resolving IP or DNS name");

                                                              pingLogEntry.TimeStamp = DateTime.Now;
                                                              pingLogEntry.Dns = dnsName.Value;
                                                              pingLogEntry.Ip = dnsName.Key;
                                                              pingLogEntry.Response = response;
                                                              pingLogEntries.Add(pingLogEntry);
                                                          });

                Parallel.ForEach(pingLogEntries, entry => { stringBuilder.Append($"{entry.TimeStamp};{entry.Dns};{entry.Ip};{entry.Response};{Environment.NewLine}"); });

                _appendAllTextWithHeadline.For($@"{_applicationSettings.LoggingPath}\{_loggingHelper.PingResponseLogFileName}", stringBuilder, "TimeStamp;DNS;IP;Response;");

                var pingLog = new PingLog
                              {
                                  LogAsText = $"{stringBuilder}-----{Environment.NewLine}",
                                  PingLogEntries = pingLogEntries
                              };
                return pingLog;
            }
        }
    }
}