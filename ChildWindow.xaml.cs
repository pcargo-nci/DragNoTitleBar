using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
    /// Collapsed height of TitleBar
    /// </summary>
    public double TitleBarCollapsedHeight = 16;

    /// <summary>
    /// Expanded height of TitleBar
    /// </summary>
    public double TitleBarExpandedHeight = 48;

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

    /// <summary>
    /// Defines how long the animation will take to complete
    /// </summary>
    private readonly Duration AnimationDuration = new(
        TimeSpan.FromSeconds(0.1)
    );

    /// <summary>
    /// Animation for Height
    /// </summary>
    private DoubleAnimation AnimationHeight;

    /// <summary>
    /// Animate the TitleBar to Collapsed Height
    /// </summary>
    private void TitleBarCollapse()
    {
        if (TitleBar.ActualHeight == TitleBarExpandedHeight)
        {
            // freeze the content height
            WindowContent.Height = WindowContent.ActualHeight;

            // animate height of TitleBar
            AnimationHeight = new DoubleAnimation(TitleBarCollapsedHeight, AnimationDuration);
            AnimationHeight.Completed += AnimationHeight_Collapse_Completed;
            TitleBar.BeginAnimation(HeightProperty, AnimationHeight);

            // animate height of Window
            root.BeginAnimation(
                HeightProperty,
                new DoubleAnimation(
                    root.ActualHeight,
                    root.ActualHeight - (TitleBarExpandedHeight - TitleBarCollapsedHeight),
                    AnimationDuration
                )
            );

            // hide TitleBarContent
            TitleBarContent.Visibility = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Handle animation complete,
    /// allows removing the set Height on the content after the title bar has changed size
    /// </summary>
    private void AnimationHeight_Collapse_Completed(object? sender, EventArgs e)
    {
        AnimationHeight.Completed -= AnimationHeight_Collapse_Completed;
        // Unfreeze the content - reset size to fill the given space
        WindowContent.Height = Double.NaN;
    }

    /// <summary>
    /// Animate the TitleBar to Expanded Height
    /// </summary>
    private void TitleBarExpand()
    {
        if (TitleBar.ActualHeight == TitleBarCollapsedHeight)
        {
            // freeze the content height
            WindowContent.Height = WindowContent.ActualHeight;

            // animate height of TitleBar
            AnimationHeight = new DoubleAnimation(TitleBarExpandedHeight, AnimationDuration);
            AnimationHeight.Completed += AnimationHeight_Expand_Completed;
            TitleBar.BeginAnimation(HeightProperty, AnimationHeight);

            // animate height of Window
            root.BeginAnimation(
                HeightProperty,
                new DoubleAnimation(
                    root.ActualHeight,
                    root.ActualHeight + (TitleBarExpandedHeight - TitleBarCollapsedHeight),
                    AnimationDuration
                )
            );

            // animate opacity of TitleBarContent
            TitleBarContent.BeginAnimation(
                OpacityProperty,
                new DoubleAnimation(
                    1,
                    AnimationDuration
                )
            );

            // show TitleBarContent
            TitleBarContent.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Handle animation complete,
    /// allows removing the set Height on the content after the title bar has changed size
    /// </summary>
    private void AnimationHeight_Expand_Completed(object? sender, EventArgs e)
    {
        AnimationHeight.Completed -= AnimationHeight_Expand_Completed;
        // Reset size to fill the given space
        WindowContent.Height = Double.NaN;
    }

    /// <summary>
    /// Handler for when the Window becomes active,
    /// used to display the expanded TitleBar
    /// </summary>
    private void Window_Activated(object sender, EventArgs e)
    {
        TitleBarExpand();
    }

    /// <summary>
    /// Handler for when the Window becomes inactive,
    /// used to display the collapsed TitleBar
    /// </summary>
    private void Window_Deactivated(object sender, EventArgs e)
    {
        TitleBarCollapse();
    }

    /// <summary>
    /// /Handle when the users mouses hovers over TitleBar,
    /// used to expand the TitleBar to make it easier to grab
    /// </summary>
    private void TitleBar_MouseEnter(object sender, MouseEventArgs e)
    {
        TitleBarExpand();
    }

    /// <summary>
    /// /Handle when the users mouses leaves the TitleBar,
    /// used to collapse the TitleBar
    /// </summary>
    private void TitleBar_MouseLeave(object sender, MouseEventArgs e)
    {
        // do not collapse if window is Active
        if (IsActive) return;

        TitleBarCollapse();
    }
}
