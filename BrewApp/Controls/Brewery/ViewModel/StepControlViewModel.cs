using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Controls.Brewery.ViewModel
{
    class StepControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string StepText { get; set; }
        public int StepNumber { get; set; }
        public int TotalSteps { get; set; }
        public string ReciepeName { get; set; }
        public string BrewerName { get; set; }

    }
}
