using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models;

internal class ClipboardManager
{
    private List<Component> _clipboard = new();
    private Point _lastPastePosition;
    private bool _isPastePreviewActive;
    private List<Component> _pastePreviewComponents = new();
    private List<Component> _pastePreviewSources = new();

    public void Copy(List<Component> selectedComponents, bool cutMode, Action deleteAction)
    {
        _clipboard.Clear();

        // First pass
        foreach (var component in selectedComponents)
        {
            if (component is Wire wire)
            {
                _clipboard.Add(wire);
            }
            else
            {
                Component c = (Component)component.Clone();
                _clipboard.Add(c);

                Canvas.SetLeft(c, Canvas.GetLeft(component));
                Canvas.SetTop(c, Canvas.GetTop(component));
            }
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
        _pastePreviewSources.Clear();
        _isPastePreviewActive = true;

        var componentMap = CloneNonWireComponents();
        var wireMap = CloneAndReconnectWires(componentMap);

        foreach (var source in _clipboard)
        {
            if (source is Wire) continue;
            if (componentMap.TryGetValue(source, out var clone))
            {
                _pastePreviewComponents.Add(clone);
                _pastePreviewSources.Add(source);
            }
        }

        foreach (var sourceWire in GetClipboardWires())
        {
            if (wireMap.TryGetValue(sourceWire, out var clonedWire))
            {
                _pastePreviewComponents.Add(clonedWire);
                _pastePreviewSources.Add(sourceWire);
            }
        }

        canvas.Children.AddRange(_pastePreviewComponents);
    }

    private Dictionary<Component, Component> CloneNonWireComponents()
    {
        Dictionary<Component, Component> clonedComponents = new();

        // First pass
        foreach (var component in _clipboard)
        {
            // new Wire objects will be made later
            if (component is Wire) continue;

            Component c = (Component)component.Clone();
            clonedComponents[component] = c;

            Canvas.SetLeft(c, Canvas.GetLeft(component));
            Canvas.SetTop(c, Canvas.GetTop(component));
            component.IsSelected = false;   // This line is a syntax error
            c.IsSelected = false;
        }

        return clonedComponents;
    }

    private Dictionary<Wire, Wire> CloneAndReconnectWires(Dictionary<Component, Component> clonedComponents)
    {
        Dictionary<Wire, Wire> clonedWires = new();
        foreach (var sourceWire in GetClipboardWires())
        {
            if (!clonedWires.ContainsKey(sourceWire))
            {
                clonedWires[sourceWire] = CreateClonedWire(sourceWire);
            }
        }

        foreach (var kvp in clonedComponents)
        {
            var source = kvp.Key;
            var clone = kvp.Value;
            if (source.Terminals == null || clone.Terminals == null) continue;

            for (int i = 0; i < source.Terminals.Length; i++)
            {
                var sourceTerminal = source.Terminals[i];
                var cloneTerminal = clone.Terminals[i];

                if (sourceTerminal == null || cloneTerminal == null) continue;

                var mappedWires = sourceTerminal.Wires
                    .Where(w => clonedWires.ContainsKey(w))
                    .Select(w => clonedWires[w])
                    .ToList();

                clone.Terminals[i] = new Terminal(cloneTerminal.Position)
                {
                    Wires = mappedWires
                };
            }
        }

        return clonedWires;
    }

    private List<Wire> GetClipboardWires()
    {
        List<Wire> wires = new();

        foreach (var component in _clipboard)
        {
            if (component is Wire wire)
            {
                if (!wires.Contains(wire))
                    wires.Add(wire);
                continue;
            }

            if (component.Terminals == null) continue;

            foreach (var terminal in component.Terminals)
            {
                foreach (var connectedWire in terminal.Wires)
                {
                    if (!wires.Contains(connectedWire))
                        wires.Add(connectedWire);
                }
            }
        }

        return wires;
    }

    private Wire CreateClonedWire(Wire originalWire)
    {
        Wire newWire = new Wire
        {
            Points = new List<Point>(originalWire.Points),
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

        if (_pastePreviewComponents.Count == 0 || _pastePreviewSources.Count == 0) return;

        Component referenceSource = _pastePreviewSources[0];
        Point reference = referenceSource is Wire referenceWire && referenceWire.Points.Count > 0
            ? referenceWire.Points[0]
            : new Point(Canvas.GetLeft(referenceSource), Canvas.GetTop(referenceSource));

        double offsetX = currentMousePos.X - reference.X;
        double offsetY = currentMousePos.Y - reference.Y;

        for (int i = 0; i < _pastePreviewComponents.Count; i++)
        {
            var original = _pastePreviewSources[i];
            var preview = _pastePreviewComponents[i];

            if (original is Wire originalWire && preview is Wire previewWire)
            {
                previewWire.Points = originalWire.Points
                    .Select(p => p == new Point(-1, -1) ? p : new Point(p.X + offsetX, p.Y + offsetY))
                    .ToList();
                previewWire.InvalidateVisual();
            }
            else
            {
                Canvas.SetLeft(preview, Canvas.GetLeft(original) + offsetX);
                Canvas.SetTop(preview, Canvas.GetTop(original) + offsetY);
            }
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