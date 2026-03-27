using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System;
using System.Globalization;


namespace IRis.Models.Components;

public class LogicToggle : CircuitComponent, IOutputProvider
{
    public LogicState Value
    {
        get => StoredStates["Value"];
        set
        {
            StoredStates["Value"] = value;

            // Redraw
            InvalidateVisual();

            // Propagate it into the wire if we have any
            if (Terminals![0].Wire != null)
            {
                Terminals[0].Wire!.Value = value;
            }
        }
    }

    public LogicToggle(double width = ComponentDefaults.DefaultWidth, double height = ComponentDefaults.DefaultHeight)
        : base(width, height)
    {
        Width = width * 1 / 2;
        Height = height * 1 / 2;

        Terminals = new Terminal[1];

        // Snap to nearest grid line
        static double Snap(double val) => Math.Round(val / 10.0) * 10;
        double x = Snap(Width + ComponentDefaults.TerminalWireLength);
        double y = Snap(Height / 2);
        // Left-oriented
        Terminals[0] = new Terminal(new Point(x, y), null!);

        // Create a dictionary entry for its value
        StoredStates["Value"] = LogicState.Low;

        // Register an event handler for DoubleClicks
        DoubleTapped += (_, _) =>
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




    public override object Clone()
    {
        LogicToggle clone = new LogicToggle();

        // Copy all base properties
        clone.Width = Width;
        clone.Height = Height;
        clone.Rotation = Rotation;
        clone.IsSelected = IsSelected;

        // Component-specific things
        if (clone.Terminals is not null && Terminals is not null)
        {
            clone.Terminals[0] = new Terminal(clone.Terminals[0].Position, Terminals[0].Wire!);
        }
        clone.Value = Value;

        // Reset visual state
        clone.VisualChildren.Clear();
        clone.InvalidateVisual();

        return clone;
    }

    public void ComputeOutput()
    {
        // Propagate the toggle value to ALL connected wires
        foreach (var wire in Terminals![0].Wires)
        {
            wire.Value = Value;
        }
    }

    public override void Draw(DrawingContext ctx)
    {
        var fill = Value switch
        {
            LogicState.High => ComponentDefaults.TrueBrush,
            LogicState.Low => ComponentDefaults.FalseBrush,
            _ => ComponentDefaults.DontCareBrush
        };

        var content = Value switch
        {
            LogicState.High => "1",
            LogicState.Low => "0",
            _ => "X"
        };


        ctx.DrawRectangle(
            fill,
            ComponentDefaults.GatePen,
            new Rect(0, 0, Width, Height)
        );

        ctx.DrawLine(ComponentDefaults.WirePen, Terminals![0].Position, new Point(Width, Terminals[0].Position.Y));
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