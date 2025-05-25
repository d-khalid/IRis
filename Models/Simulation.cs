using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models;

// Contains all the data needed for a simulation
// Currently, this handles the preview
public partial class Simulation : ObservableObject
{
    private List<Component> _components;
    
    public List<Component> Components
    {
        get => _components;
        set => _components = value;
    }

    private List<Component> _selectedComponents;

    private Canvas _canvas;

    private string? _previewCompType;

    [ObservableProperty]
    private Point _currentMousePos = new Point(0, 0);
    

    private Point _selectionStart;
    private Rectangle? _selectionRect ;

    private bool _selectionActive ;

    private Component? _previewComponent ;
    
    
    
    // For copy/pasting/cutting
    private List<Component> _clipboard = new();
    private Point _lastPastePosition;
    private bool _isPastePreviewActive ;
    private List<Component> _pastePreviewComponents = new();
    
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
               
                // TODO: THIS IS HACKY
                if (_previewComponent is Wire wire)
                {
                    wire.AddPoint(new Point(0,0)); 
                    Canvas.SetLeft(_previewComponent, CurrentMousePos.X);
                    Canvas.SetTop(_previewComponent, CurrentMousePos.Y);
                    
                    
                    // Start point
                    // wire.AddPoint(CurrentMousePos); // Moving end point
                }
                else
                {
                    Canvas.SetLeft(_previewComponent, CurrentMousePos.X);
                    Canvas.SetTop(_previewComponent, CurrentMousePos.Y);

                }

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
        
        // Important: Enable keyboard focus
        _canvas.Focusable = true;
        _canvas.Cursor = new Cursor(StandardCursorType.Arrow);
        
        
        // Update cursor pos
        _canvas.PointerMoved += (s, e) =>
        {
            // Update mouse pos w/ snap
            CurrentMousePos = SnapToGrid(e.GetPosition(_canvas));
        };
        // Register event handlers for selection
        _canvas.PointerPressed += OnPointerPressed;
        _canvas.PointerMoved += OnPointerMoved;
        _canvas.PointerReleased += OnPointerReleased;
        
        _canvas.PointerExited += OnExit;
        _canvas.PointerEntered += OnEnter;
        
        // Rotation
        _canvas.KeyDown += (s, e) =>
        {
            // Rotating wires is a terrible idea so no to that
            if (_previewComponent == null) return;

            // Press ENTER to commit a wire)
            if (_previewComponent is Wire && e.Key == Key.Enter)
            {
                CommitWirePreview();
                return;
            }

            if (e.Key == Key.Right)
                _previewComponent.Rotation = 0;
            else if (e.Key == Key.Up)
                _previewComponent.Rotation = 270;
            else if (e.Key == Key.Left)
                _previewComponent.Rotation = 180;
            else if (e.Key == Key.Down)
                _previewComponent.Rotation = 90;
            

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

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _selectionStart = e.GetPosition(_canvas);
        
        if (_isPastePreviewActive)
        {
            CommitPaste();
            //e.Handled = true;
            return;
        }
     

        // For wires
        if (_previewComponent is Wire wirePreview)
        {
            // Commit point to wire
            wirePreview.AddPoint(CurrentMousePos - new Point(Canvas.GetLeft(wirePreview), Canvas.GetTop(wirePreview)));
            e.Handled = true;
            return;
        }
        
        

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
                // For wires
                if (component is Wire wire && component != _previewComponent && component.HitTest(_selectionStart))
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
        
        // Unselect component if hitting empty space
        UnselectComponents();

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
        Canvas.SetLeft(_selectionRect, CurrentMousePos.X);
        Canvas.SetTop(_selectionRect, CurrentMousePos.Y);
        _canvas.Children.Add(_selectionRect);
    }


    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        
        // For wires only
        if (_previewComponent is Wire wirePreview)
        {
            // Update last point position
            if (wirePreview.Points.Count > 0)
            {
                wirePreview.Points[^1] = CurrentMousePos - new Point(Canvas.GetLeft(wirePreview), Canvas.GetTop(wirePreview));
                wirePreview.InvalidateVisual();
            }
            return;
        }
        
        // Update the non-wire preview component
        if (_previewComponent != null)
        {
            // if (_previewComponent is Wire wire)
            // {
            //     // Update rectangle
            //     Canvas.SetLeft(_previewComponent, CurrentMousePos.X + wire.Points[0].X);
            //     Canvas.SetTop(_previewComponent, -CurrentMousePos.Y + wire.Points[0].Y);
            // }
            // else
            // {
                // Update rectangle
                Canvas.SetLeft(_previewComponent, CurrentMousePos.X);
                Canvas.SetTop(_previewComponent, CurrentMousePos.Y);
            

        
        }

        // Update the selection rectangle
        if (_isPastePreviewActive)
        {
            // Also snaps to grid if enabled
            UpdatePastePreviewPosition(CurrentMousePos);
        }
        else if (_selectionActive)
        {
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

                    // To select a wire, the selection must contain one of its vertices
                    if (component is Wire wire && wire.Points.Any(p => selectionBounds.Contains(p + 
                            new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire)))))
                    {
                        component.IsSelected = true;
                        _selectedComponents.Add(component);
                    }
                }
            }
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
      
     
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
            component.Rotation = _previewComponent.Rotation;
            Canvas.SetLeft(component, CurrentMousePos.X);
            Canvas.SetTop(component, CurrentMousePos.Y);
            
            _canvas.Children.Add(component);
            _components.Add(component);
        }

        //PreviewCompType = "NULL";

    }
    
   
    private void CommitWirePreview()
    {
        if (_previewComponent is not Wire wirePreview) return;
        
        // Finalize wire if it has at least 2 points
        if (wirePreview.Points.Count >= 2)
        {
            _components.Add(wirePreview);
        }
        else
        {
            _canvas.Children.Remove(wirePreview);
        }
        _previewComponent = null;
    }

    private static Component CreateComponent(string componentType)
    {
        switch (componentType)
        {
            case "AND":
                return new AndGate(2);
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
            case "WIRE":
                return new Wire();
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
    
    
    // Adds selected components to clipboard
    public void CopySelected(bool cutMode = false)
    {
        _clipboard.Clear();
        foreach (var component in _selectedComponents)
        {
            Component c = (Component)component.Clone();
            _clipboard.Add(c);

           
            
            // copy component positions
            if (c is Wire wire)
            {
                Canvas.SetLeft(c, -Canvas.GetLeft(component));
                Canvas.SetTop(c, -Canvas.GetTop(component));
            }
            else
            {
                Canvas.SetLeft(c, Canvas.GetLeft(component));
                Canvas.SetTop(c, Canvas.GetTop(component));
            }




            component.IsSelected = false;
        }
        if(cutMode) DeleteSelectedComponents();
        _selectedComponents.Clear();
    }
    
    public void CutSelected()
    {
        CopySelected(true);
       
    }
    
    public void StartPastePreview()
    {
        if (_clipboard.Count == 0) return;
        
        _pastePreviewComponents.Clear();
        _isPastePreviewActive = true;
        
        foreach (var component in _clipboard)
        {
            var clone = (Component)component.Clone();
            _pastePreviewComponents.Add(clone);
            _canvas.Children.Add(clone);
            
            //Canvas.SetZIndex(clone, int.MaxValue - 1); // Below selection but above others
            //clone.Opacity = 0.7;
        }
        UpdatePastePreviewPosition(CurrentMousePos);
    }
    
    public void CommitPaste()
    {
        if (!_isPastePreviewActive) return;
 
        //_selectedComponents.Clear();
        _selectedComponents.AddRange(_pastePreviewComponents);
        _components.AddRange(_pastePreviewComponents);
        
        // Clear the paste preview so that the gates we have are stuck in the canvas
        _pastePreviewComponents.Clear();
        _isPastePreviewActive = false;
    }
    
    private void UpdatePastePreviewPosition(Point position)
    {
        if (!_isPastePreviewActive) return;
        
        _lastPastePosition = position;
        //Point offset = CalculatePasteOffset();

        
        Point reference = new Point(Canvas.GetLeft(_clipboard[0]), Canvas.GetTop(_clipboard[0]));
        for (int i = 0; i < _pastePreviewComponents.Count; i++)
        {
            var original = _clipboard[i];
            var preview = _pastePreviewComponents[i];

            Console.WriteLine($"{Canvas.GetLeft(original)}, {Canvas.GetTop(original)}");
            Canvas.SetLeft(preview,  reference.X - Canvas.GetLeft(original) + CurrentMousePos.X);
            Canvas.SetTop(preview, reference.Y - Canvas.GetTop(original) + CurrentMousePos.Y);
        }
    }
    
    // public void ClearPastePreview()
    // {
    //     foreach (var component in _pastePreviewComponents)
    //     {
    //         _canvas.Children.Remove(component);
    //     }
    //     _pastePreviewComponents.Clear();
    //     _isPastePreviewActive = false;
    // }

    
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
