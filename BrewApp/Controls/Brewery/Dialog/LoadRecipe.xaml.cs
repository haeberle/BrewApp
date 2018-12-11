using BrewApp.Controls.Brewery.Dialog.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BrewApp.Controls.Brewery.Dialog
{
    public sealed partial class LoadRecipe : ContentDialog
    {
        public bool RecipeSelected { get; private set; }
        public LoadRecipe()
        {          
            this.InitializeComponent();
            //this.Width = Window.Current.Bounds.Width;
            //PrimaryButtonText = "Selektieren";
            //SecondaryButtonText = "Abbrechen";

        }

        //private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        //{
        //}

        //private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        //{
        //}



        private void btnSelectClick(object sender, RoutedEventArgs e)
        {
            RecipeSelected = true;
            this.Hide();
        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            RecipeSelected = false;
            this.Hide();
        }
    }
}
