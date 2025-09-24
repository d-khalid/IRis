using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Vector = Avalonia.Vector;
using IRis.Models.Core;


namespace IRis.Models.Components;

// THIS WORKS, THEY GET DRAWN
public class Wire : Component, ICloneable
{
    
    
    // To identify wires in serialization
    public Guid Id { get; set; }

    
   // private Component? _lastSetter = null;

    // This value is propagated to everything connected to this wire
    public bool IsCommitted { get; set; } = false;
    public bool IsValid { get; set; } = true;
    public bool IsBeingEdited { get; set; } = true;

    public LogicState? Value { get; set; }

    public List<Point> Points { get; set; } = new List<Point>();


    public Wire() : base(0, 0)
    {
        Id = Guid.NewGuid();
        IsCommitted = false;
    }
    
    
    public void AddPoint(Point point)
    {
        Points.Add(point);
        // Reset the visuals
        this.InvalidateVisual();
    }

    public void PopPoint()
    {
         Points.RemoveAt(Points.Count - 1);  // Removes last element
         
         // Reset the visuals
         this.InvalidateVisual();
    }
    
    public bool IsPointOnWire(Point point, double tolerance)
    {
        // Check if point is close to any line segment of the wire
        for (int i = 0; i < Points.Count - 1; i++)
        {
            if (DistanceToLineSegment(point, Points[i], Points[i + 1]) <= tolerance)
                return true;
        }
        return false;
    }

    private double DistanceToLineSegment(Point point, Point lineStart, Point lineEnd)
    {
        double dx = lineEnd.X - lineStart.X;
        double dy = lineEnd.Y - lineStart.Y;
        
        // If line segment is actually a point
        if (dx == 0 && dy == 0)
            return Point.Distance(point, lineStart);
        
        // Calculate the parameter t for the closest point on the line
        double t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);
        
        // Clamp t to [0, 1] to stay within the line segment
        t = Math.Max(0, Math.Min(1, t));
        
        // Find the closest point on the line segment
        Point closestPoint = new Point(
            lineStart.X + t * dx,
            lineStart.Y + t * dy
        );
        
        return Point.Distance(point, closestPoint);
    }

    public override bool HitTest(Point point)
    {

        // Check each wire segment
        for (int i = 0; i < Points.Count - 1; i++)
        {
            Point segmentStart = Points[i];
            Point segmentEnd = Points[i + 1];

            if (IsPointNearLineSegment(point, segmentStart, segmentEnd, ComponentDefaults.WirePen.Thickness / 2))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointNearLineSegment(Point point, Point lineStart, Point lineEnd, double maxDistance)
    {
        // Vector from line start to end
        
        Vector lineVector = lineEnd - lineStart;
        double lineLengthSquared = lineVector.SquaredLength;
        

        // Project point onto the line segment
        Vector pointVector = point - lineStart;
        double t = Vector.Dot(pointVector, lineVector) / lineLengthSquared;
        t = Math.Max(0, Math.Min(1, t)); // Clamp to segment
    
        // Find nearest point on segment
        Point nearestPoint = lineStart + t * lineVector;
    
        // Check distance
        return (new Vector(point.X, point.Y) - nearestPoint).Length <= maxDistance;
    }

    public override void Draw(DrawingContext ctx)
    {
        if (Points.Count == 0) return;

        // Use ghost styling if wire is not committed OR is being edited
        bool useGhostStyling = IsBeingEdited && !IsCommitted;
        var penToUse = useGhostStyling ? ComponentDefaults.GhostWirePen : ComponentDefaults.WirePen;
        if (!IsValid) penToUse = ComponentDefaults.InvalidWirePen;
        
        if (Points.Count == 1)
        {
            var brushToUse = useGhostStyling ? ComponentDefaults.GhostTerminalBrush : ComponentDefaults.TerminalBrush;
            ctx.DrawEllipse(brushToUse, null,
                Points[0], ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
            return;
        }

        // Draw lines, breaking at (-1,-1) Points
        var polyline = new StreamGeometry();
        using (var ctxGeo = polyline.Open())
        {
            bool figureStarted = false;

            for (int i = 0; i < Points.Count; i++)
            {
                Point currentPoint = Points[i];

                // Break point detected
                if (currentPoint == new Point(-1, -1))
                {
                    if (figureStarted)
                    {
                        ctxGeo.EndFigure(false);
                        figureStarted = false;
                    }
                    continue;
                }
                
                // Start new figure or continue current one
                if (!figureStarted)
                {
                    ctxGeo.BeginFigure(currentPoint, false);
                    figureStarted = true;
                }
                else
                {
                    ctxGeo.LineTo(currentPoint);
                }
            }

            // End the last figure if it was started
            if (figureStarted)
            {
                ctxGeo.EndFigure(false);
            }
        }
        ctx.DrawGeometry(null, penToUse, polyline);
        
        for (int i = 0; i < Points.Count; i++)
        {
            // Draw first point, last point, extension point
            if (i == 0 || Points[i - 1] == new Point(-1, -1) ||
                (i < Points.Count-1 && Points[i + 1] == new Point(-1, -1)) ||
                i == Points.Count - 1)
            {
                var brushToUse = useGhostStyling ? ComponentDefaults.GhostTerminalBrush : ComponentDefaults.TerminalBrush;
                if (!IsValid) brushToUse = ComponentDefaults.InvalidTerminalBrush;
                ctx.DrawEllipse(brushToUse, null,
                    Points[i], ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
            }
        }
    }
    
    public override void DrawSelection(DrawingContext ctx)
    {
        if (Points.Count < 2) return;
        // Debug: Print all Points
        Console.WriteLine($"DrawSelection called with {Points.Count} Points:");
        for (int i = 0; i < Points.Count; i++)
        {
            Console.WriteLine($"  Point[{i}]: {Points[i]}");
        }
        Console.WriteLine("---");
        double selectionThickness = ComponentDefaults.WirePen.Thickness * 2;

        // Draw selection rectangles for line segments (skipping break Points)
        for (int i = 0; i < Points.Count - 1; i++)
        {
            Point start = Points[i];
            Point end = Points[i + 1];
            
            // Skip if either point is a break point
            if (start == new Point(-1, -1) || end == new Point(-1, -1))
                continue;
            
            // Calculate segment vector and perpendicular
            Vector segment = end - start;
            Vector normal = new Vector(-segment.Y, segment.X);
            normal = normal.Normalize() * selectionThickness / 2;

            // Create rectangle around the segment
            var rect = new StreamGeometry();
            using (var ctxGeo = rect.Open())
            {
                ctxGeo.BeginFigure(start + normal, true);
                ctxGeo.LineTo(start - normal);
                ctxGeo.LineTo(end - normal);
                ctxGeo.LineTo(end + normal);
                ctxGeo.EndFigure(true);
            }

            ctx.DrawGeometry(ComponentDefaults.SelectionBrush, ComponentDefaults.SelectionPen, rect);
        }

        // Draw selection circles at connection Points (excluding break Points)
        foreach (var point in Points)
        {
            if (point == new Point(-1, -1))
                continue;
                
            ctx.DrawEllipse(
                ComponentDefaults.SelectionBrush,
                ComponentDefaults.SelectionPen,
                point,
                selectionThickness,
                selectionThickness);
        }
    }

    // Clones the GUID too
    public override object Clone()
    {
        var clone = new Wire();
        clone.Value = this.Value;
        clone.Id = this.Id;
        for (int i = 0; i < Points.Count; i++)
        {
            clone.AddPoint(Points[i]);
        }
        
        // // Copy source and sinks by value (Terminal is a struct)
        // clone.source = source;
        // clone.sinks = new List<Terminal>(sinks);
        return clone;
    }
}