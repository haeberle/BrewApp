using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace BrewApp.Controls.Common
{
    public class MyToggleButton : ToggleButton
    {
        public MyToggleButton()
        {
            this.DefaultStyleKey = typeof(MyToggleButton);
        }

        public Brush CheckedBackground
        {
            get { return (Brush)GetValue(CheckedBackgroundProperty); }
            set { SetValue(CheckedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CheckedBackgroundProperty =
            DependencyProperty.Register("CheckedBackground", typeof(Brush), typeof(MyToggleButton), new PropertyMetadata(Application.Current.Resources["ToggleButtonBackgroundChecked"]));

        public string CheckedText
        {
            get { return (string)GetValue(CheckedTextProperty); }
            set { SetValue(CheckedTextProperty, value); }
        }

        public static readonly DependencyProperty CheckedTextProperty =
            DependencyProperty.Register("CheckedText", typeof(string), typeof(MyToggleButton), null);

    }
}
