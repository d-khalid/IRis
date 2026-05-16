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
using System.ComponentModel;


namespace IRis.Views.Main;


public partial class MainCanvasView : UserControl
{
    public SimulationViewModel Simulation = SimulationViewModel.GetInstance();
    private Point _selectionBoxStartPt;


    public MainCanvasView()
    {
        InitializeComponent();
        DataContext = new MainCanvasViewModel();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (Simulation.Preview != null && !PreviewControl.IsVisible)
        {
            PreviewControl.IsVisible = true;
        }
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (PreviewControl.IsVisible)
        {
            PreviewControl.IsVisible = false;
        }
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint pt = e.GetCurrentPoint(sender as Control);

        if (pt.Properties.IsLeftButtonPressed &&
            !e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = true;   // disable pan chan

            if (Simulation.Preview == null)     // start SelectionBox
            {
                Simulation.UnselectAll();
                foreach (ComponentViewModel comp in Simulation.Components) 
                {
                    if (!comp.IsSelected && comp.Contains(pt.Position)) 
                    {
                        comp.IsSelected = true;
                    }
                }

                SelectionBox.IsVisible = true;
                _selectionBoxStartPt = pt.Position;
                e.Pointer.Capture(sender as Control);
            }

            else if (Simulation.Preview is ComponentViewModel c)    // commit the component
            {
                Simulation.Preview = null;
                c.Opacity = 1.0;
                Simulation.Components.Add(c);
            }
        }
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Point pt = e.GetPosition((Visual)sender!);
        Point snappedPt = UtilityService.SnapPointToGrid(pt);
        Simulation.CurrentMousePos = snappedPt;

        if (SelectionBox.IsVisible)     // update SelectionBox bounds and select comp within range
        {
            SelectionBox.Width = Math.Abs(_selectionBoxStartPt.X - pt.X);
            SelectionBox.Height = Math.Abs(_selectionBoxStartPt.Y - pt.Y);
            Canvas.SetLeft(SelectionBox, Math.Min(_selectionBoxStartPt.X, pt.X));
            Canvas.SetTop(SelectionBox, Math.Min(_selectionBoxStartPt.Y, pt.Y));


            var selectionBounds = new Rect(
                Canvas.GetLeft(SelectionBox), Canvas.GetTop(SelectionBox),
                SelectionBox.Width, SelectionBox.Height
            );

            foreach (ComponentViewModel c in Simulation.Components) 
            {
                if (!c.IsSelected && c.Intersects(selectionBounds))
                {
                    c.IsSelected = true;
                }

                else if (c.IsSelected && !c.Intersects(selectionBounds)) 
                {
                    c.IsSelected = false;
                }
            }
        }

        else if (Simulation.Preview is ComponentViewModel c)    // update Component X,Y
        {
            c.X = pt.X;
            c.Y = pt.Y;
        }
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (SelectionBox.IsVisible)     // remove it
        {
            SelectionBox.IsVisible = false;
            SelectionBox.Width = 0;
            SelectionBox.Height = 0;
            e.Pointer.Capture(null);
        }
    }


    private void OnKeyDown(object? sender, KeyEventArgs e) 
    {
        if (e.Key == Key.Delete)
        {
            for (int i = Simulation.Components.Count-1; i >= 0; i--)
            {
                ComponentViewModel c = Simulation.Components[i];
                if (c.IsSelected)
                    Simulation.Components.Remove(c);
            }

            for (int i = Simulation.Wires.Count-1; i >= 0; i--)
            {
                WireViewModel w = Simulation.Wires[i];
                if (w.IsSelected)
                    Simulation.Wires.Remove(w);
            }
        }
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
