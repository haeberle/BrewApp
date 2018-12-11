using BrewApp.Controls.Brewery.ViewModel;
using BrewApp.Controls.Pasteur.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.ViewModel
{
    public class BrewingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BrewingViewModel()
        {
            BreweryViewModel.PropertyChanged += BreweryViewModel_PropertyChanged;
            PasteurViewModel.PropertyChanged += PasteurViewModel_PropertyChanged;
        }

        private void PasteurViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProcessIsRunning")
            {
                BreweryViewModel.ProcessBlocked = PasteurViewModel.ProcessIsRunning;
            }
        }

        private void BreweryViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProcessIsRunning")
            {
                PasteurViewModel.ProcessBlocked = BreweryViewModel.ProcessIsRunning;
            }
        }      

        public BreweryViewModel BreweryViewModel { get; private set; } = new BreweryViewModel();
        public PasteurViewModel PasteurViewModel { get; private set; } = new PasteurViewModel();
        public bool RecipeLoaded { get; set; }
        public string StepName { get; set; }
        public string StepNumber { get; set; }
        public bool MashIn { get; set; }
        public DateTime StartTime { get; set; }

    }
}
