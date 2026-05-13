using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using IRis.ViewModels.CircuitObjects;
using Avalonia.Input;


namespace IRis.Views.Main;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
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

    private void OnAddAndClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnAddOrClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnAddNotClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnAddNandClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnAddNorClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnAddXorClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    private void OnAddXnorClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
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


    private void OnAddOtherComponentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
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

