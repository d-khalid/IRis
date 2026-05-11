using Avalonia;
using Avalonia.Media;
using System.Collections.Generic;
using System;


namespace IRis.Models.Core;


public abstract class Component : CircuitObject
{
    public BoxSize Size { get; }
    public Point Position { get; set; }

    private ComponentOrientation _orientation;
    private readonly RotateTransform _rotateTransform = new();

    public List<(Terminal Terminal, Point Position)> Inputs { get; set; } = [];
    public List<(Terminal Terminal, Point Position)> Outputs { get; set; }= [];
    
    


    public Component(int numInputs, int numOutputs, BoxSize size)
    {
        Size = size;

        AddTerminals(Inputs, -Constants.TerminalWireLength, numInputs, Size);
        AddTerminals(Outputs, size.Width+Constants.TerminalWireLength, numOutputs, Size, true);
    }


    public ComponentOrientation Orientation
    {
        get => _orientation;
        set {
            _orientation = value;
            _rotateTransform.Angle = (double)value;
            InvalidateVisual();
        }
    }


    public void NullifyTerminalStates()
    {
        foreach (var i in Inputs)
            i.Terminal.State = LogicState.Unknown;

        foreach (var o in Outputs)
            o.Terminal.State = LogicState.Unknown;

        InvalidateVisual();
    }


    public Terminal? GetTerminalHitTest(Point point)
    {
        foreach (var i in Inputs)
        {
            if (Utils.AddPoints(i.Terminal.Position, Position) == point)
                return i.Terminal;
        }

        foreach (var o in Outputs)
        {
            if (Utils.AddPoints(o.Terminal.Position, Position) == point)
                return o.Terminal;
        }

        return null;
    }


    public override bool HitTest(Point point)
    {
        return new Rect(Position.X, Position.Y, Size.Width, Size.Height).Contains(point);
    }


    public override void Render(DrawingContext context)
    {
        using (context.PushTransform(_rotateTransform.Value))
        {
            Draw(context);
            context.DrawRectangle(
                brush: IsSelected ? new SolidColorBrush(Colors.DodgerBlue, 0.2) : Brushes.Transparent,
                pen: IsSelected ? new Pen(brush: new SolidColorBrush(Colors.DodgerBlue, 0.6), thickness: 2): null,
                rect: new Rect(
                    x: -Constants.GridSpacing, 
                    y: -Constants.GridSpacing,
                    width: Size.Width+Constants.GridSpacing*2, 
                    height: Size.Height+Constants.GridSpacing*2
                )
            );

            base.Render(context);

            foreach (var i in Inputs) i.Terminal.Draw(context, i.Position);
            foreach (var o in Outputs) o.Terminal.Draw(context, o.Position);
        }
    }


    private static void AddTerminals(List<(Terminal, Point)> target, 
        int Xdistance, int numTerminals, BoxSize size, bool isOutputProvider = false)
    {
        int spacing = size.Height / (numTerminals + 1);

        for (int i = 0; i < numTerminals; i++)
        {
            var pos = Utils.SnapPointToGrid(new Point(Xdistance, spacing * (i + 1)));
            target.Add((new Terminal(pos, isOutputProvider), pos));
        }
    }
}

