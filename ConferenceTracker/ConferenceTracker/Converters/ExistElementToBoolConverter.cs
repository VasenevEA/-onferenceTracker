using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace ConferenceTracker.Converters
{
    public class ExistElementToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int.TryParse(parameter.ToString(), out int index);
            if (value is ICollection collections)
            { 
                return collections.Count > index;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
