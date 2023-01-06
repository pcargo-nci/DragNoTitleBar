using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System;

namespace DragNoTitleBar;

/// <summary>
/// Converts the WindowState.Normal to Visibility.Visible
/// </summary>
[ValueConversion(typeof(WindowState), typeof(Visibility))]
public class WindowStateVisibilityNormalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((WindowState)value == WindowState.Normal) { return Visibility.Visible; }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((Visibility)value == Visibility.Visible) ? WindowState.Normal : WindowState.Maximized;
    }
}
