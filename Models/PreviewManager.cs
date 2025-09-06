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
using System.Runtime.Serialization;

namespace IRis.Models;

internal class PreviewManager
{
    private string? _previewCompType;
    private Component? _previewComponent;
    private bool IsCornerPointAdded = false;
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
        Point snappedMousePos = SnapToGrid(mousePos);
        if (_previewComponent == null) return;
        // TODO: fix this initial positioning of wire preview point
        if (_previewComponent is Wire wire)
        {
            wire.AddPoint(snappedMousePos);
            Canvas.SetLeft(wire, 0);
            Canvas.SetTop(wire, 0);
        }
        // Place the component outside the user's view
        else
        {
            Canvas.SetLeft(_previewComponent, snappedMousePos.X);
            Canvas.SetTop(_previewComponent, snappedMousePos.Y);
        }
    }

    // Invoked externally by Simulation.cs
    public void HandleWireCommit(object? sender, PointerPressedEventArgs? e, Point CurrentMousePos, Simulation simulation)
    {   // Returns true if event is handled
        // Explicit type conversion to avoid exception
        if (e == null) return; 
        Wire wirePreview = (Wire)_previewComponent!;
        if (!wirePreview.IsValid) return;

        Terminal? target = simulation.FindClosestSnapTerminal(CurrentMousePos);
        // Add wire to the terminal
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
        // Remove the endpoint (after the corner point) if present
        if (IsCornerPointAdded)
        {
            wirePreview.Points.RemoveAt(wirePreview.Points.Count - 1);
            IsCornerPointAdded = false;
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
        // If there is a terminal and snapping rejected it
        bool condition1 = terminal != null && !pointSnappedToTerminal;
        // Their names should be self-explanatory
        bool condition2 = simulation.IsPointInsideAnyComponent(targetPoint);
        // If Wire is not snapped to terminal and overlaps another wire
        // The snapping handles used input terminals so this works.
        bool condition3 = !pointSnappedToTerminal &&
            !simulation.DoesWireHaveExtension(wirePreview) &&
            simulation.DoesWireOverlapAnotherWire(wirePreview.Points);
        bool condition4 = !simulation.DoesWireHaveExtension(wirePreview) &&
            simulation.DoesWireSelfOverlap(wirePreview.Points);
        // If snapping was rejected and wire crosses a terminal
        List<Point> tempPoints = [.. wirePreview.Points];
        tempPoints[^1] = targetPoint;
        // Exclude the starting terminal
        Terminal? exceptionCase = simulation.FindClosestSnapTerminal(wirePreview.Points[0]);
        bool condition5 = !pointSnappedToTerminal &&
            !simulation.DoesWireHaveExtension(wirePreview) &&
            simulation.DoesWireCrossTerminal(tempPoints, exceptionCase);

        if (condition1 || condition2 || condition3 || condition4 || condition5)
        {   // PATCH: Annihiliate the wire completely
            if (condition1) Console.WriteLine("Wire cannot be drawn on a used input terminal, annihiliating it...");
            else if (condition2) Console.WriteLine("Wire cannot be drawn on a component, annihiliating it...");
            else if (condition3) Console.WriteLine("Wire cannot overlap another wire, annihiliating it...");
            else if (condition4) Console.WriteLine("Wire cannot self overlap, annihiliating it...");
            else if (condition5) Console.WriteLine("Wire cannot cross a terminal, annihiliating it...");
            wirePreview.IsValid = false;
        }
        else
        {
            wirePreview.IsValid = true;
        }
        // If target point is non-orthogonal relative to the last point
        if (wirePreview.Points.Count >= 2 &&
            targetPoint.X != wirePreview.Points[^2].X && targetPoint.Y != wirePreview.Points[^2].Y)
        {   // Build an othogonal wire
            IsCornerPointAdded = true;
            double dx = Math.Abs(targetPoint.X - wirePreview.Points[^2].X);
            double dy = Math.Abs(targetPoint.Y - wirePreview.Points[^2].Y);
            // Prefer the shorter distance for the Corner Point
            if (dx < dy) targetPoint = new Point(targetPoint.X, wirePreview.Points[^2].Y);
            else targetPoint = new Point(wirePreview.Points[^2].X, targetPoint.Y);

            wirePreview.Points[^1] = targetPoint;
            Console.WriteLine("Corner Point added " + targetPoint);

            // Handle the Closest to mouse point, its there for preview
            Point closestPoint = snappedMousePos;
            // Snap the closest point too
            Terminal? terminal2 = simulation.FindClosestSnapTerminal(closestPoint);
            if (terminal2 != null) closestPoint = simulation.GetAbsoluteTerminalPosition(terminal2);
            wirePreview.Points.Add(closestPoint);

            // Since we have a new type of Wire, we check for the conditions again.
            bool condition6 = simulation.IsPointInsideAnyComponent(targetPoint);
            bool condition7 = simulation.FindClosestSnapTerminal(wirePreview.Points[^1]) == null &&
                simulation.DoesWireOverlapAnotherWire(wirePreview.Points);
            bool condition8 = simulation.IsWireSupersetOfAnotherWire(wirePreview.Points);

            if (condition6 || condition7 || condition8)
            {
                if (condition6) Console.WriteLine("Corner Point cannot be drawn on a component, annihiliating it...");
                else if (condition7) Console.WriteLine("Orthogonal Wire cannot overlap another wire, annihiliating it...");
                else if (condition8) Console.WriteLine("Wire cannot be a superset of another wire, annihiliating it...");
                wirePreview.IsValid = false;
            }
            else
            {
                wirePreview.IsValid = true;
            }
        }
        else    // target point is orthogonal to the last point, make a straight line
        {
            wirePreview.Points[^1] = targetPoint;
        }
        // foreach (Point point in wirePreview.Points) Console.WriteLine(point);
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
        existingWire.IsCommitted = false;
        simulation.Components.Remove(existingWire);
        _previewComponent = existingWire;
        Wire wirePreview = (Wire)_previewComponent!;
        
        // Add a break point
        wirePreview.Points.Add(new Point(-1, -1));
        // Get the closest point on the line segment instead of using click point
        clickPoint = SnapToGrid(clickPoint);
        wirePreview.Points.Add(clickPoint);
        wirePreview.Points.Add(clickPoint); // Double for dragging
        wirePreview.Points.Add(clickPoint); // Triple for Corner Point
        // var addPointCommand = new AddWirePointCommand(wirePreview, CurrentMousePos);
        // simulation.CommandManager.ExecuteCommand(addPointCommand);

        wirePreview.InvalidateVisual();
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