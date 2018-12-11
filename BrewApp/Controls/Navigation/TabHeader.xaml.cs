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

namespace BrewApp.Controls.Navigation
{
    public sealed partial class TabHeader : UserControl
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(TabHeader), null);

        public string Path
        {
            get { return GetValue(PathProperty) as string; }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(TabHeader), null);

        public string Label
        {
            get { return GetValue(LabelProperty) as string; }
            set { SetValue(LabelProperty, value); }
        }

        //public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(TabHeader), null);

        //public Brush Background
        //{
        //    get { return GetValue(BackgroundProperty) as Brush; }
        //    set { SetValue(BackgroundProperty, value); }
        //}

        public TabHeader()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

    }
}
