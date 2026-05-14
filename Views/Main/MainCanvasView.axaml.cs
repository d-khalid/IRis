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
    }


    private void OnPointerEnter(object? sender, PointerEventArgs e) {}
    private void OnPointerExit(object? sender, PointerEventArgs e) {}
    private void OnPointerMoved(object? sender, PointerEventArgs e) {}
    private void OnPointerPressed(object? sender, PointerEventArgs e) {}
    private void OnPointerReleased(object? sender, PointerEventArgs e) {}


    private void OnComponentDragDelta(object? sender, VectorEventArgs e)
    {
        if (sender is Thumb thumb && thumb.DataContext is ComponentViewModel componentVm)
        {
            componentVm.X += e.Vector.X;
            componentVm.Y += e.Vector.Y;
        }
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
