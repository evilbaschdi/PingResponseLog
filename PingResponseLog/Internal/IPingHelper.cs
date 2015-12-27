using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace PingResponseLog.Internal
{
    public interface IPingHelper
    {
        List<string> AddressList { get; }

        string GetDnsName(string input);

        string GetStatus(PingReply reply);

        string PingResponseLogFileName { get; }
    }
}