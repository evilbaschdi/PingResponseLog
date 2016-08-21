using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PingResponseLog.Models
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class PingLog
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public ObservableCollection<PingLogEntry> PingLogEntries { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string LogAsText { get; set; }
    }
}