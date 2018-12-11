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
    public sealed partial class NextStap : ContentDialog
    {
        public NextStap()
        {
            this.InitializeComponent();
        }

        public bool NextStep { get; private set; }
        private void btnOkClick(object sender, RoutedEventArgs e)
        {
            NextStep = true;
            this.Hide();
        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            NextStep = false;
            this.Hide();
        }
    }
}
