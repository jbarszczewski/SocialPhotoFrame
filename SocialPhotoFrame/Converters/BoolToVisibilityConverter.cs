using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SocialPhotoFrame.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isInverted;
            bool.TryParse(parameter as string, out isInverted);

            if (value is bool && (bool)value)
                return isInverted ? Visibility.Collapsed : Visibility.Visible;

            return isInverted ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool isInverted;
            bool.TryParse(parameter as string, out isInverted);

            if (value is Visibility && value.Equals(Visibility.Visible))
                return isInverted ? false : true;

            return isInverted ? true : false;
        }
    }
}