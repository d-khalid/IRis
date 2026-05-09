// default libs
using Avalonia;
using Avalonia.Media;
using System;


namespace IRis.Models.Core;


public static class Utils {
    public static Point SnapPointToGrid(Point pt)
    {
        double snapX = Math.Round(pt.X / Constants.GridSpacing) * Constants.GridSpacing;
        double snapY = Math.Round(pt.Y / Constants.GridSpacing) * Constants.GridSpacing;
        return new Point(snapX, snapY);
    }


    public static void AddOrSymbolToFigure(PathFigure figure, BoxSize size)
    {
        if (figure.Segments == null)
            throw new Exception("Cannot draw OR symbol: figure.Segments is null.");


        // left curve (top-left to bottom-left)
        figure.Segments.Add(new ArcSegment {
            Point = new Point(0, size.Height),
            Size = new Size(size.Width / 6, size.Height / 2),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc = false
        });

        // bottom-right curve (bottom-left to center-right)
        figure.Segments.Add(new ArcSegment {
            Point = new Point(size.Width, size.Height * 0.5),
            Size = new Size(size.Width , size.Height / 2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });

        // top-right curve (center-right to top-left)
        figure.Segments.Add(new ArcSegment {
            Point = new Point(0, 0),
            Size = new Size(size.Width , size.Height / 2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });
    }


    public static void AddAndSymbolToFigure(PathFigure figure, BoxSize size)
    {
        if (figure.Segments == null)
            throw new Exception("Cannot draw And symbol: figure.Segments is null.");


        // left line (top-left to bottom-left)
        figure.Segments.Add(new LineSegment { 
            Point = new Point(0, size.Height) 
        });

        // bottom line (bottom-left to bottom-right)
        figure.Segments.Add(new LineSegment { 
            Point = new Point(size.Width - Constants.AndArcDepth, size.Height) 
        });

        // right arc (bottom-right to top-right)
        figure.Segments.Add(new ArcSegment
        {
            Point = new Point(size.Width - Constants.AndArcDepth, 0),
            Size = new Size(Constants.AndArcDepth, size.Height / 2),
            SweepDirection = SweepDirection.CounterClockwise,
            IsLargeArc = false
        });

        // top line (top-right to top-left)
        figure.Segments.Add(new LineSegment { 
            Point = new Point(0, 0)
        });
    }


    public static void AddXorCurveToFigure(PathFigure figure, BoxSize size)
    {
        if (figure.Segments == null)
            throw new Exception("Cannot draw XOR curve: figure.Segments is null.");

        // XOR extra curve
        figure.Segments.Add(new ArcSegment {
            Point = new Point(25 / 3, size.Height * 0.98),
            Size = new Size(size.Width / 6, size.Height / 2),
            SweepDirection = SweepDirection.Clockwise,
            IsLargeArc = false
        });
    }


    public static void AddNotBubbleToDrawing(DrawingContext ctx, BoxSize size)
    {
        ctx.DrawEllipse(
            Constants.NotBubbleBrush, 
            Constants.GatePen,
            new Point(size.Width + Constants.NotBubbleRadius, size.Height / 2),
            Constants.NotBubbleRadius,
            Constants.NotBubbleRadius
        );
    }
}

