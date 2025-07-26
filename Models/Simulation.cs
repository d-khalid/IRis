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
    private Canvas _canvas;
    private List<Component> _components;
    private List<Component> _selectedComponents;

    public List<Component> Components
    {
        get => _components;
        set => _components = value;
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
    private DispatcherTimer _updateTimer;
    
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
            if (_simulating) _updateTimer.Start();
            else _updateTimer.Stop();
        }
    }

    public Simulation()
    {
        // Initialize lists
        _components = new List<Component>();
        _selectedComponents = new List<Component>();

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
        _canvas.Focusable = true;
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
        _canvas.PointerPressed += OnPointerPressed;
        _canvas.PointerMoved += OnPointerMoved;
        _canvas.PointerReleased += OnPointerReleased;
        _canvas.PointerEntered += (s, e) => _previewManager.OnEnter();
        _canvas.PointerExited += (s, e) => _previewManager.OnExit();
        _canvas.KeyDown += OnKeyDown;
        _canvas.PointerWheelChanged += OnPointerWheel;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        // Update the mouse pos
        CurrentMousePos = e.GetPosition(_canvas);

        if (_previewManager.HandleUpdate(_canvas, CurrentMousePos, _gridManager.SnapToGridEnabled, 
            _gridManager.SnapToGrid, this))
            return;
        
        // Pass the selectedComponents reference and grid functions
        _selectionManager.HandleUpdate(_canvas, _selectedComponents, CurrentMousePos, _components, this,
            _gridManager.SnapToGridEnabled, _gridManager.SnapToGrid);
    }

    // Keyboard shortcut support for moving selected components
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // Handle preview manager first
        _previewManager.HandleKeyCommand(e, _components, _canvas, _commandManager);
        
        // FIXED: Correct condition check
        if (_selectedComponents.Count > 0 && _previewManager.PreviewCompType == null)
        {
            HandleMoveSelectedComponents(e);
        }
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
            var deleteCommand = new DeleteComponentsCommand(_canvas, _components, _selectedComponents);
            _commandManager.ExecuteCommand(deleteCommand);
            _selectedComponents.Clear();
        }
    }
    public void UnselectComponents() => _selectionManager.UnselectAll(_selectedComponents);

    // TODO: THESE METHODS ARE SHALLOW AND BAD! (probably)
    public void LoadComponents(List<Component> components)
    {
        _components = components;
        _canvas.Children.AddRange(_components);
    }

    public void DeleteAllComponents()
    {
        _canvas.Children.RemoveAll(_components);
        _components.Clear();
    }

    // Clipboard operations
    public void CopySelected(bool cutMode = false) => _clipboardManager.Copy(_selectedComponents, cutMode, DeleteSelectedComponents);
    public void CutSelected() => CopySelected(true);
    public void PasteSelected() => _clipboardManager.Paste(_canvas, CurrentMousePos);

    // Preview management
    public string? PreviewCompType
    {
        get => _previewManager.PreviewCompType;
        set => _previewManager.SetPreviewComponent(value, _canvas, CurrentMousePos);
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
                _gridManager.DrawGrid(_canvas);
            else
            {
                _canvas.Children.Clear();
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

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_previewManager.HandleCommit(sender, e, _components, _canvas, CurrentMousePos, _commandManager, this))
            return;
        
        // Add drag handling
        _previewManager.OnPointerPressed(sender, e, _selectedComponents);
        _selectionManager.HandleStart(_canvas, _selectedComponents, CurrentMousePos);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // Handle preview manager drag completion first
        _previewManager.OnPointerReleased(sender, e, _commandManager);
        
        // Then handle selection manager
        _selectionManager.HandleEnd(_canvas, _commandManager);
    }
}