using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using EvilBaschdi.CoreExtended.AppHelpers;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PingResponseLog.Internal;
using PingResponseLog.Internal.Core;
using PingResponseLog.Internal.Models;
using PingResponseLog.Properties;

namespace PingResponseLog
{
    /// <summary>
    ///     Interaction logic for NetworkBrowserDialog.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class NetworkBrowserDialog : MetroWindow
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly IPingHelper _pingHelper;
        private ProgressDialogController _controller;
        private INetworkBrowser _networkBrowser;
        private Task<ObservableCollection<Address>> _task;
        private bool _windowShown;

        /// <summary>
        /// </summary>
        public NetworkBrowserDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            IAppSettingsBase appSettingsBase = new AppSettingsBase(Settings.Default);
            _applicationSettings = new ApplicationSettings(appSettingsBase);
            _pingHelper = new PingHelper(_applicationSettings);
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public ObservableCollection<Address> AddressList { get; set; }


        /// <summary>
        ///     Executing code when window is shown.
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_windowShown)
            {
                return;
            }

            _windowShown = true;

            await ConfigureController();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public async Task ConfigureController()
        {
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;

            Cursor = Cursors.Wait;

            var options = new MetroDialogSettings
                          {
                              ColorScheme = MetroDialogColorScheme.Accented
                          };

            MetroDialogOptions = options;
            _controller = await this.ShowProgressAsync("Loading...", "search is running", true, options);
            _controller.SetIndeterminate();
            _controller.Canceled += ControllerCanceled;

            _task = Task<ObservableCollection<Address>>.Factory.StartNew(LoadAddressList);
            await _task;
            _task.GetAwaiter().OnCompleted(TaskCompleted);
        }

        private void TaskCompleted()
        {
            AddressList = _task.Result;
            AddressListBox.ItemsSource = AddressList;
            AddressListBox.Visibility = Visibility.Visible;
            _controller.CloseAsync();
            _controller.Closed += ControllerClosed;
        }

        private void ControllerClosed(object sender, EventArgs e)
        {
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            TaskbarItemInfo.ProgressValue = 1;
            Cursor = Cursors.Arrow;
        }

        private void ControllerCanceled(object sender, EventArgs e)
        {
            _controller.CloseAsync();
            _controller.Closed += ControllerClosed;
        }

        private ObservableCollection<Address> LoadAddressList()
        {
            _networkBrowser = new NetworkBrowser();

            var addressList = GetAddressList();
            foreach (var address in addressList)
            {
                if (_pingHelper.AddressList.Contains(address.Name))
                {
                    address.AddToAddresses = true;
                }
            }

            return addressList;
        }

        private ObservableCollection<Address> GetAddressList()
        {
            var collection = new ObservableCollection<Address>();
            _networkBrowser.Value.ForEach(c => collection.Add(new()
                                                              { Name = c.ToLower() }));

            return collection;
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (AddressListBox.IsVisible)
            {
                UpdateAddresses();
            }

            base.OnClosing(e);
        }

        private void UpdateAddresses()
        {
            var addressList = _pingHelper.AddressList;

            foreach (var address in AddressList)
            {
                if (address.AddToAddresses)
                {
                    if (!addressList.Contains(address.Name))
                    {
                        addressList.Add(address.Name);
                    }
                }
                else
                {
                    addressList.Remove(address.Name);
                }
            }

            _applicationSettings.Addresses = string.Join(", ", addressList).ToLower();
        }
    }
}