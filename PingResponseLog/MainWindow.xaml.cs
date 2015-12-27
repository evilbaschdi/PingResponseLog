using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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
        public MainWindow CurrentHiddenInstance { get; set; }

        private DispatcherTimer _dispatcherTimer;
        private readonly IApplicationStyle _style;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IApplicationBasics _basics;
        private readonly IPingHelper _pingHelper;
        private readonly IPingProcessor _pingProcessor;
        private int _overrideProtection;
        private int _timeSpanHours;
        private int _timeSpanMinutes;
        private int _timeSpanSeconds;

        public MainWindow()
        {
            _style = new ApplicationStyle(this);
            _applicationSettings = new ApplicationSettings();
            _basics = new ApplicationBasics(_applicationSettings);
            InitializeComponent();
            _style.Load();
            _pingHelper = new PingHelper(_applicationSettings);
            _pingProcessor = new PingProcessor(_pingHelper, _applicationSettings);
            Load();
        }

        private void Load()
        {
            Addresses.Text = _applicationSettings.Addresses;
            LoggingPath.Text = _applicationSettings.LoggingPath;
            TimeSpanHours.Value = _applicationSettings.TimeSpanHours;
            TimeSpanMinutes.Value = _applicationSettings.TimeSpanMinutes;
            TimeSpanSeconds.Value = _applicationSettings.TimeSpanSeconds;

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

        #endregion Logging

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
            _applicationSettings.Addresses = Addresses.Text;
            SetTimer();
            Result.Text += _pingProcessor.CallPing;
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

        #endregion Events
    }
}