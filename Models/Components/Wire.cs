using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Controls;

using IRis.Models.Core;


namespace IRis.Models.Components;


public class Wire : Control, Core.ICloneable, ISerializable, IOutputProvider, ICustomHitTest
{
    public readonly Guid Id = Guid.NewGuid();
    public readonly List<(Terminal Terminal, bool IsOutputProvider)> Nodes = [];
    public readonly List<Point> Points = [];
    public LogicState State = LogicState.Unknown;

    // drawing controls
    public bool IsValid { get; set; } = true;
    public bool IsBeingEdited { get; set; } = true;

    private bool _isSelected = false;


    public bool IsSelected
    {
        get => _isSelected;
        set {
            _isSelected = value;
            InvalidateVisual();
        }
    }


    public void Serialize()
    {
        throw new NotImplementedException();
    }


    public object Clone()
    {
        Wire clone = new();
        return clone;
    }


    public void AddNode(Terminal terminal, bool isOutputProvider)
    {
        Nodes.Add((terminal, isOutputProvider));
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


    public bool HitTest(Point point)
    {
        foreach (Point pt in Points)
        {
            if (point == pt)
                return true;
        }

        return false;
    }
    

    public void Draw(DrawingContext ctx)
    {
        if (Points.Count == 0) 
            return;

        Pen wirePen = IsValid ? 
            (IsBeingEdited ? Constants.GhostWirePen : Constants.WirePen) : 
            Constants.InvalidWirePen;

        IBrush terminalBrush = IsValid ? 
            (IsBeingEdited ? Constants.GhostTerminalBubbleBrush : Constants.TerminalBubbleBrush) :
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

        
        foreach (var node in Nodes)
        {
            ctx.DrawEllipse(
                brush: terminalBrush,
                pen: null,
                center: node.Terminal.Position,
                radiusX: Constants.TerminalBubbleRadius,
                radiusY: Constants.TerminalBubbleRadius
            );
        }
    }


    public override void Render(DrawingContext context)
    {
        Draw(context);
        context.DrawRectangle(
            brush: IsSelected ? Brushes.Transparent : new SolidColorBrush(Colors.DodgerBlue, 0.2),
            pen: null,
            rect: new Rect(0, 0, Width, Height)
        );

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