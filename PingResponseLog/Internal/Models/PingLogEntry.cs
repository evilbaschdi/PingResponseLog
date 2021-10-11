using System;
using System.Runtime.Serialization;

namespace PingResponseLog.Internal.Models
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class PingLogEntry
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public string Dns { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Ip { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Response { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public DateTime TimeStamp { get; set; }
    }
}