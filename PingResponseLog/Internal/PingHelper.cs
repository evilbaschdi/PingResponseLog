using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using PingResponseLog.Internal.Core;

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
            _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        }

        /// <summary>
        /// </summary>
        public List<string> AddressList
        {
            get
            {
                var addressList = new List<string>();
                var addressString = _applicationSettings.Addresses;
                if (string.IsNullOrWhiteSpace(addressString))
                {
                    return addressList;
                }

                if (addressString.Contains(","))
                {
                    addressList.AddRange(addressString.Split(',').Select(address => address.Trim()));
                }
                else
                {
                    addressList.Add(addressString.Trim());
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

                    if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        continue;
                    }

                    ip = ipAddress.ToString();
                    break;
                }

                return string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(hostName)
                    ? new(input, "Error resolving IP or DNS name")
                    : new KeyValuePair<string, string>(ip, hostName);
            }
            catch (Exception e)
            {
                return new(input, e.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">default.</exception>
        public string GetResponse(PingReply reply)
        {
            var status = reply.Status switch
            {
                IPStatus.Success => $"{reply.RoundtripTime} ms",
                IPStatus.TimedOut => "Timeout",
                IPStatus.DestinationNetworkUnreachable => "Destination Network Unreachable",
                IPStatus.DestinationHostUnreachable => "Destination Host Unreachable",
                IPStatus.DestinationProtocolUnreachable => "Destination Protocol Unreachable",
                IPStatus.DestinationPortUnreachable => "Destination Port Unreachable",
                IPStatus.NoResources => "No Resources",
                IPStatus.BadOption => "Bad Option",
                IPStatus.HardwareError => "Hardware Error",
                IPStatus.PacketTooBig => "PacketTooBig",
                IPStatus.BadRoute => "Bad Route",
                IPStatus.TtlExpired => "Ttl Expired",
                IPStatus.TtlReassemblyTimeExceeded => "Ttl Reassembly Time Exceeded",
                IPStatus.ParameterProblem => "Parameter Problem",
                IPStatus.SourceQuench => "Source Quench",
                IPStatus.BadDestination => "Bad Destination",
                IPStatus.DestinationUnreachable => "Destination Unreachable",
                IPStatus.TimeExceeded => "Time Exceeded",
                IPStatus.BadHeader => "Bad Header",
                IPStatus.UnrecognizedNextHeader => "Unrecognized Next Header",
                IPStatus.IcmpError => "Icmp Error",
                IPStatus.DestinationScopeMismatch => "Destination Scope Mismatch",
                IPStatus.Unknown => "Unknown",
                _ => throw new ArgumentOutOfRangeException()
            };

            return status;
        }
    }
}