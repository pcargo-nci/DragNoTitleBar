using System.Windows;
using System.Windows.Input;

namespace DragNoTitleBar;

// Defines a RoutedEventArgs for the WindowDrop Event
public class WindowDropEventArgs : RoutedEventArgs
{
    private readonly Point eventArgs;
    public Point EventArgs
    {
        get { return eventArgs; }
    }
    public WindowDropEventArgs(RoutedEvent routedEvent, Point eventArgs) : base(routedEvent)
    {
        this.eventArgs = eventArgs;
    }
}

/// <summary>
/// Interaction logic for ChildWindow.xaml
/// </summary>
public partial class ChildWindow : Window
{
    public ChildWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Define an event to allow reporting when the card-list has resized
    /// </summary>
    public static readonly RoutedEvent WindowDroppedEvent = EventManager.RegisterRoutedEvent(
        name: "WindowDroppedEvent",
        routingStrategy: RoutingStrategy.Bubble,
        handlerType: typeof(RoutedEventHandler),
        ownerType: typeof(ChildWindow)
    );

    // Provide CLR accessors for assigning an event handler.
    public event RoutedEventHandler WindowDropped
    {
        add { AddHandler(WindowDroppedEvent, value); }
        remove { RemoveHandler(WindowDroppedEvent, value); }
    }

    public bool dragging;

    public void StartDraging()
    {
        dragging = true;
        DragMove();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ColorZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        StartDraging();
    }

    private void ColorZone_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (dragging)
        {
            // create event args
            WindowDropEventArgs routedEventArgs = new(WindowDroppedEvent, new Point(Left, Top));

            // raise the event
            RaiseEvent(routedEventArgs);
        }
        dragging = false;
    }
}
