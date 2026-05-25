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
        (DataContext as TerminalViewModel)!.PointerPressed();
    }
}
