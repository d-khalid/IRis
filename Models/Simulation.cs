using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace IRis.Models;

// Contains all the data needed for a simulation
public class Simulation
{
    private List<Component> _components;

    private List<Component> _selectedComponents;

    private Canvas _canvas;

    private string? _previewCompType = null;

    private Point _lastMousePos = new Point(0, 0);

    private Point _selectionStart;
    private Rectangle? _selectionRect = null;

    private bool _selectionActive = false;

    private Component? _previewComponent = null;
    
    // Grid Options
    public bool SnapToGridEnabled { get; set; } = true;
    
    

   
    

    public string? PreviewCompType
    {
        get => _previewCompType;
        set
        {
            _previewCompType = value;

            // Remove the old preview comp
            _canvas.Children.Remove(_previewComponent);
            
            // Create a component and add it to the canvas
            _previewComponent = CreateComponent(value);
            if (_previewComponent != null)
            {
                Canvas.SetLeft(_previewComponent, _lastMousePos.X);
                Canvas.SetTop(_previewComponent, _lastMousePos.Y);
                _canvas.Children.Add(_previewComponent);
                Console.WriteLine("Added component via setter");
            }
          

          
            // _components.Add(component);


           
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
        
        
        // Update cursor pos
        _canvas.PointerMoved += (s, e) =>
        {
            // Update mouse pos w/ snap
            _lastMousePos = SnapToGrid(e.GetPosition(_canvas));
        };
        // Register event handlers for selection
        _canvas.PointerPressed += StartSelection;
        _canvas.PointerMoved += UpdateSelection;
        _canvas.PointerReleased += EndSelection;
        
        _canvas.PointerExited += OnExit;
        _canvas.PointerEntered += OnEnter;

        // Update mouse pos and preview on scroll
        // TODO: THIS IS ACCEPTABLE FOR NOW BUT 100% NEEDS POLISH LATER ON
        _canvas.PointerWheelChanged += (s, e) =>
        {
            _lastMousePos = SnapToGrid(e.GetPosition(_canvas));
            
            // Update the preview component
            if (_previewComponent != null)
            {
                // Update rectangle
                Canvas.SetLeft(_previewComponent, _lastMousePos.X);
                Canvas.SetTop(_previewComponent, _lastMousePos.Y);
            }
        };
        
        
        // Draws the main grid
        DrawGrid();

        



    }

    private void OnExit(object? sender, EventArgs e)
    {
        // Hide the preview component
        if (_previewComponent != null)
        {
            _previewComponent.Opacity = 0.0;
        }
    }

    private void OnEnter(object? sender, EventArgs e)
    {
        // Unide the preview component
        if (_previewComponent != null)
        {
            _previewComponent.Opacity = 1.0;
        }
    }

    private void StartSelection(object? sender, PointerPressedEventArgs e)
    {
        _selectionStart = e.GetPosition(_canvas);

        // Select the control we click on
        // Check each component
        foreach (var child in _canvas.Children)
        {
            if (child is Component component)
            {
                // Get component bounds (accounting for rotation if needed)
                var componentPos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
                var componentBounds = new Rect(componentPos, new Size(component.Width, component.Height));

                // Check intersection
                if (componentBounds.Contains(_selectionStart) && component != _previewComponent)
                {
                    // Select/Unselect
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

                    return;
                }
            }
        }

        // PREVIEW LOGIC
        if (_previewComponent != null)
        {
            CommitPreviewComponent();
            return;
        }

        // Start the selection if no component was hit and no preview comp needs to be committed.
        _selectionActive = true;

        // Add a selection rectangle to the canvas
        _selectionRect = new Rectangle
        {
            Width = 0,
            Height = 0,
            Fill = ComponentDefaults.SelectionBrush,
            Stroke = ComponentDefaults.SelectionPen.Brush,
            StrokeThickness = ComponentDefaults.SelectionPen.Thickness
        };
        Canvas.SetLeft(_selectionRect, _lastMousePos.X);
        Canvas.SetTop(_selectionRect, _lastMousePos.Y);
        _canvas.Children.Add(_selectionRect);
    }


    private void UpdateSelection(object? sender, PointerEventArgs e)
    {
        // Update the preview component
        if (_previewComponent != null)
        {
            // Update rectangle
            Canvas.SetLeft(_previewComponent, _lastMousePos.X);
            Canvas.SetTop(_previewComponent, _lastMousePos.Y);
        }

        // Update the selection rectangle
        if (_selectionActive)
        {
            // Calculate bounds
            double left = Math.Min(_selectionStart.X, _lastMousePos.X);
            double top = Math.Min(_selectionStart.Y, _lastMousePos.Y);
            double width = Math.Abs(_selectionStart.X - _lastMousePos.X);
            double height = Math.Abs(_selectionStart.Y - _lastMousePos.Y);

            Rect selectionBounds = new Rect(left, top, width, height);

            // Update rectangle
            Canvas.SetLeft(_selectionRect, left);
            Canvas.SetTop(_selectionRect, top);
            _selectionRect.Width = width;
            _selectionRect.Height = height;

            // Unselect everything and reselect
            UnselectComponents();

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
                }
            }
        }
    }

    private void EndSelection(object? sender, PointerReleasedEventArgs e)
    {
        // // TODO: REMOVE THIS 
        // Component c = CreateComponent("AND");
        // Canvas.SetLeft(c, _lastMousePos.X);
        // Canvas.SetTop(c, _lastMousePos.Y);
        // _canvas.Children.Add(c);
        //
        // _components.Add(c);

        // Remove the selection rect if its there
        if (_selectionActive)
        {
            _canvas.Children.Remove(_selectionRect);
            _selectionRect = null;
            _selectionActive = false;
        }
    }

    private void CommitPreviewComponent()
    {
        Console.WriteLine($"{_previewCompType} committed!");
        var component = CreateComponent(_previewCompType);
        if (component != null)
        {
            Canvas.SetLeft(component, _lastMousePos.X);
            Canvas.SetTop(component, _lastMousePos.Y);
            _canvas.Children.Add(component);
            _components.Add(component);
        }

        //PreviewCompType = "NULL";

    }

    private static Component CreateComponent(string componentType)
    {
        switch (componentType)
        {
            case "AND":
                return new AndGate(4);
            case "OR":
                return new OrGate(2);
            case "NOT":
                return new NotGate();
            case "NAND":
                return new NandGate(2);
            case "NOR":
                return new NorGate(2);
            case "XOR":
                return new XorGate(2);
            case "XNOR":
                return new XnorGate(2);
            case "PROBE":
                return new LogicProbe();
            case "TOGGLE":
                return new LogicToggle();
                break;
            default:
                return null; // TODO: DANGEROUS, THIS IS A FUCKING NULLPO WAITING TO HAPPEN
        }
    }


    // public void AddComponent(Component component)
    // {
    //     _components.Add(component);
    //     _canvasService.AddComponent(component);
    // }
    //
    // public void RemoveComponent(Component component)
    // {
    //     _components.Remove(component);
    //     _canvasService.RemoveComponent(component);
    // }

    // public void AddSelectedComponent(Component component)
    // {
    //     component.IsSelected = true;
    //     _selectedComponents.Add(component);
    // }
    //
    public void DeletedSelectedComponents()
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