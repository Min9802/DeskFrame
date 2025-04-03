using System.Globalization;
using System.Windows;
using System.Windows.Data;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace DeskFrame
{
    public class TitleTextAlignmentToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HorizontalAlignment alignment)
            {
                switch (alignment)
                {
                    case HorizontalAlignment.Left:
                        return new Thickness(0, 0, 0, 0); 
                    case HorizontalAlignment.Right:
                        return new Thickness(0, 0, 20, 0); 
                    default:
                        return new Thickness(0); 
                }
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
    }
}
