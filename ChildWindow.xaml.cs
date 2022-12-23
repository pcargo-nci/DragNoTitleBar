using System.Windows;
using System.Windows.Input;

namespace DragNoTitleBar;

/// <summary>
/// Interaction logic for ChildWindow.xaml
/// </summary>
public partial class ChildWindow : Window
{
    public ChildWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ColorZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}
