using Avalonia;
using System;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using Avalonia.Collections;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.Models.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.Views.Main.Canvas.CircuitObjects;


namespace IRis.Services;


public static class SimulationService
{
    public static Point SnapPointToGrid(Point pt)
    {
        double gridSpacing = 10.0;
        double snapX = Math.Round(pt.X / gridSpacing) * gridSpacing;
        double snapY = Math.Round(pt.Y / gridSpacing) * gridSpacing;
        return new Point(snapX, snapY);
    }


    public static Point Sum(Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }


    public static Point Difference(Point p1, Point p2)
    {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }


    public static Point Average(Point p1, Point p2)
    {
        return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
    }


    public static Point GetMaxPointInCollection(AvaloniaList<CircuitObjectViewModel> collection)
    {
        double maxX = 0.0;
        double maxY = 0.0;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                if (c.X + c.Width > maxX) maxX = c.X + c.Width;
                if (c.Y + c.Height > maxY) maxY = c.Y + c.Height;
            }
        }

        return new Point(maxX, maxY);
    }


    public static Point GetMinPointInCollection(AvaloniaList<CircuitObjectViewModel> collection)
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


    public static void SnapCollectionToPosition(
        AvaloniaList<CircuitObjectViewModel> collection,
        Point Position, Point? offset = null)
    {
        offset ??= new(0, 0);
        offset = SnapPointToGrid((Point)offset);

        Position = SnapPointToGrid(Position);
        Point min = GetMinPointInCollection(collection);

        double offsetX = min.X - Position.X;
        double offsetY = min.Y - Position.Y;

        foreach (var obj in collection)
            if (obj is WireViewModel w) w.AllowFixing = false; 

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                c.X -= offsetX + ((Point)offset).X;
                c.Y -= offsetY + ((Point)offset).Y;
            }

            else if (co is WireViewModel w)
            {
                AvaloniaList<Point> snapped = [];
                foreach (Point pt in w.Points)
                {
                    double x = pt.X - (offsetX + ((Point)offset).X);
                    double y = pt.Y - (offsetY + ((Point)offset).Y);
                    
                    snapped.Add(new(x, y));
                }

                w.Points = snapped;
            }
        }

        foreach (var obj in collection)
            if (obj is WireViewModel w) w.AllowFixing = true;
    }


    public static Point RotateTerminalPosition(double unrotatedX, double unrotatedY,
        double rotation, double width, double height, double x, double y)
    {
        if (rotation == 0) return new Point(unrotatedX, unrotatedY);

        // do I look like a MATHEMATICIAN?! NO, and
        // this was written by gemini 3.1 pro. It works

        double centerX = x + (width / 2.0);
        double centerY = y + (height / 2.0);
        double radians = rotation * Math.PI / 180.0;

        double cos = Math.Round(Math.Cos(radians));
        double sin = Math.Round(Math.Sin(radians));

        double translatedX = unrotatedX - centerX;
        double translatedY = unrotatedY - centerY;

        double rotatedX = (translatedX * cos) - (translatedY * sin) + centerX;
        double rotatedY = (translatedX * sin) + (translatedY * cos) + centerY;

        return new Point(rotatedX, rotatedY);
    }


    public static void RedrawEmptyWires(AvaloniaList<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection)
        {
            if (co is WireViewModel w && w.Points.Count == 0) 
                w.Redraw();
        }
    }


    public static double Distance(Point p1, Point p2)
    {
        double deltaX = p2.X - p1.X;
        double deltaY = p2.Y - p1.Y;
        return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
    }
}
