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

        // Check if we're clicking on a selected component to start dragging
        foreach (var child in canvas.Children)
        {
            if (child is Component component && IsComponentHit(component, mousePos))
            {
                if (component.IsSelected)
                {
                    // Start dragging all selected components
                    StartDrag(selectedComponents, mousePos);
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

    // NEW METHOD: Select all wires connected to a component
    private void SelectConnectedWires(Component component, Canvas canvas, List<Component> selectedComponents)
    {
        if (component.Terminals == null) return;

        foreach (var terminal in component.Terminals)
        {
            if (terminal.Wire != null && !terminal.Wire.IsSelected)
            {
                terminal.Wire.IsSelected = true;
                selectedComponents.Add(terminal.Wire);
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

    private void StartDrag(List<Component> selectedComponents, Point mousePos)
    {
        _isDragging = true;
        _dragStart = mousePos;
        _dragOffsets.Clear();

        // Store initial offsets for all selected components
        foreach (var component in selectedComponents)
        {
            Point componentPos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
            Point offset = new Point(componentPos.X - mousePos.X, componentPos.Y - mousePos.Y);
            _dragOffsets[component] = offset;
        }
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

    private void ToggleSelection(Component component, List<Component> selectedComponents)
    {
        if (component.IsSelected)
        {
            selectedComponents.Remove(component);
            component.IsSelected = false;
        }
        else
        {
            selectedComponents.Add(component);
            component.IsSelected = true;
        }
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

    // UPDATED METHOD: Added allComponents and simulation parameters for wire snapping
    public void HandleUpdate(Canvas canvas, List<Component> selectedComponents, Point currentMousePos, 
        List<Component> allComponents, Simulation simulation,
        bool snapToGridEnabled = false, Func<Point, Point>? snapToGrid = null)
    {
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

    // UPDATED METHOD: Added wire endpoint snapping functionality
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
            
            // NEW: Snap wire endpoints to their connected terminals
            if (component is Wire wire)
            {
                SnapWireEndpointsToTerminals(wire, allComponents);
            }
            
            // Force redraw for components that need it (like wires)
            component.InvalidateVisual();
        }
    }

    // UPDATED METHOD: Snaps wire endpoints to their connected terminals and cleans up old paths
    private void SnapWireEndpointsToTerminals(Wire wire, List<Component> allComponents)
    {
        var connectedTerminals = FindTerminalsConnectedToWire(wire, allComponents);
        
        if (connectedTerminals.Count == 0) return;
        
        // Clear the old wire path to avoid junk segments
        wire.Points.Clear();
        
        // Get the current wire position
        Point wirePos = new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire));
        
        // If we have connected terminals, recreate the wire path
        if (connectedTerminals.Count >= 1)
        {
            // For the first terminal, add it as the starting point
            Point firstTerminalWorldPos = GetTerminalWorldPosition(connectedTerminals[0], allComponents);
            Point firstTerminalLocalPos = firstTerminalWorldPos - wirePos;
            wire.Points.Add(firstTerminalLocalPos);
            
            // If there's a second terminal, create a simple path to it
            if (connectedTerminals.Count >= 2)
            {
                Point secondTerminalWorldPos = GetTerminalWorldPosition(connectedTerminals[1], allComponents);
                Point secondTerminalLocalPos = secondTerminalWorldPos - wirePos;
                
                // Create a simple L-shaped or direct path between terminals
                CreateWirePath(wire, firstTerminalLocalPos, secondTerminalLocalPos);
            }
            else
            {
                // If only one terminal is connected, the wire should extend from that point
                // You might want to add logic here for partially connected wires
                // For now, just add a small extension in the current direction
                Point extension = new Point(firstTerminalLocalPos.X + 20, firstTerminalLocalPos.Y);
                wire.Points.Add(extension);
            }
        }
    }

    // NEW METHOD: Creates an optimal path between two terminal points
    private void CreateWirePath(Wire wire, Point startLocal, Point endLocal)
    {
        // Clear any existing points except the start point (which should already be added)
        if (wire.Points.Count > 1)
        {
            // Keep only the first point and clear the rest
            Point startPoint = wire.Points[0];
            wire.Points.Clear();
            wire.Points.Add(startPoint);
        }
        
        double dx = endLocal.X - startLocal.X;
        double dy = endLocal.Y - startLocal.Y;
        
        // Create path based on direction and distance
        if (Math.Abs(dx) < 10 && Math.Abs(dy) < 10)
        {
            // Very close - direct connection
            wire.Points.Add(endLocal);
        }
        else if (Math.Abs(dx) > Math.Abs(dy) * 2)
        {
            // Primarily horizontal - go horizontal then vertical
            wire.Points.Add(new Point(startLocal.X + dx * 0.7, startLocal.Y));
            wire.Points.Add(new Point(startLocal.X + dx * 0.7, endLocal.Y));
            wire.Points.Add(endLocal);
        }
        else if (Math.Abs(dy) > Math.Abs(dx) * 2)
        {
            // Primarily vertical - go vertical then horizontal
            wire.Points.Add(new Point(startLocal.X, startLocal.Y + dy * 0.7));
            wire.Points.Add(new Point(endLocal.X, startLocal.Y + dy * 0.7));
            wire.Points.Add(endLocal);
        }
        else
        {
            // Diagonal - create L-shape
            wire.Points.Add(new Point(endLocal.X, startLocal.Y));
            wire.Points.Add(endLocal);
        }
    }

    // NEW METHOD: Find all terminals connected to a specific wire
    private List<Terminal> FindTerminalsConnectedToWire(Wire wire, List<Component> allComponents)
    {
        var connectedTerminals = new List<Terminal>();
        
        foreach (var component in allComponents)
        {
            if (component.Terminals == null) continue;
            
            foreach (var terminal in component.Terminals)
            {
                if (terminal.Wire == wire)
                {
                    connectedTerminals.Add(terminal);
                }
            }
        }
        
        return connectedTerminals;
    }

    // NEW METHOD: Get the world position of a terminal
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

    // NEW METHOD: Rotate a point around a center point by a given angle
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

    // NEW METHOD: Find the wire point closest to a target position
    private int FindClosestWirePoint(Wire wire, Point targetPos)
    {
        if (wire.Points.Count == 0) return -1;
        
        double minDistance = double.MaxValue;
        int closestIndex = -1;
        Point wirePos = new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire));
        
        for (int i = 0; i < wire.Points.Count; i++)
        {
            Point worldWirePoint = wirePos + wire.Points[i];
            double distance = CalculateDistance(worldWirePoint, targetPos);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }
        
        return closestIndex;
    }

    // NEW METHOD: Calculate distance between two points
    private double CalculateDistance(Point p1, Point p2)
    {
        double dx = p1.X - p2.X;
        double dy = p1.Y - p2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
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

    public void HandleEnd(Canvas canvas)
    {
        // End dragging
        if (_isDragging)
        {
            // Create move command if components were actually moved
            if (_dragOffsets.Count > 0)
            {
                var selectedComponents = _dragOffsets.Keys.ToList();
                var oldPositions = _dragOffsets.Values.ToList();
                var newPositions = selectedComponents.Select(c => 
                    new Point(Canvas.GetLeft(c), Canvas.GetTop(c))).ToList();
                
                // Only create command if positions actually changed
                bool moved = false;
                for (int i = 0; i < oldPositions.Count; i++)
                {
                    if (oldPositions[i] != newPositions[i])
                    {
                        moved = true;
                        break;
                    }
                }
                
                if (moved)
                {
                    var moveCommand = new MoveComponentsCommand(selectedComponents, oldPositions, newPositions);
                    // You'll need to pass the CommandManager here - see below
                }
            }
            
            _isDragging = false;
            _dragOffsets.Clear();
            return;
        }

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