using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using EvilBaschdi.Core.Application;
using EvilBaschdi.Core.Wpf;
using MahApps.Metro.Controls;
using PingResponseLog.Core;
using PingResponseLog.Internal;

namespace PingResponseLog
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// </summary>
        public MainWindow CurrentHiddenInstance { get; set; }

        private DispatcherTimer _dispatcherTimer;
        private bool _dispatcherTimerRunning;

        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly IMetroStyle _style;
        private readonly IApplicationSettings _applicationSettings;
        private readonly ISettings _coreSettings;
        private readonly IApplicationBasics _basics;
        private readonly IPingHelper _pingHelper;
        private readonly ILoggingHelper _loggingHelper;
        private readonly IPingProcessor _pingProcessor;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
        private int _overrideProtection;
        private int _timeSpanHours;
        private int _timeSpanMinutes;
        private int _timeSpanSeconds;

        /// <summary>
        /// </summary>
        public MainWindow()
        {
            _applicationSettings = new ApplicationSettings();
            _coreSettings = new CoreSettings();
            _basics = new ApplicationBasics(_applicationSettings);
            _loggingHelper = new LoggingHelper(_applicationSettings);
            InitializeComponent();
            _style = new MetroStyle(this, Accent, Dark, Light, _coreSettings);
            _style.Load(true, false);
            _pingHelper = new PingHelper(_applicationSettings);
            _pingProcessor = new PingProcessor(_pingHelper, _loggingHelper, _applicationSettings);
            Load();
        }

        private void Load()
        {
            Addresses.Text = _applicationSettings.Addresses;
            LoggingPath.Text = _applicationSettings.LoggingPath;
            TimeSpanHours.Value = _applicationSettings.TimeSpanHours;
            TimeSpanMinutes.Value = _applicationSettings.TimeSpanMinutes;
            TimeSpanSeconds.Value = _applicationSettings.TimeSpanSeconds;
            LoggingFileInterval.Text = _applicationSettings.LoggingFileInterval;

            switch(_applicationSettings.InterNetworkType)
            {
                case "V4":
                    V4.IsChecked = true;
                    V6.IsChecked = false;
                    break;

                case "V6":
                    V4.IsChecked = false;
                    V6.IsChecked = true;
                    break;
            }

            _overrideProtection = 1;
        }

        #region Ping

        private void SetTimer()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += PingTimerOnTick;
            _timeSpanHours = _applicationSettings.TimeSpanHours;
            _timeSpanMinutes = _applicationSettings.TimeSpanMinutes;
            _timeSpanSeconds = _applicationSettings.TimeSpanSeconds;
            _dispatcherTimer.Interval = new TimeSpan(_timeSpanHours, _timeSpanMinutes, _timeSpanSeconds);
            _dispatcherTimerRunning = true;
            _dispatcherTimer.Start();
        }

        #endregion Ping

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

        #region MetroStyle

        private void SaveStyleClick(object sender, RoutedEventArgs e)
        {
            if(_overrideProtection == 0)
            {
                return;
            }
            _style.SaveStyle();
        }

        private void Theme(object sender, RoutedEventArgs e)
        {
            if(_overrideProtection == 0)
            {
                return;
            }
            _style.SetTheme(sender, e);
        }

        private void AccentOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_overrideProtection == 0)
            {
                return;
            }
            _style.SetAccent(sender, e);
        }

        #endregion MetroStyle

        #region LoggingHelper

        private void BrowseLoggingPathClick(object sender, RoutedEventArgs e)
        {
            _basics.BrowseLoggingFolder();
            LoggingPath.Text = _applicationSettings.LoggingPath;
            Load();
        }

        private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if(Directory.Exists(LoggingPath.Text))
            {
                _applicationSettings.LoggingPath = LoggingPath.Text;
            }
            Load();
        }

        #endregion LoggingHelper

        #region Events

        private void TimeSpanHoursOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            _applicationSettings.TimeSpanHours = Convert.ToInt32(TimeSpanHours.Value);
        }

        private void TimeSpanMinutesOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            _applicationSettings.TimeSpanMinutes = Convert.ToInt32(TimeSpanMinutes.Value);
        }

        private void TimeSpanSecondsOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            _applicationSettings.TimeSpanSeconds = Convert.ToInt32(TimeSpanSeconds.Value);
        }

        private void PingOnClick(object sender, RoutedEventArgs e)
        {
            if(_dispatcherTimer != null && _dispatcherTimerRunning)
            {
                _dispatcherTimer.Stop();
                _dispatcherTimerRunning = false;
                PingButtonTextBlock.Text = "ping";
            }
            else
            {
                SetTimer();
                Result.Text += _pingProcessor.CallPing;
                PingButtonTextBlock.Text = "stop";
            }
        }

        private void AddressesOnLostFocus(object sender, RoutedEventArgs e)
        {
            _applicationSettings.Addresses = Addresses.Text.TrimEnd(',');
        }

        private void PingTimerOnTick(object sender, EventArgs e)
        {
            Result.Text += _pingProcessor.CallPing;
        }

        private void InterNetwork(object sender, RoutedEventArgs e)
        {
            if(_overrideProtection == 0)
            {
                return;
            }
            var radiobutton = (RadioButton) sender;
            _applicationSettings.InterNetworkType = radiobutton.Name;
        }

        private void BrowseNetworkOnClick(object sender, RoutedEventArgs e)
        {
            var networkBrowserDialog = new NetworkBrowserDialog();

            networkBrowserDialog.Closing += NetworkBrowserDialogClosing;
            networkBrowserDialog.Show();
        }

        private void NetworkBrowserDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Addresses.Text = _applicationSettings.Addresses;
        }

        private void LoggingFileIntervalOnDropDownClosed(object sender, EventArgs e)
        {
            _applicationSettings.LoggingFileInterval = LoggingFileInterval.Text;
        }

        #endregion Events
    }
}