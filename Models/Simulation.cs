using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Models.Commands;

namespace IRis.Models;

// Contains all the data needed for a simulation
// Currently, this handles the preview
public partial class Simulation : ObservableObject
{
    private Canvas? _canvas;
    private List<Component> _components;
    private List<Component> _selectedComponents;
    private List<Component> _movedWires;

    public List<Component> Components
    {
        get => _components;
        set => _components = value;
    }

    public List<Component> MovedWires
    {
        get => _movedWires;
        set => _movedWires = value;
    }

    [ObservableProperty] private Point _currentMousePos = new Point(0, 0);

    // Selection and interaction managers
    private readonly SelectionManager _selectionManager;
    private readonly PreviewManager _previewManager;
    private readonly ClipboardManager _clipboardManager;
    private readonly GridManager _gridManager;
    private readonly CommandManager _commandManager = new();

    // For simulation
    private bool _simulating;
    private DispatcherTimer? _updateTimer;

    // Expose selected components
    public List<Component> SelectedComponents => _selectedComponents;

    // External access to selection state
    public bool HasSelectedComponents => _selectedComponents.Count > 0;

    // Undo/Redo
    public bool CanUndo => _commandManager.CanUndo;
    public bool CanRedo => _commandManager.CanRedo;
    public void Undo() => _commandManager.Undo();
    public void Redo() => _commandManager.Redo();

    public bool Simulating
    {
        get => _simulating;
        set
        {
            _simulating = value;
            if (_simulating) _updateTimer!.Start();
            else _updateTimer!.Stop();
        }
    }

    public Simulation()
    {
        // Initialize lists
        _components = new List<Component>();
        _selectedComponents = new List<Component>();
        _movedWires = new List<Component>();

        // Initialize managers
        _selectionManager = new SelectionManager();
        _previewManager = new PreviewManager();
        _clipboardManager = new ClipboardManager();
        _gridManager = new GridManager();
    }

    public void Register(Canvas canvas)
    {
        _canvas = canvas;
        SetupCanvas();
        SetupSimulation();
        RegisterEventHandlers();
        _gridManager.DrawGrid(_canvas); // Draws the main grid
    }

    private void SetupCanvas()
    {
        // Important: Enable keyboard focus
        _canvas!.Focusable = true;
        _canvas.Cursor = new Cursor(StandardCursorType.Arrow);
    }

    private void SetupSimulation()
    {
        // For updating the simulation
        _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) }; // Adjust to reduce CPU load
        _updateTimer.Tick += (s, e) => SimulationStep();
        Simulating = false;
    }

    private void RegisterEventHandlers()
    {
        _canvas!.PointerPressed += OnPointerPressed;
        _canvas.PointerMoved += OnPointerMoved;
        _canvas.PointerReleased += OnPointerReleased;
        _canvas.PointerEntered += (s, e) => _previewManager.OnEnter();
        _canvas.PointerExited += (s, e) => _previewManager.OnExit();
        _canvas.KeyDown += OnKeyDown;
        _canvas.PointerWheelChanged += OnPointerWheel;
    }

    private void HandleMoveSelectedComponents(KeyEventArgs e)
    {
        if (_selectedComponents.Count == 0) return;

        double moveDistance = _gridManager.SnapToGridEnabled ? ComponentDefaults.GridSpacing : 10;
        Point offset = new Point(0, 0);

        switch (e.Key)
        {
            case Key.Left:
                offset = new Point(-moveDistance, 0);
                break;
            case Key.Right:
                offset = new Point(moveDistance, 0);
                break;
            case Key.Up:
                offset = new Point(0, -moveDistance);
                break;
            case Key.Down:
                offset = new Point(0, moveDistance);
                break;
            default:
                return;
        }

        // Use command for undo/redo support
        var moveCommand = new MoveComponentsCommand(_selectedComponents, offset);
        _commandManager.ExecuteCommand(moveCommand);

        e.Handled = true;
    }

    private void OnPointerWheel(object? sender, PointerWheelEventArgs e)
    {
        // Update mouse pos and preview on scroll
        // TODO: THIS IS ACCEPTABLE FOR NOW BUT 100% NEEDS POLISH LATER ON
        CurrentMousePos = _gridManager.SnapToGrid(e.GetPosition(_canvas));
        _previewManager.UpdateWheelPosition(CurrentMousePos);
    }

    public void SimulationStep()
    {
        //Console.WriteLine("SIMULATION STEP");
        foreach (var component in _components)
        {
            // Redraw Toggles and Probes
            if (component is LogicProbe || component is LogicToggle)
                component.InvalidateVisual();

            // Compute outputs for everything
            if (component is IOutputProvider op)
                op.ComputeOutput();
        }
    }

    // Component management
    public void DeleteSelectedComponents()
    {
        if (_selectedComponents.Count > 0)
        {
            var deleteCommand = new DeleteComponentsCommand(_canvas!, _components, _selectedComponents);
            _commandManager.ExecuteCommand(deleteCommand);
            _selectedComponents.Clear();
        }
    }
    public void UnselectComponents() => _selectionManager.UnselectAll(_selectedComponents);

    // TODO: THESE METHODS ARE SHALLOW AND BAD! (probably)
    public void LoadComponents(List<Component> components)
    {
        _components = components;
        _canvas!.Children.AddRange(_components);
    }

    public void DeleteAllComponents()
    {
        _canvas!.Children.RemoveAll(_components);
        _components.Clear();
    }

    // Clipboard operations
    public void CopySelected(bool cutMode = false) => _clipboardManager.Copy(_selectedComponents, cutMode, DeleteSelectedComponents);
    public void CutSelected() => CopySelected(true);
    public void PasteSelected() => _clipboardManager.Paste(_canvas!, CurrentMousePos);

    // Preview management
    public string? PreviewCompType
    {
        get => _previewManager.PreviewCompType;
        set => _previewManager.SetPreviewComponent(value, _canvas!, CurrentMousePos, this);
    }

    // Grid management
    public bool SnapToGridEnabled
    {
        get => _gridManager.SnapToGridEnabled;
        set => _gridManager.SnapToGridEnabled = value;
    }

    public bool GridEnabled
    {
        get => _gridManager.GridEnabled;
        set
        {
            _gridManager.GridEnabled = value;
            if (value)
                _gridManager.DrawGrid(_canvas!);
            else
            {
                _canvas!.Children.Clear();
                _canvas.Children.AddRange(_components);
            }
        }
    }

    public Terminal? FindClosestSnapTerminal(Point p, double snappingRange, out Point absolutePos)
    {
        absolutePos = p;
        Terminal? closestTerminal = null;
        double minDistance = double.MaxValue;

        foreach (Component component in _components)
        {
            if (component.Terminals == null) continue;

            foreach (Terminal terminal in component.Terminals)
            {
                // Calculate absolute terminal position
                Point absTerminalPos = new Point(
                    terminal.Position.X + Canvas.GetLeft(component),
                    terminal.Position.Y + Canvas.GetTop(component)
                );

                double distance = Point.Distance(p, absTerminalPos);
                if (distance < minDistance && distance <= snappingRange)
                {
                    minDistance = distance;
                    closestTerminal = terminal;
                    absolutePos = absTerminalPos;
                }
            }
        }

        return closestTerminal; // Returns null if no terminal is within snapping range
    }

    public Wire? FindWireAtPosition(Point position)
    {
        return _components.OfType<Wire>()
            .FirstOrDefault(wire => wire.IsPointOnWire(position, 5.0)); // 5.0 is click tolerance
    }

    public bool WouldWireOverlapComponent(Point startPoint, Point endPoint)
    {
        foreach (Component component in _components)
        {
            // Skip wires - only check gates/components
            if (component is Wire) continue;

            // Get component bounds
            double left = Canvas.GetLeft(component);
            double top = Canvas.GetTop(component);
            double right = left + component.Width;
            double bottom = top + component.Height;

            // Check if the line segment intersects with the component rectangle
            if (LineIntersectsRectangle(startPoint, endPoint, left, top, right, bottom))
            {
                return true;
            }
        }
        return false;
    }

    private bool LineIntersectsRectangle(Point p1, Point p2, double rectLeft, double rectTop, double rectRight, double rectBottom)
    {
        // Check if either endpoint is inside the rectangle
        if (IsPointInRectangle(p1, rectLeft, rectTop, rectRight, rectBottom) ||
            IsPointInRectangle(p2, rectLeft, rectTop, rectRight, rectBottom))
        {
            return true;
        }

        // Check if line intersects any of the four rectangle edges
        return LineIntersectsLine(p1, p2, new Point(rectLeft, rectTop), new Point(rectRight, rectTop)) ||     // Top edge
            LineIntersectsLine(p1, p2, new Point(rectRight, rectTop), new Point(rectRight, rectBottom)) || // Right edge
            LineIntersectsLine(p1, p2, new Point(rectRight, rectBottom), new Point(rectLeft, rectBottom)) || // Bottom edge
            LineIntersectsLine(p1, p2, new Point(rectLeft, rectBottom), new Point(rectLeft, rectTop));     // Left edge
    }

    private bool IsPointInRectangle(Point point, double left, double top, double right, double bottom)
    {
        return point.X >= left && point.X <= right && point.Y >= top && point.Y <= bottom;
    }

    private bool LineIntersectsLine(Point p1, Point p2, Point p3, Point p4)
    {
        double denominator = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);
        if (Math.Abs(denominator) < 1e-10) return false; // Lines are parallel

        double t = ((p1.X - p3.X) * (p3.Y - p4.Y) - (p1.Y - p3.Y) * (p3.X - p4.X)) / denominator;
        double u = -((p1.X - p2.X) * (p1.Y - p3.Y) - (p1.Y - p2.Y) * (p1.X - p3.X)) / denominator;

        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }

    public bool WouldWireOverlapExistingWire(Point startPoint, Point endPoint)
    {
        foreach (Component component in _components)
        {
            if (!(component is Wire existingWire)) continue;

            int intersectionCount = 0;

            // Check each segment of the existing wire
            for (int i = 0; i < existingWire.Points.Count - 1; i++)
            {
                // Skip invalid points
                if (existingWire.Points[i] == new Point(-1, -1) || 
                    existingWire.Points[i + 1] == new Point(-1, -1)) 
                    continue;

                Point segStart = existingWire.Points[i];
                Point segEnd = existingWire.Points[i + 1];

                // Check if the new wire segment intersects this existing segment
                if (LineIntersectsLine(startPoint, endPoint, segStart, segEnd))
                {
                    intersectionCount++;
                }
                
                // Also check for collinear overlap (same path, overlapping segments)
                else if (AreSegmentsCollinearAndOverlapping(startPoint, endPoint, segStart, segEnd))
                {
                    return true; // Collinear overlap is always an overlap
                }
            }
            
            // If we have 2 or more intersections with this wire, it's an overlap
            if (intersectionCount >= 2)
            {
                return true;
            }
        }
        return false;
    }

    private bool AreSegmentsCollinearAndOverlapping(Point line1Start, Point line1End, Point line2Start, Point line2End)
    {
        // Check if all four points are collinear
        if (!ArePointsCollinear(line1Start, line1End, line2Start) || 
            !ArePointsCollinear(line1Start, line1End, line2End))
        {
            return false;
        }

        // Points are collinear, now check if segments overlap
        // Determine if the line is more horizontal or vertical
        bool isHorizontal = Math.Abs(line1End.X - line1Start.X) >= Math.Abs(line1End.Y - line1Start.Y);
        
        if (isHorizontal)
        {
            // Check X-axis overlap
            double line1Min = Math.Min(line1Start.X, line1End.X);
            double line1Max = Math.Max(line1Start.X, line1End.X);
            double line2Min = Math.Min(line2Start.X, line2End.X);
            double line2Max = Math.Max(line2Start.X, line2End.X);
            
            // Segments overlap if they're not completely separate
            return !(line1Max < line2Min || line2Max < line1Min);
        }
        else
        {
            // Check Y-axis overlap
            double line1Min = Math.Min(line1Start.Y, line1End.Y);
            double line1Max = Math.Max(line1Start.Y, line1End.Y);
            double line2Min = Math.Min(line2Start.Y, line2End.Y);
            double line2Max = Math.Max(line2Start.Y, line2End.Y);
            
            // Segments overlap if they're not completely separate
            return !(line1Max < line2Min || line2Max < line1Min);
        }
    }

    private bool ArePointsCollinear(Point p1, Point p2, Point p3)
    {
        // Use cross product to check if points are collinear
        double crossProduct = (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        return Math.Abs(crossProduct) < 1e-10;
    }

    // ________________________________________________
    // ____________ Pointer/Key Handling ______________
    // ________________________________________________

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Handle wire extension when clicking on existing wire
        if (_previewManager.PreviewCompType == "WIRE" && _previewManager.PreviewComponent != null)
        {
            var clickedWire = FindWireAtPosition(CurrentMousePos);
            if (clickedWire != null)
            {
                if (IsWireInMovedWires(clickedWire, MovedWires))
                {
                    Console.WriteLine("Moved wire cannot be extended!");
                    return;
                }
                else
                {
                    _previewManager.StartWireExtension(clickedWire, _canvas!, CurrentMousePos, Components, MovedWires);
                    return;
                }
            }
        }
        if (_previewManager.HandleCommit(sender, e, _components, _canvas!, CurrentMousePos, _commandManager, this))
            return;

        // Add drag handling
        _previewManager.OnPointerPressed(sender, e, _selectedComponents);
        _selectionManager.HandleStart(_canvas!, _selectedComponents, CurrentMousePos);
    }
    
    private bool IsWireInMovedWires(Wire wire, List<Component> MovedWires)
    {
        foreach (var movedWire in MovedWires)
        {
            if (movedWire == wire) return true;
        }
        return false;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // Handle preview manager drag completion first
        _previewManager.OnPointerReleased(sender, e, _commandManager);

        // Then handle selection manager
        _selectionManager.HandleEnd(_canvas!, _commandManager);
    }
    
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        // Update the mouse pos
        CurrentMousePos = e.GetPosition(_canvas);

        if (_previewManager.HandleUpdate(_canvas!, CurrentMousePos, _gridManager.SnapToGridEnabled,
            _gridManager.SnapToGrid, this))
            return;

        // Pass the selectedComponents reference and grid functions
        _selectionManager.HandleUpdate(_canvas!, _selectedComponents, CurrentMousePos, _components, this,
            _gridManager.SnapToGridEnabled, _gridManager.SnapToGrid);
    }

    // Keyboard shortcut support for moving selected components
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // Handle preview manager first
        _previewManager.HandleKeyCommand(e, _components, _canvas!, _commandManager);

        // FIXED: Correct condition check
        if (_selectedComponents.Count > 0 && _previewManager.PreviewCompType == null)
        {
            HandleMoveSelectedComponents(e);
        }
    }

}