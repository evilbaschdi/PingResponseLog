using System;
using EvilBaschdi.CoreExtended.Metro;
using EvilBaschdi.CoreExtended.Mvvm.ViewModel;

namespace PingResponseLog.Internal.ViewModels
{
    /// <inheritdoc />
    public class NetworkBrowserDialogViewModel : ApplicationStyleViewModel
    {
        /// <summary>
        /// </summary>
        /// <param name="themeManagerHelper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public NetworkBrowserDialogViewModel(IThemeManagerHelper themeManagerHelper)
            : base(themeManagerHelper)
        {
        }
    }
}