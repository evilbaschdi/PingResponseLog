using EvilBaschdi.Core.Wpf;
using EvilBaschdi.Core.Wpf.Mvvm.ViewModel;

namespace PingResponseLog.Internal.ViewModels;

/// <inheritdoc />
public class NetworkBrowserDialogViewModel : ApplicationStyleViewModel
{
    /// <summary>
    ///     Constructor
    /// </summary>
    public NetworkBrowserDialogViewModel(IApplicationStyle applicationStyle)
        : base(applicationStyle)
    {
    }
}