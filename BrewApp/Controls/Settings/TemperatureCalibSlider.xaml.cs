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

namespace BrewApp.Controls.Settings
{
    public sealed partial class TemperatureCalibSlider : UserControl
    {
        public static readonly DependencyProperty TemperatureCalibProperty = DependencyProperty.Register("TemperatureCalib", typeof(double), typeof(TemperatureCalibSlider), null);
        public double TemperatureCalib
        {
            get { return (double)GetValue(TemperatureCalibProperty); }
            set { SetValue(TemperatureCalibProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TemperatureCalibSlider), null);
        public string Header
        {
            get { return GetValue(HeaderProperty) as string; }
            set { SetValue(HeaderProperty, value); }

        }       

        public TemperatureCalibSlider()
        {
            this.InitializeComponent();
        }
    }
}
