using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Geometry = Microsoft.UI.Xaml.Media.Geometry;

namespace LabsResponsiveShell.Shell;

public sealed partial class NavItem : UserControl
{
    public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(
        nameof(IconData),
        typeof(string),
        typeof(NavItem),
        new PropertyMetadata(string.Empty, OnIconDataChanged));

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label),
        typeof(string),
        typeof(NavItem),
        new PropertyMetadata(string.Empty, OnLabelChanged));

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(NavItem),
        new PropertyMetadata(false, OnIsSelectedChanged));

    public NavItem()
    {
        InitializeComponent();
        HitButton.PointerEntered += OnPointerEntered;
        HitButton.PointerExited += OnPointerExited;
        HitButton.PointerCanceled += OnPointerExited;
        HitButton.PointerCaptureLost += OnPointerExited;
    }

    public event RoutedEventHandler? Click;

    public string IconData
    {
        get => (string)GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private static void OnIconDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavItem item && e.NewValue is string data && !string.IsNullOrEmpty(data))
        {
            item.Icon.Data = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), data);
        }
    }

    private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavItem item)
        {
            item.LabelText.Text = e.NewValue as string ?? string.Empty;
        }
    }

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavItem item)
        {
            VisualStateManager.GoToState(item, (bool)e.NewValue ? "Selected" : "Unselected", useTransitions: true);
        }
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e) =>
        VisualStateManager.GoToState(this, "Hover", useTransitions: true);

    private void OnPointerExited(object sender, PointerRoutedEventArgs e) =>
        VisualStateManager.GoToState(this, "Rest", useTransitions: true);

    private void OnClick(object sender, RoutedEventArgs e) => Click?.Invoke(this, e);
}
