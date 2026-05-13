using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System;
using IRis.ViewModels.CircuitObjects;


namespace IRis.Views.Main;


public partial class MainCanvasView : UserControl
{
    public MainCanvasView()
    {
        AvaloniaXamlLoader.Load(this);
    }


    private void OnComponentDragDelta(object? sender, VectorEventArgs e)
    {
        if (sender is Thumb thumb && thumb.DataContext is ComponentViewModel componentVm)
        {
            // Update the data. The UI will instantly follow because of the bindings!
            componentVm.X += e.Vector.X;
            componentVm.Y += e.Vector.Y;
        }
    }


    private void OnAddToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnAddProbeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
    
    private void OnAddDLatchClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnAddWireClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnSimulationToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnContextMenuCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnContextMenuCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnContextMenuPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnContextMenuDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
