using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace PingResponseLog.Internal
{
    /// <summary>
    /// </summary>
    public interface IPingHelper
    {
        /// <summary>
        /// </summary>
        List<string> AddressList { get; }

        /// <summary>
        /// </summary>
        KeyValuePair<string, string> GetDnsName(string input);

        /// <summary>
        /// </summary>
        string GetResponse(PingReply reply);
    }
}