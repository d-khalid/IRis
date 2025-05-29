using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using IRis.Models.Core;


namespace IRis.Models.Components;

public class LogicProbe : Component
{
   

    public LogicProbe(double width = ComponentDefaults.DefaultWidth, double height = ComponentDefaults.DefaultHeight)
        : base(width, height)
    {
        Width = width * 2 / 3;
        Height = height * 2 / 3;
        
        Terminals = new Terminal[1];
        
        // Left-oriented
        Terminals[0] = new Terminal(new Point(-ComponentDefaults.TerminalWireLength, Height/2), null);
        
    }
    
    public override object Clone()
    {
        var clone = (LogicProbe)base.Clone();
        clone.Terminals = new Terminal[1];
        clone.Terminals[0] = new Terminal(this.Terminals[0].Position, this.Terminals[0].Wire);
        return clone;
    }
    

    public override void Draw(DrawingContext ctx)
    {
        IImmutableSolidColorBrush fill;
        string content;
        if (Terminals[0].Wire != null)
        {
            fill = Terminals[0].Wire.Value switch
            {
               LogicState.High => ComponentDefaults.TrueBrush,
                LogicState.Low => ComponentDefaults.FalseBrush,
                LogicState.DontCare => ComponentDefaults.DontCareBrush,
               null => ComponentDefaults.DontCareBrush,
            };

            content = Terminals[0].Wire.Value switch
            {
                LogicState.High => "1",
                LogicState.Low => "0",
                LogicState.DontCare => "X",
                null => "X!"
            };

        }
        else
        {
            content = "X!";
            fill = ComponentDefaults.DontCareBrush;
        }




        ctx.DrawEllipse(
            fill, 
            ComponentDefaults.GatePen,
            new Point(Width/2, Height/2),
            Width/2, Height/2
            );
        
        ctx.DrawLine(ComponentDefaults.WirePen, Terminals[0].Position, new Point(0, Terminals[0].Position.Y));
        ctx.DrawEllipse(ComponentDefaults.TerminalBrush , null, 
            Terminals[0].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
        
        // Draw the text label
        var text = new FormattedText(
            content,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(fontFamily:"Arial", weight:FontWeight.Bold), 
            24, 
            Brushes.White 
        );
        
        
    
        // Center the text in the ellipse
        ctx.DrawText(
            text,
            new Point(
                Width/2 - text.Width/2,
                Height/2 - text.Height/2
            )
        );
    }

    public override void DrawSelection(DrawingContext ctx)
    {
        ctx.DrawRectangle(
            ComponentDefaults.SelectionBrush, 
            ComponentDefaults.SelectionPen, 
            new Rect(-10,-10, Width + 20, Height + 20)
        );
    }
}