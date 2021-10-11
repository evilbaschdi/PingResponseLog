using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using EvilBaschdi.Core.Logging;
using EvilBaschdi.CoreExtended;
using EvilBaschdi.CoreExtended.AppHelpers;
using EvilBaschdi.CoreExtended.Browsers;
using EvilBaschdi.CoreExtended.Controls.About;
using MahApps.Metro.Controls;
using PingResponseLog.Internal;
using PingResponseLog.Internal.Core;
using PingResponseLog.Internal.Models;
using PingResponseLog.Internal.ViewModels;

namespace PingResponseLog
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private DispatcherTimer _dispatcherTimer;
        private bool _dispatcherTimerRunning; // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly IApplicationStyle _applicationStyle;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IAppSettingsBase _appSettingsBase;
        private readonly IPingHelper _pingHelper;
        private readonly ILoggingHelper _loggingHelper;
        private readonly IPingProcessor _pingProcessor;

        private ObservableCollection<PingLogEntry> _pingLogEntries;

        private int _overrideProtection;
        private int _timeSpanHours;
        private int _timeSpanMinutes;
        private int _timeSpanSeconds;
        private readonly IRoundCorners _roundCorners;

        /// <summary>
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _appSettingsBase = new AppSettingsBase(Properties.Settings.Default);
            _applicationSettings = new ApplicationSettings(_appSettingsBase);
            _loggingHelper = new LoggingHelper(_applicationSettings);

            _roundCorners = new RoundCorners();
            _applicationStyle = new ApplicationStyle(_roundCorners, true);
            _applicationStyle.Run();
            _pingHelper = new PingHelper(_applicationSettings);
            IAppendAllTextWithHeadline appendAllTextWithHeadline = new AppendAllTextWithHeadline();
            _pingProcessor = new PingProcessor(_pingHelper, _loggingHelper, _applicationSettings, appendAllTextWithHeadline);
            _pingLogEntries = new();
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

            InterNetworkSwitch.IsOn = _applicationSettings.InterNetworkType switch
            {
                "V4" => false,
                "V6" => true,
                _ => InterNetworkSwitch.IsOn
            };

            _overrideProtection = 1;
        }

        #region Ping

        private void SetTimer(TimeSpan diff)
        {
            _dispatcherTimer = new();
            _dispatcherTimer.Tick += PingTimerOnTick;
            _timeSpanHours = _applicationSettings.TimeSpanHours;
            _timeSpanMinutes = _applicationSettings.TimeSpanMinutes;
            _timeSpanSeconds = _applicationSettings.TimeSpanSeconds;
            var configTimeSpan = new TimeSpan(_timeSpanHours, _timeSpanMinutes, _timeSpanSeconds);
            _dispatcherTimer.Interval = configTimeSpan - diff;
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
            var activeFlyout = (Flyout)Flyouts.Items[index];
            if (activeFlyout == null)
            {
                return;
            }

            foreach (var nonactiveFlyout in Flyouts.Items.Cast<Flyout>().Where(nonactiveFlyout => nonactiveFlyout.IsOpen && nonactiveFlyout.Name != activeFlyout.Name))
            {
                nonactiveFlyout.IsOpen = false;
            }

            activeFlyout.IsOpen = activeFlyout.IsOpen && stayOpen || !activeFlyout.IsOpen;
        }

        #endregion Flyout

        #region LoggingHelper

        private void BrowseLoggingPathClick(object sender, RoutedEventArgs e)
        {
            var browser = new ExplorerFolderBrowser
                          {
                              SelectedPath = _applicationSettings.LoggingPath
                          };
            browser.ShowDialog();
            _applicationSettings.LoggingPath = browser.SelectedPath;
            Load();
        }

        private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(LoggingPath.Text))
            {
                return;
            }

            _applicationSettings.LoggingPath = LoggingPath.Text;
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
            if (_dispatcherTimer != null && _dispatcherTimerRunning)
            {
                _dispatcherTimer.Stop();
                _dispatcherTimerRunning = false;
                PingButtonTextBlock.Text = "ping";
            }
            else
            {
                var start = DateTime.Now;
                var pingProcessorValue = _pingProcessor.Value;
                _pingLogEntries = pingProcessorValue.PingLogEntries;
                ResultGrid.ItemsSource = _pingLogEntries;
                var end = DateTime.Now;
                var diff = end - start;
                SetTimer(diff);
                PingButtonTextBlock.Text = "stop";
            }
        }

        private void AddressesOnLostFocus(object sender, RoutedEventArgs e)
        {
            _applicationSettings.Addresses = Addresses.Text.TrimEnd(',').ToLower();
        }

        private void PingTimerOnTick(object sender, EventArgs e)
        {
            var pingProcessorValue = _pingProcessor.Value;

            foreach (var pingLogEntry in pingProcessorValue.PingLogEntries)
            {
                _pingLogEntries.Add(pingLogEntry);
            }

            ResultGrid.ItemsSource = _pingLogEntries;


            if (ResultGrid.Items.Count <= 0)
            {
                return;
            }

            var border = VisualTreeHelper.GetChild(ResultGrid, 0) as Decorator;
            var scroll = border?.Child as ScrollViewer;
            scroll?.ScrollToEnd();
        }

        private void InterNetwork(object sender, RoutedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }

            var toggleSwitch = (ToggleSwitch)sender;
            _applicationSettings.InterNetworkType = toggleSwitch.IsOn ? "V6" : "V4";
        }

        private void BrowseNetworkOnClick(object sender, RoutedEventArgs e)
        {
            var networkBrowserDialog = new NetworkBrowserDialog();
            {
                DataContext = new NetworkBrowserDialogViewModel(_roundCorners);
            }

            networkBrowserDialog.Closing += NetworkBrowserDialogClosing;
            networkBrowserDialog.ShowDialog();
        }

        private void NetworkBrowserDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Addresses.Text = _applicationSettings.Addresses;
        }

        private void LoggingFileIntervalOnDropDownClosed(object sender, EventArgs e)
        {
            _applicationSettings.LoggingFileInterval = LoggingFileInterval.Text;
        }

        private void AboutWindowClick(object sender, RoutedEventArgs e)
        {
            var assembly = typeof(MainWindow).Assembly;
            IAboutContent aboutWindowContent = new AboutContent(assembly, $@"{AppDomain.CurrentDomain.BaseDirectory}\b.png");

            var aboutWindow = new AboutWindow
                              {
                                  DataContext = new AboutViewModel(aboutWindowContent, _roundCorners)
                              };

            aboutWindow.ShowDialog();
        }

        #endregion Events
    }
}