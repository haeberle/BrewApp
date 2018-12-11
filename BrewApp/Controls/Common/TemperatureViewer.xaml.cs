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
    public sealed partial class TemperatureViewer : UserControl
    {
        public static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register("Temperature", typeof(string), typeof(TemperatureViewer), null);
        public string Temperature
        {
            get { return GetValue(TemperatureProperty) as string; }
            set { SetValue(TemperatureProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TemperatureViewer), null);
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }
        public TemperatureViewer()
        {
            this.InitializeComponent();
        }
    }
}
