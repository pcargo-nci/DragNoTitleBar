using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DragNoTitleBar;

/// <summary>
/// Provides the arguments sent with the WindowDropEvent
/// </summary>
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
/// The ChildCard provides a Window wrapper for a Card.
/// The component handles dragging, resizing, and dropping the Window.
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

    /// <summary>
    /// Provide CLR accessors for WindowDroppedEvent
    /// </summary>
    public event RoutedEventHandler WindowDropped
    {
        add { AddHandler(WindowDroppedEvent, value); }
        remove { RemoveHandler(WindowDroppedEvent, value); }
    }

    /// <summary>
    /// Indicates if the Window is being dragged
    /// </summary>
    public bool Dragging;

    /// <summary>
    /// Indicates the location of the mouse relative to the TitleBar
    /// </summary>
    public Point TitleBarOffset = new(0, 0);

    /// <summary>
    /// Handle when the Window close option is selected
    /// </summary>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Handle MouseLeftButtonDown, handles drag start and double click to full screen
    /// </summary>
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // swtich window states on double click
        if (e.ClickCount == 2)
        {
            SwitchState();
            e.Handled = true;
            return;
        }

        // start the drag if the window is normal
        if (WindowState == WindowState.Normal)
        {
            StartDraging(e);
        }
    }

    /// <summary>
    /// Start Drag from event position, used to start dragging the Window leverage DragMove().
    /// DragMove() enables built in functionality such as docking to edges, shake to minimize all, etc.
    /// </summary>
    public void StartDraging(MouseButtonEventArgs e)
    {
        Dragging = true;
        TitleBarOffset = e.GetPosition(this);
        DragMove();
    }

    /// <summary>
    /// Raise the WindowDropped if Dragging
    /// </summary>
    private void RaiseWindowDropped(MouseButtonEventArgs e)
    {
        // stop dragging if started, send out drop event
        if (Dragging)
        {
            Dragging = false;

            // create event args
            WindowDropEventArgs routedEventArgs = new(
                WindowDroppedEvent,
                TitleBarOffset
            );

            // raise the event
            RaiseEvent(routedEventArgs);
        }
    }

    /// <summary>
    /// Handle mouse up, calls RaiseWindowDropped()
    /// </summary>
    private void Window_MouseUp(object sender, MouseButtonEventArgs e)
    {
        RaiseWindowDropped(e);
    }

    /// <summary>
    /// Handle the Maximize/Restore option
    /// </summary>
    private void WindowSwitchState(object sender, RoutedEventArgs e)
    {
        SwitchState();
    }

    /// <summary>
    /// Handle the Minimize option
    /// </summary>
    private void WindowMinimize(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Toggle between the Maximized and Normal Window state
    /// </summary>
    private void SwitchState()
    {
        switch (WindowState)
        {
            case WindowState.Normal:
                {
                    WindowState = WindowState.Maximized;
                    break;
                }
            case WindowState.Maximized:
                {
                    WindowState = WindowState.Normal;
                    break;
                }
        }
    }

    private void Window_Activated(object sender, System.EventArgs e)
    {
        TitleBarContent.Height = double.NaN;
    }

    private void Window_Deactivated(object sender, System.EventArgs e)
    {
        TitleBarContent.Height = 0;
    }
}
