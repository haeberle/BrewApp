using BrewApp.Logic.Recipes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewApp.Controls.Brewery.Dialog.ViewModel
{
    public class LoadRecipeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IList<Recipe> Recipes { get; set; }
        public Recipe SelectedRecipe { get; set; }
    }
}
