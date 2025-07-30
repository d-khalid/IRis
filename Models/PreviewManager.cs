using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Models.Commands;

namespace IRis.Models;

internal class PreviewManager
{
    private string? _previewCompType;
    private Component? _previewComponent;
    private bool removelShapePoint = false;

    public string? PreviewCompType => _previewCompType;
    public Component? PreviewComponent => _previewComponent;

    // Invoked externally by Simulation.cs
    public void SetPreviewComponent(string? value, Canvas canvas, Point mousePos, Simulation simulation)
    {
        _previewCompType = value;
        // Remove the old preview comp if it exists
        if (_previewComponent != null)
        {
            canvas.Children.Remove(_previewComponent);
            _previewComponent = null;
        }

        // Create and add new component if value is provided
        if (!string.IsNullOrEmpty(value))
        {
            _previewComponent = Component.Create(value);
            if (_previewComponent != null)
            {
                PositionPreviewComponent(mousePos);
                canvas.Children.Add(_previewComponent);

                Console.WriteLine("Added component via setter");
            }
        }
    }

    private void PositionPreviewComponent(Point mousePos)
    {
        if (_previewComponent == null) return;
        // TODO: fix this initial positioning of wire preview point
        if (_previewComponent is Wire wire)
        {
            wire.AddPoint(mousePos);
            Canvas.SetLeft(wire, 0);
            Canvas.SetTop(wire, 0);
        }
        // Place the component outside the user's view
        else
        {
            Canvas.SetLeft(_previewComponent,
            -ComponentDefaults.DefaultWidth - ComponentDefaults.TerminalWireLength * 2);
            Canvas.SetTop(_previewComponent, 0);
        }
    }

    // Invoked externally by Simulation.cs
    public bool HandleCommit(object? sender, PointerPressedEventArgs? e, List<Component> components,
        Canvas canvas, Point mousePos, CommandManager commandManager, Simulation simulation)
    {
        if (_previewComponent is Wire wirePreview)
        {
            return HandleWireCommit(sender, e, components, wirePreview, mousePos, simulation, commandManager);
        }

        // Commit component on click
        if (_previewComponent != null)
        {
            return HandleComponentCommit(canvas, components, mousePos, commandManager);
        }

        return false; // Continue
    }

    private bool HandleWireCommit(object? sender, PointerPressedEventArgs? e, List<Component> components,
        Wire wirePreview, Point mousePos, Simulation simulation, CommandManager commandManager)
    {
        if (e == null) return true;
        Terminal? target = simulation.FindClosestSnapTerminal(mousePos, ComponentDefaults.TerminalSnappingRange, out var pos);

        if (target != null)     // Condition: Wire is starting from a gate terminal
        {
            target.AddWire(wirePreview);
        }
        else if (simulation.FindWireAtPosition(mousePos) != null) // Edit this to reject is wire is on top of a wire 
        {
            Console.WriteLine($"Wire rejected due to being on top of another wire.");
            return false;       // Couldn't handle
        }

        if (removelShapePoint)
            {
                wirePreview.Points.RemoveAt(wirePreview.Points.Count - 1);
                removelShapePoint = false;
            }

        // Use command for adding point
        pos = SnapToGrid(pos);
        var addPointCommand = new AddWirePointCommand(wirePreview, pos);
        commandManager.ExecuteCommand(addPointCommand);

        var point = e.GetCurrentPoint(sender as Control);
        // Commits the WIRE ON DOUBLE-CLICK, or RIGHT-CLICK
        if (wirePreview.Points.Count >= 2 && (point.Properties.IsRightButtonPressed || e.ClickCount >= 2))
        {
            // Snap to grid all points
            for (int i = 0; i < wirePreview.Points.Count - 1; i++)
            {
                if (wirePreview.Points[i] != new Point(-1, -1))
                    wirePreview.Points[i] = SnapToGrid(wirePreview.Points[i]);
            }
            // Remove duplicates
            wirePreview.Points = RemoveDuplicatePoints(wirePreview.Points);
            var commitCommand = new CommitWireCommand(components, wirePreview);
            commandManager.ExecuteCommand(commitCommand);
            _previewComponent = null;
            simulation.PreviewCompType = "WIRE";   // Keep placing wires
        }

        return true;
    }

    private bool HandleComponentCommit(Canvas canvas, List<Component> components, Point mousePos, CommandManager commandManager)
    {
        if (string.IsNullOrEmpty(_previewCompType)) return true;
        Component? component = Component.Create(_previewCompType);
        if (component == null) return true;

        if (_previewComponent != null)
        {
            component.Rotation = _previewComponent.Rotation;
        }
        
        Point position = new Point(
            Math.Round(mousePos.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing,
            Math.Round(mousePos.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing
        );
        // Add command for undo/redo
        var addCommand = new AddComponentCommand(canvas, components, component, position);
        commandManager.ExecuteCommand(addCommand);

        Console.WriteLine($"{_previewCompType} committed!");
        return true;
    }

    // Invoked externally by Simulation.cs
    public bool HandleUpdate(Canvas canvas, Point mousePos, bool snapToGridEnabled,
        Func<Point, Point> snapToGrid, Simulation simulation)
    {
        // For wires only
        if (_previewComponent is Wire wirePreview)
        {
            return HandleWireUpdate(wirePreview, mousePos, snapToGridEnabled, simulation);
        }

        else if (_previewComponent != null) // Update the non-wire preview component
        {
            Point pos = snapToGridEnabled ? snapToGrid(mousePos) : mousePos;
            Canvas.SetLeft(_previewComponent, pos.X);
            Canvas.SetTop(_previewComponent, pos.Y);
            return true; // Terminate
        }

        return false; // Continue
    }

    private bool HandleWireUpdate(Wire wirePreview, Point mousePos, bool snapToGridEnabled, Simulation simulation)
    {
        // TODO: Remake this function
        if (wirePreview.Points.Count == 0) return true;

        // Clean up temporary L-shape point if it exists
        if (removelShapePoint)
        {
            wirePreview.Points.RemoveAt(wirePreview.Points.Count - 1);
            removelShapePoint = false;
        }

        Point snappedMousePos = SnapToGrid(mousePos);

        // Handle single point - just add the second point directly
        if (wirePreview.Points.Count == 1)
        {
            wirePreview.Points[^1] = snappedMousePos;
            wirePreview.InvalidateVisual();
            return true;
        }
        Point targetPoint = snappedMousePos;

        // Update the last point
        wirePreview.Points[^1] = targetPoint;

        wirePreview.InvalidateVisual();
        return true;
    }

    Point SnapToGrid(Point pt)
    {
        double snapX = (int)Math.Round(Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing);
        double snapY = (int)Math.Round(Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing);
        return new Point(snapX, snapY);
    }

    public void StartWireExtension(Wire existingWire, Canvas canvas, Point clickPoint,
                                    List<Component> components, List<Component> MovedWires)
    {
        Console.WriteLine("Starting wire extension");
        existingWire.IsBeingEdited = true; 
        components.Remove(existingWire);
        _previewComponent = existingWire;

        // Add a break point
        removelShapePoint = false;
        existingWire.Points.Add(new Point(-1, -1));
        // Get the closest point on the line segment instead of using click point
        clickPoint = SnapToGrid(clickPoint);
        existingWire.Points.Add(clickPoint);
        existingWire.Points.Add(clickPoint); // Duplicate for dragging
        
        existingWire.InvalidateVisual();
    }

    public static List<Point> RemoveDuplicatePoints(List<Point> points)
    {
        // Only remove adjacent duplicates
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (points[i] == points[i + 1])
            {
                points.RemoveAt(i + 1);
            }
        }
        return points;
    }
    
    // ________________________________________________
    // __________ Pointer/Key Event Handling __________
    // ________________________________________________

    public void OnExit()
    {
        // Hide the preview component
        if (_previewComponent != null)
            _previewComponent.Opacity = 0.0;
    }

    public void OnEnter()
    {
        // Unhide the preview component
        if (_previewComponent != null)
            _previewComponent.Opacity = 1.0;
    }
}