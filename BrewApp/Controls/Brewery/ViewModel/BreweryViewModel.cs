using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace BrewApp.Controls.Brewery.ViewModel
{
    public class BreweryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //public double VesselTemperature { get; set; }
        public double StepTemperature { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public TimeSpan ExpectedFinish { get; set; }
        public TimeSpan StepRestDuration { get; set; }
        public TimeSpan StepCountDown { get; set; }

        public bool RecipeLoaded { get; set; }
        public bool ProcessIsRunning { get; set; }
        public bool ProcessBlocked { get; set; }
        public bool ProcessLastStep { get; set; }

        public bool CanStartProcess => RecipeLoaded && !ProcessIsRunning && !ProcessBlocked;
        public bool CanStopProcess => ProcessIsRunning;
        public bool CanNextStep => ProcessIsRunning && !ProcessLastStep;
        public bool CanLoadRecipe => !ProcessIsRunning;

        public Brush NextStepBackground { get; set; } = new SolidColorBrush(Colors.Orange);

        public BrewKettleViewModel BrewKettleViewModel { get; set; } = new BrewKettleViewModel();
        public InfoBoxViewModel InfoBoxViewModel { get; set; } = new InfoBoxViewModel();
        //public string StepText { get; set; }
        //public int StepNumber { get; set; }
        //public int TotalSteps { get; set; }
        //public string ReciepeName { get; set; }
        //public string BrewerName { get; set; }
        //public bool Heater1On { get; set; }
        //public bool Heater2On { get; set; }
        //public bool PumpOn { get; set; } 
        //public bool StirrerLeftOn { get; set; }
        //public bool StirrerRightOn { get; set; }
        //public bool StirrerSpeed { get; set; }

    }
}
