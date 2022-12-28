using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DragNoTitleBar;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ObservableObject]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // add the initial cards
        Cards.Add(CardCreate());
        Cards.Add(CardCreate());
        Cards.Add(CardCreate());
        Cards.Add(CardCreate());
        Cards.Add(CardCreate());
        Cards.Add(CardCreate());
    }

    /// <summary>
    /// Represents list of Cards
    /// </summary>
    public ObservableCollection<Card> Cards { get; } = new ObservableCollection<Card>();

    /// <summary>
    /// Represents open ChildWindows
    /// </summary>
    List<ChildWindow> ChildWindows = new();

    // Create a dummy card
    /// <summary>
    /// Creates a Card
    /// </summary>
    private Card CardCreate()
    {
        Card newCard = new()
        {
            Highlight = false
        };
        newCard.MouseLeftButtonDown += Card_MouseLeftButtonDown;
        return newCard;
    }

    /// <summary>
    /// Handle MouseDown on Card
    /// </summary>
    private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // get reference to Card
        var card = (Card)sender;

        // get anchor point
        var anchor = card.PointToScreen(new Point(0, 0));

        // create window
        ChildWindow childWindow = new()
        {
            // position window
            Top = anchor.Y,
            Left = anchor.X
        };

        ChildWindows.Add(childWindow);

        card.MouseLeftButtonDown -= Card_MouseLeftButtonDown;
        Cards.Remove(card);

        // listen for window drop
        childWindow.WindowDropped += ChildWindow_WindowDropped;

        // show window
        childWindow.Show();

        // start dragging
        childWindow.StartDraging();
    }

    private void ChildWindow_WindowDropped(object sender, RoutedEventArgs e)
    {
        var args = (WindowDropEventArgs)e;
        var dropPoint = args.EventArgs;

        // check to see if drop was within the bounds of the window
        if (
            dropPoint.X > Left
            && dropPoint.X < Left + ActualWidth
            && dropPoint.Y > Top
            && dropPoint.Y < Top + ActualHeight
        )
        {
            var childWindow = (ChildWindow)sender;
            ChildWindowDrop(childWindow);
        }
    }

    private void ChildWindowDrop(ChildWindow childWindow)
    {
        // close down the Child Window
        childWindow.WindowDropped -= ChildWindow_WindowDropped;
        childWindow.Close();

        // place card into Main Window
        Cards.Add(CardCreate());
    }

    /// <summary>
    /// Handle MainWindow Closing
    /// </summary>
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // close the ChildWindows
        foreach (ChildWindow childWindow in ChildWindows)
        {
            childWindow.WindowDropped -= ChildWindow_WindowDropped;
            childWindow.Close();
        }
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
