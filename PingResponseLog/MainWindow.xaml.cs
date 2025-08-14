using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using EvilBaschdi.About.Core;
using EvilBaschdi.About.Core.Models;
using EvilBaschdi.About.Wpf;
using EvilBaschdi.Core;
using EvilBaschdi.Core.Logging;
using EvilBaschdi.Core.Settings.ByMachineAndUser;
using EvilBaschdi.Core.Wpf;
using EvilBaschdi.Core.Wpf.Browsers;
using EvilBaschdi.Core.Wpf.FlyOut;
using MahApps.Metro.Controls;
using PingResponseLog.Internal;
using PingResponseLog.Internal.Core;
using PingResponseLog.Internal.Models;
using PingResponseLog.Internal.ViewModels;

namespace PingResponseLog;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private DispatcherTimer _dispatcherTimer;
    private bool _dispatcherTimerRunning; // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    private readonly IApplicationLayout _applicationLayout;
    private readonly IApplicationStyle _applicationStyle;
    private readonly IApplicationSettings _applicationSettings;

    private readonly IPingHelper _pingHelper;
    private readonly ILoggingHelper _loggingHelper;
    private readonly IPingProcessor _pingProcessor;

    private ObservableCollection<PingLogEntry> _pingLogEntries;

    private int _overrideProtection;
    private int _timeSpanHours;
    private int _timeSpanMinutes;
    private int _timeSpanSeconds;

    private readonly IToggleFlyOut _toggleFlyOut;
    private readonly ICurrentFlyOuts _currentFlyOuts;
    private readonly IAppSettingByKey _appSettingByKey;

    /// <summary>
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        IAppSettingsFromJsonFile appSettingsFromJsonFile = new AppSettingsFromJsonFile();
        IAppSettingsFromJsonFileByMachineAndUser appSettingsFromJsonFileByMachineAndUser = new AppSettingsFromJsonFileByMachineAndUser();
        _appSettingByKey = new AppSettingByKey(appSettingsFromJsonFile, appSettingsFromJsonFileByMachineAndUser);
        _applicationSettings = new ApplicationSettings(_appSettingByKey);
        _loggingHelper = new LoggingHelper(_applicationSettings);

        _applicationStyle = new ApplicationStyle();
        _applicationLayout = new ApplicationLayout();
        _applicationStyle.Run();
        _applicationLayout.RunFor((true, false));
        _pingHelper = new PingHelper(_applicationSettings);
        IAppendAllTextWithHeadline appendAllTextWithHeadline = new AppendAllTextWithHeadline();
        _pingProcessor = new PingProcessor(_pingHelper, _loggingHelper, _applicationSettings, appendAllTextWithHeadline);
        _pingLogEntries = new();
        _currentFlyOuts = new CurrentFlyOuts();
        _toggleFlyOut = new ToggleFlyOut();
        Load();
    }

    private void Load()
    {
        Addresses.SetCurrentValue(TextBox.TextProperty, _applicationSettings.Addresses);
        LoggingPath.SetCurrentValue(TextBox.TextProperty, _applicationSettings.LoggingPath);
        TimeSpanHours.SetCurrentValue(NumericUpDown.ValueProperty, _applicationSettings.TimeSpanHours);
        TimeSpanMinutes.SetCurrentValue(NumericUpDown.ValueProperty, _applicationSettings.TimeSpanMinutes);
        TimeSpanSeconds.SetCurrentValue(NumericUpDown.ValueProperty, _applicationSettings.TimeSpanSeconds);
        LoggingFileInterval.SetCurrentValue(ComboBox.TextProperty, _applicationSettings.LoggingFileInterval);

        InterNetworkSwitch.SetCurrentValue(ToggleSwitch.IsOnProperty, _applicationSettings.InterNetworkType switch
        {
            "V4" => false,
            "V6" => true,
            _ => InterNetworkSwitch.IsOn
        });

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
        var currentFlyOutsModel = _currentFlyOuts.ValueFor(Flyouts, 0);
        _toggleFlyOut.RunFor(currentFlyOutsModel);
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
            PingButtonTextBlock.SetCurrentValue(TextBlock.TextProperty, "ping");
        }
        else
        {
            var start = DateTime.Now;
            var pingProcessorValue = _pingProcessor.Value;
            _pingLogEntries = pingProcessorValue.PingLogEntries;
            ResultGrid.SetCurrentValue(ItemsControl.ItemsSourceProperty, _pingLogEntries);
            var end = DateTime.Now;
            var diff = end - start;
            SetTimer(diff);
            PingButtonTextBlock.SetCurrentValue(TextBlock.TextProperty, "stop");
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

        ResultGrid.SetCurrentValue(ItemsControl.ItemsSourceProperty, _pingLogEntries);

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
            DataContext = new NetworkBrowserDialogViewModel(_applicationLayout, _applicationStyle);
        }

        networkBrowserDialog.Closing += NetworkBrowserDialogClosing;
        networkBrowserDialog.ShowDialog();
    }

    private void NetworkBrowserDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Addresses.SetCurrentValue(TextBox.TextProperty, _applicationSettings.Addresses);
    }

    private void LoggingFileIntervalOnDropDownClosed(object sender, EventArgs e)
    {
        _applicationSettings.LoggingFileInterval = LoggingFileInterval.Text;
    }

    private void AboutWindowClick(object sender, RoutedEventArgs e)
    {
        ICurrentAssembly currentAssembly = new CurrentAssembly();
        IAboutContent aboutContent = new AboutContent(currentAssembly);
        IAboutViewModel aboutModel = new AboutViewModel(aboutContent);
        IApplyMicaBrush applyMicaBrush = new ApplyMicaBrush();
        IApplicationLayout applicationLayout = new ApplicationLayout();
        var aboutWindow = new AboutWindow(aboutModel, applicationLayout, applyMicaBrush);

        aboutWindow.ShowDialog();
    }

    #endregion Events
}