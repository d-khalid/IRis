using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Views;


namespace IRis.Models;

public partial class Simulation : ObservableObject
{
     [ObservableProperty] private List<Component> _components = new();
     [ObservableProperty] private List<Component> _selectedComponents = new();
     [ObservableProperty] private List<Component> _movedWires = new();
     [ObservableProperty] private Point _currentMousePos;
     
     
     [ObservableProperty]
     private bool _isSimulating;
     
     private DispatcherTimer? _updateTimer;

     public Simulation()
     {
          var or = new OrGate();
          _components.Add(or);

          or.X = 100;
          or.Y = 100;

     }
     
     
     
}

//
// public partial class Simulation : ObservableObject
// {
//     // private readonly Canvas _canvas;
//     private bool _isSimulating;f
//
//
//     public Simulation()
//     {
//
//
//         // Initialize empty lists
//         _components = new List<Component>();
//         _selectedComponents = new List<Component>();
//         _movedWires = new List<Component>();
//
//         // Initialize managers
//         _previewManager = new PreviewManager();
//         _clipboardManager = new ClipboardManager();
//         _gridManager = new GridManager();
//
//
//         // _canvas.Focusable = true;
//         // _canvas.Cursor = new Cursor(StandardCursorType.Arrow);
//
//         // For updating the simulation everytime after some time span
//         // Adjust time span from here to reduce CPU load
//         _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) }; 
//         _updateTimer.Tick += (s, e) => SimulationStep();
//         IsSimulating = false;
//
//         // // Register event handlers
//         // _canvas.PointerPressed += OnPointerPressed;
//         // _canvas.PointerMoved += OnPointerMoved;
//         // _canvas.PointerReleased += OnPointerReleased;
//         // _canvas.PointerEntered += (s, e) => _previewManager.OnEnter();
//         // _canvas.PointerExited += (s, e) => _previewManager.OnExit();
//         // _canvas.KeyDown += OnKeyDown;
//         // _canvas.PointerWheelChanged += OnPointerWheel;
//         // _gridManager.DrawGrid(_canvas); // Draws the main grid
//     }
//     
//
//     public bool IsSimulating
//     {
//         get => _isSimulating;
//         set 
//         {
//             _isSimulating = value;
//
//             if (_isSimulating)
//             {
//                 Selection_UnselectAll();
//                 _previewManager.PreviewComponent = null;
//                 _previewManager.PreviewCompType = "NULL";
//                 _updateTimer!.Start();
//             }
//             else 
//             {
//                 _updateTimer!.Stop();
//             }
//         }
//     }
//
//
//
//     private List<Component> _components;
//     private List<Component> _selectedComponents;
//     private List<Component> _movedWires;
//     [ObservableProperty] private Point _currentMousePos = new Point(0, 0);
//
//     // Selection and interaction managers
//     private readonly PreviewManager _previewManager;
//     private readonly ClipboardManager _clipboardManager;
//     private readonly GridManager _gridManager;
//     private readonly CommandManager _commandManager = new();
//
//     // For simulation
//     private DispatcherTimer? _updateTimer;
//
//     // --------------------
//     // Public attributes
//     // --------------------
//     public CustomComponentData CustomComponent { get; set; } = null!;
//
//     // Public access for Managers
//     public CommandManager CommandManager => _commandManager;
//     public PreviewManager PreviewManager => _previewManager;
//
//     // Public access for Components-related
//     public List<Component> SelectedComponents => _selectedComponents;
//     public bool HasSelectedComponents => _selectedComponents.Count > 0;
//
//     public List<Component> Components
//     {
//         get => _components;
//         set => _components = value;
//     }
//
//     public List<Component> MovedWires
//     {
//         get => _movedWires;
//         set => _movedWires = value;
//     }
//
//     // Preview management
//     public string? PreviewCompType
//     {
//         get => _previewManager.PreviewCompType;
//         set => _previewManager.SetPreviewComponent(value, _canvas, CurrentMousePos, this);
//     }
//
//     // Grid management
//     public bool SnapToGridEnabled
//     {
//         get => _gridManager.SnapToGridEnabled;
//         set => _gridManager.SnapToGridEnabled = value;
//     }
//
//     public bool GridEnabled
//     {
//         get => _gridManager.GridEnabled;
//         set
//         {
//             _gridManager.GridEnabled = value;
//             if (value)
//                 _gridManager.DrawGrid(_canvas);
//             else
//             {
//                 _canvas.Children.Clear();
//                 _canvas.Children.AddRange(_components);
//             }
//         }
//     }
//
//     // -------------------------------------------------
//     // Public methods for simulation construction
//     // -------------------------------------------------
//
//     // ----------------------------------------------------
//     // Private helper Methods for the above constructors
//     // ----------------------------------------------------
//     private void SimulationStep()
//     {
//         foreach (var component in _components)
//         {
//             // Compute outputs for everything first
//             if (component is IOutputProvider op)
//                 op.ComputeOutput();
//
//             // TODO: Needs examination
//             // Redraw Toggles and Probes
//             // if (component is LogicProbe || component is LogicToggle)
//             //     component.InvalidateVisual();
//         }
//     }
//
//     // --------------------------------
//     // Component Management Methods
//     // --------------------------------
//     public void DeleteSelectedComponents()
//     {
//         if (_selectedComponents.Count > 0)
//         {
//             var deleteCommand = new DeleteComponentsCommand(
//                 _canvas, _components, _selectedComponents);
//             _commandManager.ExecuteCommand(deleteCommand);
//             _selectedComponents.Clear();
//         }
//     }
//
//     // TODO: THESE METHODS ARE SHALLOW AND BAD! (probably)
//     public void LoadComponents(List<Component> components)
//     {
//         _components = components;
//         _canvas.Children.AddRange(_components);
//     }
//
//     // sus? amogus?
//     public void DeleteAllComponents()
//     {
//         _canvas.Children.RemoveAll(_components);
//         _components.Clear();
//     }
//
//     // ----------------------------------------------
//     // Important Wrappers for Clipboard Managers
//     // ----------------------------------------------
//     public void CopySelected(bool cutMode = false) => _clipboardManager.Copy(_selectedComponents, cutMode, DeleteSelectedComponents);
//     public void CutSelected() => CopySelected(true);
//     public void PasteSelected() => _clipboardManager.Paste(_canvas, _components, _commandManager, CurrentMousePos);
//
//     // -------------------------
//     // Handling Mouse Actions
//     // -------------------------
//     private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
//     {
//         if (IsSimulating || !GridEnabled)
//         {
//             Console.WriteLine("Cannot edit while simulating");
//             return;
//         }
//         // If a component is being previewed, try to commit it.
//         if (!string.IsNullOrWhiteSpace(_previewManager.PreviewCompType) && !_previewManager.PreviewCompType.Equals("NULL", StringComparison.OrdinalIgnoreCase))
//         {
//             // Try to commit a component or wire preview
//             if (_previewManager.HandleComponentCommit(_canvas, _components, CurrentMousePos, _commandManager, this))
//                 return;
//         }
//
//         // Selection handling through selection manager
//         Selection_HandleStart(CurrentMousePos);
//     }
//
//     private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
//     {
//         Selection_HandleEnd(_commandManager);
//     }
//     
//     private void OnPointerMoved(object? sender, PointerEventArgs e)
//     {
//         // Update the mouse pos
//         CurrentMousePos = e.GetPosition(_canvas);
//
//         if (_previewManager.HandleUpdate(_canvas, CurrentMousePos, _gridManager.SnapToGridEnabled,
//             _gridManager.SnapToGrid, this))
//             return;
//
//         // Pass the selectedComponents reference and grid functions
//         Selection_HandleUpdate(CurrentMousePos, _gridManager.SnapToGridEnabled, _gridManager.SnapToGrid);
//     }
//
//     private void OnPointerWheel(object? sender, PointerWheelEventArgs e)
//     {
//         // TODO: THIS IS ACCEPTABLE FOR NOW BUT 100% NEEDS POLISH LATER ON
//         CurrentMousePos = _gridManager.SnapToGrid(e.GetPosition(_canvas));
//         
//         // Update the preview component
//         if (_previewManager.PreviewComponent != null)
//         {
//             // Update the canvas on Scroll
//             Canvas.SetLeft(_previewManager.PreviewComponent, CurrentMousePos.X);
//             Canvas.SetTop(_previewManager.PreviewComponent, CurrentMousePos.Y);
//         }
//     }
//
//     // Keyboard shortcut support for moving selected components
//     private void OnKeyDown(object? sender, KeyEventArgs e)
//     {
//         // Use this to Handle keys
//     }
//
// }