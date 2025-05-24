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
    public List<Component> SelectedComponents { get; } = new();

    private Canvas _canvas;
    
    
    // TODO: I CANT PASS AROUND A REFERENCE TO THIS POINT OBJECT BECAUSE IT KEEPS BEING REASSIGNED
    // I NEED MOUSEPOS IN MY OTHER CLASSES
    [ObservableProperty]
    private Point _currentMousePos = new Point(0, 0);
    
    // Getter is passed to manager classes
    public Point GetCurrentMousePos() => CurrentMousePos;
    
    // Grid Options
    public bool SnapToGridEnabled { get; set; } = true;
    
    public List<Component> Components
    {
        get => _components;
    }
    
    private PreviewManager _previewManager;
    private SelectionManager _selectionManager;
    private ClipboardManager _clipboardManager;
    
   
    public Simulation()
    {
        // Initialize lists
        _components = new List<Component>();
        
    }

    
    // Initalizes the manager objects
    // Registers all event handlers
    // NOTE: BE CAREFUL ABOUT THE BOOL RETURN TYPES, True == return from event handler
    public void Register(Canvas canvas)
    {
        _canvas = canvas;

        // Initialize the managing components
        _previewManager = new PreviewManager(canvas, GetCurrentMousePos, Components);
        _selectionManager = new SelectionManager(canvas, GetCurrentMousePos, SelectedComponents);
        _clipboardManager = new ClipboardManager(canvas, GetCurrentMousePos, Components, SelectedComponents);
        
        
        // Important: Enable keyboard focus
        _canvas.Focusable = true;
        _canvas.Cursor = new Cursor(StandardCursorType.Arrow);
        
        // Register event handlers
        _canvas.PointerPressed += (s, e) =>
        {
            if (_previewManager.HandlePreviewCommit()) return;
            if (_selectionManager.HandleSelectionStart()) return;
        };

        _canvas.PointerMoved += (s, e) =>
        {
            // Update mouse pos w/ snap
            CurrentMousePos = SnapToGrid(e.GetPosition(_canvas));
            
            if(_previewManager.HandlePreviewUpdate()) return;
            
            if(_selectionManager.HandleSelectionUpdate()) return;
            
        };
        // Register event handlers for selection

        _canvas.PointerReleased += (s, e) =>
        {
            if(_selectionManager.HandleSelectionEnd()) return;
        };
        
        _canvas.PointerEntered+= (s, e) =>
        {
            _previewManager.PreviewEnter();
        };

        _canvas.PointerExited += (s, e) =>
        {
            _previewManager.PreviewExit();
        };
       
        
        
        // Update mouse pos and preview on scroll
        // TODO: THIS IS ACCEPTABLE FOR NOW BUT 100% NEEDS POLISH LATER ON
        _canvas.PointerWheelChanged += (s, e) =>
        {
            // Update mouse pos
            CurrentMousePos = SnapToGrid(e.GetPosition(_canvas));
            
            // Update the preview component
            if(_previewManager.HandlePreviewUpdate()) return;
        };
        
        
        // Draws the main grid
        DrawGrid();

        



    }

    
    // TODO: I ENDED UP WITH THIS RANDOM WRAPPER FUNCTION
    public void SetPreviewCompType(string compType)
    {
        _previewManager.PreviewComponentType = compType;
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