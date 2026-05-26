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


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        (DataContext as ComponentViewModel)!.PointerPressed();
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = true;
        (DataContext as ComponentViewModel)!.PointerReleased();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        (DataContext as ComponentViewModel)!.PointerEntered();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        (DataContext as ComponentViewModel)!.PointerExited();
    }
}
