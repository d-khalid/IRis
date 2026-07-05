using System;
using Avalonia;
using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;

namespace IRis.Services;

public class SimulationService
{
    public Point SnapPointToGrid(Point pt)
    {
        double gridSpacing = 10.0;
        double snapX = Math.Round(pt.X / gridSpacing) * gridSpacing;
        double snapY = Math.Round(pt.Y / gridSpacing) * gridSpacing;
        return new Point(snapX, snapY);
    }

    public Point Sum(Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }

    public Point Difference(Point p1, Point p2)
    {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }

    public Point Average(Point p1, Point p2)
    {
        return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
    }

    public Point GetMaxPointInCollection(AvaloniaList<CircuitObjectViewModel> collection)
    {
        double maxX = 0.0;
        double maxY = 0.0;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                double x = c.X + c.Width;
                double y = c.Y + c.Height;

                if (x > 999999 || y > 999999)
                    continue;

                if (x > maxX)
                    maxX = x;
                if (y > maxY)
                    maxY = y;
            }
        }

        return new Point(maxX, maxY);
    }

    public Point GetMinPointInCollection(AvaloniaList<CircuitObjectViewModel> collection)
    {
        double minX = double.MaxValue;
        double minY = double.MaxValue;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                if (c.X < minX)
                    minX = c.X;
                if (c.Y < minY)
                    minY = c.Y;
            }
        }

        return new Point(minX, minY);
    }

    public void SnapCollectionToPosition(
        AvaloniaList<CircuitObjectViewModel> collection,
        Point Position,
        Point? offset = null
    )
    {
        offset ??= new(0, 0);
        offset = SnapPointToGrid((Point)offset);

        Position = SnapPointToGrid(Position);
        Point min = GetMinPointInCollection(collection);

        double offsetX = min.X - Position.X;
        double offsetY = min.Y - Position.Y;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                c.X -= offsetX + ((Point)offset).X;
                c.Y -= offsetY + ((Point)offset).Y;
            }

            // wires will auto-snap
        }
    }

    public Point RotateTerminalPosition(
        double unrotatedX,
        double unrotatedY,
        double rotation,
        double width,
        double height,
        double x,
        double y
    )
    {
        if (rotation == 0)
            return new Point(unrotatedX, unrotatedY);

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

    public void RedrawEmptyWires(AvaloniaList<CircuitObjectViewModel> collection)
    {
        foreach (var co in collection)
        {
            if (co is WireViewModel w && w.Points.Count == 0)
                w.Redraw();
        }
    }

    public double Distance(Point p1, Point p2)
    {
        double deltaX = p2.X - p1.X;
        double deltaY = p2.Y - p1.Y;
        return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
    }
}
