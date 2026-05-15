using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia;
using System;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.ViewModels.Main;
using IRis.Models.Core;
using IRis.Services;
using IRis.ViewModels;
using IRis.ViewModels.Circuit;


namespace IRis.Views.Main;


public partial class MainCanvasView : UserControl
{
    public SimulationViewModel Simulation = SimulationViewModel.GetInstance();


    public MainCanvasView()
    {
        InitializeComponent();
        DataContext = new MainCanvasViewModel();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        PreviewControl.IsVisible = true;
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        PreviewControl.IsVisible = false;
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Simulation.UnselectAll();

        if (Simulation.Preview == null) return;
        if (Simulation.Preview is ComponentViewModel c)
        {
            Simulation.Preview = null;
            c.Opacity = 1.0;
            Simulation.Components.Add(c);
        }
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Point pt = UtilityService.SnapPointToGrid(e.GetPosition((Visual)sender!));
        Simulation.CurrentMousePos = pt;

        if (Simulation.Preview == null) return;
        if (Simulation.Preview is not ComponentViewModel c) return;

        c.X = pt.X;
        c.Y = pt.Y;
    }


    private void OnPointerReleased(object? sender, PointerEventArgs e) {}


    private void OnCircuitObjectClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control c && c.DataContext is CircuitObjectViewModel co)
        {
            Simulation.UnselectAll();
            co.IsSelected = true;
            e.Handled = true;
        }
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
