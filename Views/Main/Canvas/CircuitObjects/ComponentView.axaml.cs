using Avalonia.Controls;
using Avalonia.Input;
using IRis.ViewModels.Main.Canvas.CircuitObjects;

namespace IRis.Views.Main.Canvas.CircuitObjects;

public partial class ComponentView : UserControl
{
    public ComponentView()
    {
        InitializeComponent();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
        (DataContext as ComponentViewModel)?.OnPointerPressed(sender, e);

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e) =>
        (DataContext as ComponentViewModel)?.OnPointerReleased(sender, e);

    private void OnPointerEntered(object? sender, PointerEventArgs e) =>
        (DataContext as ComponentViewModel)?.OnPointerEntered(sender, e);

    private void OnPointerExited(object? sender, PointerEventArgs e) =>
        (DataContext as ComponentViewModel)?.OnPointerExited(sender, e);
}
