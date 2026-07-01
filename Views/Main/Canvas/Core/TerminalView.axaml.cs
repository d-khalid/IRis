using Avalonia.Controls;
using Avalonia.Input;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.Views.Main.Canvas.Core;

public partial class TerminalView : UserControl
{
    public TerminalView()
    {
        InitializeComponent();
    }

    public void OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
        (DataContext as TerminalViewModel)?.OnPointerPressed(sender, e);

    public void OnPointerEntered(object? sender, PointerEventArgs e) =>
        (DataContext as TerminalViewModel)?.OnPointerEntered(sender, e);

    public void OnPointerExited(object? sender, PointerEventArgs e) =>
        (DataContext as TerminalViewModel)?.OnPointerExited(sender, e);
}
