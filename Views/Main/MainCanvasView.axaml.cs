using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using System;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.ViewModels.Main;
using IRis.Services;
using IRis.ViewModels;
using IRis.ViewModels.Circuit;


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
        if (Simulation.PreviewObjects.Count > 0 && !Simulation.IsPreviewVisible)
        {
            Simulation.IsPreviewVisible = true;
        }
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (Simulation.IsPreviewVisible)
        {
            Simulation.IsPreviewVisible = false;
        }
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint pt = e.GetCurrentPoint(sender as Control);

        if (pt.Properties.IsLeftButtonPressed &&
            !e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = true;   // disable pan chan

            if (Simulation.PreviewObjects.Count == 0)     // start SelectionBox
            {
                Simulation.UnselectAll();
                foreach (CircuitObjectViewModel co in Simulation.CircuitObjects) 
                {
                    if (co is ComponentViewModel c && !c.IsSelected && c.Contains(pt.Position))
                    {
                        c.IsSelected = true;
                    }
                }

                SelectionBox.IsVisible = true;
                _selectionBoxStartPt = pt.Position;
                e.Pointer.Capture(sender as Control);
            }

            else if (Simulation.PreviewObjects.Count > 0)   // handle CircuitObject commits
            {
                foreach (CircuitObjectViewModel co in Simulation.PreviewObjects)
                {
                    if (co is ComponentViewModel c)
                    {
                        ComponentViewModel clone = UtilityService.Clone(c);
                        clone.Opacity = 1.0;
                        Simulation.CircuitObjects.Add(clone);
                    }
                }
            }
        }
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Point pt = e.GetPosition((Visual)sender!);
        Simulation.CurrentMousePos = new Point((int)pt.X, (int)pt.Y);


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

            foreach (CircuitObjectViewModel co in Simulation.CircuitObjects) 
            {
                if (co is ComponentViewModel c)
                {
                    if (!c.IsSelected && c.Intersects(selectionBounds))
                        c.IsSelected = true;

                    else if (c.IsSelected && !c.Intersects(selectionBounds)) 
                        c.IsSelected = false;
                }
            }
        }

        else if (Simulation.PreviewObjects.Count > 0)    // update Component X,Y
        {
            UtilityService.SnapCollectionToPosition(
                Simulation.PreviewObjects, Simulation.CurrentMousePos
            );
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


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
