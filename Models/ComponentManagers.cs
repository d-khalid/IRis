using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using IRis.Models.Components;
using IRis.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRis.Models;

internal class ClipboardManager
{
    private List<Component> _clipboard = new();
    private Point _lastPastePosition;
    private bool _isPastePreviewActive;
    private List<Component> _pastePreviewComponents = new();

    public void Copy(List<Component> selectedComponents, bool cutMode, Action deleteAction)
    {
        _clipboard.Clear();

        // First pass
        foreach (var component in selectedComponents)
        {
            Component c = (Component)component.Clone();
            _clipboard.Add(c);

            Canvas.SetLeft(c, Canvas.GetLeft(component));
            Canvas.SetTop(c, Canvas.GetTop(component));
            component.IsSelected = false;
        }

        if (cutMode) deleteAction();
        selectedComponents.Clear();
    }

    public void Paste(Canvas canvas, Point currentMousePos)
    {
        if (_clipboard.Count == 0) return;

        CreatePastePreview(canvas);
        UpdatePastePreviewPosition(currentMousePos);
    }

    private void CreatePastePreview(Canvas canvas)
    {
        _pastePreviewComponents.Clear();
        _isPastePreviewActive = true;

        List<Component> clonedComponents = CloneNonWireComponents();
        Dictionary<Wire, Wire> clonedWires = CloneAndReconnectWires(clonedComponents);

        // Add things to preview list
        _pastePreviewComponents.AddRange(clonedComponents);
        _pastePreviewComponents.AddRange(clonedWires.Values.ToList());
        canvas.Children.AddRange(_pastePreviewComponents);
    }

    private List<Component> CloneNonWireComponents()
    {
        List<Component> clonedComponents = new();

        // First pass
        foreach (var component in _clipboard)
        {
            // new Wire objects will be made later
            if (component is Wire) continue;

            Component c = (Component)component.Clone();
            clonedComponents.Add(c);

            Canvas.SetLeft(c, Canvas.GetLeft(component));
            Canvas.SetTop(c, Canvas.GetTop(component));
            component.IsSelected = false;   // This line is a syntax error
            c.IsSelected = false;
        }

        return clonedComponents;
    }

    private Dictionary<Wire, Wire> CloneAndReconnectWires(List<Component> clonedComponents)
    {
        // Second pass
        Dictionary<Wire, Wire> clonedWires = new();
        foreach (var component in clonedComponents)
        {
            if (component.Terminals == null) continue;

            foreach (var terminal in component.Terminals)
            {
                if (terminal.Wire == null) continue;

                // If there's a matching wire for an original wire already, make it
                if (!clonedWires.TryGetValue(terminal.Wire, out var clonedWire))
                {
                    clonedWire = CreateClonedWire(terminal.Wire);
                    clonedWires.Add(terminal.Wire, clonedWire);
                }

                terminal.Wire = clonedWires[terminal.Wire];
            }
        }

        return clonedWires;
    }

    private Wire CreateClonedWire(Wire originalWire)
    {
        Wire newWire = new Wire
        {
            Points = originalWire.Points,
            Id = Guid.NewGuid(),
            Value = originalWire.Value, // enums are value types!
            IsSelected = false
        };

        Canvas.SetLeft(newWire, Canvas.GetLeft(originalWire));
        Canvas.SetTop(newWire, Canvas.GetTop(originalWire));
        // FIX: prevent wires from being drawn as ghosts
        newWire.IsBeingEdited = false;
        newWire.IsCommitted = true;

        return newWire;
    }

    // TODO: MAKE THIS ALWAYS USE THE TOP-LEFT-MOST ELEMENT IN THE CLIPBOARD
    private void UpdatePastePreviewPosition(Point currentMousePos)
    {
        if (!_isPastePreviewActive) return;

        _lastPastePosition = currentMousePos;

        // Use the wire's first point as reference
        Point reference = _clipboard[0] is Wire wire
            ? wire.Points[0]
            : new Point(Canvas.GetLeft(_clipboard[0]), Canvas.GetTop(_clipboard[0]));

        for (int i = 0; i < _pastePreviewComponents.Count; i++)
        {
            var original = _clipboard[i];
            var preview = _pastePreviewComponents[i];

            Console.WriteLine($"{Canvas.GetLeft(original)}, {Canvas.GetTop(original)}");
            Canvas.SetLeft(preview, Canvas.GetLeft(original) + currentMousePos.X - reference.X);
            Canvas.SetTop(preview, Canvas.GetTop(original) + currentMousePos.Y - reference.Y);
        }
    }
}

internal class GridManager
{
    public bool SnapToGridEnabled { get; set; } = true;
    public bool GridEnabled { get; set; } = true;

    public Point SnapToGrid(Point point)
    {
        if (!SnapToGridEnabled) return point;

        double snappedX = Math.Round(point.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
        double snappedY = Math.Round(point.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
        return new Point(snappedX, snappedY);
    }

    public void DrawGrid(Canvas canvas)
    {
        if (canvas == null) return;

        // Clear existing grid lines (if any)
        // Useful for redraws
        var gridLines = canvas.Children.OfType<Line>().Where(l => l.Tag?.ToString() == "grid").ToList();
        foreach (var line in gridLines)
            canvas.Children.Remove(line);

        double width = canvas.MinWidth;
        double height = canvas.MinHeight;

        DrawGridLines(canvas, width, height);
    }

    private void DrawGridLines(Canvas canvas, double width, double height)
    {
        // Draw vertical lines
        for (double x = 0; x < width; x += ComponentDefaults.GridSpacing)
            canvas.Children.Insert(0, CreateGridLine(new Point(x, 0), new Point(x, height)));

        // Draw horizontal lines
        for (double y = 0; y < height; y += ComponentDefaults.GridSpacing)
            canvas.Children.Insert(0, CreateGridLine(new Point(0, y), new Point(width, y)));
    }

    private Line CreateGridLine(Point start, Point end)
    {
        return new Line
        {
            StartPoint = start,
            EndPoint = end,
            Stroke = ComponentDefaults.GridBrush,
            StrokeThickness = ComponentDefaults.GridThickness,
            Tag = "grid" // For easy identification
        };
    }
}