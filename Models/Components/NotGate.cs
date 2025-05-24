using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;

namespace  IRis.Models.Components;


public class NotGate : Gate
{
    public NotGate() : base(1, ComponentDefaults.DefaultWidth * 0.75, notMode:true)
    {
        
    }

    public override void Draw(DrawingContext ctx)
    {
       
        // 1. Create the triangular NOT gate shape
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        // Left vertical line down
        figure.Segments.Add(new LineSegment { Point = new Point(0, Height) });

        // Diagonal to tip (right-middle)
        figure.Segments.Add(new LineSegment { Point = new Point(Width , Height / 2) });

        // Diagonal back to start
        figure.Segments.Add(new LineSegment { Point = new Point(0, 0) });

        gatePath.Figures.Add(figure);
      
        // 4. Draw terminals (lines + circles)
        DrawTerminals(ctx);
        
        // 2. Draw the triangle
        ctx.DrawGeometry(ComponentDefaults.GateFillBrush, ComponentDefaults.GatePen, gatePath);
        
        // 3. Draw the bubble at the end
        ctx.DrawEllipse(
            Brushes.White, // Fill (none)
            ComponentDefaults.GatePen, // Use same pen as gate
            new Point(Width + ComponentDefaults.BubbleRadius, Height / 2),
            ComponentDefaults.BubbleRadius,
            ComponentDefaults.BubbleRadius);
        
        base.Draw(ctx);

    }
}