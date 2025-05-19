using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;

namespace IRis.Models;

public abstract class Component : Control, ICustomHitTest
{
    
    private bool _isSelected = false;
    private double _rotation = 0;

    protected RotateTransform RotateTransform;

    public double Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            RotateTransform = new RotateTransform(value, Width/2, Height/2);
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            InvalidateVisual(); 
        }
    }
    public Component( double width , double height)
    {
        Width = width;
        Height = height;
        
        RotateTransform = new RotateTransform(_rotation, Width, Height);
        
        //Rotation = 100;
        
        // Register OnPointerPressed()
        this.PointerPressed += OnPointerPressed;
        
    }
    
    // Override for wires
    public virtual bool HitTest(Point point)
    {   
        point = RotateTransform.Value.Transform(point);
        
        return new Rect(0,0,Width,Height).Contains(point);
    }

    public virtual void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        // {
        //     IsSelected = !IsSelected;
        //     e.Handled = false;
        // }
    }


    public override void Render(DrawingContext context)
    {
        // Applies rotation to the drawing
        using (context.PushTransform(RotateTransform.Value))
        {
            // Regular Drawing
            Draw(context);

            // 1. Draw hit-testable area (equivalent to Fill)
            context.DrawRectangle(
                Brushes.Transparent, // Invisible but clickable
                null,
                new Rect(0, 0, Width, Height));

            // TESTING
            if (IsSelected)
            {
                // context.DrawRectangle(ComponentDefaults.SelectionBrush, null, 
                //     new Rect(0,0,Width,Height)
                //     );
                DrawSelection(context);

            }
            base.Render(context);
        }
    }
    // Can be overriden for custom implementations
    public virtual void Draw(DrawingContext ctx)
    {
      
    }

    public virtual void DrawSelection(DrawingContext ctx)
    {
        
    }
    
    
  
}
