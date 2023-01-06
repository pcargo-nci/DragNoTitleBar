using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System;

namespace DragNoTitleBar;

/// <summary>
/// Converts the WindowState.Maximized to Visibility.Visible
/// </summary>
[ValueConversion(typeof(WindowState), typeof(Visibility))]
public class WindowStateVisibilityMaximizedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((WindowState)value == WindowState.Maximized) { return Visibility.Visible; }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((Visibility)value == Visibility.Visible) ? WindowState.Maximized : WindowState.Maximized;
    }
}
