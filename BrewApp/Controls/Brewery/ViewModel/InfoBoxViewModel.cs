using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Controls.Brewery.ViewModel
{
    public class InfoBoxViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string BrewerName { get; set; }
        public int StepNumber { get; set; }
        public int TotalSteps { get; set; }
        public string RecipeName { get; set; }
        public string StepName { get; set; }

    }
}
