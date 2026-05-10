using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;

using IRis.Models.Core;


namespace IRis.Models.Components;


public class Wire : CircuitObject, IOutputProvider
{
    public readonly List<(Terminal Terminal, Point Position, bool IsOutputProvider)> Nodes = [];
    public readonly List<Point> Points = [];

    public LogicState State = LogicState.Unknown;


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override object Clone()
    {
        Wire clone = new();
        return clone;
    }


    public void AddNode(Terminal terminal, Point position, bool isOutputProvider)
    {
        Nodes.Add((terminal, position, isOutputProvider));
        AddPoint(position);
        InvalidateVisual();
    }


    public void RemoveLastNode(bool removeAssociatedPoint = true)
    {
        var nodeToRemove = Nodes[^1];
        Nodes.Remove(nodeToRemove);
        if (removeAssociatedPoint) Points.Remove(nodeToRemove.Position);
        InvalidateVisual();
    }
    
    
    public void AddPoint(Point point)
    {
        Points.Add(point);
        InvalidateVisual();
    }


    public void PopPoints(int numOfPointsToPop)
    {
        Points.RemoveAt(Points.Count - numOfPointsToPop);
        InvalidateVisual();
    }


    public void NullifyTerminalStates()
    {
        foreach (var node in Nodes)
            node.Terminal.State = LogicState.Unknown;

        InvalidateVisual();
    }


    public override bool HitTest(Point point)
    {
        foreach (Point pt in Points)
        {
            if (point == pt)
                return true;
        }

        return false;
    }
    

    public override void Draw(DrawingContext ctx)
    {
        if (Points.Count == 0) 
            return;

        Pen wirePen = IsValid ? 
            (IsPreview ? Constants.GhostWirePen : Constants.WirePen) : 
            Constants.InvalidWirePen;

        IBrush terminalBrush = IsValid ? 
            (IsPreview ? Constants.GhostTerminalBubbleBrush : Constants.TerminalBubbleBrush) :
            Constants.InvalidTerminalBubbleBrush;


        var polyline = new StreamGeometry();
        using (var ctxGeo = polyline.Open())
        {
            ctxGeo.BeginFigure(Points[0], false);

            foreach (Point pt in Points)
            {
                if (pt == Points[0]) 
                    continue;

                ctxGeo.LineTo(pt);
            }

            ctxGeo.EndFigure(false);
        }

        ctx.DrawGeometry(
            brush: null,
            pen: wirePen,
            geometry: polyline
        );
    }


    public override void Render(DrawingContext context)
    {
        Draw(context);
        context.DrawRectangle(
            brush: IsSelected ? Brushes.Transparent : new SolidColorBrush(Colors.DodgerBlue, 0.2),
            pen: null,
            rect: new Rect(0, 0, Width, Height)
        );

        foreach (var node in Nodes) node.Terminal.Draw(context);
        base.Render(context);
    }


    public void ComputeOutput()
    {
        LogicState output = LogicState.Unknown;

        // Get the state from the output providers
        foreach (var node in Nodes)
        {
            if (node.IsOutputProvider)
            {
                if (output == LogicState.Unknown)
                {
                    output = node.Terminal.State;
                }
                else
                {
                    Console.WriteLine("Error: wire has OutputProviders with different outputs");
                }
            }
        }

        // propagate the state to non-output providers
        foreach (var node in Nodes)
        {
            if (!node.IsOutputProvider)
            {
                node.Terminal.State = output;
            }
        }

        InvalidateVisual();
    }
}