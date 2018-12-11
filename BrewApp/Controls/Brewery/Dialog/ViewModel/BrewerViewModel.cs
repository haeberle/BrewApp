using BrewApp.Logic.Brewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Controls.Brewery.Dialog.ViewModel
{
    public class BrewerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IList<Brewer> Brewers { get; set; }
        public Brewer SelectedBrewer { get; set; }
    }

}
