using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        /// <summary>
        /// </summary>
        public ObservableCollection<Address> AddressList { get; set; }

        /// <summary>
        /// </summary>
        public NetworkBrowserDialog()
        {
            _applicationSettings = new ApplicationSettings();
            _pingHelper = new PingHelper(_applicationSettings);
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            _networkBrowser = new NetworkBrowser();
            AddressList = new ObservableCollection<Address>();
            AddressListBox.IsEnabled = false;
            AddressList = GetAddressList();
            DataContext = this;
            AddressListBox.ItemsSource = AddressList;
            AddressListBox.IsEnabled = true;
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
            UpdateAddresses();
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

            foreach(var s in addressList)
            {
                addresses = addresses + $"{s}, ";
            }
            _applicationSettings.Addresses = addresses.TrimStart(',').Trim().TrimEnd(',');
        }
    }
}