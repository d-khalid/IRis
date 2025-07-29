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
    private bool _isDragging = false;
    private bool removelShapePoint = false;
    private List<Point> _dragStartPositions = new();
    private List<Component> _draggedComponents = new();

    public string? PreviewCompType => _previewCompType;
    public Component? PreviewComponent => _previewComponent;

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

        // Handle multiple points with L-shape routing
        Point previousPoint = wirePreview.Points[^2];
        Point targetPoint = CalculateRoutedPoint(previousPoint, snappedMousePos, simulation, wirePreview);

        // Update the last point
        wirePreview.Points[^1] = targetPoint;

        // Add L-shape completion point if needed
        if (targetPoint != snappedMousePos && ShouldAddLShapePoint(targetPoint, snappedMousePos, simulation, wirePreview))
        {
            wirePreview.Points.Add(snappedMousePos);
            removelShapePoint = true;
        }

        wirePreview.InvalidateVisual();
        return true;
    }

    private Point CalculateRoutedPoint(Point fromPoint, Point toPoint, Simulation simulation, Wire wirePreview)
    {
        // Calculate horizontal and vertical routing options
        double deltaX = Math.Abs(toPoint.X - fromPoint.X);
        double deltaY = Math.Abs(toPoint.Y - fromPoint.Y);

        Point horizontalFirst = new Point(toPoint.X, fromPoint.Y);
        Point verticalFirst = new Point(fromPoint.X, toPoint.Y);

        // Determine preferred direction based on larger delta
        Point preferredPoint = (deltaX > deltaY) ? horizontalFirst : verticalFirst;
        Point alternatePoint = (deltaX > deltaY) ? verticalFirst : horizontalFirst;

        // Check for terminal snapping on preferred route
        Terminal? preferredSnap = simulation.FindClosestSnapTerminal(preferredPoint, 
            ComponentDefaults.TerminalSnappingRange, out Point preferredSnapPos);
        if (preferredSnap != null)
        {
            preferredPoint = SnapToGrid(preferredSnapPos);
        }

        // Check for terminal snapping on alternate route
        Terminal? alternateSnap = simulation.FindClosestSnapTerminal(alternatePoint, 
            ComponentDefaults.TerminalSnappingRange, out Point alternateSnapPos);
        if (alternateSnap != null)
        {
            alternatePoint = SnapToGrid(alternateSnapPos);
        }

        // Check path clearance and return the best option
        bool preferredClear = IsPathClearForWire(fromPoint, preferredPoint, wirePreview, simulation);
        bool alternateClear = IsPathClearForWire(fromPoint, alternatePoint, wirePreview, simulation);

        if (preferredClear)
            return preferredPoint;
        else if (alternateClear)
            return alternatePoint;
        else
            return preferredPoint; // Default to preferred even if blocked
    }

    private bool ShouldAddLShapePoint(Point routedPoint, Point finalTarget, Simulation simulation, Wire wirePreview)
    {
        // Don't add L-shape if we're already at the target
        if (routedPoint == finalTarget)
            return false;

        // Check if the second leg of the L-shape is clear
        return IsPathClearForWire(routedPoint, finalTarget, wirePreview, simulation);
    }

    private bool IsPathClearForWire(Point start, Point end, Wire wirePreview, Simulation simulation)
    {
        Console.WriteLine(simulation.WouldWireOverlapExistingWire(start, end));
        // Check if preferred direction is clear
        if (!simulation.WouldWireOverlapComponent(start, end) &&
            !simulation.WouldWireOverlapExistingWire(start, end))
        {
            return true;
        }
        else
        {
            return false; // Both directions blocked, can't move
        }
    }

    Point SnapToGrid(Point pt)
    {
        double snapX = (int)Math.Round(Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing);
        double snapY = (int)Math.Round(Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing);
        return new Point(snapX, snapY);
    }

    public void UpdateWheelPosition(Point mousePos)
    {
        // Update the preview component
        if (_previewComponent != null)
        {
            // Update rectangle
            Canvas.SetLeft(_previewComponent, mousePos.X);
            Canvas.SetTop(_previewComponent, mousePos.Y);
        }
    }

    public void HandleKeyCommand(KeyEventArgs e, List<Component> components, Canvas canvas, CommandManager commandManager)
    {
        if (_previewComponent == null) return;

        if (_previewComponent is Wire wire && e.Key == Key.Enter)
        {
            HandleWireEnterCommit(wire, components, canvas, commandManager);
            return;
        }

        HandleRotationKeys(e);
    }

    private void HandleWireEnterCommit(Wire wire, List<Component> components, Canvas canvas, CommandManager commandManager)
    {
        // Debug: Print all points
        if (wire.Points.Count >= 2)
        {
            var commitCommand = new CommitWireCommand(components, wire);
            commandManager.ExecuteCommand(commitCommand);
        }
        else
            canvas.Children.Remove(wire);

        _previewComponent = null;
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

    private void HandleRotationKeys(KeyEventArgs e)
    {
        if (_previewComponent == null) return;
        _previewComponent.Rotation = e.Key switch
        {
            Key.Right => 0,
            Key.Up => 270,
            Key.Left => 180,
            Key.Down => 90,
            _ => _previewComponent.Rotation
        };
    }

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

    public void OnPointerPressed(object? sender, PointerPressedEventArgs e, List<Component> selectedComponents)
    {
        if (selectedComponents.Any())
        {
            _isDragging = true;
            _draggedComponents = new List<Component>(selectedComponents);
            _dragStartPositions = _draggedComponents.Select(c => 
                new Point(Canvas.GetLeft(c), Canvas.GetTop(c))).ToList();
        }
    }

    public void OnPointerReleased(object? sender, PointerReleasedEventArgs e, CommandManager commandManager)
    {
        if (_isDragging && _draggedComponents.Any())
        {
            // Calculate the total movement and create a move command
            var currentPositions = _draggedComponents.Select(c => 
                new Point(Canvas.GetLeft(c), Canvas.GetTop(c))).ToList();
            
            // Check if components actually moved
            bool hasMoved = false;
            for (int i = 0; i < _dragStartPositions.Count; i++)
            {
                if (_dragStartPositions[i] != currentPositions[i])
                {
                    hasMoved = true;
                    break;
                }
            }

            if (hasMoved)
            {
                // Calculate offset from first component
                var offset = currentPositions[0] - _dragStartPositions[0];
                
                // Reset components to original positions
                for (int i = 0; i < _draggedComponents.Count; i++)
                {
                    Canvas.SetLeft(_draggedComponents[i], _dragStartPositions[i].X);
                    Canvas.SetTop(_draggedComponents[i], _dragStartPositions[i].Y);
                }

                // Create and execute move command
                var moveCommand = new MoveComponentsCommand(_draggedComponents, offset);
                commandManager.ExecuteCommand(moveCommand);
            }
        }

        _isDragging = false;
        _draggedComponents.Clear();
        _dragStartPositions.Clear();
    }
}