using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using EvilBaschdi.Core.Browsers;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PingResponseLog.Core;
using PingResponseLog.Internal;
using PingResponseLog.Models;

namespace PingResponseLog
{
    /// <summary>
    ///     Interaction logic for NetworkBrowserDialog.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class NetworkBrowserDialog : MetroWindow
    {
        private INetworkBrowser _networkBrowser;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IPingHelper _pingHelper;
        private bool _windowShown;
        private ProgressDialogController _controller;
        private Task<ObservableCollection<Address>> _task;

        /// <summary>
        /// </summary>
        public ObservableCollection<Address> AddressList { get; set; }

        /// <summary>
        /// </summary>
        public NetworkBrowserDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _applicationSettings = new ApplicationSettings();
            _pingHelper = new PingHelper(_applicationSettings);
            InitializeComponent();
        }


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
            var networkComputers = _networkBrowser.GetNetworkComputers;
            var collection = new ObservableCollection<Address>();

            foreach (string computer in networkComputers)
            {
                collection.Add(new Address
                               {
                                   Name = computer.ToLower()
                               });
            }

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