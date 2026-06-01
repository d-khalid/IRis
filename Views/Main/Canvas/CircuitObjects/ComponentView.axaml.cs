using System;
using Avalonia.Controls;
using Avalonia.Input;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


namespace IRis.Views.Main.Canvas.CircuitObjects;


public partial class ComponentView : UserControl
{
    public ComponentView()
    {
        InitializeComponent();
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (AppState.Get().EditingAllowed)
            (DataContext as ComponentViewModel)?.PointerPressed();

        else if (!AppState.Get().EditingAllowed && DataContext is ToggleViewModel t)
            t.Toggle();
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (AppState.Get().EditingAllowed)
            ComponentViewModel.PointerReleased();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

        if (AppState.Get().EditingAllowed)
            (DataContext as ComponentViewModel)?.PointerEntered();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

        if (AppState.Get().EditingAllowed)
            (DataContext as ComponentViewModel)?.PointerExited();
    }
}
