using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Media;

using IRis.Models;
using IRis.Services;
using IRis.ViewModels;
using IRis.Views;


namespace IRis.Views;


public partial class MainWindow : Window
{
    public Simulation Simulation { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        Simulation = new(MainCanvas);
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
        Simulation.IsSimulating = !Simulation.IsSimulating;

        if (sender is Button btn)
        {
            btn.Content = Simulation.IsSimulating ? "Simulation: ON" : "Simulation: OFF";
            btn.Background = Simulation.IsSimulating ? Brushes.Green : Brushes.DarkRed;
        }
    }


    private void OnFileMenuNewClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnFileMenuOpenClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnFileMenuSaveClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnFileMenuSaveAsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnExportMenuComponentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnExportMenuCircuitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnFileMenuExitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnEditMenuUndoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnEditMenuRedoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnEditMenuCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnEditMenuCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnEditMenuPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnEditMenuDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnAiMenuGenerateFromPromptClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnAiMenuGenerateFromImageClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }


    private void OnHelpMenuAboutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        throw new NotImplementedException();
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

