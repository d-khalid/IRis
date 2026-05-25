using Avalonia.Controls;
using Avalonia.Input;
using IRis.ViewModels.Main;


namespace IRis.Views.Main;


public partial class CanvasView : UserControl
{
    public CanvasView()
    {
        InitializeComponent();
        DataContext = new CanvasViewModel();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        (DataContext as CanvasViewModel)!.PointerEntered();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        (DataContext as CanvasViewModel)!.PointerExited();
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        (DataContext as CanvasViewModel)!.PointerPressed(sender, e);
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        (DataContext as CanvasViewModel)!.PointerMoved(sender, e);
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        (DataContext as CanvasViewModel)!.PointerReleased(e);
    }
}
