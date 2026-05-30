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

        if (Simulation.Get().Running && DataContext is ToggleViewModel t)
            t.Toggle();

        else if (!Simulation.Get().Running)
            (DataContext as ComponentViewModel)?.PointerPressed();
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        e.Handled = true;

        if (!Simulation.Get().Running)
            ComponentViewModel.PointerReleased();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

        if (!Simulation.Get().Running)
            (DataContext as ComponentViewModel)?.PointerEntered();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

        if (!Simulation.Get().Running)
            (DataContext as ComponentViewModel)?.PointerExited();
    }
}
