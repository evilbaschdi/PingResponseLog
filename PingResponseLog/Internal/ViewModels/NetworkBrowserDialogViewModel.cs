using EvilBaschdi.Core.Wpf;
using EvilBaschdi.Core.Wpf.Mvvm.ViewModel;
using JetBrains.Annotations;

namespace PingResponseLog.Internal.ViewModels;

/// <inheritdoc />
public class NetworkBrowserDialogViewModel : ApplicationLayoutViewModel
{
    /// <summary>
    ///     Constructor
    /// </summary>
    public NetworkBrowserDialogViewModel(
        [NotNull] IApplicationLayout applicationLayout,
        [NotNull] IApplicationStyle applicationStyle)
        : base(applicationLayout, applicationStyle, true, false)
    {
    }
}