// File: SelectionManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Models.Commands;

namespace IRis.Models;

internal class SelectionManager
{
    private Point _selectionStart;
    private Rectangle? _selectionRect;
    
    // Dragging state
    private bool _isDragging;
    private Point _dragStart;
    private Dictionary<Component, Point> _dragOffsets = new();

    public void HandleStart(Canvas canvas, List<Component> selectedComponents, Point mousePos)
    {
        _selectionStart = mousePos;

        // Check if we're clicking on a selected component to prepare for potential dragging
        foreach (var child in canvas.Children)
        {
            if (child is Component component && IsComponentHit(component, mousePos))
            {
                if (component.IsSelected)
                {
                    // DON'T start dragging immediately - just prepare for it
                    // Store the drag start position and offsets, but don't set _isDragging = true yet
                    _dragStart = mousePos;
                    _dragOffsets.Clear();

                    // Store initial offsets for all selected components
                    foreach (var selectedComponent in selectedComponents)
                    {
                        Point componentPos = new Point(Canvas.GetLeft(selectedComponent), Canvas.GetTop(selectedComponent));
                        Point offset = new Point(componentPos.X - mousePos.X, componentPos.Y - mousePos.Y);
                        _dragOffsets[selectedComponent] = offset;
                    }
                    return;
                }
                else
                {
                    // FIXED: Clear other selections first, then select this component
                    UnselectAll(selectedComponents);
                    selectedComponents.Add(component);
                    component.IsSelected = true;
                    
                    // NEW: Select connected wires
                    SelectConnectedWires(component, canvas, selectedComponents);
                    return;
                }
            }
        }

        // Unselect components if hitting empty space
        UnselectAll(selectedComponents);
        StartSelectionRectangle(canvas);
    }

    // Select all wires connected to a component
    private void SelectConnectedWires(Component component, Canvas canvas, List<Component> selectedComponents)
    {
        if (component.Terminals == null) return;

        foreach (var terminal in component.Terminals)
        {
            // Change from single wire to multiple wires
            foreach (var wire in terminal.Wires)
            {
                if (wire != null && !wire.IsSelected)
                {
                    wire.IsSelected = true;
                    selectedComponents.Add(wire);
                }
            }
        }
    }

    // NEW METHOD: Select wires connected to any of the selected components
    private void SelectConnectedWiresForComponents(List<Component> componentsToCheck, Canvas canvas, List<Component> selectedComponents)
    {
        foreach (var component in componentsToCheck)
        {
            if (component is Wire) continue; // Skip wires themselves
            
            SelectConnectedWires(component, canvas, selectedComponents);
        }
    }

    // NEW METHOD: Get all components connected to selected wires
    private List<Component> GetComponentsConnectedToWires(List<Component> selectedWires, Canvas canvas)
    {
        var connectedComponents = new List<Component>();
        
        foreach (var wire in selectedWires.OfType<Wire>())
        {
            foreach (var child in canvas.Children)
            {
                if (child is Component component && component != wire && component.Terminals != null)
                {
                    foreach (var terminal in component.Terminals)
                    {
                        if (terminal.Wire == wire && !connectedComponents.Contains(component))
                        {
                            connectedComponents.Add(component);
                        }
                    }
                }
            }
        }
        
        return connectedComponents;
    }

    private bool IsComponentHit(Component component, Point point)
    {
        var componentPos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
        var componentBounds = new Rect(componentPos, new Size(component.Width, component.Height));

        // Check intersection for components (gates)
        if (componentBounds.Contains(point)) return true;

        // Check For wires
        return component is Wire wire && component.HitTest(point);
    }

    public void UnselectAll(List<Component> selectedComponents)
    {
        foreach (Component c in selectedComponents)
            c.IsSelected = false;
        selectedComponents.Clear();
    }

    private void StartSelectionRectangle(Canvas canvas)
    {
        // Add a selection rectangle to the canvas
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
        canvas.Children.Add(_selectionRect);
    }

    // Add allComponents and simulation parameters for wire snapping
    public void HandleUpdate(Canvas canvas, List<Component> selectedComponents, Point currentMousePos, 
        List<Component> allComponents, Simulation simulation,
        bool snapToGridEnabled = false, Func<Point, Point>? snapToGrid = null)
    {
        // Check if we should start dragging (mouse moved far enough from start position)
        if (!_isDragging && _dragOffsets.Count > 0)
        {
            double dragThreshold = 3.0; // Minimum distance to start dragging
            double distance = Math.Sqrt(Math.Pow(currentMousePos.X - _dragStart.X, 2) + 
                                    Math.Pow(currentMousePos.Y - _dragStart.Y, 2));
            
            if (distance > dragThreshold)
            {
                _isDragging = true;
            }
        }

        // Handle dragging selected components
        if (_isDragging)
        {
            UpdateDraggedComponents(selectedComponents, currentMousePos, allComponents, simulation, snapToGridEnabled, snapToGrid);
            return;
        }

        // No selection area to update, let the event handler go on
        if (_selectionRect == null) return;

        UpdateSelectionRectangle(currentMousePos);
        UpdateSelectedComponents(canvas, selectedComponents);
    }

    // Added wire endpoint snapping functionality
    private void UpdateDraggedComponents(List<Component> selectedComponents, Point currentMousePos,
        List<Component> allComponents, Simulation simulation,
        bool snapToGridEnabled, Func<Point, Point>? snapToGrid)
    {
        foreach (var component in selectedComponents)
        {
            if (!_dragOffsets.TryGetValue(component, out Point offset)) continue;

            Point newPos = new Point(currentMousePos.X + offset.X, currentMousePos.Y + offset.Y);
            
            // Apply grid snapping if enabled
            if (snapToGridEnabled && snapToGrid != null)
                newPos = snapToGrid(newPos);

            Canvas.SetLeft(component, newPos.X);
            Canvas.SetTop(component, newPos.Y);
            
            // Snap wire endpoints to their connected terminals
            if (component is Wire wire)
            {
                SnapWireEndpointsToTerminals(wire, allComponents);
            }
            // Force redraw for components that need it (like wires)
            component.InvalidateVisual();
        }
    }

    // Only updates wire endpoints connected to terminals, preserves internal path
    private void SnapWireEndpointsToTerminals(Wire wire, List<Component> allComponents)
    {
        var connectedTerminals = FindTerminalsConnectedToWire(wire, allComponents);
        
        if (connectedTerminals.Count == 0 || wire.Points.Count == 0) return;
        
        // Get the current wire position
        Point wirePos = new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire));
        
        // Update first endpoint if connected to a terminal
        if (connectedTerminals.Count >= 1)
        {
            Point firstTerminalWorldPos = GetTerminalWorldPosition(connectedTerminals[0], allComponents);
            Point firstTerminalLocalPos = firstTerminalWorldPos - wirePos;
            wire.Points[0] = firstTerminalLocalPos;
        }
        
        // Update last endpoint if connected to a second terminal
        if (connectedTerminals.Count >= 2 && wire.Points.Count > 1)
        {
            Point secondTerminalWorldPos = GetTerminalWorldPosition(connectedTerminals[1], allComponents);
            Point secondTerminalLocalPos = secondTerminalWorldPos - wirePos;
            wire.Points[wire.Points.Count - 1] = secondTerminalLocalPos;
        }
    }

    // Find all terminals connected to a specific wire
    private List<Terminal> FindTerminalsConnectedToWire(Wire wire, List<Component> allComponents)
    {
        var connectedTerminals = new List<Terminal>();
        
        foreach (var component in allComponents)
        {
            if (component.Terminals == null) continue;
            
            foreach (var terminal in component.Terminals)
            {
                // Change from single wire check to multiple wires check
                if (terminal.Wires.Contains(wire))
                {
                    connectedTerminals.Add(terminal);
                }
            }
        }
        
        return connectedTerminals;
    }

    // Get the world position of a terminal
    private Point GetTerminalWorldPosition(Terminal terminal, List<Component> allComponents)
    {
        // Find the component that owns this terminal
        Component? ownerComponent = null;
        foreach (var component in allComponents)
        {
            if (component.Terminals?.Contains(terminal) == true)
            {
                ownerComponent = component;
                break;
            }
        }
        
        if (ownerComponent == null)
            return new Point(0, 0);
        
        // Get component's world position
        Point componentPos = new Point(Canvas.GetLeft(ownerComponent), Canvas.GetTop(ownerComponent));
        
        // Add terminal's relative position to component position
        // Note: You may need to adjust this based on how Terminal.Position is defined
        // and whether it accounts for component rotation
        Point terminalLocalPos = terminal.Position;
        
        // If the component is rotated, we need to transform the terminal position
        if (ownerComponent.Rotation != 0)
        {
            terminalLocalPos = RotatePoint(terminalLocalPos, ownerComponent.Rotation, 
                new Point(ownerComponent.Width / 2, ownerComponent.Height / 2));
        }
        
        return componentPos + terminalLocalPos;
    }

    // Rotate a point around a center point by a given angle
    private Point RotatePoint(Point point, double angleDegrees, Point center)
    {
        double angleRadians = angleDegrees * Math.PI / 180.0;
        double cos = Math.Cos(angleRadians);
        double sin = Math.Sin(angleRadians);
        
        // Translate point to origin
        double translatedX = point.X - center.X;
        double translatedY = point.Y - center.Y;
        
        // Rotate
        double rotatedX = translatedX * cos - translatedY * sin;
        double rotatedY = translatedX * sin + translatedY * cos;
        
        // Translate back
        return new Point(rotatedX + center.X, rotatedY + center.Y);
    }

    private void UpdateSelectionRectangle(Point currentMousePos)
    {
        if (_selectionRect == null) return;     // Truly exceptional case
        // Calculate bounds
        double left = Math.Min(_selectionStart.X, currentMousePos.X);
        double top = Math.Min(_selectionStart.Y, currentMousePos.Y);
        double width = Math.Abs(_selectionStart.X - currentMousePos.X);
        double height = Math.Abs(_selectionStart.Y - currentMousePos.Y);

        // Update rectangle
        Canvas.SetLeft(_selectionRect, left);
        Canvas.SetTop(_selectionRect, top);
        _selectionRect.Width = width;
        _selectionRect.Height = height;
    }

    private void UpdateSelectedComponents(Canvas canvas, List<Component> selectedComponents)
    {
        // Unselect everything and reselect again
        UnselectAll(selectedComponents);

        var selectionBounds = GetSelectionBounds();
        var componentsInSelection = new List<Component>();

        // Check each component
        foreach (var child in canvas.Children)
        {
            if (child is Component component && IsComponentInSelection(component, selectionBounds))
            {
                component.IsSelected = true;
                selectedComponents.Add(component);
                componentsInSelection.Add(component);
            }
        }

        // NEW: Select connected wires for all selected components
        SelectConnectedWiresForComponents(componentsInSelection, canvas, selectedComponents);
    }

    private Rect GetSelectionBounds()
    {
        if (_selectionRect == null) return new Rect();  // Not Expected to occur
        double left = Canvas.GetLeft(_selectionRect);
        double top = Canvas.GetTop(_selectionRect);
        return new Rect(left, top, _selectionRect.Width, _selectionRect.Height);
    }

    private bool IsComponentInSelection(Component component, Rect selectionBounds)
    {
        var componentPos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
        var componentBounds = new Rect(componentPos, new Size(component.Width, component.Height));

        // Check intersection
        if (selectionBounds.Intersects(componentBounds)) return true;

        // To select a wire, the selection must contain one of its vertices
        if (component is Wire wire)
        {
            return wire.Points.Any(p => selectionBounds.Contains(p + new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire))));
        }

        return false;
    }

    public void HandleEnd(Canvas canvas, CommandManager? commandManager = null)
    {
        // End dragging
        if (_isDragging)
        {
            // Create move command if components were actually moved
            if (_dragOffsets.Count > 0)
            {
                var selectedComponents = _dragOffsets.Keys.ToList();
                var originalPositions = selectedComponents.Select(c => 
                    new Point(Canvas.GetLeft(c) - _dragOffsets[c].X - (_dragStart.X - _dragStart.X), 
                            Canvas.GetTop(c) - _dragOffsets[c].Y - (_dragStart.Y - _dragStart.Y))).ToList();
                var newPositions = selectedComponents.Select(c => 
                    new Point(Canvas.GetLeft(c), Canvas.GetTop(c))).ToList();
                
                // Only create command if positions actually changed
                bool moved = false;
                for (int i = 0; i < originalPositions.Count; i++)
                {
                    var originalPos = new Point(Canvas.GetLeft(selectedComponents[i]), Canvas.GetTop(selectedComponents[i]));
                    originalPos = new Point(originalPos.X - _dragOffsets[selectedComponents[i]].X, 
                                        originalPos.Y - _dragOffsets[selectedComponents[i]].Y);
                    originalPos = new Point(originalPos.X + _dragStart.X, originalPos.Y + _dragStart.Y);
                    
                    if (Math.Abs(originalPos.X - newPositions[i].X) > 0.1 || 
                        Math.Abs(originalPos.Y - newPositions[i].Y) > 0.1)
                    {
                        moved = true;
                        break;
                    }
                }
                
                if (moved && commandManager != null)
                {
                    // Use the 3-argument constructor: canvas, components, newPositions
                    var moveCommand = new MoveComponentsCommand(canvas, selectedComponents, newPositions);
                    commandManager.ExecuteCommand(moveCommand);
                }
            }
            
            _isDragging = false;
        }
        
        // Clean up drag preparation state even if we never actually started dragging
        _dragOffsets.Clear();

        // Remove the selection rect if its there
        if (_selectionRect != null)
        {
            canvas.Children.Remove(_selectionRect);
            _selectionRect = null;
        }
    }

    public void DeleteSelected(Canvas canvas, List<Component> components, List<Component> selectedComponents)
    {
        foreach (var component in selectedComponents)
        {
            canvas.Children.Remove(component);
            components.Remove(component);
        }
        selectedComponents.Clear();
    }
}