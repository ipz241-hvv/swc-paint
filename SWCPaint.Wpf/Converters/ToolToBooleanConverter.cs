using System.Globalization;
using System.Windows.Data;
using SWCPaint.Core.Interfaces.Tools;

namespace SWCPaint.Wpf.Converters;

public class ToolToBooleanConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not ITool currentTool || values[1] is not string targetName)
            return false;

        var type = currentTool.GetType();
        string currentName = type.IsGenericType
            ? type.GetGenericArguments()[0].Name
            : type.Name.Replace("Tool", "");

        return currentName.Equals(targetName, StringComparison.OrdinalIgnoreCase);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}