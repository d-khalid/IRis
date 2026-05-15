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


namespace IRis.Views.Main;


public partial class MainCanvasView : UserControl
{
    public SimulationViewModel Simulation = SimulationViewModel.GetInstance();


    public MainCanvasView()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new MainCanvasViewModel();
    }


    private void OnPointerEnter(object? sender, PointerEventArgs e) {}
    private void OnPointerExited(object? sender, PointerEventArgs e) {}
    private void OnPointerReleased(object? sender, PointerEventArgs e) {}


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Simulation.Preview == null) return;
        if (Simulation.Preview is not ComponentViewModel c) return;

        Simulation.Preview = null;
        c.Opacity = 1.0;
        Simulation.Components.Add(c);
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


    // private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    // {
    //     double newSpacing = Simulation.GridSpacing + (e.Delta.Y * 1.5);
    //     // Simulation.GridSpacing = Math.Clamp(newSpacing, 5.0, 100.0);
    //     Console.WriteLine(Simulation.GridSpacing);

    //     e.Handled = true;
    // }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
