using Avalonia;
using Avalonia.Media;
using System.Collections.Generic;


namespace IRis.Models.Core;


public abstract class Component : CircuitObject
{
    public BoxSize Size { get; }

    private ComponentOrientation _orientation;
    private readonly RotateTransform _rotateTransform = new();

    protected readonly List<(Terminal Terminal, Point Position)> _inputs = [];
    protected readonly List<(Terminal Terminal, Point Position)> _outputs = [];


    public Component(int numInputs, int numOutputs, BoxSize size)
    {
        Size = size;

        AddTerminals(_inputs, -Constants.TerminalWireLength, numInputs, Size);
        AddTerminals(_outputs, size.Width+Constants.TerminalWireLength, numOutputs, Size);
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


    public override bool HitTest(Point point)
    {
        point = _rotateTransform.Value.Transform(point);
        return new Rect(0, 0, Width, Height).Contains(point);
    }


    public override void Render(DrawingContext context)
    {
        using (context.PushTransform(_rotateTransform.Value))
        {
            Draw(context);
            context.DrawRectangle(
                brush: IsSelected ? Brushes.Transparent : new SolidColorBrush(Colors.DodgerBlue, 0.2),
                pen: null,
                rect: new Rect(0, 0, Width, Height)
            );

            base.Render(context);
        }
    }


    private static void AddTerminals(List<(Terminal, Point)> target, 
        int Xdistance, int numTerminals, BoxSize size)
    {
        int spacing = size.Height / (numTerminals + 1);

        for (int i = 0; i < numTerminals; i++)
        {
            var pos = Utils.SnapPointToGrid(new Point(Xdistance, spacing * (i + 1)));
            target.Add((new Terminal(pos), pos));
        }
    }
}

