using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Models.Commands;

namespace IRis.Models;

public class PreviewManager
{
    private string? _previewCompType;
    private Component? _previewComponent;
    private IRis.Models.Components.Wire? _previewWire;
    public string? PreviewCompType
    {
        get => _previewCompType ?? "NULL";
        set => _previewCompType = value;
    } 
    public Component? PreviewComponent
    {
        get => _previewComponent;
        set => _previewComponent = value;
    }

    public void ClearPreview(Canvas canvas)
    {
        if (_previewComponent != null)
        {
            canvas.Children.Remove(_previewComponent);
            _previewComponent = null;
        }

        if (_previewWire != null)
        {
            canvas.Children.Remove(_previewWire);
            _previewWire = null;
        }

        _previewCompType = null;
    }

    public void SetPreviewComponent(string? value, Canvas canvas, Point mousePos, Simulation simulation)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
        {
            ClearPreview(canvas);
            return;
        }
        _previewCompType = value;

        // Special-case wire preview (Wire is not a Component)
        if (value.Equals("WIRE", StringComparison.OrdinalIgnoreCase))
        {
            if (_previewWire != null)
                canvas.Children.Remove(_previewWire);

            _previewWire = new IRis.Models.Components.Wire();
            _previewWire.IsBeingEdited = true;
            // place the wire at origin and set two identical points for preview
            Canvas.SetLeft(_previewWire, 0);
            Canvas.SetTop(_previewWire, 0);
            _previewWire.Points.Add(mousePos);
            _previewWire.Points.Add(mousePos);
            canvas.Children.Add(_previewWire);
            return;
        }

        if (!CircuitComponentFactory.IsSupportedComponentType(value))
        {
            ClearPreview(canvas);
            return;
        }

        if (_previewComponent != null)
        {
            canvas.Children.Remove(_previewComponent);
        }

        _previewComponent = CircuitComponentFactory.Create(value);

        if (_previewComponent == null)
            return;

        PositionPreviewComponent(mousePos);
        canvas.Children.Add(_previewComponent);
    }

    private void PositionPreviewComponent(Point mousePos)
    {
        if (_previewComponent == null) return;
        Canvas.SetLeft(_previewComponent, mousePos.X);
        Canvas.SetTop(_previewComponent, mousePos.Y);
    }

    public bool HandleComponentCommit(Canvas canvas, List<Component> components, Point mousePos, CommandManager commandManager, Simulation simulation)
    {
        if (string.IsNullOrWhiteSpace(_previewCompType))
            return false;

        // If preview is a wire, commit the preview wire into the canvas as a permanent wire
        if (_previewCompType.Equals("WIRE", StringComparison.OrdinalIgnoreCase) && _previewWire != null)
        {
            var wire = new IRis.Models.Components.Wire();
            // copy points
            foreach (var p in _previewWire.Points)
                wire.Points.Add(p);

            wire.IsBeingEdited = false;
            wire.IsValid = true;

            // Add to canvas as a permanent visual
            Canvas.SetLeft(wire, 0);
            Canvas.SetTop(wire, 0);
            canvas.Children.Add(wire);

            // remove preview
            canvas.Children.Remove(_previewWire);
            _previewWire = null;
            _previewCompType = null;
            return true;
        }

        Component? component = CircuitComponentFactory.Create(_previewCompType);
        if (component == null)
            return false;

        if (_previewComponent != null)
        {
            component.Orientation = _previewComponent.Orientation;
        }
        
        Point position = simulation.SnapToGridEnabled
            ? SnapToGrid(mousePos)
            : mousePos;

        var addCommand = new AddComponentCommand(canvas, components, component, position);
        commandManager.ExecuteCommand(addCommand);

        PositionPreviewComponent(mousePos);
        return true;
    }

    public bool HandleUpdate(Canvas canvas, Point mousePos, bool snapToGridEnabled,
        Func<Point, Point> snapToGrid, Simulation simulation)
    {
        if (_previewWire != null)
        {
            // update preview wire last point
            Point pos = snapToGridEnabled ? snapToGrid(mousePos) : mousePos;
            if (_previewWire.Points.Count == 0)
            {
                _previewWire.Points.Add(pos);
                _previewWire.Points.Add(pos);
            }
            else
            {
                _previewWire.Points[_previewWire.Points.Count - 1] = pos;
            }
            _previewWire.InvalidateVisual();
            return true;
        }

        if (_previewComponent != null)
        {
            Point pos = snapToGridEnabled ? snapToGrid(mousePos) : mousePos;
            Canvas.SetLeft(_previewComponent, pos.X);
            Canvas.SetTop(_previewComponent, pos.Y);
            return true;
        }

        return false;
    }

    private Point SnapToGrid(Point pt)
    {
        double snapX = Math.Round(pt.X / Constants.GridSpacing) * Constants.GridSpacing;
        double snapY = Math.Round(pt.Y / Constants.GridSpacing) * Constants.GridSpacing;
        return new Point(snapX, snapY);
    }

    public void OnExit()
    {
        if (_previewComponent != null)
            _previewComponent.Opacity = 0.0;
    }
    public void OnEnter()
    {
        if (_previewComponent != null)
            _previewComponent.Opacity = 1.0;
    }
}