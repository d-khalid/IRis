using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;

using IRis.Models.Core;
using IRis.Models.Commands;


namespace IRis.Models;


public partial class Simulation
{
    private Point _selectionStart;
    private Rectangle? _selectionRect;
    private bool _isDragging;
    private Point _dragStart;
    private readonly Dictionary<Component, Point> _dragOffsets = new();
    private readonly List<Point> _dragStartPositions = new();
    private readonly List<Component> _draggedComponents = new();

    private void Selection_HandleStart(Point mousePos)
    {
        _selectionStart = mousePos;

        foreach (var component in _components.AsEnumerable().Reverse())
        {
            if (!Selection_IsComponentHit(component, mousePos))
                continue;

            if (!component.IsSelected)
            {
                Selection_UnselectAll();
                component.IsSelected = true;
                _selectedComponents.Add(component);
            }

            PrepareDrag(mousePos);
            return;
        }

        Selection_UnselectAll();
        Selection_StartSelectionRectangle();
    }

    private void Selection_HandleUpdate(Point currentMousePos, bool snapToGridEnabled, Func<Point, Point> snapToGrid)
    {
        if (!_isDragging && _dragOffsets.Count > 0)
        {
            const double dragThreshold = 3.0;
            double dx = currentMousePos.X - _dragStart.X;
            double dy = currentMousePos.Y - _dragStart.Y;

            if (Math.Sqrt(dx * dx + dy * dy) > dragThreshold)
                _isDragging = true;
        }

        if (_isDragging)
        {
            Selection_UpdateDraggedComponents(currentMousePos, snapToGridEnabled, snapToGrid);
            return;
        }

        if (_selectionRect == null)
            return;

        Selection_UpdateSelectionRectangle(currentMousePos);
        Selection_UpdateSelectedComponents();
    }

    private void Selection_HandleEnd(Commands.CommandManager? commandManager = null)
    {
        if (_isDragging && _draggedComponents.Count > 0)
        {
            var newPositions = _draggedComponents
                .Select(c => new Point(Canvas.GetLeft(c), Canvas.GetTop(c)))
                .ToList();

            bool moved = false;
            for (int i = 0; i < _draggedComponents.Count && i < _dragStartPositions.Count; i++)
            {
                if (_dragStartPositions[i] != newPositions[i])
                {
                    moved = true;
                    break;
                }
            }

            if (moved && commandManager != null)
            {
                var moveCommand = new MoveComponentsCommand(_canvas, _draggedComponents, newPositions);
                commandManager.ExecuteCommand(moveCommand);
            }
        }

        _isDragging = false;
        _dragOffsets.Clear();
        _draggedComponents.Clear();
        _dragStartPositions.Clear();

        if (_selectionRect != null)
        {
            _canvas.Children.Remove(_selectionRect);
            _selectionRect = null;
        }
    }

    private void PrepareDrag(Point mousePos)
    {
        _dragStart = mousePos;
        _dragOffsets.Clear();
        _draggedComponents.Clear();
        _dragStartPositions.Clear();

        foreach (var selectedComponent in _selectedComponents)
        {
            var compPos = new Point(Canvas.GetLeft(selectedComponent), Canvas.GetTop(selectedComponent));
            _dragOffsets[selectedComponent] = new Point(compPos.X - mousePos.X, compPos.Y - mousePos.Y);
            _draggedComponents.Add(selectedComponent);
            _dragStartPositions.Add(compPos);
        }
    }

    private void Selection_UnselectAll()
    {
        foreach (var c in _selectedComponents)
            c.IsSelected = false;

        _selectedComponents.Clear();
        OnPropertyChanged(nameof(HasSelectedComponents));
    }

    private bool Selection_IsComponentHit(Component component, Point point)
    {
        var pos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
        var bounds = new Rect(pos, new Size(component.Size.Width, component.Size.Height));
        return bounds.Contains(point);
    }

    private void Selection_StartSelectionRectangle()
    {
        _selectionRect = new Rectangle
        {
            Width = 0,
            Height = 0,
            Fill = Constants.SelectionBrush,
            Stroke = Constants.SelectionPen.Brush,
            StrokeThickness = Constants.SelectionPen.Thickness
        };

        Canvas.SetLeft(_selectionRect, _selectionStart.X);
        Canvas.SetTop(_selectionRect, _selectionStart.Y);
        _canvas.Children.Add(_selectionRect);
    }

    private void Selection_UpdateDraggedComponents(Point currentMousePos, bool snapToGridEnabled, Func<Point, Point> snapToGrid)
    {
        foreach (var component in _selectedComponents)
        {
            if (!_dragOffsets.TryGetValue(component, out var offset))
                continue;

            var newPos = new Point(currentMousePos.X + offset.X, currentMousePos.Y + offset.Y);
            if (snapToGridEnabled)
                newPos = snapToGrid(newPos);

            Canvas.SetLeft(component, newPos.X);
            Canvas.SetTop(component, newPos.Y);
            component.InvalidateVisual();
        }
    }

    private void Selection_UpdateSelectionRectangle(Point currentMousePos)
    {
        if (_selectionRect == null) return;

        double left = Math.Min(_selectionStart.X, currentMousePos.X);
        double top = Math.Min(_selectionStart.Y, currentMousePos.Y);
        double width = Math.Abs(currentMousePos.X - _selectionStart.X);
        double height = Math.Abs(currentMousePos.Y - _selectionStart.Y);

        Canvas.SetLeft(_selectionRect, left);
        Canvas.SetTop(_selectionRect, top);
        _selectionRect.Width = width;
        _selectionRect.Height = height;
    }

    private void Selection_UpdateSelectedComponents()
    {
        if (_selectionRect == null) return;

        var selectionBounds = new Rect(
            Canvas.GetLeft(_selectionRect),
            Canvas.GetTop(_selectionRect),
            _selectionRect.Width, _selectionRect.Height);

        Selection_UnselectAll();

        foreach (var component in _components)
        {
            if (Selection_IsComponentInSelection(component, selectionBounds))
            {
                component.IsSelected = true;
                _selectedComponents.Add(component);
            }
        }

        OnPropertyChanged(nameof(HasSelectedComponents));
    }

    private bool Selection_IsComponentInSelection(Component component, Rect selectionBounds)
    {
        var pos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
        var bounds = new Rect(pos, new Size(component.Size.Width, component.Size.Height));
        return selectionBounds.Intersects(bounds);
    }
}
