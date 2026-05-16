using Avalonia;
using System;
using Newtonsoft.Json;
using IRis.ViewModels.Circuit;
using System.Collections.ObjectModel;
using IRis.ViewModels.Circuit.CircuitObjects;


namespace IRis.Services;


public static class UtilityService {
    public static Point SnapPointToGrid(Point pt)
    {
        double gridSpacing = 10.0;
        double snapX = Math.Round(pt.X / gridSpacing) * gridSpacing;
        double snapY = Math.Round(pt.Y / gridSpacing) * gridSpacing;
        return new Point(snapX, snapY);
    }


    public static T Clone<T>(T source)
    {
        JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        return JsonConvert.DeserializeObject<T>(
            JsonConvert.SerializeObject(source, settings), settings
        )!;
    }


    public static Point GetMinPointFromCollection(ObservableCollection<CircuitObjectViewModel> collection)
    {
        double minX = double.MaxValue;
        double minY = double.MaxValue;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                if (c.X < minX) minX = c.X;
                if (c.Y < minY) minY = c.Y;
            }
        }

        return new Point(minX, minY);
    }


    // public static void AddOrSymbolToFigure(PathFigure figure, BoxSize size)
    // {
    //     if (figure.Segments == null)
    //         throw new Exception("Cannot draw OR symbol: figure.Segments is null.");


    //     // left curve (top-left to bottom-left)
    //     figure.Segments.Add(new ArcSegment {
    //         Point = new Point(0, size.Height),
    //         Size = new Size(size.Width / 6, size.Height / 2),
    //         SweepDirection = SweepDirection.Clockwise,
    //         IsLargeArc = false
    //     });

    //     // bottom-right curve (bottom-left to center-right)
    //     figure.Segments.Add(new ArcSegment {
    //         Point = new Point(size.Width, size.Height * 0.5),
    //         Size = new Size(size.Width , size.Height / 2),
    //         SweepDirection = SweepDirection.CounterClockwise,
    //         IsLargeArc = false
    //     });

    //     // top-right curve (center-right to top-left)
    //     figure.Segments.Add(new ArcSegment {
    //         Point = new Point(0, 0),
    //         Size = new Size(size.Width , size.Height / 2),
    //         SweepDirection = SweepDirection.CounterClockwise,
    //         IsLargeArc = false
    //     });
    // }


    // public static void AddAndSymbolToFigure(PathFigure figure, BoxSize size)
    // {
    //     if (figure.Segments == null)
    //         throw new Exception("Cannot draw And symbol: figure.Segments is null.");


    //     // left line (top-left to bottom-left)
    //     figure.Segments.Add(new LineSegment { 
    //         Point = new Point(0, size.Height) 
    //     });

    //     // bottom line (bottom-left to bottom-right)
    //     figure.Segments.Add(new LineSegment { 
    //         Point = new Point(size.Width - Constants.AndArcDepth, size.Height) 
    //     });

    //     // right arc (bottom-right to top-right)
    //     figure.Segments.Add(new ArcSegment
    //     {
    //         Point = new Point(size.Width - Constants.AndArcDepth, 0),
    //         Size = new Size(Constants.AndArcDepth, size.Height / 2),
    //         SweepDirection = SweepDirection.CounterClockwise,
    //         IsLargeArc = false
    //     });

    //     // top line (top-right to top-left)
    //     figure.Segments.Add(new LineSegment { 
    //         Point = new Point(0, 0)
    //     });
    // }


    // public static void AddNotSymbolToFigure(PathFigure figure, BoxSize size)
    // {
    //     if (figure.Segments == null)
    //         throw new Exception("Cannot draw NOT symbol: figure.Segments is null.");


    //     // left line (top-left to bottom-left)
    //     figure.Segments.Add(new LineSegment { 
    //         Point = new Point(0, size.Height) 
    //     });

    //     // lower diagonal (bottom-left to center-right)
    //     figure.Segments.Add(new LineSegment { 
    //         Point = new Point(size.Width, size.Height / 2) 
    //     });

    //     // upper diagonal (center-right to top-left)
    //     figure.Segments.Add(new LineSegment { 
    //         Point = new Point(0, 0) 
    //     });
    // }


    // public static void AddXorCurveToFigure(PathFigure figure, BoxSize size)
    // {
    //     if (figure.Segments == null)
    //         throw new Exception("Cannot draw XOR curve: figure.Segments is null.");


    //     // XOR extra curve
    //     figure.Segments.Add(new ArcSegment {
    //         Point = new Point(-Constants.XorArcDistance, size.Height*0.98),
    //         Size = new Size(size.Width / 6, size.Height / 2),
    //         SweepDirection = SweepDirection.Clockwise,
    //         IsLargeArc = false
    //     });
    // }


    // public static void AddNotBubbleToDrawing(DrawingContext ctx, BoxSize size)
    // {
    //     ctx.DrawEllipse(
    //         Constants.NotBubbleBrush, 
    //         Constants.GatePen,
    //         new Point(size.Width + Constants.NotBubbleRadius, size.Height / 2),
    //         Constants.NotBubbleRadius,
    //         Constants.NotBubbleRadius
    //     );
    // }


    // public static void AddBigTextToDrawing(DrawingContext ctx, Point position, string text)
    // {
    //     FormattedText formattedText = new(
    //         textToFormat: text,
    //         culture: CultureInfo.CurrentCulture,
    //         flowDirection: FlowDirection.LeftToRight,
    //         typeface: Constants.DrawingBigTextTypeFace,
    //         emSize: Constants.DrawingBigTextSize,
    //         foreground: Constants.LogicProbeBrush
    //     );

    //     ctx.DrawText(formattedText, position);
    // }


    // public static void DrawLineOnCanvas(Canvas canvas, Point p1, Point p2) 
    // {
    //     canvas.Children.Insert(
    //         index: 0,
    //         item: new Line
    //         {
    //             StartPoint = p1,
    //             EndPoint = p2,
    //             Stroke = Constants.CanvasGridBrush,
    //             StrokeThickness = Constants.CanvasGridThickness
    //         }
    //     );
    // }
}

