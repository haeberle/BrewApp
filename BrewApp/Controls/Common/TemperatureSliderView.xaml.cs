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
    public sealed partial class TemperatureSliderView : UserControl
    {
        public static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register("Temperature", typeof(double), typeof(TemperatureSliderView), null);
        public double Temperature
        {
            get { return (double)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); }
        }
        public static readonly DependencyProperty IsChangeEnabledProperty = DependencyProperty.Register("IsChangeEnabledProperty", typeof(bool), typeof(TemperatureSliderView), null);
        public bool IsChangeEnabled
        {
            get { return (bool)GetValue(IsChangeEnabledProperty); }
            set { SetValue(IsChangeEnabledProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TemperatureSliderView), null);
        public string Header
        {
            get { return GetValue(HeaderProperty) as string; }
            set { SetValue(HeaderProperty, value); }
        }
        public TemperatureSliderView()
        {
            this.InitializeComponent();
        }

        private void tgbChangeToggled(object sender, RoutedEventArgs e)
        {

        }
    }
}
