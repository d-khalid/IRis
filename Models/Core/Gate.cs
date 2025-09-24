using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using IRis.Models.Components;
using Avalonia.Threading;
using System.Linq;

namespace IRis.Models.Core;


// Contains the position and truth value of a connection point
public class Terminal
{
    public Point Position { get; }
    
    // Change from single Wire to List of Wires
    public List<Wire> Wires { get; set; } = new List<Wire>();
    
    // Keep this for backward compatibility if needed
    public Wire? Wire 
    { 
        get => Wires.FirstOrDefault(); 
        set 
        { 
            if (value != null && !Wires.Contains(value))
            {
                Wires.Add(value);
            }
        } 
    }

    public Terminal(Point position)
    {
        Position = position;
    }
    
    public Terminal(Point position, Wire wire) : this(position)
    {
        if (wire != null)
            Wires.Add(wire);
    }
    
    // Add a wire to this terminal
    public void AddWire(Wire wire)
    {
        if (wire != null && !Wires.Contains(wire))
        {
            Wires.Add(wire);
        }
    }
    
    // Remove a wire from this terminal
    public void RemoveWire(Wire wire)
    {
        Wires.Remove(wire);
    }
    
    
}

// Has some gate specific things
public abstract class Gate : Component, IOutputProvider
{
    

    // Uses default values if none are given
    public Gate(int numInputs, double width = ComponentDefaults.DefaultWidth,
        double height = ComponentDefaults.DefaultHeight, bool notMode = false)
        : base(width, height)
    {
        InputLineCount = numInputs;
        Terminals = new Terminal[InputLineCount + 1];

        AddTerminalPoints(notMode);

        IsHitTestVisible = true;

        // Dispatcher timer, calls CheckIfInputsChanged()
        // _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };   // Adjust to reduce CPU load
        // _updateTimer.Tick += (s, e) => CheckIfInputsChanged();
        // _updateTimer.Start();
    }

    public abstract void ComputeOutput();
    
    
    // Implement ICloneable for copies
    public override object Clone()
    {
        // Create new instance based on concrete type
        Gate clone = this switch
        {
            AndGate _ => new AndGate(this.InputLineCount),
            OrGate _ => new OrGate(this.InputLineCount),
            NotGate _ => new NotGate(),  // Special constructor
            NandGate _ => new NandGate(this.InputLineCount),
            NorGate _ => new NorGate(this.InputLineCount),
            XorGate _ => new XorGate(this.InputLineCount),
            XnorGate _ => new XnorGate(this.InputLineCount),
            _ => throw new NotSupportedException($"Unsupported gate type: {this.GetType().Name}")
        };

        // Copy all base properties
        clone.Width = this.Width;
        clone.Height = this.Height;
        clone.Rotation = this.Rotation;
        clone.IsSelected = this.IsSelected;

        // Copy terminal values (positions are set in constructor)
        for (int i = 0; i < this.InputLineCount; i++)
        {
            clone.Terminals![i] = new Terminal(
                clone.Terminals[i].Position,  // Use new position
                this.Terminals![i].Wire!      // Copy original value
            );
        }
        clone.Terminals![^1] = new Terminal(
            clone.Terminals[^1].Position,
            this.Terminals![^1].Wire!
        );

        // Reset visual state
        clone.VisualChildren.Clear();
        clone.InvalidateVisual();

        return clone;
    }

    // Draws a translucent box around the gate
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
    public override void AddTerminalPoints(bool notMode = false)
    {
        double spacing = Height / (InputLineCount + 1);

        // Helper to snap to nearest multiple of 10
        Point SnapToGrid(Point pt)
        {
            double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            return new Point(snapX, snapY);
        }

        // Input terminals
        for (int i = 0; i < InputLineCount; i++)
        {
            var pos = new Point(-ComponentDefaults.TerminalWireLength, spacing * (i + 1));
            Terminals![i] = new Terminal(SnapToGrid(pos), null!);
        }

        // Output terminal
        double outputX = Width + ComponentDefaults.TerminalWireLength -5;   // WARNING: this is a Patch
        if (notMode) outputX += ComponentDefaults.BubbleRadius * 2;

        var outputPos = SnapToGrid(new Point(outputX, Height / 2));
        Terminals![^1] = new Terminal(SnapToGrid(outputPos), null!);
    }


    // For drawing the terminals
    // notMode: a bubble drawn with the output terminal
    protected void DrawTerminals(DrawingContext ctx)
    {
       

        // Input lines extend into the gate and covered up by the fill color
        for (int i = 0; i < InputLineCount; i++)
        {
            if (Terminals![i] == null) continue;
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals[i].Position, new Point(Width / ComponentDefaults.OrArcFactor, Terminals[i].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush , null, 
                Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

        }
        // For Terminals[^1]
        // SUSPEND: I STOPPED HERE
     
        ctx.DrawLine(ComponentDefaults.WirePen, Terminals![^1].Position,
            new Point(Terminals[^1].Position.X - ComponentDefaults.TerminalWireLength, Terminals[^1].Position.Y));
        ctx.DrawEllipse(ComponentDefaults.TerminalBrush , null, 
            Terminals[^1].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
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
        figure.Segments!.Add(new ArcSegment
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

        gatePath.Figures!.Add(figure);
        
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

            arcFigure.Segments!.Add(new ArcSegment
            {
                Point = new Point(-ComponentDefaults.TerminalWireLength / ComponentDefaults.XorArcDistFactor, Height * 0.98),
                Size = new Size(Width / ComponentDefaults.OrArcFactor, Height /2),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = false
            });
            
            xorArc.Figures!.Add(arcFigure);
            
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
        figure.Segments!.Add(new LineSegment { Point = new Point(0, Height) });

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

        gatePath.Figures!.Add(figure);
        
        // 2. Draw the complete gate
        ctx.DrawGeometry(ComponentDefaults.GateFillBrush, ComponentDefaults.GatePen, gatePath);

    }

}
