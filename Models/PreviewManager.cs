using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models;

internal class PreviewManager
{
    private string? _previewCompType;
    private Component? _previewComponent;

    public string? PreviewCompType => _previewCompType;

    public void SetPreviewComponent(string? value, Canvas canvas, Point mousePos)
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
        // TODO: THIS IS HACKY
        if (_previewComponent == null) return;
        if (_previewComponent is Wire wire)
        {
            wire.AddPoint(mousePos);
            Canvas.SetLeft(wire, 0);
            Canvas.SetTop(wire, 0);
        }
        else
        {
            Canvas.SetLeft(_previewComponent, mousePos.X);
            Canvas.SetTop(_previewComponent, mousePos.Y);
        }
    }

    public bool HandleCommit(object? sender, PointerPressedEventArgs? e, List<Component> components, 
        Canvas canvas, Point mousePos, Simulation simulation)
    {
        // Handle wire preview commit
        if (_previewComponent is Wire wirePreview)
        {
            return HandleWireCommit(sender, e, components, wirePreview, mousePos, simulation);
        }

        // Commit component on click
        if (_previewComponent != null)
        {
            return HandleComponentCommit(canvas, components, mousePos);
        }

        return false; // Continue
    }

    private bool HandleWireCommit(object? sender, PointerPressedEventArgs? e, List<Component> components,
        Wire wirePreview, Point mousePos, Simulation simulation)
    {
        if (e == null) return true; // Can't handle wire commit without event args
        // Check for a terminal we can snap to
        Terminal? target = simulation.FindClosestSnapTerminal(mousePos, ComponentDefaults.TerminalSnappingRange, out var pos);

        if (target != null)
            target.Wire = wirePreview;

        wirePreview.AddPoint(pos);

        // If RIGHT-CLICK or DOUBLE-CLICK, then commit the wire
        // Finalize wire if it has at least 2 points
        var point = e.GetCurrentPoint(sender as Control);
        if (wirePreview.Points.Count >= 2 && (point.Properties.IsRightButtonPressed || e.ClickCount >= 2))
        {
            components.Add(wirePreview);
            _previewComponent = null;
        }

        return true; // Terminate
    }

    private bool HandleComponentCommit(Canvas canvas, List<Component> components, Point mousePos)
    {
        if (string.IsNullOrEmpty(_previewCompType)) return true;    // Terminate
        Component? component = Component.Create(_previewCompType);
        if (component == null) return true; // Terminate

        if (_previewComponent != null)
        {
            component.Rotation = _previewComponent.Rotation;
        } // If _previewComponent is null, component.Rotation will use its default value
        Canvas.SetLeft(component, mousePos.X);
        Canvas.SetTop(component, mousePos.Y);

        canvas.Children.Add(component);
        components.Add(component);
        Console.WriteLine($"{_previewCompType} committed!");

        return true; // Terminate
    }

    public bool HandleUpdate(Canvas canvas, Point mousePos, bool snapToGridEnabled, 
        Func<Point, Point> snapToGrid, Simulation simulation)
    {
        // For wires only
        if (_previewComponent is Wire wirePreview)
        {
            return HandleWireUpdate(wirePreview, mousePos, simulation);
        }

        // Update the non-wire preview component
        if (_previewComponent != null)
        {
            Point pos = snapToGridEnabled ? snapToGrid(mousePos) : mousePos;
            Canvas.SetLeft(_previewComponent, pos.X);
            Canvas.SetTop(_previewComponent, pos.Y);
            return true; // Terminate
        }

        return false; // Continue
    }

    private bool HandleWireUpdate(Wire wirePreview, Point mousePos, Simulation simulation)
    {
        if (wirePreview.Points.Count > 0)
        {
            // Make wires snap to the closest terminal in range
            Terminal? snap = simulation.FindClosestSnapTerminal(mousePos, ComponentDefaults.TerminalSnappingRange, out Point pos);
            wirePreview.Points[^1] = pos;
            wirePreview.InvalidateVisual();
        }
        return true; // Terminate
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

    public void HandleKeyCommand(KeyEventArgs e, List<Component> components, Canvas canvas)
    {
        // Rotating wires is a terrible idea so no to that
        if (_previewComponent == null) return; // terminate

        // Press ENTER to commit a wire)
        if (_previewComponent is Wire wire && e.Key == Key.Enter)
        {
            HandleWireEnterCommit(wire, components, canvas);
            return; // Terminate
        }

        HandleRotationKeys(e);
    }

    private void HandleWireEnterCommit(Wire wire, List<Component> components, Canvas canvas)
    {
        // Finalize wire if it has at least 2 points
        if (wire.Points.Count >= 2)
            components.Add(wire);
        else
            canvas.Children.Remove(wire);

        _previewComponent = null;
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
}