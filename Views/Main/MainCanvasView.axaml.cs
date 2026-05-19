using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using System;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.ViewModels.Main;
using IRis.Services;
using IRis.ViewModels;
using IRis.ViewModels.Circuit;
using IRis.Models.Core;
using IRis.ViewModels.Circuit.CircuitObjects;


namespace IRis.Views.Main;


public partial class MainCanvasView : UserControl
{
    public SimulationManager Simulation = SimulationManager.GetInstance();
    public PreviewManager Preview = PreviewManager.GetInstance();
    private Point _selectionBoxStartPt;
    private bool _draggedObjectsMoved;


    public MainCanvasView()
    {
        InitializeComponent();
        DataContext = new MainCanvasViewModel();
    }


    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        Preview.SetVisible(true);
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        Preview.SetVisible(false);
    }


    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PointerPoint pt = e.GetCurrentPoint(sender as Control);
        bool notPanningCanvas = pt.Properties.IsLeftButtonPressed &&
            !e.KeyModifiers.HasFlag(KeyModifiers.Control);


        if (notPanningCanvas)
        {
            e.Handled = true;

            if (Preview.HasObjects()) Preview.Commit();

            else
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
                        Simulation.UnselectAll();
                        Simulation.SelectObject(obj);
                        Simulation.DraggedObjects.Add(obj);
                    }

                    _draggedObjectsMoved = false;
                    Point min = SimulationService.GetMinPointInCollection(Simulation.DraggedObjects);
                    Preview.MouseOffset = SimulationService.Difference(pt.Position, min);
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
        }
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Point pt = e.GetPosition((Visual)sender!);
        Simulation.CurrentMousePos = SimulationService.SnapPointToGrid(pt);


        if (SelectionBox.IsVisible)     // update SelectionBox bounds and select comp within range
        {
            SimulationService.SetObjectBounds(
                obj: SelectionBox,
                width: Math.Abs(_selectionBoxStartPt.X - pt.X),
                height: Math.Abs(_selectionBoxStartPt.Y - pt.Y),
                x: Math.Min(_selectionBoxStartPt.X, pt.X),
                y: Math.Min(_selectionBoxStartPt.Y, pt.Y)
            );

            var selectionBounds = SimulationService.GetObjectBounds(SelectionBox);

            foreach (CircuitObjectViewModel co in Simulation.Objects) 
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

        else if (Preview.HasObjects())
        {
            SimulationService.SnapCollectionToPosition(
                Preview.Objects, 
                Simulation.CurrentMousePos, 
                Preview.MouseOffset
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

            SimulationService.SnapCollectionToPosition(
                Simulation.DraggedObjects,
                Simulation.CurrentMousePos,
                Preview.MouseOffset
            );

            _draggedObjectsMoved = true;
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
            if (_draggedObjectsMoved)
            {
                foreach (CircuitObjectViewModel co in Simulation.DraggedObjects)
                {
                    Simulation.SelectObject(co);
                }
            }

            else
            {
                Simulation.UnselectAll();
                var obj = Simulation.GetContainerObject(Simulation.CurrentMousePos);
                if (obj != null) Simulation.SelectObject(obj);
            }

            Simulation.DraggedObjects.Clear();
        }
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
