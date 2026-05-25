using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using IRis.Models.Core;
using IRis.ViewModels.Main;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using IRis.ViewModels.Circuit.CircuitObjects.Core;
using IRis.Models.Circuit.CircuitObjects.Core;
using Avalonia;


namespace IRis.Views.Main;


public partial class LeftSidebarView : UserControl
{
    public LeftSidebarView()
    {
        InitializeComponent();
    }


    private void OnAddAndClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var prev = Preview.GetInstance();
        AndGateViewModel gate = new() { Opacity = 0.5 };

        TerminalViewModel i1 = new(TerminalType.Input, gate);
        TerminalViewModel i2 = new(TerminalType.Input, gate);

        gate.AddInput(i1);
        gate.AddInput(i2);
        prev.Add(gate);
        prev.MouseOffset = new Point(25, 25);
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


    private void OnAddOtherComponentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }
}
