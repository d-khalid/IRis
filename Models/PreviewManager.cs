using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Models.Commands;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace IRis.Models;

internal class PreviewManager
{
    private string? _previewCompType;
    private Component? _previewComponent;

    public string? PreviewCompType => _previewCompType;     // in case of null, value is set to "NULL"
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
            _previewComponent = Component.Create(value, simulation);
            if (_previewComponent != null)
            {
                PositionPreviewComponent(mousePos);
                canvas.Children.Add(_previewComponent);

                Console.WriteLine("Added component via SetPreviewComponent");
            }
        }
    }

    // Helper for SetPreviewComponent
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
    public void HandleWireCommit(object? sender, PointerPressedEventArgs? e, Point CurrentMousePos, Simulation simulation)
    {   // Returns true if event is handled
        // Explicit type conversion to avoid exception
        if (e == null) return; 
        Wire wirePreview = (Wire)_previewComponent!;

        Terminal? target = simulation.FindClosestSnapTerminal(CurrentMousePos);
        // Condition: Wire is starting from a component terminal
        if (target != null) target.AddWire(wirePreview);

        // Use command for adding point
        CurrentMousePos = SnapToGrid(CurrentMousePos);
        var addPointCommand = new AddWirePointCommand(wirePreview, CurrentMousePos);
        simulation.CommandManager.ExecuteCommand(addPointCommand);

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
            var commitCommand = new CommitWireCommand(simulation.Components, wirePreview);
            simulation.CommandManager.ExecuteCommand(commitCommand);
            _previewComponent = null;
            simulation.PreviewCompType = "WIRE";   // Keep placing wires
        }
    }

    public bool HandleComponentCommit(Canvas canvas, List<Component> components, Point mousePos, CommandManager commandManager, Simulation simulation)
    {
        if (string.IsNullOrEmpty(_previewCompType)) return true;
        Component? component = Component.Create(_previewCompType, simulation);
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

    // Helper for HandleUpdate
    private bool HandleWireUpdate(Wire wirePreview, Point mousePos, bool snapToGridEnabled, Simulation simulation)
    {
        if (wirePreview.Points.Count == 0) return true;

        Point snappedMousePos = SnapToGrid(mousePos);

        // Handle single point - just add the second point directly
        if (wirePreview.Points.Count == 1)
        {
            wirePreview.Points[^1] = snappedMousePos;
            wirePreview.InvalidateVisual();
            return true;
        }
        Point targetPoint = snappedMousePos;

        bool pointSnappedToTerminal = false;
        Terminal? terminal = simulation.FindClosestSnapTerminal(targetPoint);
        if (terminal != null)
        {
            Point temp = simulation.GetAbsoluteTerminalPosition(terminal);

            if (simulation.FindWireAtPosition(temp) == null ||
                (simulation.FindWireAtPosition(temp) != null &&
                !simulation.IsInputTerminal(terminal)))
            {   // If no wire is on the terminal OR (If wire is on the terminal AND is not an input terminal)
                targetPoint = temp;
                pointSnappedToTerminal = true;
            }
        }
        // If there is a terminal
        if (terminal != null && !pointSnappedToTerminal)// snappedMousePos == simulation.GetAbsoluteTerminalPosition(terminal))
        {   // PATCH: Annihiliate the wire completely
            Console.WriteLine("Wire cannot be drawn on a used input terminal, annihiliating it...");
            _previewComponent = null;
            simulation.PreviewCompType = "WIRE";   // Keep the wire preview dot
            wirePreview.Points.Clear();
        }
        else
        {
            wirePreview.Points[^1] = targetPoint;
        }

        wirePreview.InvalidateVisual();
        return true;
    }

    Point SnapToGrid(Point pt)
    {
        double snapX = (int)Math.Round(Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing);
        double snapY = (int)Math.Round(Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing);
        return new Point(snapX, snapY);
    }

    public void StartWireExtension(Point clickPoint, Wire existingWire,  Simulation simulation)
    {
        existingWire.IsBeingEdited = true;
        simulation.Components.Remove(existingWire);
        _previewComponent = existingWire;

        // Add a break point
        existingWire.Points.Add(new Point(-1, -1));
        // Get the closest point on the line segment instead of using click point
        clickPoint = SnapToGrid(clickPoint);
        existingWire.Points.Add(clickPoint);
        // existingWire.Points.Add(clickPoint); // Duplicate for dragging
        
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

    // Hide the preview component if the pointer leaves the canvas
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