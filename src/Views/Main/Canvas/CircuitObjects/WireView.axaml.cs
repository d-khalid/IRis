using Avalonia.Controls;
using Avalonia.Input;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.CircuitObjects;

namespace IRis.Views.Main.Canvas.CircuitObjects;

public partial class WireView : UserControl
{
    public WireView()
    {
        InitializeComponent();
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e) =>
        (DataContext as WireViewModel)?.OnPointerEntered(sender, e);

    private void OnPointerExited(object? sender, PointerEventArgs e) =>
        (DataContext as WireViewModel)?.OnPointerExited(sender, e);
}
