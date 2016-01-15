using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using EvilBaschdi.Core.Browsers;
using MahApps.Metro.Controls;
using PingResponseLog.Core;
using PingResponseLog.Internal;

namespace PingResponseLog
{
    /// <summary>
    ///     Interaction logic for NetworkBrowserDialog.xaml
    /// </summary>
    public partial class NetworkBrowserDialog : MetroWindow
    {
        private INetworkBrowser _networkBrowser;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IPingHelper _pingHelper;
        private bool _windowShown;
        private readonly BackgroundWorker _bw;

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
            _bw = new BackgroundWorker();
        }


        /// <summary>
        ///     Executing code when window is shown.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if(_windowShown)
            {
                return;
            }

            _windowShown = true;

            _bw.DoWork += (o, args) => LoadAddressList();
            _bw.WorkerReportsProgress = true;
            _bw.RunWorkerCompleted += SetAddressListBox;
            _bw.RunWorkerAsync();
        }


        private void SetAddressListBox(object sender, RunWorkerCompletedEventArgs e)
        {
            AddressListBox.ItemsSource = AddressList;
            AddressListBox.Visibility = Visibility.Visible;
            Loading.Visibility = Visibility.Hidden;
        }

        private void LoadAddressList()
        {
            _networkBrowser = new NetworkBrowser();
            AddressList = GetAddressList();
        }

        private ObservableCollection<Address> GetAddressList()
        {
            var networkComputers = _networkBrowser.GetNetworkComputers;
            var collection = new ObservableCollection<Address>();

            foreach(string computer in networkComputers)
            {
                collection.Add(new Address
                {
                    Name = computer
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
            if(AddressListBox.IsVisible)
            {
                UpdateAddresses();
            }
            base.OnClosing(e);
        }

        private void UpdateAddresses()
        {
            var checkedItems = AddressList.Where(attribute => attribute.AddToAddresses);
            var addressList = _pingHelper.AddressList;
            var addresses = string.Empty;

            foreach(var checkedItem in checkedItems.Where(checkedItem => !addressList.Contains(checkedItem.Name)))
            {
                addressList.Add(checkedItem.Name);
            }

            addresses = addressList.Aggregate(addresses, (current, s) => current + $"{s}, ");
            _applicationSettings.Addresses = addresses.TrimStart(',').Trim().TrimEnd(',');
        }
    }
}