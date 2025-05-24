using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using IRis.Models.Core;


namespace IRis.Models.Components;

public class LogicToggle : Component
{
    private Terminal _output;
    
    public LogicToggle(double width = ComponentDefaults.DefaultWidth, double height = ComponentDefaults.DefaultHeight)
        : base(width, height)
    {
        Width = width * 2 / 3;
        Height = height * 2 / 3;
        
        // Left-oriented
        _output = new Terminal(new Point(-ComponentDefaults.TerminalWireLength, Height/2), true);
        
    }
    
    public override object Clone()
    {
        var clone = (LogicToggle)base.Clone();
        clone._output = new Terminal(this._output.Position, this._output.Value);
        return clone;
    }

    public override void Draw(DrawingContext ctx)
    {
        IImmutableSolidColorBrush fill = _output.Value switch
        {
            true => ComponentDefaults.TrueBrush,
            false => ComponentDefaults.FalseBrush,
            null => ComponentDefaults.DontCareBrush
        };
        

        ctx.DrawRectangle(
            fill, 
            ComponentDefaults.GatePen,
            new Rect(0,0,Width,Height)
            );
        
        ctx.DrawLine(ComponentDefaults.WirePen, _output.Position, new Point(0, _output.Position.Y));
        ctx.DrawEllipse(ComponentDefaults.TerminalBrush , null, 
            _output.Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
        
        // Draw the text label
        var text = new FormattedText(
            _output.Value switch
            {
                true => "1",
                false => "0",
                null => "X"
            },
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