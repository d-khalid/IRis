using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;
using IRis.Models.Components;

namespace IRis.Models.Core;

public abstract class Component : Control, ICustomHitTest
{
    
    private bool _isSelected = false;
    private double _rotation = 0;

    protected RotateTransform RotateTransform;
    
    // Last one is output
    public Terminal[]? Terminals = null;

    public double Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            RotateTransform = new RotateTransform(value, Width/2, Height/2);
            InvalidateVisual();
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
        
        
    }

    // Implement ICloneable for copies
    // Override for child classes
    public virtual object Clone()
    {
        return null;
    }

    // Override for wires
    public virtual bool HitTest(Point point)
    {   
        point = RotateTransform.Value.Transform(point);
        
        return new Rect(0,0,Width,Height).Contains(point);
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
    
    // DTO pattern implementation for serialization
    public virtual ComponentDto ToDto()
    {
        return new ComponentDto
        {
            Type = this.GetType().Name,
            X = Canvas.GetLeft(this),
            Y = Canvas.GetTop(this),
            // Terminals = this.Terminals,
            
            Properties = GetSerializableProperties()
        };
    }

    protected virtual List<PropertyDto> GetSerializableProperties()
    {
        return new List<PropertyDto>
        {
            new() { Name = "Width", Value = Width.ToString() },
            new() { Name = "Height", Value = Height.ToString() },
            new() { Name = "Rotation", Value = Rotation.ToString() }
            // Add other serializable properties in subclasses
        };
    }
    
    // A method for making components by type
    public static Component Create(string componentType)
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

    
    
  
}
