using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using System;
using IRis.ViewModels.Main;
using IRis.Services;
using IRis.ViewModels;
using IRis.ViewModels.Circuit;
using IRis.Models.Core;
using IRis.ViewModels.Circuit.CircuitObjects;
using Avalonia.Media;
using IRis.ViewModels.Circuit.CircuitObjects.Core;
using IRis.Models.Circuit.CircuitObjects.Core;
using Avalonia.VisualTree;
using System.Linq;


namespace IRis.Views.Main;


public partial class MainCanvasView : UserControl
{
    public SimulationManager Simulation = SimulationManager.GetInstance();
    public PreviewManager Preview = PreviewManager.GetInstance();
    public SelectionManager SelectionBox = SelectionManager.GetInstance();
    public DragManager Drag = DragManager.GetInstance();


    public MainCanvasView()
    {
        InitializeComponent();
        DataContext = new MainCanvasViewModel();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        Preview.Show();
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        Preview.Hide();
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed || 
            e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;     // if user wants to move canvas

        e.Handled = true;

        if (Preview.HasObjects()) 
            Preview.CommitAll();
        else
        {
            SelectionBox.Start();
            e.Pointer.Capture(sender as Control);   // keeps focus till released
        }
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        e.Handled = true;

        Simulation.CurrentMousePos = SimulationService.SnapPointToGrid(
            e.GetPosition((Visual)sender!));

        if (SelectionBox.IsVisible)
            SelectionBox.Update();

        else if (Preview.HasObjects())
            Preview.Update();

        else if (Drag.HasObjects())
        {
            if (SelectionBox.HasObjects()) 
                SelectionBox.Ditch();

            Drag.Update();
        }
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = true;

        if (SelectionBox.IsVisible)
        {
            SelectionBox.Hide();
            e.Pointer.Capture(null);
        }
    }


    private void OnCircuitObjectPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        var co = (sender as Control)!.DataContext as CircuitObjectViewModel;

        if (co is ComponentViewModel c)
        {
            if (c.IsSelected) 
                Drag.AddCollection(SelectionBox.Objects);
            else
            {
                SelectionBox.Focus(c);
                Drag.Add(c);
            }

            Point min = SimulationService.GetMinPointInCollection(Drag.Objects);
            Preview.MouseOffset = SimulationService.Difference(Simulation.CurrentMousePos, min);
        }
    }


    private void OnCircuitObjectPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = true;
        var co = ((sender as Control)!.DataContext as CircuitObjectViewModel)!;

        if (Drag.HasObjects())
        {
            Drag.Ditch();

            if (Drag.Used)
                SelectionBox.AddCollection(Drag.Objects);
            else
                SelectionBox.Focus(co);
        }
    }


    private void OnCircuitObjectPointerEntered(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        if (Preview.HasObjects()) return;

        var co = ((sender as Control)!.DataContext as CircuitObjectViewModel)!;

    }


    private void OnCircuitObjectPointerExited(object? sender, PointerEventArgs e)
    {
        e.Handled = true;
        if (Preview.HasObjects()) return;

        var co = ((sender as Control)!.DataContext as CircuitObjectViewModel)!;

    }


    private void OnDotPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        e.Handled = true;
        var t = ((sender as Control)!.DataContext as TerminalViewModel)!;

        if (Preview.HasObjects())
        {
            if (!Preview.IsNewWire()) return;
            WireViewModel w = (Preview.Objects[0] as WireViewModel)!;

            if (w.MainOutput.IsOrphan)
                w.MainOutput = t;
            else if (w.MainInput.IsOrphan)
                w.MainInput = t;

            Preview.CommitAll();
            return;
        }

        TerminalViewModel input = t;
        TerminalViewModel output = t;

        if (t.FetchType() is TerminalType.Input)
            output = new(TerminalType.Output, null);
        else
            input = new(TerminalType.Input, null);

        WireViewModel wire = new(input, output);
        Preview.Add(wire);
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
