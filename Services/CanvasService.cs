using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using IRis.Models;

namespace IRis.Services;

// This is a service that provides controlled access to the canvas
// and provides some useful abstractions for basic ops

// THIS CLASS SUCKS SO MUCH I HATE IT AAAAAA
public class CanvasService 
{
    private Canvas? _canvas;

    private Component? _currentPreview;
    
    private Point _lastMousePosition;

    private Point _selectionStart;
    
    private Point _selectionEnd;

    private Control _selectionBox;
    
    
    private bool _isPreviewActive;

    // Registers a canvas with this service
    public CanvasService()
    {
        
    }

    public void Register(Canvas canvas)
    {
        _canvas = canvas;
        
        
      
        // Register required event handlers
        _canvas.PointerMoved += OnPointerMoved;
        _canvas.PointerReleased += OnPointerReleased;
        _canvas.PointerPressed += OnPointerPressed;
        
       
        

    }
    
    

    public void StartPreview(Component component)
    {
        if (_currentPreview != null)
        {
            // Remove existing preview first
            _canvas.Children.Remove(_currentPreview);
        }

        _currentPreview = component;
        _canvas.Children.Add(_currentPreview);
        _isPreviewActive = true;
        
        // Set initial position (center on cursor)
        // NOTE: I THINK THE ISSUE THAT THIS ONLY GETS CALLED ONCE
        Canvas.SetLeft(_currentPreview, _lastMousePosition.X - _currentPreview.Width / 2); // Start off-screen
        Canvas.SetTop(_currentPreview, _lastMousePosition.Y - _currentPreview.Height / 2);
        
        // Minimal required updates
        _canvas.InvalidateArrange();  // Updates layout
        _currentPreview.InvalidateVisual();  // Redraws the element
        
        
    }
    
    private void InitializeSelectionBox()
    {
        _selectionBox = new Border
        {
            Background = Brushes.Transparent,
            BorderBrush = Brushes.DodgerBlue,
            BorderThickness = new Thickness(1),
            IsVisible = false,
            Opacity = 0.5
        };
        _canvas.Children.Add(_selectionBox);

    }
    
    private void UpdateSelectionBox(Point currentPosition)
    {
        double left = Math.Min(_selectionStart.X, currentPosition.X);
        double top = Math.Min(_selectionStart.Y, currentPosition.Y);
        double width = Math.Abs(currentPosition.X - _selectionStart.X);
        double height = Math.Abs(currentPosition.Y - _selectionStart.Y);

        Canvas.SetLeft(_selectionBox, left);
        Canvas.SetTop(_selectionBox, top);
        _selectionBox.Width = width;
        _selectionBox.Height = height;
    }

    public void EndSelection()
    {
        _selectionBox.IsVisible = false;
        // SelectComponentsInBounds(new Rect(
        //     Canvas.GetLeft(_selectionBox),
        //     Canvas.GetTop(_selectionBox),
        //     _selectionBox.Width,
        //     _selectionBox.Height));
    }


    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        // Store the starting point of a selection
        _selectionStart = e.GetPosition(_canvas);

        e.Handled = false; //so it goes down to the children

        Console.WriteLine("CANVAS PRESSED");
        foreach (var child in _canvas.Children)
            
            Console.WriteLine($"{child.GetType().Name} - ZIndex: {child.ZIndex}");
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!_isPreviewActive || _currentPreview == null) return;

        var newPos = e.GetPosition(_canvas);
    
        // Only update if moved ≥1 pixel (reduces CPU/GPU load)
        if (Math.Abs(newPos.X - _lastMousePosition.X) < 1 && 
            Math.Abs(newPos.Y - _lastMousePosition.Y) < 1) 
            return;

        _lastMousePosition = newPos;

        // Calculate centered position
        Canvas.SetLeft(_currentPreview, newPos.X - _currentPreview.Width / 2);
        Canvas.SetTop(_currentPreview, newPos.Y - _currentPreview.Height / 2);

        // Minimal required updates
        _canvas.InvalidateArrange();  // Updates layout
        _currentPreview.InvalidateVisual();  // Redraws the element
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _isPreviewActive = false;
        _currentPreview = null;

        // var position = e.GetPosition(_canvas);
        // CommitPreview(position);
    }
    

    // public void ClearPreview()
    // {
    //     if (_currentPreview != null)
    //     {
    //         _canvas.Children.Remove(_currentPreview);
    //         _currentPreview = null;
    //     }
    //     _isPreviewActive = false;
    // }

    public void Dispose()
    {
        _canvas.PointerMoved -= OnPointerMoved;
        _canvas.PointerReleased -= OnPointerReleased;
    }
    // public void AddComponent(CircuitComponent component, Point position)
    // {
    //     Dispatcher.UIThread.Post(() =>
    //     {
    //         Canvas.SetLeft(component, position.X);
    //         Canvas.SetTop(component, position.Y);
    //         _canvas?.Children.Add(component);
    //     });
    // }
    
    public void AddComponent(Component component)
    {
        Dispatcher.UIThread.Post(() =>
        {
            
            Canvas.SetLeft(component, _lastMousePosition.X);
            Canvas.SetTop(component, _lastMousePosition.Y);
            _canvas?.Children.Add(component);
        });
    }
    
    // Implement other interface methods...

    // For drawing a grid
    public void DrawGrid(double gridspacing)
    {
        Console.WriteLine("Width: {0}" , _canvas.MinWidth);

        for (double x = 0; x < _canvas.MinWidth; x += gridspacing)
        {
            Line l = new Line();
            l.StartPoint = new Point(0, 0);
            l.EndPoint = new Point(0, _canvas.MinHeight);
            l.Stroke = Brushes.Black;
            l.StrokeThickness = 0.5;
            
            for (double y = 0; y < _canvas.MinWidth; y += gridspacing)
            {
                Line m = new Line();
                m.StartPoint = new Point(0, 0);
                m.EndPoint = new Point(_canvas.MinWidth, 0);
                m.Stroke = Brushes.Black;
                m.StrokeThickness = 0.5;
            
            
            
                Canvas.SetLeft(m, 0);
                Canvas.SetTop(m, y);
                _canvas.Children.Add(m);
            }
            
            Canvas.SetLeft(l, x);
            Canvas.SetTop(l, 0);
            _canvas.Children.Add(l);
        }
    }
    
    
}