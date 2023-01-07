using System.Runtime.Serialization;

namespace PingResponseLog.Internal.Models;

/// <summary>
/// </summary>
[DataContract]
public class Address
{
    /// <summary>
    /// </summary>
    [DataMember]
    public bool AddToAddresses { get; set; }

    /// <summary>
    /// </summary>
    [DataMember]
    public string Name { get; set; }
}