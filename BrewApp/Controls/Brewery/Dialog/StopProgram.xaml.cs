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
    public sealed partial class StopProgram : ContentDialog
    {
        public StopProgram()
        {
            this.InitializeComponent();
        }        

        public bool Stop { get; private set; }
        private void btnOkClick(object sender, RoutedEventArgs e)
        {
            Stop = true;
            this.Hide();
        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            Stop = false;
            this.Hide();
        }
    }
}
