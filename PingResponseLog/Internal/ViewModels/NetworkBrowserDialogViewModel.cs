using EvilBaschdi.CoreExtended;
using EvilBaschdi.CoreExtended.Mvvm.ViewModel;

namespace PingResponseLog.Internal.ViewModels
{
    /// <inheritdoc />
    public class NetworkBrowserDialogViewModel : ApplicationStyleViewModel
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="roundCorners"></param>
        /// <param name="center"></param>
        /// <param name="resizeWithBorder400"></param>
        public NetworkBrowserDialogViewModel(IRoundCorners roundCorners, bool center = false, bool resizeWithBorder400 = false)
            : base(roundCorners, center, resizeWithBorder400)
        {
        }
    }
}