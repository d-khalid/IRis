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
    public SelectionManager Selection = SelectionManager.GetInstance();

    private bool _draggedObjectsMoved;


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
        PointerPoint pt = e.GetCurrentPoint(sender as Control);
        bool isNotPanningCanvas = pt.Properties.IsLeftButtonPressed &&
            !e.KeyModifiers.HasFlag(KeyModifiers.Control);


        if (isNotPanningCanvas)
        {
            e.Handled = true;

            if (Preview.HasObjects()) Preview.CommitAll();

            else
            {
                CircuitObjectViewModel? obj = Simulation.GetContainerObject(pt.Position);

                if (obj != null)    // clicked to drag
                {
                    if (obj.IsSelected)
                    {
                        foreach (CircuitObjectViewModel co in Selection.Objects)
                        {
                            Simulation.DraggedObjects.Add(co);
                        }
                    }

                    else
                    {
                        Selection.Ditch();
                        Selection.Add(obj);
                        Simulation.DraggedObjects.Add(obj);
                    }

                    _draggedObjectsMoved = false;
                    Point min = SimulationService.GetMinPointInCollection(Simulation.DraggedObjects);
                    Preview.MouseOffset = SimulationService.Difference(pt.Position, min);
                }

                else              // empty space => start selection box
                {
                    Selection.Start();
                    e.Pointer.Capture(sender as Control);   // keeps focus till released
                }
            }
        }
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        Point pt = e.GetPosition((Visual)sender!);
        Simulation.CurrentMousePos = SimulationService.SnapPointToGrid(pt);


        if (Selection.IsVisible)     // update SelectionBox bounds and select comp within range
        {
            Selection.Update();
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
            Selection.Ditch();

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
        if (Selection.IsVisible)
        {
            Selection.Hide();
            e.Pointer.Capture(null);
        }

        else if (Simulation.DraggedObjects.Count > 0)   // remove dragging references
        {
            if (_draggedObjectsMoved)
            {
                Selection.AddCollection(Simulation.DraggedObjects);
            }

            else
            {
                Selection.Ditch();
                var obj = Simulation.GetContainerObject(Simulation.CurrentMousePos);
                if (obj != null) Selection.Add(obj);
            }

            Simulation.DraggedObjects.Clear();
        }
    }


    private void OnCopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnCutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnPasteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
    private void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {}
}
