using Avalonia;
using System;
using Newtonsoft.Json;
using IRis.ViewModels.Main.Canvas;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.Models.Core;
using System.Collections.Generic;


namespace IRis.Services;


public static class SimulationService {
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


    public static Point GetMaxPointInCollection(ObservableCollection<CircuitObjectViewModel> collection)
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


    public static Point GetMinPointInCollection(ObservableCollection<CircuitObjectViewModel> collection)
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
        ObservableCollection<CircuitObjectViewModel> collection, 
        Point Position, Point PositionMouseOffset) 
    {
        PositionMouseOffset = SnapPointToGrid(PositionMouseOffset);
        Position = SnapPointToGrid(Position);
        Point min = GetMinPointInCollection(collection);

        double offsetX = min.X - Position.X;
        double offsetY = min.Y - Position.Y;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                c.X -= offsetX + PositionMouseOffset.X;
                c.Y -= offsetY + PositionMouseOffset.Y;
            }

            else if (co is WireViewModel w)
            {
                if (w.MainInput.IsOrphan)
                {
                    w.MainInput.X = Position.X;
                    w.MainInput.Y = Position.Y;
                }

                else if (w.MainOutput.IsOrphan)
                {
                    w.MainOutput.X = Position.X;
                    w.MainOutput.Y = Position.Y;
                }
            }
        }
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
}
