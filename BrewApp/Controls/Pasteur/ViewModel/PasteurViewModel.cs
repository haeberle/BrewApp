using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace BrewApp.Controls.Pasteur.ViewModel
{
    public class PasteurViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //public Brush AckButtonBackground { get; set; }
        //public Brush AckButtonForeground { get; set; }
        public bool ProcessIsRunning { get; set; }
        public bool CanStartProcess => !ProcessIsRunning && !ProcessBlocked || ProcessIsRunning;
        public bool ProcessBlocked { get; set; }
        public double AsIsTemperature { get; set; }
        public double ToBeTemperature { get; set; }
        //public string VesselStartStopButtonText { get; set; }
        //public Brush VesselStartStopButtonBackground { get; set; }
        //public Brush VesselStartStopButtonForeground { get; set; }
        public TimeSpan AsIsTime { get; set; }
        public TimeSpan ToBeTime { get;set;}
        public bool TimerStartIsEnabled { get; set; }
        public bool TimerOver { get; set; }
        //public string TimerStartStopButtonText { get; set; }
        //public Brush TimerStartStopButtonBackground { get; set; }
        //public Brush TimerStartStopButtonForeground{ get; set; }
    }
}
