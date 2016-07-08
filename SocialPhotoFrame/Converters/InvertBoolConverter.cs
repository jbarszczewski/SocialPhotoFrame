using System;
using Windows.UI.Xaml.Data;

namespace SocialPhotoFrame.Converters
{
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is bool)
            {
                return !((bool)value);
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, language);
        }
    }
}