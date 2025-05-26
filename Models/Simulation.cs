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
    private Canvas _canvas;
    
    private List<Component> _components;
    private List<Component> _selectedComponents;
    public List<Component> Components
    {
        get => _components;
        set => _components = value;
    }

    [ObservableProperty]
    private Point _currentMousePos = new Point(0, 0);
    
    // For selection
    private Point _selectionStart;
    private Rectangle? _selectionRect ;
    
    // For copy/pasting/cutting
    private List<Component> _clipboard = new();
    private Point _lastPastePosition;
    private bool _isPastePreviewActive ;
    private List<Component> _pastePreviewComponents = new();
    
    // Grid Options
    public bool SnapToGridEnabled { get; set; } = true;
    
    private string? _previewCompType;
    private Component? _previewComponent ;

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
                    Canvas.SetTop(wire , 0);
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
        
        // Registering event handlers
        _canvas.PointerPressed += (s, e) =>
        {
            if (HandlePreviewCommit()) return;
            if (HandleSelectionStart()) return;
        };

        _canvas.PointerMoved += (s, e) =>
        {
            // Update the mouse pos
            CurrentMousePos = e.GetPosition(_canvas);

            if (HandlePreviewUpdate()) return;
            if (HandleSelectionUpdate()) return;
        };

        _canvas.PointerReleased += (s, e) =>
        {
            if (HandleSelectionEnd()) return;
        };

        _canvas.PointerEntered += (s, e) =>
        {
            PreviewEnter();
        };
        _canvas.PointerExited += (s, e) =>
        {
            PreviewExit();
        };

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

    public void LoadComponents(List<Component> components)
    {
        _components = components;
        foreach (Component c in components)
        {
            _canvas.Children.Add(c);
        }
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
                Canvas.SetLeft(c, Canvas.GetLeft(component));
                Canvas.SetTop(c, Canvas.GetTop(component));
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
    
    public void PasteSelected()
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
            // Commit point to wire
            wirePreview.AddPoint(CurrentMousePos);
            return true; // Terminate
        }
        
        // Commit component on click
        if(_previewComponent != null)
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
        Point reference = _clipboard[0] is Wire wire ? wire.Points[0] : new Point(Canvas.GetLeft(_clipboard[0]), Canvas.GetTop(_clipboard[0]));
        
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
            // Update last point position
            if (wirePreview.Points.Count > 0)
            {
                wirePreview.Points[^1] = CurrentMousePos;
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

                // Check intersection
                if (componentBounds.Contains(_selectionStart))
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

                    return true; 
                }
                // For wires
                if (component is Wire wire && component.HitTest(_selectionStart))
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
                if (component is Wire wire && wire.Points.Any(p => selectionBounds.Contains(p)))
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
