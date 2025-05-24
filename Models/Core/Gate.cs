using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using IRis.Models.Components;

namespace IRis.Models.Core;


// Contains the position and truth value of a connection point
public readonly struct Terminal
{
    public Point Position { get; }
    
    // Value is nullable to account for dont cares
    public bool? Value { get; }

    public Terminal(Point position, bool? value)
    {
        Position = position;
        Value = value;
    }
}

// Has some gate specific things
public abstract class Gate : Component
{
    protected int NumInputs;

    protected Terminal[] Inputs;
    protected Terminal Output;

    // Uses default values if none are given
    public Gate(int numInputs, double width = ComponentDefaults.DefaultWidth,
        double height = ComponentDefaults.DefaultHeight, bool notMode = false) 
        : base(width, height)
    {
        NumInputs = numInputs;
        Inputs = new Terminal[NumInputs];
        
        AddTerminalPoints(notMode);

        IsHitTestVisible = true;
        
        
    }
    
    // Implement ICloneable for copies
    public override object Clone()
    {
        // Create new instance based on concrete type
        Gate clone = this switch
        {
            AndGate _ => new AndGate(this.NumInputs),
            OrGate _ => new OrGate(this.NumInputs),
            NotGate _ => new NotGate(),  // Special constructor
            NandGate _ => new NandGate(this.NumInputs),
            NorGate _ => new NorGate(this.NumInputs),
            XorGate _ => new XorGate(this.NumInputs),
            XnorGate _ => new XnorGate(this.NumInputs),
            _ => throw new NotSupportedException($"Unsupported gate type: {this.GetType().Name}")
        };

        // Copy all base properties
        clone.Width = this.Width;
        clone.Height = this.Height;
        clone.Rotation = this.Rotation;
        clone.IsSelected = this.IsSelected;

        // Copy terminal values (positions are set in constructor)
        for (int i = 0; i < this.NumInputs; i++)
        {
            clone.Inputs[i] = new Terminal(
                clone.Inputs[i].Position,  // Use new position
                this.Inputs[i].Value       // Copy original value
            );
        }
        clone.Output = new Terminal(
            clone.Output.Position,
            this.Output.Value
        );

        // Reset visual state
        clone.VisualChildren.Clear();
        clone.InvalidateVisual();

        return clone;
    }

    // Draws an translucent box around the gate
    public override void DrawSelection(DrawingContext ctx)
    {
        double expandX = ComponentDefaults.TerminalWireLength + ComponentDefaults.TerminalRadius ;
        double expandY = ComponentDefaults.TerminalRadius ;
        // Subtle fill
        ctx.DrawRectangle(
            ComponentDefaults.SelectionBrush, 
            ComponentDefaults.SelectionPen, 
            new Rect(
                -expandX,
               -expandY,
                Bounds.Width + 2 * expandX ,
                Bounds.Height + 2 * expandY)
            );
    }

    // The points are added differently for NOT-variants (NAND, NOT, NOR, XNOR)
    // notMode: Extra wire length to account for the bubble
    protected void AddTerminalPoints(bool notMode = false)
    {
        double spacing = Height / (NumInputs + 1);
        for (int i = 0; i < NumInputs; i++)
        {
            Inputs[i] = new Terminal(new Point(-ComponentDefaults.TerminalWireLength, spacing * (i + 1)), null);
            
        }
        //Output
        double outputX = Width + ComponentDefaults.TerminalWireLength;
        if(notMode) outputX += ComponentDefaults.BubbleRadius * 2;
        
        Output = new Terminal(new Point(outputX, Height / 2), null);
    }

    // For drawing the terminals
    // notMode: a bubble drawn with the output terminal
    protected void DrawTerminals(DrawingContext ctx)
    {
       

        // Input lines extend into the gate and covered up by the fill color
        for (int i = 0; i < NumInputs; i++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Inputs[i].Position, new Point(Width / ComponentDefaults.OrArcFactor, Inputs[i].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush , null, 
                Inputs[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

        }
        // For Output
        // SUSPEND: I STOPPED HERE
     
        ctx.DrawLine(ComponentDefaults.WirePen, Output.Position,
            new Point(Output.Position.X - ComponentDefaults.TerminalWireLength, Output.Position.Y));
        ctx.DrawEllipse(ComponentDefaults.TerminalBrush , null, 
            Output.Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
    }
    
    // Methods for drawing the body of AND and OR
    // xorMode: Adds an extra arc
    protected void DrawOr(DrawingContext ctx, bool xorMode = false)
    {
        // 1. Create the OR gate using 3 arcs
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        // Arc 1: Top curve (right to left)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(0, Height),
            Size = new Size(Width / ComponentDefaults.OrArcFactor, Height /2),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc = false
        });

        // Arc 2: Bottom right curve (left to right)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(Width, Height * 0.5),
            Size = new Size(Width , Height /2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });

        // Arc 3: Top right curve (right to left)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(0, 0),
            Size = new Size(Width , Height /2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });

        gatePath.Figures.Add(figure);
        
        // 2. Draw the complete gate
        ctx.DrawGeometry(ComponentDefaults.GateFillBrush, ComponentDefaults.GatePen, gatePath);  
        
        // Draw the XOR arc, if in xorMode
        if (xorMode)
        {
            var xorArc = new PathGeometry();
            var arcFigure = new PathFigure()
            {
                StartPoint = new Point(-ComponentDefaults.TerminalWireLength / ComponentDefaults.XorArcDistFactor, Height * 0.02),
                IsClosed = false
                
            };

            arcFigure.Segments.Add(new ArcSegment
            {
                Point = new Point(-ComponentDefaults.TerminalWireLength / ComponentDefaults.XorArcDistFactor, Height * 0.98),
                Size = new Size(Width / ComponentDefaults.OrArcFactor, Height /2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = false
            });
            
            xorArc.Figures.Add(arcFigure);
            
            ctx.DrawGeometry(null, ComponentDefaults.GatePen, xorArc);
        }
    }

    protected void DrawAnd(DrawingContext ctx)
    {
        // 1. Create the AND gate shape as a single PathGeometry
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        double arcLen = Width / 3;

        // Left vertical line
        figure.Segments.Add(new LineSegment { Point = new Point(0, Height) });

        // Bottom horizontal line (left to right)
        figure.Segments.Add(new LineSegment { Point = new Point(Width - arcLen, Height) });

        // Right semicircular arc (bottom to top)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(Width - arcLen, 0),
            Size = new Size(arcLen, Height / 2),
            SweepDirection = SweepDirection.CounterClockwise,

            IsLargeArc = false
        });

        // Top horizontal line (right to left)
        figure.Segments.Add(new LineSegment { Point = new Point(0, 0) });

        gatePath.Figures.Add(figure);
        
        // 2. Draw the complete gate
        ctx.DrawGeometry(ComponentDefaults.GateFillBrush, ComponentDefaults.GatePen, gatePath);

    }

}
