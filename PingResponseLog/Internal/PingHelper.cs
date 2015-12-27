using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using PingResponseLog.Core;

namespace PingResponseLog.Internal
{
    public class PingHelper : IPingHelper
    {
        private readonly IApplicationSettings _applicationSettings;
        private string _fileName;

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        public PingHelper(IApplicationSettings applicationSettings)
        {
            if(applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

        public List<string> AddressList
        {
            get
            {
                var addressList = new List<string>();
                var addressString = _applicationSettings.Addresses;
                if(addressString.Contains(","))
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

        public string GetDnsName(string input)
        {
            IPAddress address;
            var ipHostEntry = Dns.GetHostEntry(input);
            if(IPAddress.TryParse(input, out address))
            {
                return ipHostEntry.HostName;
            }

            foreach(var ip in ipHostEntry.AddressList)
            {
                if(_applicationSettings.InterNetworkType == "V6" && ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return ip.ToString();
                }
                if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "error resolving ip or dns name";
        }

        public string GetStatus(PingReply reply)
        {
            string status;
            switch(reply.Status)
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

        public string PingResponseLogFileName
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_fileName))
                {
                    _fileName = $"PingResponseLog_{DateTime.Now.ToString("yyyy-MM-dd_HHmm")}.txt";
                }
                return _fileName;
            }
        }
    }
}