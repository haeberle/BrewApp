using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BrewApp.Controls.Common
{
    public sealed partial class TimeSliderView : UserControl
    {
        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds", typeof(double), typeof(TimeSliderView), null);
        public double Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        public static readonly DependencyProperty AsIsTimeProperty = DependencyProperty.Register("AsIsTime", typeof(TimeSpan), typeof(TimeSliderView), null);
        public TimeSpan AsIsTime
        {
            get { return (TimeSpan)GetValue(AsIsTimeProperty); }
            set
            {
                SetValue(AsIsTimeProperty, value);                
            }
        }

        public static readonly DependencyProperty ToBeTimeProperty = DependencyProperty.Register("ToBeTime", typeof(TimeSpan), typeof(TimeSliderView), null);
        public TimeSpan ToBeTime
        {
            get { return (TimeSpan)GetValue(ToBeTimeProperty); }
            set
            {
                SetValue(ToBeTimeProperty, value);
                if (!_updating)
                {
                    Seconds = value.TotalSeconds;
                }
            }
        }
        //public static readonly DependencyProperty IsChangeEnabledProperty = DependencyProperty.Register("IsChangeEnabledProperty", typeof(bool), typeof(TemperatureSliderView), null);
        //public bool IsChangeEnabled
        //{
        //    get { return (bool)GetValue(IsChangeEnabledProperty); }
        //    set { SetValue(IsChangeEnabledProperty, value); }
        //}

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TimeSliderView), null);
        public string Header
        {
            get { return GetValue(HeaderProperty) as string; }
            set { SetValue(HeaderProperty, value); }
        }
        public TimeSliderView()
        {
            this.InitializeComponent();
        }

        bool _updating = false;
        private void sldValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            try
            {
                _updating = true;
                ToBeTime = new TimeSpan((int)(e.NewValue / 3600), (int)((e.NewValue % 3600) / 60), (int)e.NewValue % 60);
            }
            finally
            {
                _updating = false;
            }
        }
    }
}
