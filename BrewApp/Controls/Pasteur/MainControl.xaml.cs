using BrewApp.Controls.Pasteur.ViewModel;
using BrewApp.Hardware;
using BrewApp.Hardware.Interfaces;
using BrewApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewApp.Controls.Pasteur
{
    public sealed partial class MainControl : UserControl
    {
        Vessel _vessel = null;
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
            _blinker.EnbableBlinker(false);
        }
        private async void _vessel_VesselEvent(object sender, VesselValues vesselValues)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _viewModel.AsIsTemperature = vesselValues.MashCurrentTemperature;
            });
        }

        PasteurViewModel _viewModel;
        private void MainControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var dc = DataContext as BrewingViewModel;

            if (dc != null)
            {
                _viewModel = dc.PasteurViewModel;

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
                _vessel.SetStirrer(0);
                _vessel.SetStirrerSpeed(0);
                _vessel.SetTargetTemperature(_viewModel.ToBeTemperature);
                _vessel.Start();
                //_viewModel.VesselStartStopButtonBackground = new SolidColorBrush(Colors.Red);
                //_viewModel.VesselStartStopButtonForeground = new SolidColorBrush(Colors.Wheat);
                //_viewModel.VesselStartStopButtonText = "Stop";
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

                    _blinker?.EnbableBlinker(true);
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
                _blinker?.EnbableBlinker(false);
                _blinkToggle = false;
            }
            else
            {
                _blinker?.EnbableBlinker(true);
                _blinkToggle = true;
            }
        }

        private void ClickbtnTimerOver(object sender, RoutedEventArgs e)
        {
            _viewModel.TimerOver = false;
            _blinker?.EnbableBlinker(false); 
        }
    }
}
