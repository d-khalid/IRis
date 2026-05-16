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
        Simulation.IsPreviewVisible = true;
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        Simulation.IsPreviewVisible = false;
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint pt = e.GetCurrentPoint(sender as Control);

        if (pt.Properties.IsLeftButtonPressed &&
            !e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = true;   // disable pan chan

            if (Simulation.PreviewObjects.Count == 0)
            {
                CircuitObjectViewModel? obj = Simulation.GetContainerObject(pt.Position);

                if (obj != null)    // clicked to drag
                {
                    if (obj.IsSelected)
                    {
                        foreach (CircuitObjectViewModel co in Simulation.SelectedObjects)
                        {
                            Simulation.DraggedObjects.Add(co);
                        }
                    }

                    else
                    {
                        Simulation.DraggedObjects.Add(obj);
                    }

                    Point min = UtilityService.GetMinPointInCollection(Simulation.DraggedObjects);
                    Simulation.PreviewMouseOffset = UtilityService.Difference(pt.Position, min);
                }

                else              // empty space => start selection box
                {
                    Simulation.UnselectAll();

                    var c = Simulation.GetContainerObject(pt.Position);
                    if (c != null) Simulation.SelectObject(c);

                    SelectionBox.IsVisible = true;
                    _selectionBoxStartPt = pt.Position;
                    e.Pointer.Capture(sender as Control);
                }
            }

            else               // handle CircuitObject commits
            {
                foreach (CircuitObjectViewModel co in Simulation.PreviewObjects)
                {
                    if (co is ComponentViewModel c)
                    {
                        ComponentViewModel clone = CloningService.Clone(c);
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
        Simulation.CurrentMousePos = UtilityService.SnapPointToGrid(pt);


        if (SelectionBox.IsVisible)     // update SelectionBox bounds and select comp within range
        {
            UtilityService.SetObjectBounds(
                obj: SelectionBox,
                width: Math.Abs(_selectionBoxStartPt.X - pt.X),
                height: Math.Abs(_selectionBoxStartPt.Y - pt.Y),
                x: Math.Min(_selectionBoxStartPt.X, pt.X),
                y: Math.Min(_selectionBoxStartPt.Y, pt.Y)
            );

            var selectionBounds = UtilityService.GetObjectBounds(SelectionBox);

            foreach (CircuitObjectViewModel co in Simulation.CircuitObjects) 
            {
                if (co is ComponentViewModel c)
                {
                    if (!c.IsSelected && c.Intersects(selectionBounds))
                    {
                        Simulation.SelectObject(c);
                    }

                    else if (c.IsSelected && !c.Intersects(selectionBounds))
                    {
                        Simulation.UnselectObject(c);
                    }
                }
            }
        }

        else if (Simulation.PreviewObjects.Count > 0)    // update Preview X,Y
        {
            UtilityService.SnapCollectionToPosition(
                Simulation.PreviewObjects, 
                Simulation.CurrentMousePos, 
                Simulation.PreviewMouseOffset
            );
        }

        else if (Simulation.DraggedObjects.Count > 0)    // update Dragged X,Y
        {
            if (Simulation.DraggedObjects[0].IsSelected)
            {
                foreach (CircuitObjectViewModel co in Simulation.DraggedObjects)
                {
                    Simulation.UnselectObject(co);
                }
            }

            UtilityService.SnapCollectionToPosition(
                Simulation.DraggedObjects,
                Simulation.CurrentMousePos,
                Simulation.PreviewMouseOffset
            );
        }
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (SelectionBox.IsVisible)                     // remove selectionBox
        {
            SelectionBox.IsVisible = false;
            SelectionBox.Width = 0;
            SelectionBox.Height = 0;
            e.Pointer.Capture(null);
        }

        else if (Simulation.DraggedObjects.Count > 0)   // remove dragging references
        {
            Simulation.DraggedObjects.Clear();
        }
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
