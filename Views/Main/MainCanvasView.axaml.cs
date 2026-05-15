using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia;
using System;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.ViewModels.Main;


namespace IRis.Views.Main;


public partial class MainCanvasView : UserControl
{
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
        if (DataContext is not MainCanvasViewModel vm) return;
        if (vm.Simulation.Preview == null) return;
        if (vm.Simulation.Preview is not ComponentViewModel c) return;

        vm.Simulation.Preview = null;
        c.Opacity = 1.0;
        vm.Simulation.Components.Add(c);
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is not MainCanvasViewModel vm) return;
        if (vm.Simulation.Preview == null) return;
        if (vm.Simulation.Preview is not ComponentViewModel c) return;

        Point pt = e.GetPosition((Visual)sender!);
        c.X = pt.X;
        c.Y = pt.Y;
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
