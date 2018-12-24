using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Controls.Settings.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public double MashCalibTemperature { get; set; }
        public double VesselCalibTemperature { get; set; }
    }
}
