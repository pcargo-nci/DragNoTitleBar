using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;


namespace DragNoTitleBar;

/// <summary>
/// Interaction logic for Card.xaml
/// </summary>
[ObservableObject]
public partial class Card : UserControl
{
    public Card()
    {
        InitializeComponent();
    }
    [ObservableProperty]
    public bool highlight = true;
}
