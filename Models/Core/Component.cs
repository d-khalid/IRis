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
    
    // Most components won't use all 3
    public int InputLineCount { get; protected set; } = 0;
    public int SelectionLineCount { get; protected set; } = 0;
    public int OutputLineCount { get; protected set; } = 0;
    
    // Used by Latches/Flip-Flops
    public Dictionary<string, LogicState> StoredStates { get; set; } = new();
    
    
    

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
    public Component(double width , double height)
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
        return null!;
    }

    // Override for wires
    public virtual bool HitTest(Point point)
    {   
        point = RotateTransform.Value.Transform(point);
        
        return new Rect(0,0,Width,Height).Contains(point);
    }

    public virtual void AddTerminalPoints(bool notMode = false)
    {
        
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
    
    // A method for making components by type
    public static Component Create(string componentType, Simulation simulation, int numInputs=2)
    {
        switch (componentType)
        {
            case "AND":
                return new AndGate(numInputs);
            case "OR":
                return new OrGate(numInputs);
            case "NOT":
                return new NotGate();
            case "NAND":
                return new NandGate(numInputs);
            case "NOR":
                return new NorGate(numInputs);
            case "XOR":
                return new XorGate(numInputs);
            case "XNOR":
                return new XnorGate(numInputs);
            case "MUX":
                return new Multiplexer(selectionLineCount: numInputs);
            case "DEMUX":
                return new Demultiplexer(selectionLineCount: numInputs);
            case "ENCODER":
                return new Encoder(selectionLineCount: numInputs);
            case "DECODER":
                return new Decoder(selectionLineCount: numInputs);
            case "SRL":
                return new SRLatch();
            case "DL":
                return new DLatch();
            case "JKL":
                return new JKLatch();
            case "TL":
                return new TLatch();

            case "PROBE":
                return new LogicProbe();
            case "TOGGLE":
                return new LogicToggle();
            case "CUSTOM":
                return new CustomComponent(simulation.CustomComponent.Name, simulation.CustomComponent.InputCount,
                    simulation.CustomComponent.OutputCount, simulation.CustomComponent.Formulas);
            case "WIRE":
                return new Wire();
            
            default:
                return null!; // TODO: DANGEROUS, THIS IS A FUCKING NULLPO WAITING TO HAPPEN
        }
    }

    
    
  
}
