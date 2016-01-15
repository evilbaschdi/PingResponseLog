using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using PingResponseLog.Core;

namespace PingResponseLog.Internal
{
    /// <summary>
    /// </summary>
    public class PingProcessor : IPingProcessor
    {
        private readonly IPingHelper _pingHelper;
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        public PingProcessor(IPingHelper pingHelper, IApplicationSettings applicationSettings)
        {
            if(pingHelper == null)
            {
                throw new ArgumentNullException(nameof(pingHelper));
            }
            if(applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }

            _pingHelper = pingHelper;
            _applicationSettings = applicationSettings;
        }

        /// <summary>
        /// </summary>
        public string CallPing
        {
            get
            {
                var text = string.Empty;
                Parallel.ForEach(_pingHelper.AddressList, address =>
                {
                    PingReply reply = null;
                    string status;
                    try
                    {
                        reply = new Ping().Send(address);
                        status = _pingHelper.GetStatus(reply);
                    }
                    catch(PingException e)
                    {
                        status = e.Message;
                    }

                    var dns = reply != null && reply.Status == IPStatus.Success ? _pingHelper.GetDnsName(address) : "error resolving ip or dns name";

                    var info = $"{DateTime.Now}, {address} ({dns}),";

                    var result = $"{info} {status}{Environment.NewLine}";

                    text += result;
                });
                File.AppendAllText($@"{_applicationSettings.LoggingPath}\{_pingHelper.PingResponseLogFileName}", text);

                return $"{text}-----{Environment.NewLine}";
            }
        }
    }
}