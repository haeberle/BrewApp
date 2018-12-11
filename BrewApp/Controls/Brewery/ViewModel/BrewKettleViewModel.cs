using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Controls.Brewery.ViewModel
{
    public class BrewKettleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public double VesselTemperature { get; set; }
        public double MashTemperature { get; set; }
        public bool Heater1On { get; set; }
        public bool Heater2On { get; set; }
        public bool PumpOn { get; set; }
        public bool StirrerLeftOn { get; set; }
        public bool StirrerRightOn { get; set; }
        public int StirrerSpeed { get; set; }
        public bool EmemergencyStop { get; set; }
    }
}
