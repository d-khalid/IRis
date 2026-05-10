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

    public void Copy(List<Component> selectedComponents, bool cutMode, Action deleteAction)
    {
        _clipboard.Clear();

        foreach (var component in selectedComponents)
        {
            var clone = CircuitComponentFactory.CloneComponent(component);
            if (clone == null)
                continue;

            Canvas.SetLeft(clone, Canvas.GetLeft(component));
            Canvas.SetTop(clone, Canvas.GetTop(component));
            clone.IsSelected = false;

            _clipboard.Add(clone);
            component.IsSelected = false;
        }

        if (cutMode) deleteAction();
        selectedComponents.Clear();
    }

    public void Paste(Canvas canvas, List<Component> components, Commands.CommandManager commandManager, Point currentMousePos)
    {
        if (_clipboard.Count == 0) return;

        Point reference = new(Canvas.GetLeft(_clipboard[0]), Canvas.GetTop(_clipboard[0]));
        Point offset = new(Constants.GridSpacing * 2, Constants.GridSpacing * 2);

        foreach (var source in _clipboard)
        {
            var clone = CircuitComponentFactory.CloneComponent(source);
            if (clone == null)
                continue;

            Point sourcePosition = new(Canvas.GetLeft(source), Canvas.GetTop(source));
            Point newPosition = new(
                currentMousePos.X + (sourcePosition.X - reference.X) + offset.X,
                currentMousePos.Y + (sourcePosition.Y - reference.Y) + offset.Y);

            var addCommand = new Commands.AddComponentCommand(canvas, components, clone, newPosition);
            commandManager.ExecuteCommand(addCommand);
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

        double snappedX = Math.Round(point.X / Constants.GridSpacing) * Constants.GridSpacing;
        double snappedY = Math.Round(point.Y / Constants.GridSpacing) * Constants.GridSpacing;
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
        for (double x = 0; x < width; x += Constants.GridSpacing)
            canvas.Children.Insert(0, CreateGridLine(new Point(x, 0), new Point(x, height)));

        // Draw horizontal lines
        for (double y = 0; y < height; y += Constants.GridSpacing)
            canvas.Children.Insert(0, CreateGridLine(new Point(0, y), new Point(width, y)));
    }

    private Line CreateGridLine(Point start, Point end)
    {
        return new Line
        {
            StartPoint = start,
            EndPoint = end,
            Stroke = Constants.GridBrush,
            StrokeThickness = Constants.GridThickness,
            Tag = "grid" // For easy identification
        };
    }
}