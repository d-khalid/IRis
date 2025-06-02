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

    // For selection
    private Point _selectionStart;
    private Rectangle? _selectionRect;

    // For copy/pasting/cutting
    private List<Component> _clipboard = new();
    private Point _lastPastePosition;
    private bool _isPastePreviewActive;
    private List<Component> _pastePreviewComponents = new();

    // Grid Options
    public bool SnapToGridEnabled { get; set; } = false;

    private string? _previewCompType;
    private Component? _previewComponent;

    private DispatcherTimer _updateTimer;

    public string? PreviewCompType
    {
        get => _previewCompType;
        set
        {
            _previewCompType = value;

            // Remove the old preview comp
            _canvas.Children.Remove(_previewComponent);

            // Create a component and add it to the canvas
            _previewComponent = Component.Create(value);
            if (_previewComponent != null)
            {
                // TODO: THIS IS HACKY
                if (_previewComponent is Wire wire)
                {
                    wire.AddPoint(CurrentMousePos);
                    Canvas.SetLeft(wire, 0);
                    Canvas.SetTop(wire, 0);
                }
                else
                {
                    Canvas.SetLeft(_previewComponent, CurrentMousePos.X);
                    Canvas.SetTop(_previewComponent, CurrentMousePos.Y);
                }

                _canvas.Children.Add(_previewComponent);
                Console.WriteLine("Added component via setter");
            }
        }
    }

    public Simulation()
    {
        // Initialize lists
        _components = new List<Component>();
        _selectedComponents = new List<Component>();
    }

    public void Register(Canvas canvas)
    {
        _canvas = canvas;

        // Important: Enable keyboard focus
        _canvas.Focusable = true;
        _canvas.Cursor = new Cursor(StandardCursorType.Arrow);

        // For updating the simulation
        _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) }; // Adjust to reduce CPU load
        _updateTimer.Tick += (s, e) => SimulationStep();
        _updateTimer.Start();

        // Registering event handlers
        _canvas.PointerPressed += (s, e) =>
        {
            if (HandlePreviewCommit()) return;
            if (HandleSelectionStart()) return;
        };

        _canvas.PointerMoved += (s, e) =>
        {
            // Update the mouse pos
            CurrentMousePos = SnapToGridEnabled ? SnapToGrid(e.GetPosition(_canvas)) : e.GetPosition(_canvas);

            if (HandlePreviewUpdate()) return;
            if (HandleSelectionUpdate()) return;
        };

        _canvas.PointerReleased += (s, e) =>
        {
            if (HandleSelectionEnd()) return;
        };

        _canvas.PointerEntered += (s, e) => { PreviewEnter(); };
        _canvas.PointerExited += (s, e) => { PreviewExit(); };

        _canvas.KeyDown += (s, e) =>
        {
            if (HandlePreviewKeyCommand(e)) return;
        };


        // Update mouse pos and preview on scroll
        // TODO: THIS IS ACCEPTABLE FOR NOW BUT 100% NEEDS POLISH LATER ON
        _canvas.PointerWheelChanged += (s, e) =>
        {
            CurrentMousePos = SnapToGrid(e.GetPosition(_canvas));

            // Update the preview component
            if (_previewComponent != null)
            {
                // Update rectangle
                Canvas.SetLeft(_previewComponent, CurrentMousePos.X);
                Canvas.SetTop(_previewComponent, CurrentMousePos.Y);
            }
        };


        // Draws the main grid
        DrawGrid();
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


    public void DeleteSelectedComponents()
    {
        for (int i = 0; i < _selectedComponents.Count; i++)
        {
            _canvas.Children.Remove(_selectedComponents[i]);
            _components.Remove(_selectedComponents[i]);
        }

        _selectedComponents.Clear();
    }

    public void UnselectComponents()
    {
        foreach (Component c in _selectedComponents)
        {
            c.IsSelected = false;
        }

        _selectedComponents.Clear();
    }

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


    // Adds selected components to clipboard
    public void CopySelected(bool cutMode = false)
    {
        _clipboard.Clear(); 
    
        
        // First pass
        foreach (var component in _selectedComponents)
        {
            Component c = (Component)component.Clone();
            _clipboard.Add(c);
            
            Canvas.SetLeft(c, Canvas.GetLeft(component));
            Canvas.SetTop(c, Canvas.GetTop(component));
            component.IsSelected = false;
        }

        if (cutMode) DeleteSelectedComponents();
        
        _selectedComponents.Clear();
    }
    
    // public void CopySelected(bool cutMode = false)
    // {
    //     _clipboard.Clear();
    //
    //     // Track original wires and their clones
    //     var wireClones = new Dictionary<Wire, Wire>();
    //
    //     // First pass: Clone all components (keeping original wire references temporarily)
    //     var componentClones = new List<Component>();
    //     foreach (var component in _selectedComponents)
    //     {
    //         // Basic clone without fixing wires yet
    //         Component clone = (Component)component.Clone();
    //         componentClones.Add(clone);
    //
    //         // Preserve position
    //         Canvas.SetLeft(clone, Canvas.GetLeft(component));
    //         Canvas.SetTop(clone, Canvas.GetTop(component));
    //         component.IsSelected = false;
    //     }
    //
    //     // Second pass: Clone wires and reconnect terminals
    //     for (int i = 0; i < _selectedComponents.Count; i++)
    //     {
    //         var original = _selectedComponents[i];
    //         var clone = componentClones[i];
    //
    //         // No need to check anything that doesn't have terminals
    //         if (original.Terminals == null) continue;
    //
    //         // Process all terminals
    //         for (int t = 0; t < original.Terminals.Length; t++)
    //         {
    //             var originalWire = original.Terminals[t].Wire;
    //             if (originalWire == null) continue;
    //
    //             // Get or create cloned wire
    //             if (!wireClones.TryGetValue(originalWire, out Wire clonedWire))
    //             {
    //                 clonedWire = new Wire()
    //                 {
    //                     // Copy all wire properties
    //                     Id = Guid.NewGuid(), // Generate new ID for the clone
    //                     Value = originalWire.Value, // Copy current logic state
    //                     Points = new List<Point>(originalWire.Points) // Deep clone points
    //                 };
    //                 wireClones.Add(originalWire, clonedWire);
    //
    //
    //                 //clone.Terminals[t].Wire = clonedWire;
    //             }
    //             clone.Terminals[t].Wire = wireClones[originalWire];
    //
    //             _clipboard.Add(clone);
    //         }
    //
    //         _clipboard.AddRange(wireClones.Values.ToList());
    //         
    //
    //         if (cutMode) DeleteSelectedComponents();
    //         _selectedComponents.Clear();
    //     }
    // }

    public void CutSelected()
    {
        CopySelected(true);
    }

    public void PasteSelected()
    {
        if (_clipboard.Count == 0) return;

        _pastePreviewComponents.Clear();
        _isPastePreviewActive = true;

        List<Component> clonedComponents = new();
        
        // First pass
        foreach (var component in _clipboard)
        {
            // new Wire objects will be made later
            if (component is Wire) continue;
            
            Component c = (Component)component.Clone();
            clonedComponents.Add(c);
            
            Canvas.SetLeft(c, Canvas.GetLeft(component));
            Canvas.SetTop(c, Canvas.GetTop(component));
            component.IsSelected = false;
        }
        
        // Second pass
        Dictionary<Wire, Wire> clonedWires = new();
        foreach (var component in clonedComponents)
        {
            if (component.Terminals == null) continue;

            foreach (var terminal in component.Terminals)
            {
                if(terminal.Wire == null) continue;

                // If there's a matching wire for an original wire already, make it
                if (!clonedWires.TryGetValue(terminal.Wire, out var clonedWire))
                {
                    Wire newWire = new Wire();
                    
                    newWire.Points = terminal.Wire.Points;
                    newWire.Id = Guid.NewGuid();
                    
                    // enums are value types!
                    newWire.Value = terminal.Wire.Value;
                    
                    Canvas.SetLeft(newWire, Canvas.GetLeft(terminal.Wire));
                    Canvas.SetTop(newWire, Canvas.GetTop(terminal.Wire));

                    
                    clonedWires.Add(terminal.Wire, newWire);
                }

                terminal.Wire = clonedWires[terminal.Wire];
            }
        }
        
        // Add things to preview list
        _pastePreviewComponents.AddRange(clonedComponents);
        _pastePreviewComponents.AddRange(clonedWires.Values.ToList());
        _canvas.Children.AddRange(_pastePreviewComponents);
        

        UpdatePastePreviewPosition();
    }


    // PREVIEW FUNCTIONS START
    private bool HandlePreviewCommit()
    {
        // Commit pasted components
        if (_isPastePreviewActive)
        {
            //_selectedComponents.Clear();
            _selectedComponents.AddRange(_pastePreviewComponents);
            _components.AddRange(_pastePreviewComponents);

            // Clear the paste preview so that the gates we have are stuck in the canvas
            _pastePreviewComponents.Clear();
            _isPastePreviewActive = false;

            return true; // Terminate
        }

        // Commit points to wire
        if (_previewComponent is Wire wirePreview)
        {
            // // Commit point to wire
            // wirePreview.AddPoint(CurrentMousePos);
            //
            // Check for a terminal we can snap to
            Terminal? target =
                FindClosestSnapTerminal(CurrentMousePos, ComponentDefaults.TerminalSnappingRange, out var pos);

            if (target != null)
            {
                target.Wire = wirePreview;
            }

            wirePreview.AddPoint(pos);

            return true; // Terminate
        }

        // Commit component on click
        if (_previewComponent != null)
        {
            Component? component = Component.Create(_previewCompType);

            if (component == null) return true; // Terminate

            component.Rotation = _previewComponent.Rotation;
            Canvas.SetLeft(component, CurrentMousePos.X);
            Canvas.SetTop(component, CurrentMousePos.Y);

            _canvas.Children.Add(component);
            _components.Add(component);
            Console.WriteLine($"{_previewCompType} committed!");

            return true; // Terminate
        }

        return false; // Continue
    }

    // TODO: MAKE THIS ALWAYS USE THE TOP-LEFT-MOST ELEMENT IN THE CLIPPBOARD
    private void UpdatePastePreviewPosition()
    {
        if (!_isPastePreviewActive) return;

        _lastPastePosition = CurrentMousePos;
        //Point offset = CalculatePasteOffset();

        // Use the wire's first point as reference
        Point reference = _clipboard[0] is Wire wire
            ? wire.Points[0]
            : new Point(Canvas.GetLeft(_clipboard[0]), Canvas.GetTop(_clipboard[0]));

        for (int i = 0; i < _pastePreviewComponents.Count; i++)
        {
            var original = _clipboard[i];
            var preview = _pastePreviewComponents[i];


            Console.WriteLine($"{Canvas.GetLeft(original)}, {Canvas.GetTop(original)}");
            Canvas.SetLeft(preview, Canvas.GetLeft(original) + CurrentMousePos.X - reference.X);
            Canvas.SetTop(preview, Canvas.GetTop(original) + CurrentMousePos.Y - reference.Y);
        }
    }

    private bool HandlePreviewUpdate()
    {
        if (_isPastePreviewActive)
        {
            UpdatePastePreviewPosition();
            return true; // Terminate
        }

        // For wires only
        if (_previewComponent is Wire wirePreview)
        {
            if (wirePreview.Points.Count > 0)
            {
                // Make wires snap to the closest terminal in range
                Terminal? snap = FindClosestSnapTerminal(CurrentMousePos, ComponentDefaults.TerminalSnappingRange,
                    out Point pos);

                wirePreview.Points[^1] = pos;
                wirePreview.InvalidateVisual();
            }

            return true; // Terminate
        }

        // Update the non-wire preview component
        if (_previewComponent != null)
        {
            Canvas.SetLeft(_previewComponent, CurrentMousePos.X);
            Canvas.SetTop(_previewComponent, CurrentMousePos.Y);

            return true; // Terminate
        }

        return false; // Continue
    }

    private Terminal? FindClosestSnapTerminal(Point p, double snappingRange, out Point absolutePos)
    {
        absolutePos = p;
        Terminal? closestTerminal = null;
        double minDistance = double.MaxValue;

        foreach (Component component in _components)
        {
            if (component.Terminals == null)
                continue;

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

    public bool HandlePreviewKeyCommand(KeyEventArgs e)
    {
        // Rotating wires is a terrible idea so no to that
        if (_previewComponent == null) return true; // terminate

        // Press ENTER to commit a wire)
        if (_previewComponent is Wire wire && e.Key == Key.Enter)
        {
            // Finalize wire if it has at least 2 points
            if (wire.Points.Count >= 2)
            {
                _components.Add(wire);
            }
            else
            {
                _canvas.Children.Remove(wire);
            }

            _previewComponent = null;
            return true; // Terminate
        }

        if (e.Key == Key.Right)
            _previewComponent.Rotation = 0;
        else if (e.Key == Key.Up)
            _previewComponent.Rotation = 270;
        else if (e.Key == Key.Left)
            _previewComponent.Rotation = 180;
        else if (e.Key == Key.Down)
            _previewComponent.Rotation = 90;

        return false; // Continue
    }

    public void PreviewExit()
    {
        // Hide the preview component
        if (_previewComponent != null)
        {
            _previewComponent.Opacity = 0.0;
        }
    }

    public void PreviewEnter()
    {
        // Unide the preview component
        if (_previewComponent != null)
        {
            _previewComponent.Opacity = 1.0;
        }
    }
    // PREVIEW FUNCTIONS END

    // SELECTION FUNCTIONS START
    public bool HandleSelectionStart()
    {
        _selectionStart = _currentMousePos;

        // Select the control we click on, the preview component won't be selectable
        foreach (var child in _canvas.Children)
        {
            if (child is Component component)
            {
                // Get component bounds (accounting for rotation if needed)
                var componentPos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
                var componentBounds = new Rect(componentPos, new Size(component.Width, component.Height));

                // Check intersection for components (gates)
                if (componentBounds.Contains(_selectionStart))
                {
                    // Select/Unselect based on if it's already selected
                    ToggleSelection(component);
                    return true;
                }

                // Check For wires
                if (component is Wire wire && component.HitTest(_selectionStart))
                {
                    // Select/Unselect based on if it's already selected
                    ToggleSelection(component);
                    return true;
                }
            }
        }

        // Unselect components if hitting empty space
        foreach (Component c in _selectedComponents)
        {
            c.IsSelected = false;
        }

        _selectedComponents.Clear();

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
        _canvas.Children.Add(_selectionRect);

        return false; // Continue running the main event handler
    }

    // Helper for HandleSelectionStart()
    private void ToggleSelection(Component component)
    {
        switch (component.IsSelected)
        {
            case true:
                _selectedComponents.Remove(component);
                component.IsSelected = false;
                break;
            case false:
                _selectedComponents.Add(component);
                component.IsSelected = true;
                break;
        }
    }


    public bool HandleSelectionUpdate()
    {
        // No selection area to update, let the event handler go on
        if (_selectionRect == null) return false;

        // Calculate bounds
        double left = Math.Min(_selectionStart.X, CurrentMousePos.X);
        double top = Math.Min(_selectionStart.Y, CurrentMousePos.Y);
        double width = Math.Abs(_selectionStart.X - CurrentMousePos.X);
        double height = Math.Abs(_selectionStart.Y - CurrentMousePos.Y);

        Rect selectionBounds = new Rect(left, top, width, height);

        // Update rectangle
        Canvas.SetLeft(_selectionRect, left);
        Canvas.SetTop(_selectionRect, top);
        _selectionRect.Width = width;
        _selectionRect.Height = height;

        // Unselect everything and reselect again
        foreach (Component c in _selectedComponents)
        {
            c.IsSelected = false;
        }

        _selectedComponents.Clear();

        // Check each component
        foreach (var child in _canvas.Children)
        {
            if (child is Component component)
            {
                // Get component bounds (accounting for rotation if needed)
                var componentPos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
                var componentBounds = new Rect(componentPos, new Size(component.Width, component.Height));

                // Check intersection
                if (selectionBounds.Intersects(componentBounds))
                {
                    component.IsSelected = true;
                    _selectedComponents.Add(component);
                }

                // To select a wire, the selection must contain one of its vertices
                if (component is Wire wire && wire.Points.Any(p => selectionBounds.Contains(p + new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire)))))
                {
                    component.IsSelected = true;
                    _selectedComponents.Add(component);
                }
            }
        }

        return false; // Resume
    }

    public bool HandleSelectionEnd()
    {
        // Remove the selection rect if its there
        if (_selectionRect != null)
        {
            _canvas.Children.Remove(_selectionRect);
            _selectionRect = null;
        }

        return false; // Resume 
    }
    // SELECTION FUNCTIONS END


    private Point SnapToGrid(Point point)
    {
        if (!SnapToGridEnabled) return point;

        double snappedX = Math.Round(point.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
        double snappedY = Math.Round(point.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
        return new Point(snappedX, snappedY);
    }

    public void DrawGrid()
    {
        if (_canvas == null) return;

        // Clear existing grid lines (if any)
        // Useful for redraws
        var gridLines = _canvas.Children.OfType<Line>().Where(l => l.Tag?.ToString() == "grid").ToList();
        foreach (var line in gridLines)
        {
            _canvas.Children.Remove(line);
        }

        double width = _canvas.MinWidth;
        double height = _canvas.MinHeight;

        // Draw vertical lines
        for (double x = 0; x < width; x += ComponentDefaults.GridSpacing)
        {
            var line = new Line
            {
                StartPoint = new Point(x, 0),
                EndPoint = new Point(x, height),
                Stroke = ComponentDefaults.GridBrush,
                StrokeThickness = ComponentDefaults.GridThickness,
                Tag = "grid" // For easy identification
            };
            _canvas.Children.Add(line);
        }

        // Draw horizontal lines
        for (double y = 0; y < height; y += ComponentDefaults.GridSpacing)
        {
            var line = new Line
            {
                StartPoint = new Point(0, y),
                EndPoint = new Point(width, y),
                Stroke = ComponentDefaults.GridBrush,
                StrokeThickness = ComponentDefaults.GridThickness,
                Tag = "grid"
            };
            _canvas.Children.Add(line);
        }
    }
}