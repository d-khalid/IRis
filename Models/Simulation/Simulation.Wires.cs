// Models/Simulation.Wires.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models;

// Contains Wire drawing Helpers
public partial class Simulation : ObservableObject
{
    // --------------------------------
    // Contains Wire drawing Helpers
    // --------------------------------
    public Terminal? FindClosestSnapTerminal(Point p)
    {
        Terminal? closestTerminal = null;
        double minDistance = double.MaxValue;

        foreach (Component component in _components)
        {
            if (component.Terminals == null) continue;
            foreach (Terminal terminal in component.Terminals)
            {
                // Calculate absolute terminal position
                Point absTerminalPos = new Point(
                    terminal.Position.X + Canvas.GetLeft(component),
                    terminal.Position.Y + Canvas.GetTop(component)
                );

                double distance = Point.Distance(p, absTerminalPos);
                if (distance < minDistance && distance <= ComponentDefaults.TerminalSnappingRange)
                {
                    minDistance = distance;
                    closestTerminal = terminal;
                }
            }
        }
        return closestTerminal; // Returns null if no terminal is within snapping range
    }

    public Point GetAbsoluteTerminalPosition(Terminal terminal)
    {
        foreach (Component component in _components)
        {
            if (component.Terminals == null) continue;
            foreach (Terminal compTerminal in component.Terminals)
            {
                if (compTerminal == terminal)
                {
                    return new Point(
                        compTerminal.Position.X + Canvas.GetLeft(component),
                        compTerminal.Position.Y + Canvas.GetTop(component)
                    );
                }
            }
        }
        return new Point(-2, -2);   // Error case; should not occur
    }

    public bool IsInputTerminal(Terminal terminal)
    {
        if (terminal == null) return false;
        
        foreach (Component component in _components)
        {
            if (component.Terminals == null) continue;
            
            // For Gate components, check if terminal is not the last one (output)
            if (component is Gate gate)
            {
                for (int i = 0; i < gate.Terminals!.Length - 1; i++) // Exclude last terminal (output)
                {
                    if (gate.Terminals[i] == terminal)
                        return true;
                }
            }
            // For Multiplexer components
            else if (component is Multiplexer mux)
            {
                // Selection lines (indices 0 to SelectionLineCount-1) and 
                // Input lines (indices SelectionLineCount to SelectionLineCount+InputLineCount-1) are inputs
                // Only the last terminal (^1) is output
                for (int i = 0; i < mux.Terminals!.Length - 1; i++) // Exclude last terminal (output)
                {
                    if (mux.Terminals[i] == terminal)
                        return true;
                }
            }
            // For CustomComponent
            else if (component is CustomComponent customComp)
            {
                // First InputCount terminals are inputs, remaining are outputs
                for (int i = 0; i < customComp.InputCount; i++)
                {
                    if (i < customComp.Terminals!.Length && customComp.Terminals[i] == terminal)
                        return true;
                }
            }
        }
        
        return false;
    }

    public Wire? FindWireAtPosition(Point position)
    {
        return _components.OfType<Wire>()
            .FirstOrDefault(wire => wire.IsPointOnWire(position, 5.0)); // 5.0 is click tolerance
    }

    private bool IsPointInsideAnyComponent(Point point)
    {
        return Components.Any(component =>
        {
            if (component is Wire) return false;

            Rect bounds = component.Bounds;

            if (component.Rotation == 0)    // PATCH: this is a safe check for when rotations are added
            {
                Rect adjustedBounds = new Rect(
                    bounds.X - 10,
                    bounds.Y,
                    bounds.Width + 20,  // prevent negative width
                    bounds.Height
                );
                return adjustedBounds.Contains(point);
            }
            else
            {
                return bounds.Contains(point);
            }
        });
    }

    public bool IsWireInsideAnyComponent(List<Point> points)
    {
        Point InvalidPoint = new Point(-1, -1);
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (points[i] == InvalidPoint || points[i + 1] == InvalidPoint) continue;
            List<Point> checkablePoints = [points[i]];
            double dx = points[i].X - points[i + 1].X;
            double dy = points[i].Y - points[i + 1].Y;

            if (dx != 0)
            {
                while (dx != 0)
                {
                    checkablePoints.Add(new Point(points[i].X - dx, points[i].Y));
                    if (dx > 0) dx -= 10;
                    else dx += 10;
                }
            }
            else if (dy != 0)
            {
                while (dy != 0)
                {
                    checkablePoints.Add(new Point(points[i].X, points[i].Y - dy));
                    if (dy > 0) dy -= 10;
                    else dy += 10;
                }
            }
            else
            {
                continue;   // Handling for duplicate points
            }
            foreach (Point pt in checkablePoints)
            {
                if (IsPointInsideAnyComponent(pt)) return true;
            }
        }
        return false;
    }

    public bool DoesWireOverlapAnotherWire(List<Point> points)
    {
        var existingWirePoints = Components.OfType<Wire>().ToList()
            .SelectMany(w => w.Points.Where(p => p.X != -1 && p.Y != -1))
            .ToHashSet();
        List<Point> validPoints = [.. points];
        foreach (Point point in points)
            if (FindClosestSnapTerminal(point) == null) validPoints.Add(point);

        return validPoints.Any(existingWirePoints.Contains);
    }
    
    public bool IsWireSupersetOfAnotherWire(List<Point> wirePoints)
    {
        var wirePointsSet = wirePoints.Where(p => p.X != -1 && p.Y != -1).ToHashSet();
        bool IsWireSuperset = false;
        // Check if wire is a superset of another wire
        foreach (Component component in _components)
        {
            if (component is Wire existingWire)
            {
                if (Components.OfType<Wire>().ToList()
                    .Any(existingWire => existingWire.Points.Where(p => p.X != -1 && p.Y != -1)
                        .All(wirePointsSet.Contains)))
                {
                    IsWireSuperset = true;
                    break;
                }
            }
        }
        return IsWireSuperset;
    }

    public bool DoesWireSelfOverlap(List<Point> points)
    {
        var allLinePoints = new HashSet<Point>();
        var validPoints = points.Where(p => p.X != -1 && p.Y != -1).ToList();

        for (int i = 0; i < validPoints.Count - 1; i++)
        {
            int dx = (int)(validPoints[i + 1].X - validPoints[i].X);
            int dy = (int)(validPoints[i + 1].Y - validPoints[i].Y);
            int steps = (int)(Math.Max(Math.Abs(dx), Math.Abs(dy)) / ComponentDefaults.GridSpacing);
            if (steps == 0) continue;

            for (int j = 1; j < steps; j++) // Skip endpoints to avoid false positives
            {
                var point = new Point((int)(validPoints[i].X + dx * j / steps), (int)(validPoints[i].Y + dy * j / steps));
                if (!allLinePoints.Add(point)) return true;
            }
        }
        return false;
    }

    public bool DoesWireCrossTerminal(List<Point> points, Terminal? exceptionCase=null)
    {
        var terminalPositions = Components.ToList().Where(c => c is not Wire && c.Terminals != null)
            .SelectMany(c => c.Terminals!.Where(t => t != exceptionCase).Select(t => GetAbsoluteTerminalPosition(t)))
            .ToHashSet();

        Point InvalidPoint = new Point(-1, -1);
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (points[i] == InvalidPoint || points[i + 1] == InvalidPoint) continue;
            List<Point> checkablePoints = [];
            double dx = points[i].X - points[i + 1].X;
            double dy = points[i].Y - points[i + 1].Y;

            if (dx != 0)
            {
                while (dx != 0)
                {
                    checkablePoints.Add(new Point(points[i].X - dx, points[i].Y));
                    if (dx > 0) dx -= 10;
                    else dx += 10;
                }
            }
            else if (dy != 0)
            {
                while (dy != 0)
                {
                    checkablePoints.Add(new Point(points[i].X, points[i].Y - dy));
                    if (dy > 0) dy -= 10;
                    else dy += 10;
                }
            }
            else
            {

                continue;   // Handling for duplicate points
            }

            // PATCH: this needs a replacement later
            checkablePoints.RemoveAll((point) =>
            {
                return points.Contains(point);
            });

            foreach (Point pt in checkablePoints)
            {
                Console.WriteLine(pt);
                foreach (Point pos in terminalPositions)
                {
                    if (pt == pos)
                    {
                        Console.WriteLine("FOUND");
                        Console.WriteLine(pt);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool DoesWireHaveExtension(Wire wire)
    {
        foreach (Point point in wire.Points)
        {
            if (point == new Point(-1, -1)) return true;
        }
        return false;
    }
    
    // private bool IsWireInMovedWires(Wire wire, List<Component> MovedWires)
    // {
    //     foreach (var movedWire in MovedWires)
    //     {
    //         if (movedWire == wire) return true;
    //     }
    //     return false;
    // }

    // Point SnapToGrid(Point pt)
    // {
    //     double snapX = (int)Math.Round(Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing);
    //     double snapY = (int)Math.Round(Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing);
    //     return new Point(snapX, snapY);
    // }
}