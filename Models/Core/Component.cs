// default libs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;
using System.Collections.Generic;
using System;


namespace IRis.Models.Core;


public abstract class Component : Control, ICustomHitTest, ISerializable, ICloneable
{
    public readonly Guid Id = Guid.NewGuid();
    public BoxSize Size { get; }

    // selection and orientation
    private bool _isSelected = false;
    private ComponentOrientation _orientation;
    private readonly RotateTransform _rotateTransform = new();

    // input + output terminals
    protected readonly List<Terminal> _inputs = [];
    protected readonly List<Terminal> _outputs = [];


    public Component(int numInputs, int numOutputs, BoxSize size)
    {
        Size = size;

        AddTerminals(_inputs, -Constants.TerminalWireLength, numInputs, Size, false);
        AddTerminals(_outputs, size.Width+Constants.TerminalWireLength, numOutputs, Size, true);
    }


    public bool IsSelected
    {
        get => _isSelected;
        set {
            _isSelected = value;
            InvalidateVisual();
        }
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


    public bool HitTest(Point point)
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


    private static void AddTerminals(List<Terminal> target, int Xdistance, int numTerminals, BoxSize size, bool areOutputProviders)
    {
        int spacing = size.Height / (numTerminals + 1);

        for (int i = 0; i < numTerminals; i++)
        {
            var pos = new Point(Xdistance, spacing * (i + 1));
            target.Add(new Terminal(Utils.SnapPointToGrid(pos), areOutputProviders));
        }
    }


    public abstract void Serialize();
    public abstract object Clone();
    public abstract void Draw(DrawingContext ctx);
}

