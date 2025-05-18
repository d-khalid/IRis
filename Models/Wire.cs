using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;

namespace IRis.Models;

// THIS WORKS, THEY GET DRAWN
public class Wire : Component
{
    private List<Point> points = new List<Point>();

    private Terminal source;
    
    private List<Terminal> sinks = new List<Terminal>();

    public Wire(Point start) : base(0,0)
    {
        points.Add(start);
    }

    public void AddPoint(Point point)
    {
        points.Add(point);
        // Reset the visuals
        this.InvalidateVisual();
    }

    public override void Draw(DrawingContext ctx)
    {
        
        if (points.Count < 2) return; // Need at least 2 points to draw a line

        // Create the polyline geometry
        var gatePath = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = points[0],
            IsClosed = false
        };

        for (int i = 0; i < points.Count; i++)
        {
            figure.Segments.Add(new LineSegment { Point = points[i] });

        }

        gatePath.Figures.Add(figure);

       
    
        // Draw the wire
        ctx.DrawGeometry(null, ComponentDefaults.WirePen, gatePath);

        // Draw selection highlight
        // if (IsSelected)
        // {
        //     var highlightPen = new Pen(Brushes.DodgerBlue, 3)
        //     {
        //         DashStyle = DashStyle.Dash
        //     };
        //     ctx.DrawGeometry(null, highlightPen, geometry);
        // }

        // Draw terminals if they exist
        // if (source.Position != default)
        // {
        //     DrawTerminal(ctx, source.Position, source.Value);
        // }
        //
        // foreach (var sink in sinks)
        // {
        //     DrawTerminal(ctx, sink.Position, sink.Value);
        // }
    }
}