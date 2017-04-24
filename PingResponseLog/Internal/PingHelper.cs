using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using PingResponseLog.Core;

namespace PingResponseLog.Internal
{
    /// <summary>
    /// </summary>
    public class PingHelper : IPingHelper
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="applicationSettings" /> is <see langword="null" />.</exception>
        public PingHelper(IApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

        /// <summary>
        /// </summary>
        public List<string> AddressList
        {
            get
            {
                var addressList = new List<string>();
                var addressString = _applicationSettings.Addresses;
                if (!string.IsNullOrWhiteSpace(addressString))
                {
                    if (addressString.Contains(","))
                    {
                        addressList.AddRange(addressString.Split(',').Select(address => address.Trim()));
                    }
                    else
                    {
                        addressList.Add(addressString.Trim());
                    }
                }
                return addressList;
            }
        }

        /// <summary>
        /// </summary>
        public KeyValuePair<string, string> GetDnsName(string input)
        {
            try
            {
                var ipHostEntry = Dns.GetHostEntry(input);
                var hostName = ipHostEntry.HostName;
                var ip = string.Empty;

                foreach (var ipAddress in ipHostEntry.AddressList)
                {
                    if (_applicationSettings.InterNetworkType == "V6" && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        ip = ipAddress.ToString();
                        break;
                    }
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ip = ipAddress.ToString();
                        break;
                    }
                }
                return string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(hostName)
                    ? new KeyValuePair<string, string>(input, "Error resolving IP or DNS name")
                    : new KeyValuePair<string, string>(ip, hostName);
            }
            catch (Exception e)
            {
                return new KeyValuePair<string, string>(input, e.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">default.</exception>
        public string GetResponse(PingReply reply)
        {
            string status;
            switch (reply.Status)
            {
                case IPStatus.Success:
                    status = $"{reply.RoundtripTime} ms";
                    break;

                case IPStatus.TimedOut:
                    status = "Timeout";
                    break;

                case IPStatus.DestinationNetworkUnreachable:
                    status = "Destination Network Unreachable";
                    break;

                case IPStatus.DestinationHostUnreachable:
                    status = "Destination Host Unreachable";
                    break;

                case IPStatus.DestinationProtocolUnreachable:
                    status = "Destination Protocol Unreachable";
                    break;

                case IPStatus.DestinationPortUnreachable:
                    status = "Destination Port Unreachable";
                    break;

                case IPStatus.NoResources:
                    status = "No Resources";
                    break;

                case IPStatus.BadOption:
                    status = "Bad Option";
                    break;

                case IPStatus.HardwareError:
                    status = "Hardware Error";
                    break;

                case IPStatus.PacketTooBig:
                    status = "PacketTooBig";
                    break;

                case IPStatus.BadRoute:
                    status = "Bad Route";
                    break;

                case IPStatus.TtlExpired:
                    status = "Ttl Expired";
                    break;

                case IPStatus.TtlReassemblyTimeExceeded:
                    status = "Ttl Reassembly Time Exceeded";
                    break;

                case IPStatus.ParameterProblem:
                    status = "Parameter Problem";
                    break;

                case IPStatus.SourceQuench:
                    status = "Source Quench";
                    break;

                case IPStatus.BadDestination:
                    status = "Bad Destination";
                    break;

                case IPStatus.DestinationUnreachable:
                    status = "Destination Unreachable";
                    break;

                case IPStatus.TimeExceeded:
                    status = "Time Exceeded";
                    break;

                case IPStatus.BadHeader:
                    status = "Bad Header";
                    break;

                case IPStatus.UnrecognizedNextHeader:
                    status = "Unrecognized Next Header";
                    break;

                case IPStatus.IcmpError:
                    status = "Icmp Error";
                    break;

                case IPStatus.DestinationScopeMismatch:
                    status = "Destination Scope Mismatch";
                    break;

                case IPStatus.Unknown:
                    status = "Unknown";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return status;
        }
    }
}