using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using CoreColor = SWCPaint.Core.Models.Color;

namespace SWCPaint.Wpf.Converters;

public class CoreColorToWpfBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CoreColor c)
        {
            var wpfColor = Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
            return new SolidColorBrush(wpfColor);
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}