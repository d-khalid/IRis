using Avalonia.Controls;
using Avalonia.Input;
using IRis.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace IRis.Views.Main;

public partial class CanvasView : UserControl
{
    public CanvasView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<CanvasViewModel>();
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e) =>
        (DataContext as CanvasViewModel)?.OnPointerEntered(sender, e);

    private void OnPointerExited(object? sender, PointerEventArgs e) =>
        (DataContext as CanvasViewModel)?.OnPointerExited(sender, e);

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
        (DataContext as CanvasViewModel)?.OnPointerPressed(sender, e);

    private void OnPointerMoved(object? sender, PointerEventArgs e) =>
        (DataContext as CanvasViewModel)?.OnPointerMoved(sender, e);

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e) =>
        (DataContext as CanvasViewModel)?.OnPointerReleased(sender, e);

    private void OnKeyDown(object? sender, KeyEventArgs e) =>
        (DataContext as CanvasViewModel)?.OnKeyDown(sender, e);
}
