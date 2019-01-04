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
    public sealed partial class TimeEditView : UserControl
    {
        public TimeEditView()
        {
            this.InitializeComponent();
        }

        //public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds", typeof(double), typeof(TimeEditView), null);
        //public double Seconds
        //{
        //    get { return (int)GetValue(SecondsProperty); }
        //    set { SetValue(SecondsProperty, value); }
        //}

        public static readonly DependencyProperty AsIsTimeProperty = DependencyProperty.Register("AsIsTime", typeof(TimeSpan), typeof(TimeEditView), null);
        public TimeSpan AsIsTime
        {
            get { return (TimeSpan)GetValue(AsIsTimeProperty); }
            set
            {
                SetValue(AsIsTimeProperty, value);
            }
        }

        public static readonly DependencyProperty ToBeTimeProperty = DependencyProperty.Register("ToBeTime", typeof(TimeSpan), typeof(TimeEditView), null);
        public TimeSpan ToBeTime
        {
            get { return (TimeSpan)GetValue(ToBeTimeProperty); }
            set
            {
                SetValue(ToBeTimeProperty, value);
                //if (!_updating)
                //{
                //    Seconds = value.TotalSeconds;
                //}
            }
        }
       
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TimeEditView), null);
        public string Header
        {
            get { return GetValue(HeaderProperty) as string; }
            set { SetValue(HeaderProperty, value); }
        }
    
        //bool _updating = false;
        //private void sldValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        //{
        //    try
        //    {
        //        _updating = true;
        //        ToBeTime = new TimeSpan((int)(e.NewValue / 3600), (int)((e.NewValue % 3600) / 60), (int)e.NewValue % 60);
        //    }
        //    finally
        //    {
        //        _updating = false;
        //    }
        //}

        //private void bxValueChanged(object sender, TextChangedEventArgs e)
        //{
        //    try
        //    {
        //        _updating = true;
        //        //ToBeTime = new TimeSpan((int)(e.NewValue / 3600), (int)((e.NewValue % 3600) / 60), (int)e.NewValue % 60);
        //        ToBeTime = TimeSpan.Parse(bxTimerValue.Text);
        //    }
        //    finally
        //    {
        //        _updating = false;
        //    }
        //}
    }
}
