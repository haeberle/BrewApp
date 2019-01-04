using BrewApp.Controls.Pasteur.ViewModel;
using BrewApp.Hardware;
using BrewApp.Hardware.BK500;
using BrewApp.Hardware.Interfaces;
using BrewApp.Logic;
using BrewApp.ViewModel;
using Newtonsoft.Json;
using System;
using System.IO;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewApp.Controls.Pasteur
{
    public sealed partial class MainControl : UserControl
    {
        public class Settings
        {
            public double TargetTemperature { get; set; }
            public TimeSpan PasteurTime { get; set; }
        }
        IVessel _vessel = null;
        IBlinker _blinker = null;
        public MainControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += MainControl_DataContextChanged;
            _vessel = ((App)App.Current).Vessel;
            _vessel.VesselEvent += _vessel_VesselEvent;
            _blinker = new Blinker();
        }

        ~MainControl()
        {
            _vessel.VesselEvent -= _vessel_VesselEvent;
            _blinker.EnableBlinker(false);
        }
        private async void _vessel_VesselEvent(object sender, VesselValues vesselValues)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (_viewModel != null)
                {
                    _viewModel.AsIsTemperature = vesselValues.MashCurrentTemperature;
                }
            });
        }

        PasteurViewModel _viewModel;
        private void MainControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var dc = DataContext as BrewingViewModel;

            if (dc != null)
            {
                _viewModel = dc.PasteurViewModel;

                var settings = GetSettings();

                _viewModel.ToBeTime = settings.PasteurTime;
                _viewModel.ToBeTemperature = settings.TargetTemperature;

                //_viewModel.AckButtonBackground = new SolidColorBrush(Colors.Orange);
                //_viewModel.AckButtonForeground = new SolidColorBrush(Colors.DarkGoldenrod);

                //_viewModel.TimerStartStopButtonBackground = new SolidColorBrush(Colors.Green);
                //_viewModel.TimerStartStopButtonForeground = new SolidColorBrush(Colors.Wheat);
                //_viewModel.TimerStartStopButtonText = "Start";

                //_viewModel.VesselStartStopButtonBackground = new SolidColorBrush(Colors.Green);
                //_viewModel.VesselStartStopButtonForeground = new SolidColorBrush(Colors.Wheat);
                //_viewModel.VesselStartStopButtonText = "Start";
            }
        }

        private void tglStartStopVessel(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ProcessIsRunning)
            {
                (_vessel as IStirrer)?.SetStirrer(0);
                (_vessel as IStirrer)?.SetStirrerSpeed(0);
                _vessel.SetTargetTemperature(_viewModel.ToBeTemperature);

                var settings = GetSettings();

                settings.PasteurTime = _viewModel.ToBeTime;
                settings.TargetTemperature = _viewModel.ToBeTemperature;

                SetSettings(settings);

                _vessel.Start();

            }
            else
            {
                _vessel.Stop();
                //_viewModel.VesselStartStopButtonBackground = new SolidColorBrush(Colors.Green);
                //_viewModel.VesselStartStopButtonForeground = new SolidColorBrush(Colors.Wheat);
                //_viewModel.VesselStartStopButtonText = "Start";
            }
        }

        System.Timers.Timer _stepTimer = null; // new System.Timers.Timer();
        DateTime _endTime;

        private void tglStartStopTimer(object sender, RoutedEventArgs e)
        {
            if (_viewModel.TimerStartIsEnabled)
            {
                if (_stepTimer == null)
                {
                    _stepTimer = new System.Timers.Timer(1000);
                    _stepTimer.Elapsed += _stepTimer_Elapsed;
                }
                //_stepTimer.Interval = _viewModel.ToBeTime.TotalMilliseconds;
                _endTime = DateTime.Now + _viewModel.ToBeTime;
                _viewModel.AsIsTime = _endTime - DateTime.Now;
                _stepTimer.Start();
                _viewModel.TimerOver = false;
            }
            else
            {
                if (_stepTimer != null)
                {
                    _stepTimer.Stop();
                    _viewModel.TimerOver = false;
                }
            }
        }

        private async void _stepTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_endTime < DateTime.Now)
            {
                if (_stepTimer != null)
                {
                    _stepTimer.Stop();
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        _viewModel.TimerStartIsEnabled = false;
                        _viewModel.TimerOver = true;
                    });

                    _blinker?.EnableBlinker(true);
                }
            }
            else
            {
                //Console.WriteLine($"{_endTime} {DateTime.Now}");
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _viewModel.AsIsTime = _endTime - DateTime.Now;
                    //Console.WriteLine($"{_viewModel.AsIsTime }");
                });
            }
        }

        bool _blinkToggle = false;
        private void _blinkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_blinkToggle)
            {
                _blinker?.EnableBlinker(false);
                _blinkToggle = false;
            }
            else
            {
                _blinker?.EnableBlinker(true);
                _blinkToggle = true;
            }
        }

        private void ClickbtnTimerOver(object sender, RoutedEventArgs e)
        {
            _viewModel.TimerOver = false;
            _blinker?.EnableBlinker(false);
        }

        const string PATH = "PasteurSettings.cfg";
        Settings GetSettings()
        {
            var path = Path.Combine(Constants.ApplicationSettingsPath, PATH);
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Constants.ApplicationSettingsPath);
                var settings = new Settings();
                var content = JsonConvert.SerializeObject(settings, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
                File.WriteAllText(path, content);
                return settings;
            }
            else
            {
                var content = File.ReadAllText(path);

                //var /*bytes*/ = System.Text.Encoding.Unicode.GetBytes(content);

                var settings = JsonConvert.DeserializeObject<Settings>(content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
                return settings;
            }
        }

        void SetSettings(Settings settings)
        {
            var path = Path.Combine(Constants.ApplicationSettingsPath, PATH);
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Constants.ApplicationSettingsPath);
            }
            string content = string.Empty;
            content = JsonConvert.SerializeObject(settings, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(path, content);

        }

    }
}
