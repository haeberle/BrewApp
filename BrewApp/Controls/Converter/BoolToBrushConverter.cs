using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BrewApp.Controls.Converter
{
    public class BoolToBrushConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TrueBrushProperty =
            DependencyProperty.Register("TrueBrush", typeof(Brush), typeof(BoolToBrushConverter),
                new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Green)));

        public static readonly DependencyProperty FalseBrushProperty =
            DependencyProperty.Register("FalseBrush", typeof(Brush), typeof(BoolToBrushConverter),
                new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Red)));

        public Brush TrueBrush
        {
            get { return (Brush)this.GetValue(TrueBrushProperty); }
            set { this.SetValue(TrueBrushProperty, value); }
        }

        public Brush FalseBrush
        {
            get { return (Brush)this.GetValue(FalseBrushProperty); }
            set { this.SetValue(FalseBrushProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return this.TrueBrush;
            }
            else
            {
                return this.FalseBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
