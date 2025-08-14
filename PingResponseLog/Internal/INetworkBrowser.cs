using EvilBaschdi.Core;

namespace PingResponseLog.Internal;

/// <inheritdoc />
/// <summary>
///     Interface for NetworkBrowser.
/// </summary>
public interface INetworkBrowser : IListOf<string>
{
    /// <summary>
    ///     Contains an Exception if Value has thrown some.
    /// </summary>
    // ReSharper disable once UnusedMemberInSuper.Global
    // ReSharper disable once UnusedMember.Global
    Exception Exception { get; }
}