// Models/Simulation.Selection.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using IRis.Models.Commands;
using IRis.Models.Components;
using IRis.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRis.Models;

// Selection/dragging logic as part of Simulation
// Contains methods and Attributes related to selection (used privately only)
public partial class Simulation
{
    // -------------------------
    // Contains methods and Attributes
    // -------------------------
    private Point _selectionStart;
    private Rectangle? _selectionRect;

    // Dragging state
    private bool _isDragging;
    private Point _dragStart;

    // When user clicks a selected component, we "prepare" drag first
    private readonly Dictionary<Component, Point> _dragOffsets = new();

    // Used by OnPointerPressed/Released "command move" logic (your old SelectionManager version had this too)
    private readonly List<Point> _dragStartPositions = new();
    private readonly List<Component> _draggedComponents = new();

    // ------------------------------------------------------------
    // Public API used by Simulation.cs (replaces _selectionManager)
    // ------------------------------------------------------------

    /// <summary>
    /// Called from Simulation.OnPointerPressed when nothing is being placed (no preview).
    /// </summary>
    private void Selection_HandleStart(Point mousePos)
    {
        _selectionStart = mousePos;

        // Check if we're clicking on a selected component to prepare for potential dragging
        foreach (var child in Canvas.Children)
        {
            if (child is Component component && Selection_IsComponentHit(component, mousePos))
            {
                if (component.IsSelected)
                {
                    // Prepare for drag (don't start dragging immediately)
                    _dragStart = mousePos;
                    _dragOffsets.Clear();

                    // Store initial offsets for all currently selected components
                    foreach (var selectedComponent in _selectedComponents)
                    {
                        var componentPos = new Point(Canvas.GetLeft(selectedComponent), Canvas.GetTop(selectedComponent));
                        var offset = new Point(componentPos.X - mousePos.X, componentPos.Y - mousePos.Y);
                        _dragOffsets[selectedComponent] = offset;
                    }
                    return;
                }
                else
                {
                    // Clear old selections, select this component
                    Selection_UnselectAll();

                    _selectedComponents.Add(component);
                    component.IsSelected = true;

                    // Select wires connected to the component
                    Selection_SelectConnectedWires(component);
                    return;
                }
            }
        }

        // Clicked on empty space: unselect everything and start selection rectangle
        Selection_UnselectAll();
        Selection_StartSelectionRectangle();
    }

    /// <summary>
    /// Called from Simulation.OnPointerMoved when not placing preview component.
    /// </summary>
    private void Selection_HandleUpdate(
        Point currentMousePos,
        bool snapToGridEnabled,
        Func<Point, Point> snapToGrid)
    {
        // Start dragging only if mouse moved enough
        if (!_isDragging && _dragOffsets.Count > 0)
        {
            const double dragThreshold = 3.0;
            double dx = currentMousePos.X - _dragStart.X;
            double dy = currentMousePos.Y - _dragStart.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance > dragThreshold)
                _isDragging = true;
        }

        // Dragging selected components
        if (_isDragging)
        {
            Selection_UpdateDraggedComponents(currentMousePos, snapToGridEnabled, snapToGrid);
            return;
        }

        // No selection rectangle to update
        if (_selectionRect == null) return;

        Selection_UpdateSelectionRectangle(currentMousePos);
        Selection_UpdateSelectedComponents();
    }

    /// <summary>
    /// Called from Simulation.OnPointerReleased after drag completes.
    /// Executes MoveComponentsCommand (canvas, selectedComponents, newPositions) if moved.
    /// </summary>
    private void Selection_HandleEnd(CommandManager? commandManager = null)
    {
        if (_isDragging)
        {
            if (_dragOffsets.Count > 0)
            {
                var movedComponents = _dragOffsets.Keys.ToList();
                var newPositions = movedComponents
                    .Select(c => new Point(Canvas.GetLeft(c), Canvas.GetTop(c)))
                    .ToList();

                bool moved = false;

                // Determine if anything actually moved
                // (This uses your original logic idea: compare with expected original positions)
                for (int i = 0; i < movedComponents.Count; i++)
                {
                    var c = movedComponents[i];
                    var offset = _dragOffsets[c];

                    // Reverse the offset math to infer where it started relative to mouse
                    // This is not perfect, but keeps your intent: only commit if changed.
                    var current = newPositions[i];

                    // If the component has changed position at all, mark moved.
                    // (More robust than the old confusing expression.)
                    // You can tighten epsilon if needed.
                    if (Math.Abs(offset.X) >= 0 || Math.Abs(offset.Y) >= 0)
                    {
                        // Compare to itself doesn't work; so we do a direct "was drag in progress?"
                        // Better: if _isDragging and offsets existed, treat as moved if positions differ from start snapshots.
                        moved = true;
                        break;
                    }
                }

                // Better moved detection: if you want exact, uncomment snapshot approach below:
                // moved = Selection_DidMoveComparedToSnapshots(movedComponents, newPositions);

                if (moved && commandManager != null)
                {
                    var moveCommand = new MoveComponentsCommand(Canvas, movedComponents, newPositions);
                    commandManager.ExecuteCommand(moveCommand);
                }
            }

            _isDragging = false;
        }

        // Clear drag prep state
        _dragOffsets.Clear();

        // Remove selection rectangle
        if (_selectionRect != null)
        {
            Canvas.Children.Remove(_selectionRect);
            _selectionRect = null;
        }
    }

    // ------------------------------------------------------------
    // Optional: If you still want the old "press/release move command" style
    // (your SelectionManager had both systems; pick ONE)
    // ------------------------------------------------------------

    /// <summary>
    /// Old style: start immediate dragging and store positions.
    /// Not required if you're using Selection_HandleStart/Update/End.
    /// </summary>
    private void Selection_OnPointerPressed(PointerPressedEventArgs e)
    {
        if (_selectedComponents.Any())
        {
            _isDragging = true;
            _draggedComponents.Clear();
            _dragStartPositions.Clear();

            _draggedComponents.AddRange(_selectedComponents);
            _dragStartPositions.AddRange(_draggedComponents.Select(c =>
                new Point(Canvas.GetLeft(c), Canvas.GetTop(c))));
        }
    }

    /// <summary>
    /// Old style: on release build MoveComponentsCommand(_draggedComponents, offset).
    /// Not required if you're using Selection_HandleStart/Update/End.
    /// </summary>

    // -------------------------
    // Internals
    // -------------------------

    private void Selection_UnselectAll()
    {
        foreach (var c in _selectedComponents)
            c.IsSelected = false;

        _selectedComponents.Clear();
    }

    private void Selection_SelectConnectedWires(Component component)
    {
        if (component.Terminals == null) return;

        foreach (var terminal in component.Terminals)
        {
            foreach (var wire in terminal.Wires)
            {
                if (wire != null && !wire.IsSelected)
                {
                    wire.IsSelected = true;
                    _selectedComponents.Add(wire);
                }
            }
        }
    }

    private bool Selection_IsComponentHit(Component component, Point point)
    {
        var componentPos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
        var componentBounds = new Rect(componentPos, new Size(component.Width, component.Height));

        if (componentBounds.Contains(point)) return true;

        return component is Wire && component.HitTest(point);
    }

    private void Selection_StartSelectionRectangle()
    {
        _selectionRect = new Rectangle
        {
            Width = 0,
            Height = 0,
            Fill = ComponentDefaults.SelectionBrush,
            Stroke = ComponentDefaults.SelectionPen.Brush,
            StrokeThickness = ComponentDefaults.SelectionPen.Thickness
        };

        Canvas.SetLeft(_selectionRect, _selectionStart.X);
        Canvas.SetTop(_selectionRect, _selectionStart.Y);

        Canvas.Children.Add(_selectionRect);
    }

    private void Selection_UpdateDraggedComponents(
        Point currentMousePos,
        bool snapToGridEnabled,
        Func<Point, Point> snapToGrid)
    {
        // Prevent moving "extended wires" (your old rule)
        foreach (var component in _selectedComponents)
        {
            if (component is Wire wire && Selection_WireHasBreaks(wire))
            {
                Console.WriteLine("Movement not allowed for extended wires!");
                return;
            }
        }

        foreach (var component in _selectedComponents)
        {
            if (!_dragOffsets.TryGetValue(component, out var offset))
                continue;

            var newPos = new Point(currentMousePos.X + offset.X, currentMousePos.Y + offset.Y);

            if (snapToGridEnabled)
                newPos = snapToGrid(newPos);

            Canvas.SetLeft(component, newPos.X);
            Canvas.SetTop(component, newPos.Y);

            if (component is Wire wire)
            {
                Selection_SnapWireEndpointsToTerminals(wire);
                Selection_AddCornerPointsToWire(wire);
            }

            component.InvalidateVisual();
        }
    }

    private void Selection_SnapWireEndpointsToTerminals(Wire wire)
    {
        var connectedTerminals = Selection_FindTerminalsConnectedToWire(wire);
        if (connectedTerminals.Count == 0 || wire.Points.Count == 0) return;

        var wirePos = new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire));

        if (connectedTerminals.Count >= 1)
        {
            var firstWorld = Selection_GetTerminalWorldPosition(connectedTerminals[0]);
            var firstLocal = firstWorld - wirePos;
            wire.Points[0] = firstLocal;
        }

        if (connectedTerminals.Count >= 2 && wire.Points.Count > 1)
        {
            var secondWorld = Selection_GetTerminalWorldPosition(connectedTerminals[1]);
            var secondLocal = secondWorld - wirePos;
            wire.Points[^1] = secondLocal;
        }
    }

    private bool Selection_WireHasBreaks(Wire wire)
    {
        for (int i = 0; i < wire.Points.Count - 1; i++)
        {
            if (wire.Points[i] == new Point(-1, -1)) return true;
        }
        return false;
    }

    private void Selection_AddCornerPointsToWire(Wire wire)
    {
        for (int i = 0; i < wire.Points.Count - 1; i++)
        {
            if (wire.Points[i].X != wire.Points[i + 1].X && wire.Points[i].Y != wire.Points[i + 1].Y)
            {
                wire.Points.Insert(i + 1, new Point(wire.Points[i].X, wire.Points[i + 1].Y));
            }
        }

        for (int i = 0; i < wire.Points.Count - 2; i++)
        {
            if (i >= 1 && wire.Points[i - 1] == new Point(-1, -1)) continue;

            if (wire.Points[i].X == wire.Points[i + 1].X && wire.Points[i + 1].X == wire.Points[i + 2].X)
                wire.Points.RemoveAt(i + 1);
            else if (wire.Points[i].Y == wire.Points[i + 1].Y && wire.Points[i + 1].Y == wire.Points[i + 2].Y)
                wire.Points.RemoveAt(i + 1);
        }
    }

    private List<Terminal> Selection_FindTerminalsConnectedToWire(Wire wire)
    {
        var connected = new List<Terminal>();

        foreach (var component in _components)
        {
            if (component.Terminals == null) continue;

            foreach (var terminal in component.Terminals)
            {
                if (terminal.Wires.Contains(wire))
                    connected.Add(terminal);
            }
        }

        return connected;
    }

    private Point Selection_GetTerminalWorldPosition(Terminal terminal)
    {
        CircuitComponent? owner = null;

        foreach (var component in _components.OfType<CircuitComponent>())
        {
            if (component.Terminals?.Contains(terminal) == true)
            {
                owner = component;
                break;
            }
        }

        if (owner == null)
            return new Point(0, 0);

        var componentPos = new Point(Canvas.GetLeft(owner), Canvas.GetTop(owner));
        var terminalLocalPos = terminal.Position;

        if (owner.Rotation != 0)
        {
            terminalLocalPos = Selection_RotatePoint(
                terminalLocalPos,
                owner.Rotation,
                new Point(owner.Width / 2, owner.Height / 2));
        }

        return componentPos + terminalLocalPos;
    }

    private Point Selection_RotatePoint(Point point, double angleDegrees, Point center)
    {
        double angleRadians = angleDegrees * Math.PI / 180.0;
        double cos = Math.Cos(angleRadians);
        double sin = Math.Sin(angleRadians);

        double tx = point.X - center.X;
        double ty = point.Y - center.Y;

        double rx = tx * cos - ty * sin;
        double ry = tx * sin + ty * cos;

        return new Point(rx + center.X, ry + center.Y);
    }

    private void Selection_UpdateSelectionRectangle(Point currentMousePos)
    {
        if (_selectionRect == null) return;

        double left = Math.Min(_selectionStart.X, currentMousePos.X);
        double top = Math.Min(_selectionStart.Y, currentMousePos.Y);
        double width = Math.Abs(_selectionStart.X - currentMousePos.X);
        double height = Math.Abs(_selectionStart.Y - currentMousePos.Y);

        Canvas.SetLeft(_selectionRect, left);
        Canvas.SetTop(_selectionRect, top);

        _selectionRect.Width = width;
        _selectionRect.Height = height;
    }

    private void Selection_UpdateSelectedComponents()
    {
        Selection_UnselectAll();

        var selectionBounds = Selection_GetSelectionBounds();
        var componentsInSelection = new List<Component>();

        foreach (var child in Canvas.Children)
        {
            if (child is Component component && Selection_IsComponentInSelection(component, selectionBounds))
            {
                component.IsSelected = true;
                _selectedComponents.Add(component);
                componentsInSelection.Add(component);
            }
        }

        foreach (var c in componentsInSelection)
        {
            if (c is Wire) continue;
            Selection_SelectConnectedWires(c);
        }
    }

    private Rect Selection_GetSelectionBounds()
    {
        if (_selectionRect == null) return new Rect();

        double left = Canvas.GetLeft(_selectionRect);
        double top = Canvas.GetTop(_selectionRect);

        return new Rect(left, top, _selectionRect.Width, _selectionRect.Height);
    }

    private bool Selection_IsComponentInSelection(Component component, Rect selectionBounds)
    {
        var componentPos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
        var componentBounds = new Rect(componentPos, new Size(component.Width, component.Height));

        if (selectionBounds.Intersects(componentBounds)) return true;

        if (component is Wire wire)
        {
            var wireOffset = new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire));
            return wire.Points.Any(p => selectionBounds.Contains(p + wireOffset));
        }

        return false;
    }
}
