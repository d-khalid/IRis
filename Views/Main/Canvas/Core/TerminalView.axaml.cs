using Avalonia.Controls;
using Avalonia.Input;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.Views.Main.Canvas.Core;


public partial class TerminalView : UserControl
{
    public TerminalView()
    {
        InitializeComponent();
    }


    public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        (DataContext as TerminalViewModel)!.PointerPressed();
    }


    public void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        (DataContext as TerminalViewModel)!.PointerEntered();
    }


    public void OnPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        (DataContext as TerminalViewModel)!.PointerExited();
    }
}
