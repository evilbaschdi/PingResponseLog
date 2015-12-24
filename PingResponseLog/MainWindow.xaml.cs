using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using PingResponseLog.Core;

namespace PingResponseLog
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow CurrentHiddenInstance { get; set; }

        private List<string> _addressList;
        private DispatcherTimer _dispatcherTimer;
        private readonly IApplicationStyle _style;
        private readonly IApplicationBasics _basics;
        private string _loggingPath;
        private DateTime _initDateTime;

        public MainWindow()
        {
            _style = new ApplicationStyle(this);
            _basics = new ApplicationBasics();
            InitializeComponent();
            _style.Load();
            ValidateForm();
        }

        private void ValidateForm()
        {
            Addresses.Text = Properties.Settings.Default.Addresses;
            _loggingPath = _basics.GetLoggingPath();
            LoggingPath.Text = _loggingPath;
            _initDateTime = DateTime.Now;
        }

        #region ping

        private void SetTimer()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += PingTimerOnTick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            _dispatcherTimer.Start();
        }

        private void SetAddressList()
        {
            _addressList = new List<string>();

            if(Addresses.Text.Contains(","))
            {
                foreach(var address in Addresses.Text.Split(','))
                {
                    _addressList.Add(address.Trim());
                }
            }
            else
            {
                _addressList.Add(Addresses.Text.Trim());
            }
        }

        private void PingTimerOnTick(object sender, EventArgs e)
        {
            CallPing();
        }

        private async void CallPing()
        {
            foreach(var address in _addressList)
            {
                var reply = await new Ping().SendPingAsync(address);
                var result = $"{DateTime.Now}, {address}, ";
                switch(reply.Status)
                {
                    case IPStatus.Success:
                        result += $"{reply.RoundtripTime} ms";
                        break;

                    case IPStatus.TimedOut:
                        result += $"timeout";
                        break;

                    case IPStatus.DestinationNetworkUnreachable:
                        break;

                    case IPStatus.DestinationHostUnreachable:
                        break;

                    case IPStatus.DestinationProtocolUnreachable:
                        break;

                    case IPStatus.DestinationPortUnreachable:
                        break;

                    case IPStatus.NoResources:
                        break;

                    case IPStatus.BadOption:
                        break;

                    case IPStatus.HardwareError:
                        break;

                    case IPStatus.PacketTooBig:
                        break;

                    case IPStatus.BadRoute:
                        break;

                    case IPStatus.TtlExpired:
                        break;

                    case IPStatus.TtlReassemblyTimeExceeded:
                        break;

                    case IPStatus.ParameterProblem:
                        break;

                    case IPStatus.SourceQuench:
                        break;

                    case IPStatus.BadDestination:
                        break;

                    case IPStatus.DestinationUnreachable:
                        break;

                    case IPStatus.TimeExceeded:
                        break;

                    case IPStatus.BadHeader:
                        break;

                    case IPStatus.UnrecognizedNextHeader:
                        break;

                    case IPStatus.IcmpError:
                        break;

                    case IPStatus.DestinationScopeMismatch:
                        break;

                    case IPStatus.Unknown:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                result += Environment.NewLine;
                Result.Text += result;
                File.AppendAllText($@"{_loggingPath}\PingResponseLog_{_initDateTime.ToString("yyyy-MM-dd_HHmm")}.txt", result);
            }
        }

        private void PingOnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Addresses = Addresses.Text;
            Properties.Settings.Default.Save();
            SetAddressList();
            SetTimer();
        }

        #endregion ping

        #region Flyout

        private void ToggleSettingsFlyoutClick(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void ToggleFlyout(int index, bool stayOpen = false)
        {
            var activeFlyout = (Flyout) Flyouts.Items[index];
            if(activeFlyout == null)
            {
                return;
            }

            foreach(
                var nonactiveFlyout in
                    Flyouts.Items.Cast<Flyout>()
                        .Where(nonactiveFlyout => nonactiveFlyout.IsOpen && nonactiveFlyout.Name != activeFlyout.Name))
            {
                nonactiveFlyout.IsOpen = false;
            }

            activeFlyout.IsOpen = activeFlyout.IsOpen && stayOpen || !activeFlyout.IsOpen;
        }

        #endregion Flyout

        #region Style

        private void SaveStyleClick(object sender, RoutedEventArgs e)
        {
            _style.SaveStyle();
        }

        private void Theme(object sender, RoutedEventArgs e)
        {
            _style.SetTheme(sender, e);
        }

        private void AccentOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _style.SetAccent(sender, e);
        }

        #endregion Style

        #region Logging

        private void BrowseLoggingPathClick(object sender, RoutedEventArgs e)
        {
            _basics.BrowseLoggingFolder();
            LoggingPath.Text = Properties.Settings.Default.LoggingPath;
            _loggingPath = Properties.Settings.Default.LoggingPath;
            ValidateForm();
        }

        private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if(Directory.Exists(LoggingPath.Text))
            {
                Properties.Settings.Default.LoggingPath = LoggingPath.Text;
                Properties.Settings.Default.Save();
                _loggingPath = Properties.Settings.Default.LoggingPath;
            }
            ValidateForm();
        }

        #endregion Logging
    }
}