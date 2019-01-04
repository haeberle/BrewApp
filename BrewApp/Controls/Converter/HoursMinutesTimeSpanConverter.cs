using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BrewApp.Controls.Converter
{
    //[ValueConversion(typeof(TimeSpan), typeof(String))]
    public class HoursMinutesTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeSpan)value).ToString("mm\\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            TimeSpan ts = new TimeSpan();

            if (TimeSpan.TryParseExact((string)value, "mm\\:ss", CultureInfo.CurrentCulture, out ts))
            {
                return ts;
            }
            else
            {
                return new TimeSpan(0,0,0);
            }
        }
    }
}
