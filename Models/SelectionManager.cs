using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models;

public class SelectionManager
{
    private readonly Canvas _canvas;

    private Point _selectionStart;
    private Rectangle? _selectionRect; // 'null' means that selection is inactive
    
    private Point _currentMousePos;
    private Func<Point> _getMousePos;

    private List<Component> _selectedComponents;

    // Register a reference to the Canvas and a Point object
    public SelectionManager(Canvas canvas, Func<Point> mousePosGetter, List<Component> selectedComponents)
    {
        this._canvas = canvas;
        _getMousePos = mousePosGetter;
        _selectedComponents = selectedComponents;
    }

    // Returns true if we want to stop further execution of the main eventhandler
    // False to keep running
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
        // Update the mouse pos
        _currentMousePos = _getMousePos();

        
        // No selection area to update, let the event handler go on
        if (_selectionRect == null) return false;
         // Update the mouse pos
        _currentMousePos = _getMousePos();
        
        // Calculate bounds
        double left = Math.Min(_selectionStart.X, _currentMousePos.X);
        double top = Math.Min(_selectionStart.Y, _currentMousePos.Y);
        double width = Math.Abs(_selectionStart.X - _currentMousePos.X);
        double height = Math.Abs(_selectionStart.Y - _currentMousePos.Y);

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
                if (component is Wire wire && wire.Points.Any(p => selectionBounds.Contains(p + 
                        new Point(Canvas.GetLeft(wire), Canvas.GetTop(wire)))))
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


}