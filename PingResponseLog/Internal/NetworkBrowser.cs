using System.DirectoryServices;

namespace PingResponseLog.Internal;

/// <inheritdoc />
/// <summary>
///     Class for NetworkBrowser.
/// </summary>
public sealed class NetworkBrowser : INetworkBrowser
{
    /// <inheritdoc />
    /// <summary>
    ///     Contains an ArrayList of computers found in the network.
    /// </summary>
    public List<string> Value
    {
        get
        {
            var networkComputers = new List<string>();

            try
            {
                var root = new DirectoryEntry("WinNT:");

                networkComputers.AddRange(from DirectoryEntry computers in root.Children
                                          from DirectoryEntry computer in computers.Children
                                          where computer.SchemaClassName == "Computer"
                                          select computer.Name.ToLower());

                return networkComputers;
            }
            catch (Exception e)
            {
                Exception = e;
                return null;
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Contains an Exception if Value has thrown some.
    /// </summary>
    public Exception Exception { get; private set; }
}