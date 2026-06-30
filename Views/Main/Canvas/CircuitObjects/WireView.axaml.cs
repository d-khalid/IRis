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

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        if (AppState.Get().EditingAllowed)
            (DataContext as WireViewModel)!.PointerEntered();
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        if (AppState.Get().EditingAllowed)
            (DataContext as WireViewModel)?.PointerExited();
    }
}
