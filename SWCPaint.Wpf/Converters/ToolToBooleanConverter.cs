using System.Globalization;
using System.Windows.Data;

namespace SWCPaint.Wpf.Converters;

public class ToolToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return false;

        string? targetToolName = parameter?.ToString();

        if (string.IsNullOrEmpty(targetToolName)) return false;

        return value.GetType().Name.Contains(targetToolName, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}