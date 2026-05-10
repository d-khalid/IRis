using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;


namespace IRis.Models.Core;


// A simple data object for your nodes
public partial class WireNode : ObservableObject
{
    [ObservableProperty] private Terminal _terminal;
    [ObservableProperty] private bool _isOutputProvider;
}

public class Wire : CircuitObject, IOutputProvider
{
    public ObservableCollection<WireNode> Nodes { get; } = new();
    public ObservableCollection<Point> Points { get; } = new();

    public LogicState State = LogicState.Unknown;


    // public override void Serialize()
    // {
    //     throw new NotImplementedException();
    // }
    
    public override object Clone()
    {
        Wire clone = new();
        return clone;
        
    }


    
    // NOTE: Don't see the point of these when you can directly do wire.Node.Add() or 

    // public void AddNode(WireNode node)
    // {
    //     Nodes.Add(node);
    // }
    //
    //
    // public void AddPoint(Point point)
    // {
    //     Points.Add(point);
    //     InvalidateVisual();
    // }
    //
    //
    public void PopPoints(int numOfPointsToPop)
    {
        Points.RemoveAt(Points.Count - numOfPointsToPop);
    }


    // TODO: THIS LOOKS WRONG, IT SHOULD CHECK FOR COLLISIONS ALONG THE POLYLINE
    // I WONDER IF THE DEFAULT HitTest IMPLEMENTATION WILL DO THAT FOR ME
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


    // public override void Render(DrawingContext context)
    // {
    //     Draw(context);
    //     context.DrawRectangle(
    //         brush: IsSelected ? Brushes.Transparent : new SolidColorBrush(Colors.DodgerBlue, 0.2),
    //         pen: null,
    //         rect: new Rect(0, 0, Width, Height)
    //     );
    //
    //     base.Render(context);
    // }


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

        //InvalidateVisual();
    }
}