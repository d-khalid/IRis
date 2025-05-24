using System;
using System.Collections.Generic;
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
    private List<Point> points = new List<Point>();

    private Terminal source;
    
    private List<Terminal> sinks = new List<Terminal>();

    public List<Point> Points
    {
        get => points;
        set => points = value;
    }
    

    public Wire() : base(0,0)
    {
        //points.Add(start);

    }

    public void AddPoint(Point point)
    {
        points.Add(point);
        // Reset the visuals
        this.InvalidateVisual();
    }

    public void PopPoint()
    {
         points.RemoveAt(points.Count - 1);  // Removes last element
         
         // Reset the visuals
         this.InvalidateVisual();
    }

    public override bool HitTest(Point point)
    {
        // Translation
        Point offset = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
        // Check each wire segment
        for (int i = 0; i < points.Count - 1; i++)
        {
            Point segmentStart = points[i] + offset;
            Point segmentEnd = points[i + 1] + offset;
        
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
        if (points.Count == 0) return;
        
        if (points.Count == 1)
        {
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush , null, 
                points[0], ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);
            return;
        }
    
        // Draw lines for >2 points
        var polyline = new StreamGeometry();
        using (var ctxGeo = polyline.Open())
        {
            ctxGeo.BeginFigure(points[0], false);
            for (int i = 1; i < points.Count; i++)
            {
                ctxGeo.LineTo(points[i]);
            }
            ctxGeo.EndFigure(false);
        }
        ctx.DrawGeometry(null, ComponentDefaults.WirePen, polyline);
    }
    
    public override void DrawSelection(DrawingContext ctx)
    {
        if (points.Count < 2) return;

        double selectionThickness = ComponentDefaults.WirePen.Thickness * 2;
       

        for (int i = 0; i < points.Count - 1; i++)
        {
            Point start = points[i];
            Point end = points[i + 1];
        
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

        // Draw selection circles at connection points
        foreach (var point in points)
        {
            ctx.DrawEllipse(
                ComponentDefaults.SelectionBrush,
                ComponentDefaults.SelectionPen,
                point,
                selectionThickness,
                selectionThickness);
        }
    }

    public override object Clone()
    {
        var clone = new Wire();
        for (int i = 0; i < points.Count; i++)
        {
            clone.AddPoint(points[i]);
        }
        // Copy source and sinks by value (Terminal is a struct)
        clone.source = source;
        clone.sinks = new List<Terminal>(sinks);
        return clone;
    }
}