using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using IRis.Models.Circuit;

namespace IRis.Services;

// This is a service that provides controlled access to the canvas
// and provides some useful abstractions for basic ops
public class CanvasService 
{
    private Canvas? _mainCanvas;
    
    Point _lastMousePosition;

    public void RegisterCanvas(Canvas canvas)
    {
        _mainCanvas = canvas;
        
        // NOTE EXPERIMENTAL

        // Store the cursor pos and update it
        canvas.PointerMoved += (s, e) =>
        {
            _lastMousePosition = e.GetPosition(canvas); 
           
        };

       DrawGrid(15);




    }

    // public void AddComponent(CircuitComponent component, Point position)
    // {
    //     Dispatcher.UIThread.Post(() =>
    //     {
    //         Canvas.SetLeft(component, position.X);
    //         Canvas.SetTop(component, position.Y);
    //         _mainCanvas?.Children.Add(component);
    //     });
    // }
    
    public void AddComponent(CircuitComponent component)
    {
        Dispatcher.UIThread.Post(() =>
        {
            
            Canvas.SetLeft(component, _lastMousePosition.X);
            Canvas.SetTop(component, _lastMousePosition.Y);
            _mainCanvas?.Children.Add(component);
        });
    }
    
    // Implement other interface methods...

    // For drawing a grid
    public void DrawGrid(double gridspacing)
    {
        Console.WriteLine("Width: {0}" , _mainCanvas.MinWidth);

        for (double x = 0; x < _mainCanvas.MinWidth; x += gridspacing)
        {
            Line l = new Line();
            l.StartPoint = new Point(0, 0);
            l.EndPoint = new Point(0, _mainCanvas.MinHeight);
            l.Stroke = Brushes.Black;
            l.StrokeThickness = 0.5;
            
            for (double y = 0; y < _mainCanvas.MinWidth; y += gridspacing)
            {
                Line m = new Line();
                m.StartPoint = new Point(0, 0);
                m.EndPoint = new Point(_mainCanvas.MinWidth, 0);
                m.Stroke = Brushes.Black;
                m.StrokeThickness = 0.5;
            
            
            
                Canvas.SetLeft(m, 0);
                Canvas.SetTop(m, y);
                _mainCanvas.Children.Add(m);
            }
            
            Canvas.SetLeft(l, x);
            Canvas.SetTop(l, 0);
            _mainCanvas.Children.Add(l);
        }
    }
    
    
}