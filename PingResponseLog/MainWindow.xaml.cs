using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Threading;

namespace PingResponseLog
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _addressList;
        private DispatcherTimer _dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            Addresses.Text = Properties.Settings.Default.Addresses;
        }

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
            }
        }

        private void PingOnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Addresses = Addresses.Text;
            Properties.Settings.Default.Save();
            SetAddressList();
            SetTimer();
        }
    }
}