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
using IRis.Services;
using IRis.Views;

namespace IRis.Models;

// Contains all the data needed for a simulation
public partial class Simulation : ObservableObject
{
    private Canvas? _canvas;
    private List<Component> _components;
    private List<Component> _selectedComponents;
    private List<Component> _movedWires;
    public CustomComponentData CustomComponent { get; set; } = null!;

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

    public CommandManager CommandManager => _commandManager;

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

    public void SimulationStep()
    {
        foreach (var component in _components)
        {
            // Compute outputs for everything first
            if (component is IOutputProvider op)
                op.ComputeOutput();

            // Redraw Toggles and Probes
            if (component is LogicProbe || component is LogicToggle)
                component.InvalidateVisual();
        }
    }

    // Component Management
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

    // sus? amogus?
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

    // Terminal Snapping
    public Terminal? FindClosestSnapTerminal(Point p)
    {
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
                if (distance < minDistance && distance <= ComponentDefaults.TerminalSnappingRange)
                {
                    minDistance = distance;
                    closestTerminal = terminal;
                }
            }
        }
        return closestTerminal; // Returns null if no terminal is within snapping range
    }

    public Point GetAbsoluteTerminalPosition(Terminal terminal)
    {
        foreach (Component component in _components)
        {
            if (component.Terminals == null) continue;
            foreach (Terminal compTerminal in component.Terminals)
            {
                if (compTerminal == terminal)
                {
                    return new Point(
                        compTerminal.Position.X + Canvas.GetLeft(component),
                        compTerminal.Position.Y + Canvas.GetTop(component)
                    );
                }
            }
        }
        return new Point(-2, -2);   // Error case; should not occur
    }

    public bool IsInputTerminal(Terminal terminal)
    {
        if (terminal == null) return false;
        
        foreach (Component component in _components)
        {
            if (component.Terminals == null) continue;
            
            // For Gate components, check if terminal is not the last one (output)
            if (component is Gate gate)
            {
                for (int i = 0; i < gate.Terminals!.Length - 1; i++) // Exclude last terminal (output)
                {
                    if (gate.Terminals[i] == terminal)
                        return true;
                }
            }
            // For Multiplexer components
            else if (component is Multiplexer mux)
            {
                // Selection lines (indices 0 to SelectionLineCount-1) and 
                // Input lines (indices SelectionLineCount to SelectionLineCount+InputLineCount-1) are inputs
                // Only the last terminal (^1) is output
                for (int i = 0; i < mux.Terminals!.Length - 1; i++) // Exclude last terminal (output)
                {
                    if (mux.Terminals[i] == terminal)
                        return true;
                }
            }
            // For CustomComponent
            else if (component is CustomComponent customComp)
            {
                // First InputCount terminals are inputs, remaining are outputs
                for (int i = 0; i < customComp.InputCount; i++)
                {
                    if (i < customComp.Terminals!.Length && customComp.Terminals[i] == terminal)
                        return true;
                }
            }
        }
        
        return false;
    }

    public Wire? FindWireAtPosition(Point position)
    {
        return _components.OfType<Wire>()
            .FirstOrDefault(wire => wire.IsPointOnWire(position, 5.0)); // 5.0 is click tolerance
    }
    
    private bool IsWireInMovedWires(Wire wire, List<Component> MovedWires)
    {
        foreach (var movedWire in MovedWires)
        {
            if (movedWire == wire) return true;
        }
        return false;
    }

    // _______________________________________________
    // ____________ Pointer/key handling _____________
    // _______________________________________________

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_previewManager.PreviewCompType != "NULL")  // If a component is being previewed
        {
            if (_previewManager.PreviewCompType == "WIRE")  // If component is wire
            {
                if (FindWireAtPosition(CurrentMousePos) != null)  // if Created on an already present wire
                {
                    Terminal? terminal = FindClosestSnapTerminal(CurrentMousePos);
                    if (terminal != null && !IsInputTerminal(terminal))   // if no terminal is present
                    {
                        Console.WriteLine("Registering new wire ()...");
                        _previewManager.HandleWireCommit(sender, e, CurrentMousePos, this);
                        return;
                    }
                    else if (terminal != null && IsInputTerminal(terminal))   // If wire & terminal are present bellow
                    {
                        Console.WriteLine("Terminal is Already connected to a wire!");
                        return;
                    }
                    else    // Only wire is present bellow
                    {
                        // WIRE EXTENSION LOGIC
                        Console.WriteLine("Registering Wire Extension...");
                        Wire existingWire = FindWireAtPosition(CurrentMousePos)!;
                        _previewManager.StartWireExtension(CurrentMousePos, existingWire, this);
                        return;
                    }
                }
                else    // if Created on some empty space
                {
                    // NEW WIRE LOGIC
                    Console.WriteLine("Registering New Wire...");
                    _previewManager.HandleWireCommit(sender, e, CurrentMousePos, this);
                    return;
                }
            }
            else
            {
                // NEW COMPONENT LOGIC
                Console.WriteLine("Registering New Component...");
                _previewManager.HandleComponentCommit(_canvas!, _components, CurrentMousePos, _commandManager, this);
                return;
            }
        }
        // Selection handling through selection manager
        _selectionManager.OnPointerPressed(sender, e, _selectedComponents);
        _selectionManager.HandleStart(_canvas!, _selectedComponents, CurrentMousePos);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // Handle preview manager drag completion first
        _selectionManager.OnPointerReleased(sender, e, _commandManager);

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

    private void OnPointerWheel(object? sender, PointerWheelEventArgs e)
    {
        // TODO: THIS IS ACCEPTABLE FOR NOW BUT 100% NEEDS POLISH LATER ON
        CurrentMousePos = _gridManager.SnapToGrid(e.GetPosition(_canvas));
        
        // Update the preview component
        if (_previewManager.PreviewComponent != null)
        {
            // Update the canvas on Scroll
            Canvas.SetLeft(_previewManager.PreviewComponent, CurrentMousePos.X);
            Canvas.SetTop(_previewManager.PreviewComponent, CurrentMousePos.Y);
        }
    }

    // Keyboard shortcut support for moving selected components
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // Use this to Handle keys
    }

}