using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;

namespace IRis.Models;

public class LogicProbe : Component
{
    private Terminal _input;
    
    public LogicProbe(double width = ComponentDefaults.DefaultWidth, double height = ComponentDefaults.DefaultHeight)
        : base(width, height)
    {
        Width = width * 2 / 3;
        Height = height * 2 / 3;
        
        // Left-oriented
        _input = new Terminal(new Point(-ComponentDefaults.TerminalWireLength, Height/2), false);
        
    }
    

    public override void Draw(DrawingContext ctx)
    {
        IImmutableSolidColorBrush fill = _input.Value switch
        {
            true => ComponentDefaults.TrueBrush,
            false => ComponentDefaults.FalseBrush,
            null => ComponentDefaults.DontCareBrush
        };
        

        ctx.DrawEllipse(
            fill, 
            ComponentDefaults.GatePen,
            new Point(Width/2, Height/2),
            Width/2, Height/2
            );
        
        ctx.DrawLine(ComponentDefaults.WirePen, _input.Position, new Point(0, _input.Position.Y));
        ctx.DrawEllipse(ComponentDefaults.TerminalBrush , null, 
            _input.Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
        
        // Draw the text label
        var text = new FormattedText(
            _input.Value switch
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