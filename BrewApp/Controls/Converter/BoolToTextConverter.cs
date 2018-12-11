using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BrewApp.Controls.Converter
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = new string[] { "Ein", "Aus" };

            if (!string.IsNullOrEmpty(parameter as string))
            {
                str = (parameter as string).Split(',');
            }

            if ((bool)value)
            {
                return str[0];
            }
            else
            {
                return str[1];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
