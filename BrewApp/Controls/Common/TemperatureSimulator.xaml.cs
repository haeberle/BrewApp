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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewApp.Controls.Common
{
    public sealed partial class TemperatureSimulator : UserControl
    {
        public TemperatureSimulator()
        {
            this.InitializeComponent();
            if (!((App)App.Current).Properties.ContainsKey("VesselTemperatureProbe"))
            {
                ((App)App.Current).Properties.Add("VesselTemperatureProbe", "10.0");
            }
            if (!((App)App.Current).Properties.ContainsKey("MashTemperatureProbe"))
            {
                ((App)App.Current).Properties.Add("MashTemperatureProbe", "10.0");
            }
        }

        private void vesselTemp_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ((App)App.Current).Properties["VesselTemperatureProbe"] = e.NewValue.ToString();
        }

        private void mashTemp_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ((App)App.Current).Properties["MashTemperatureProbe"] = e.NewValue.ToString();
        }
    }
}
