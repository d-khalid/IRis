using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models;

public class PreviewManager
{
    private string _previewComponentType;
    private Component? _previewComponent;
    private Point _currentMousePos;
    private Canvas _canvas;
    private List<Component> _components;

    private readonly Func<Point> _getMousePos;


    // Register a canvas + reference to a Point object + Ref to the main comp list
    public PreviewManager(Canvas canvas, Func<Point> mousePosGetter, List<Component> componentList)
    {
        this._canvas = canvas;
        _components = componentList;
        _getMousePos = mousePosGetter;
        // _currentMousePos = currentMousePos;
    }

    public string PreviewComponentType
    {
        get => _previewComponentType;
        set
        {
            _previewComponentType = value;

            // Remove the old preview comp
            _canvas.Children.Remove(_previewComponent);

            // Create a component and add it to the canvas
            _previewComponent = Component.Create(value);
            if (_previewComponent != null)
            {
                // TODO: THIS IS HACKY
                if (_previewComponent is Wire wire)
                {
                    wire.AddPoint(new Point(0, 0));
                    Canvas.SetLeft(_previewComponent, _currentMousePos.X);
                    Canvas.SetTop(_previewComponent, _currentMousePos.Y);

                    // Start point
                    // wire.AddPoint(_currentMousePos); // Moving end point
                }
                else
                {
                    Canvas.SetLeft(_previewComponent, _currentMousePos.X);
                    Canvas.SetTop(_previewComponent, _currentMousePos.Y);
                }

                _canvas.Children.Add(_previewComponent);
                Console.WriteLine("Added component via setter");
            }
        }
    }

    public bool HandlePreviewCommit()
    {
        // Commit point to wire
        if (_previewComponent is Wire wirePreview)
        {
            wirePreview.AddPoint(_currentMousePos - new Point(Canvas.GetLeft(wirePreview), Canvas.GetTop(wirePreview)));
            return true; // Terminate
        }

        // Commit the component on click
        if (_previewComponent != null)
        {
            Component? component = Component.Create(_previewComponentType);

            if (component == null) return true; // Terminate

            component.Rotation = _previewComponent.Rotation;
            Canvas.SetLeft(component, _currentMousePos.X);
            Canvas.SetTop(component, _currentMousePos.Y);

            _canvas.Children.Add(component);
            _components.Add(component);
            Console.WriteLine($"{_previewComponentType} committed!");

            return true; // Terminate
        }

        return false; //Continue
    }

    public bool HandlePreviewUpdate()
    {
        // Update the mouse pos
        _currentMousePos = _getMousePos();
        
        // For wires only
        if (_previewComponent is Wire wirePreview)
        {
            // Update last point position
            if (wirePreview.Points.Count > 0)
            {
                wirePreview.Points[^1] =
                    _currentMousePos - new Point(Canvas.GetLeft(wirePreview), Canvas.GetTop(wirePreview));
                wirePreview.InvalidateVisual();
            }

            return true; // Terminate
        }

        // Update the non-wire preview component
        if (_previewComponent != null)
        {
            Canvas.SetLeft(_previewComponent, _currentMousePos.X);
            Canvas.SetTop(_previewComponent, _currentMousePos.Y);

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
}