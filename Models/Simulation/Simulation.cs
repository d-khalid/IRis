using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Models.Commands;
using IRis.Views;


namespace IRis.Models;


public partial class Simulation : ObservableObject
{
    // --------------------
    // Private attributes
    // --------------------
    // NOTE: _canvas is set to nullable to suppress warnings since it is not 
    // initialized in the constructor but by app.axaml.cs instead.
    // --------------------
    private Canvas? _canvas;       // nullable attribute to store the canvas
    private Canvas Canvas =>       // Use this private getter to access canvas
        _canvas ?? throw new InvalidOperationException("Register(canvas) first.");
    private List<Component> _components;
    private List<Component> _selectedComponents;
    private List<Component> _movedWires;
    [ObservableProperty] private Point _currentMousePos = new Point(0, 0);

    // Selection and interaction managers
    private readonly PreviewManager _previewManager;
    private readonly ClipboardManager _clipboardManager;
    private readonly GridManager _gridManager;
    private readonly CommandManager _commandManager = new();

    // For simulation
    private bool _simulating;
    private DispatcherTimer? _updateTimer;

    // --------------------
    // Public attributes
    // --------------------
    public CustomComponentData CustomComponent { get; set; } = null!;

    // Public access for Managers
    public CommandManager CommandManager => _commandManager;
    public PreviewManager PreviewManager => _previewManager;

    // Public access for Components-related
    public List<Component> SelectedComponents => _selectedComponents;
    public bool HasSelectedComponents => _selectedComponents.Count > 0;

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

    // Preview management
    public string? PreviewCompType
    {
        get => _previewManager.PreviewCompType;
        set => _previewManager.SetPreviewComponent(value, Canvas, CurrentMousePos, this);
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
                _gridManager.DrawGrid(Canvas);
            else
            {
                Canvas.Children.Clear();
                Canvas.Children.AddRange(_components);
            }
        }
    }

    // -------------------------------------------------
    // Public methods for simulation construction
    // -------------------------------------------------
    public Simulation()
    {
        // Initialize empty lists
        _components = [];
        _selectedComponents = [];
        _movedWires = [];

        // Initialize managers
        _previewManager = new PreviewManager();
        _clipboardManager = new ClipboardManager();
        _gridManager = new GridManager();
    }
    
    public bool Simulating
    {
        get => _simulating;
        set
        {
            _simulating = value;
            if (_simulating)
            {
                Selection_UnselectAll();
                _previewManager.PreviewComponent = null;
                _previewManager.PreviewCompType = "NULL";
                _updateTimer!.Start();
            }
            else _updateTimer!.Stop();
        }
    }

    public void Register(Canvas canvas)
    {
        _canvas = canvas;
        SetupCanvas();
        SetupSimulation();
        RegisterEventHandlers();
        _gridManager.DrawGrid(_canvas); // Draws the main grid
    }

    // ----------------------------------------------------
    // Private helper Methods for the above constructors
    // ----------------------------------------------------
    private void SimulationStep()
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

    private void SetupCanvas()
    {
        // Important: Enable keyboard focus
        Canvas.Focusable = true;
        Canvas.Cursor = new Cursor(StandardCursorType.Arrow);
    }

    private void SetupSimulation()
    {
        // For updating the simulation everytime after some time span
        // Adjust time span from here to reduce CPU load
        _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) }; 
        _updateTimer.Tick += (s, e) => SimulationStep();
        Simulating = false;
    }

    private void RegisterEventHandlers()
    {
        Canvas.PointerPressed += OnPointerPressed;
        Canvas.PointerMoved += OnPointerMoved;
        Canvas.PointerReleased += OnPointerReleased;
        Canvas.PointerEntered += (s, e) => _previewManager.OnEnter();
        Canvas.PointerExited += (s, e) => _previewManager.OnExit();
        Canvas.KeyDown += OnKeyDown;
        Canvas.PointerWheelChanged += OnPointerWheel;
    }

    // --------------------------------
    // Component Management Methods
    // --------------------------------
    public void DeleteSelectedComponents()
    {
        if (_selectedComponents.Count > 0)
        {
            var deleteCommand = new DeleteComponentsCommand(
                Canvas, _components, _selectedComponents);
            _commandManager.ExecuteCommand(deleteCommand);
            _selectedComponents.Clear();
        }
    }

    // TODO: THESE METHODS ARE SHALLOW AND BAD! (probably)
    public void LoadComponents(List<Component> components)
    {
        _components = components;
        Canvas.Children.AddRange(_components);
    }

    // sus? amogus?
    public void DeleteAllComponents()
    {
        Canvas.Children.RemoveAll(_components);
        _components.Clear();
    }

    // ----------------------------------------------
    // Important Wrappers for Clipboard Managers
    // ----------------------------------------------
    public void CopySelected(bool cutMode = false) => _clipboardManager.Copy(_selectedComponents, cutMode, DeleteSelectedComponents);
    public void CutSelected() => CopySelected(true);
    public void PasteSelected() => _clipboardManager.Paste(Canvas, CurrentMousePos);

    // -------------------------
    // Handling Mouse Actions
    // -------------------------
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Simulating || !GridEnabled)
        {
            Console.WriteLine("Cannot edit while simulating");
            return;
        }
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
                            if (_previewManager.PreviewComponent is Wire temp &&
                                temp.Points.Count > 1)  // if trying to put a checkpoint on an existing wire
                            {   // Trying to put a checkpoint on a wire
                                Console.WriteLine("Cannot Put a Checkpoint on a Wire!");
                                return;
                            }
                            else
                            {
                                // WIRE EXTENSION LOGIC
                                Wire existingWire = FindWireAtPosition(CurrentMousePos)!;
                                if (FindClosestSnapTerminal(CurrentMousePos) != null)
                                {
                                    Console.WriteLine("Invalid Extension! Please choose a distance away from the terminals.");
                                    return;
                                }
                                Console.WriteLine("Registering Wire Extension...");
                                _previewManager.StartWireExtension(Canvas, CurrentMousePos, existingWire, this);
                                return;
                            }
                        }
                    }
                    else    // if Created on some empty space
                    {
                        if (_previewManager.PreviewComponent is not null)
                        {
                            Console.WriteLine("Registering a Checkpoint...");
                            _previewManager.HandleWireCommit(sender, e, CurrentMousePos, this);
                            return;
                        }
                        else
                        {
                            // NEW WIRE LOGIC
                            Console.WriteLine("Registering New Wire...");
                            _previewManager.HandleWireCommit(sender, e, CurrentMousePos, this);
                            return;
                        }
                    }
                }
                else
                {
                    // NEW COMPONENT LOGIC
                    Console.WriteLine("Registering New Component...");
                    _previewManager.HandleComponentCommit(Canvas, _components, CurrentMousePos, _commandManager, this);
                    return;
                }
            }
        // Selection handling through selection manager
        Selection_OnPointerPressed(e);
        Selection_HandleStart(CurrentMousePos);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Selection_HandleEnd(_commandManager);
    }
    
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        // Update the mouse pos
        CurrentMousePos = e.GetPosition(Canvas);

        if (_previewManager.HandleUpdate(Canvas, CurrentMousePos, _gridManager.SnapToGridEnabled,
            _gridManager.SnapToGrid, this))
            return;

        // Pass the selectedComponents reference and grid functions
        Selection_HandleUpdate(CurrentMousePos, _gridManager.SnapToGridEnabled, _gridManager.SnapToGrid);
    }

    private void OnPointerWheel(object? sender, PointerWheelEventArgs e)
    {
        // TODO: THIS IS ACCEPTABLE FOR NOW BUT 100% NEEDS POLISH LATER ON
        CurrentMousePos = _gridManager.SnapToGrid(e.GetPosition(Canvas));
        
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