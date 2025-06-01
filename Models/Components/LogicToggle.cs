using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using IRis.Models.Core;


namespace IRis.Models.Components;

public class LogicToggle : Component, IOutputProvider
{
    private LogicState? _value;

    public LogicState? Value
    {
        get => _value;
        set
        {
            _value = value;

            // Redraw
            InvalidateVisual();

            // Propagate it into the wire if we have any
            if (Terminals[0].Wire != null)
            {
                Terminals[0].Wire.Value = value;
            }
        }
    }

    public LogicToggle(double width = ComponentDefaults.DefaultWidth, double height = ComponentDefaults.DefaultHeight)
        : base(width, height)
    {
        Width = width * 2 / 3;
        Height = height * 2 / 3;

        Terminals = new Terminal[1];
        // Left-oriented
        Terminals[0] = new Terminal(new Point(Width + ComponentDefaults.TerminalWireLength, Height / 2), null);

        Value = LogicState.Low;
        
        // Register an event handler for DoubleClicks
        this.DoubleTapped += (s, e) =>
        {
            Toggle();
        };
    }

    private void Toggle()
    {
        switch (Value)
        {
            case LogicState.Low:
                Value = LogicState.High;
                break;
            case LogicState.High:
                Value = LogicState.Low;
                break;
        }
    }


    protected override List<PropertyDto> GetSerializableProperties()
    {
        return new List<PropertyDto>
        {
            new() { Name = "Width", Value = Width.ToString() },
            new() { Name = "Height", Value = Height.ToString() },
            new() { Name = "Rotation", Value = Rotation.ToString() },
            new() { Name = "Value", Value = this.Value.ToString() }
            
            // Add other serializable properties in subclasses
        };
    }

    public override object Clone()
    {
        LogicToggle clone = new LogicToggle();
        
        // Copy all base properties
        clone.Width = this.Width;
        clone.Height = this.Height;
        clone.Rotation = this.Rotation;
        clone.IsSelected = this.IsSelected;
        
        // Component-specific things
        clone.Terminals[0] = new Terminal(clone.Terminals[0].Position, this.Terminals[0].Wire);
        clone.Value = this.Value;
        
        // Reset visual state
        clone.VisualChildren.Clear();
        clone.InvalidateVisual();

        return clone;
    }

    public void ComputeOutput()
    {
        // If there is a wire, propagate the value to it
        if (Terminals[0].Wire != null)
        {
            Terminals[0].Wire.Value = this.Value;
        }
    }

    public override void Draw(DrawingContext ctx)
    {
        IImmutableSolidColorBrush fill = ComponentDefaults.DontCareBrush;
        string content = "X";

        fill = Value switch
        {
            LogicState.High => ComponentDefaults.TrueBrush,
            LogicState.Low => ComponentDefaults.FalseBrush,
            LogicState.DontCare => ComponentDefaults.DontCareBrush
        };

        content = Value switch
        {
            LogicState.High => "1",
            LogicState.Low => "0",
            LogicState.DontCare => "X",
        };


        ctx.DrawRectangle(
            fill,
            ComponentDefaults.GatePen,
            new Rect(0, 0, Width, Height)
        );

        ctx.DrawLine(ComponentDefaults.WirePen, Terminals[0].Position, new Point(Width, Terminals[0].Position.Y));
        ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
            Terminals[0].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

        // Draw the text label
        var text = new FormattedText(
            content,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(fontFamily: "Arial", weight: FontWeight.Bold),
            24,
            Brushes.White
        );


        // Center the text in the ellipse
        ctx.DrawText(
            text,
            new Point(
                Width / 2 - text.Width / 2,
                Height / 2 - text.Height / 2
            )
        );
    }

    public override void DrawSelection(DrawingContext ctx)
    {
        ctx.DrawRectangle(
            ComponentDefaults.SelectionBrush,
            ComponentDefaults.SelectionPen,
            new Rect(-10, -10, Width + 20, Height + 20)
        );
    }
}