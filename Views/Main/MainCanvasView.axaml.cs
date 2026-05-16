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
        DraggedObjectsControl.IsVisible = true;
    }


    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        Simulation.IsPreviewVisible = false;
        DraggedObjectsControl.IsVisible = false;
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
                bool hasClickedObject = false;  // drag objects
                double minX = double.MaxValue;
                double minY = double.MaxValue;

                foreach (CircuitObjectViewModel co in Simulation.CircuitObjects)
                {
                    if (co is ComponentViewModel c && c.Contains(pt.Position))
                    {
                        if (c.X < minX || c.Y < minY)
                        {
                            minX = c.X;
                            minY = c.Y;
                        }

                        hasClickedObject = true;
                        Simulation.DraggedObjects.Add(co);

                        if (co.IsSelected)
                        {
                            foreach (CircuitObjectViewModel cobj in Simulation.CircuitObjects)
                            {
                                if (cobj != co && cobj.IsSelected)
                                {
                                    if (cobj is ComponentViewModel cc && (cc.X < minX || cc.Y < minY))
                                    {
                                        minX = cc.X;
                                        minY = cc.Y;
                                    }

                                    Simulation.DraggedObjects.Add(cobj);
                                }
                            }
                        }

                        break;
                    }
                }

                if (hasClickedObject)
                {
                    Simulation.PreviewMouseOffset = new Point(
                        pt.Position.X - minX, pt.Position.Y - minY
                    );
                }

                if (!hasClickedObject)            // draw selection box from empty space
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
            }

            else if (Simulation.PreviewObjects.Count > 0)   // handle CircuitObject commits
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
                        c.IsSelected = true;

                    else if (c.IsSelected && !c.Intersects(selectionBounds)) 
                        c.IsSelected = false;
                }
            }
        }

        else if (Simulation.PreviewObjects.Count > 0)    // update Component X,Y
        {
            UtilityService.SnapCollectionToPosition(
                Simulation.PreviewObjects, 
                Simulation.CurrentMousePos, 
                Simulation.PreviewMouseOffset
            );
        }

        else if (Simulation.DraggedObjects.Count > 0)
        {
            foreach (CircuitObjectViewModel co in Simulation.DraggedObjects)
            {
                co.IsSelected = false;
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
        if (SelectionBox.IsVisible)     // remove it
        {
            SelectionBox.IsVisible = false;
            SelectionBox.Width = 0;
            SelectionBox.Height = 0;
            e.Pointer.Capture(null);
        }

        else if (Simulation.DraggedObjects.Count > 0)
        {
            Simulation.DraggedObjects.Clear();
        }
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
