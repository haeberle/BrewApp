using BrewApp.Controller;
using BrewApp.Controls.Brewery.Dialog;
using BrewApp.Controls.Brewery.Dialog.ViewModel;
using BrewApp.Controls.Brewery.ViewModel;
using BrewApp.Hardware;
using BrewApp.Logic.Recipes;
using BrewApp.ViewModel;
using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brewer = BrewApp.Logic.Brewer;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewApp.Controls.Brewery
{
    public sealed partial class MainControl : UserControl
    {
        BreweryViewModel _mainViewModel = new BreweryViewModel();

        MainController _mainController = new MainController();

        //Vessel _vessel = new Vessel();
        Recipe _recipe = null;

        
        public MainControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += MainControl_DataContextChanged;


            //_mainViewModel.BrewKettleViewModel.MashTemperature = 134.0;
            //_mainViewModel.BrewKettleViewModel.VesselTemperature = 23.34;

            //DataContext = _mainViewModel;

            //timertest.Time = new TimeSpan(3, 12, 2);
            //timertest1.Time = new TimeSpan(21, 46, 58);
            //_mainViewModel.StepTemperature = 0.0;
            //_mainViewModel.BrewKettleViewModel.
            //tempTest2.Temperature = 98.6;

            //_mainViewModel.InfoBoxViewModel.BrewerName = "-";
            //_mainViewModel.InfoBoxViewModel.RecipeName = "Zwickel";
            //_mainViewModel.InfoBoxViewModel.StepName = "MaltoseRast";
            //_mainViewModel.InfoBoxViewModel.StepNumber = 0;
            //_mainViewModel.InfoBoxViewModel.TotalSteps = 0;

            _mainController.StepEvent += _mainController_StepEvent;
            _mainController.VesselEvent += _mainController_VesselEvent;
            _mainController.ProcessStopEvent += _mainController_ProcessStopEvent;

            //var _dispatcherTimer = new DispatcherTimer();
            //_dispatcherTimer.Tick += _dispatcherTimer_Tick; ;
            //_dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            //_dispatcherTimer.Start();
        }

        private void MainControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var dc = DataContext as BrewingViewModel;

            if (dc != null)
            {
                _mainViewModel = dc.BreweryViewModel;

                _mainViewModel.StepTemperature = 0.0;
                //_mainViewModel.BrewKettleViewModel.
                //tempTest2.Temperature = 98.6;

                _mainViewModel.InfoBoxViewModel.BrewerName = "-";
                //_mainViewModel.InfoBoxViewModel.RecipeName = "Zwickel";
                //_mainViewModel.InfoBoxViewModel.StepName = "MaltoseRast";
                _mainViewModel.InfoBoxViewModel.StepNumber = 0;
                _mainViewModel.InfoBoxViewModel.TotalSteps = 0;
            }

        }

        private async void _mainController_ProcessStopEvent(object sender, int stepNumber)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                _mainController.Stop();
                _mainViewModel.ProcessIsRunning = false;

                var dialog = new ProgramEnd();

                await dialog.ShowAsync();
                //if (result == ContentDialogResult.Primary)
                //{
                //    //_vessel.Stop();
                //    _mainViewModel.ProcessIsRunning = false;
                //    _mainController.Stop();
                //}
            });
        }

        private async void _mainController_VesselEvent(object sender, VesselValues vesselValues)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _mainViewModel.BrewKettleViewModel.EmemergencyStop = vesselValues.EmergencyOn;
                _mainViewModel.BrewKettleViewModel.Heater1On = vesselValues.HeaterLevel1On;
                _mainViewModel.BrewKettleViewModel.Heater2On = vesselValues.HeaterLevel2On;
                _mainViewModel.BrewKettleViewModel.MashTemperature = vesselValues.MashCurrentTemperature;
                _mainViewModel.BrewKettleViewModel.PumpOn = vesselValues.PumpOn;
                _mainViewModel.BrewKettleViewModel.StirrerLeftOn = vesselValues.StirrerDirection == StirrerDirection.Left;
                _mainViewModel.BrewKettleViewModel.StirrerRightOn = vesselValues.StirrerDirection == StirrerDirection.Right;
                _mainViewModel.BrewKettleViewModel.StirrerSpeed = vesselValues.StirrerSpeed;
                _mainViewModel.BrewKettleViewModel.VesselTemperature = vesselValues.VesselTemperature;
                _mainViewModel.StepTemperature = vesselValues.MashTargetTemperature;
            });
        }

        private async void _mainController_StepEvent(object sender, StepEvents stepEvents)
        {
            //_mainController_VesselEvent(sender, stepEvents.VesselValues);

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _mainViewModel.ExpectedFinish = new TimeSpan(stepEvents.EndTime.Hour, stepEvents.EndTime.Minute, stepEvents.EndTime.Second);
                _mainViewModel.StepCountDown = stepEvents.TimeLeft;
                _mainViewModel.InfoBoxViewModel.StepName = stepEvents.StepName;
                _mainViewModel.InfoBoxViewModel.StepNumber = stepEvents.StepNo;

                _mainViewModel.ProcessLastStep = stepEvents.StepNo == _mainController.StepCount;
                _mainViewModel.NextStepBackground = stepEvents.StepType == StepType.HoldMaunally ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Orange);
            });
        }

        //private void _dispatcherTimer_Tick(object sender, object e)
        //{
        //    //_mainViewModel.BrewKettleViewModel.MashTemperature = _vessel.GetMashTemperature();
        //    //_mainViewModel.BrewKettleViewModel.VesselTemperature = _vessel.GetVesselTemperature();
        //}

        //string _logfilePath;
        private async void btnStartClick(object sender, RoutedEventArgs e)
        {
            var vm = new BrewerViewModel();
            vm.Brewers = Brewer.Loader.Load();
            vm.SelectedBrewer = vm.Brewers[0];

            var dialog = new StartProgram();
            dialog.DataContext = vm;

            await dialog.ShowAsync();

            // Show Dialog
            
            if (dialog.BrewerSelected)
            {
                _mainController.Start(vm.SelectedBrewer.Name);
                _mainViewModel.InfoBoxViewModel.BrewerName = vm.SelectedBrewer.Name;
                _mainViewModel.ExpectedFinish = new TimeSpan(0, 0, 0);
                _mainViewModel.StepCountDown = new TimeSpan(0, 0, 0);
                _mainViewModel.StepTemperature = 0.0;
                _mainViewModel.ProcessIsRunning = true;

                //_logfilePath = "";
            }
        }

        private async void btnStopProgramm(object sender, RoutedEventArgs e)
        {
            var dialog = new StopProgram();

            // Show Dialog
            await dialog.ShowAsync();
            if (dialog.Stop)
            {
                //_vessel.Stop();
                _mainViewModel.ProcessIsRunning = false;
                _mainController.Stop();
            }
        }

        private async void btnLoadProgram(object sender, RoutedEventArgs e)
        {

            var recipes = Loader.LoadRecipes();
            var vm = new LoadRecipeViewModel()
            {
                Recipes = recipes
            };
            vm.SelectedRecipe = recipes[0];

            var dialog = new LoadRecipe();
            dialog.DataContext = vm;

            await dialog.ShowAsync();

            if (dialog.RecipeSelected)
            {
                //_vessel.Stop();
                // set recipe in controller;
                _recipe = vm.SelectedRecipe;
                _mainViewModel.InfoBoxViewModel.RecipeName = _recipe.Name;
                _mainViewModel.InfoBoxViewModel.StepName = _recipe.Steps.Count > 0 ? _recipe.Steps[0].Name : "";
                _mainViewModel.InfoBoxViewModel.StepNumber = _recipe.Steps.Count > 0 ? 1 : 0;
                _mainViewModel.InfoBoxViewModel.TotalSteps = _recipe.Steps.Count;
                _mainController.LoadRecipe(_recipe);
                _mainViewModel.RecipeLoaded = true;
            }
        }

        private async void btnNextStep(object sender, RoutedEventArgs e)
        {
            var dialog = new NextStap();
            // Show Dialog
            await dialog.ShowAsync();
            if (dialog.NextStep)
            {
                _mainController.NextStep();
                //_vessel.Stop();
                //_mainViewModel.ProcessIsRunning = false;
            }
        }
    }
}
