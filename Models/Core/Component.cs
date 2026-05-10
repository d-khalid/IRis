using Avalonia;
using Avalonia.Media;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.Models.Core;


public abstract partial class Component : CircuitObject
{
    [ObservableProperty] private BoxSize _size;

    [ObservableProperty]
    private ComponentOrientation _orientation;
    
    [ObservableProperty]
    private double _rotation = new();

    [ObservableProperty]
    protected List<Terminal> _inputs = [];
    
    [ObservableProperty]
    protected List<Terminal> _outputs = [];


    public Component(int numInputs, int numOutputs, BoxSize size)
    {
        Size = size;

        AddTerminals(_inputs, -Constants.TerminalWireLength, numInputs, Size, false);
        AddTerminals(_outputs, size.Width+Constants.TerminalWireLength, numOutputs, Size, true);
    }
    

    
    // Not needed anymore, you can directly set component.Rotation.Angle = ComponentOrientation.Left or whatever
    
    // public ComponentOrientation Orientation
    // {
    //     get => _orientation;
    //     set {
    //         _orientation = value;
    //         _rotateTransform.Angle = (double)value;
    //     }
    // }

    
    //  NOTE: HitTest() work should be done by the XAML now

    // public override bool HitTest(Point point)
    // {
    //     point = Rotation.Value.Transform(point);
    //     return new Rect(0, 0, Size.Width, Size.Height).Contains(point);
    // }


    // public override void Render(DrawingContext context)
    // {
    //     using (context.PushTransform(_rotateTransform.Value))
    //     {
    //         Draw(context);
    //         context.DrawRectangle(
    //             brush: IsSelected ? Brushes.Transparent : new SolidColorBrush(Colors.DodgerBlue, 0.2),
    //             pen: null,
    //             rect: new Rect(0, 0, Width, Height)
    //         );
    //
    //         base.Render(context);
    //     }
    // }


    private static void AddTerminals(List<Terminal> target, int Xdistance, int numTerminals, BoxSize size, bool areOutputProviders)
    {
        int spacing = size.Height / (numTerminals + 1);

        for (int i = 0; i < numTerminals; i++)
        {
            var pos = new Point(Xdistance, spacing * (i + 1));
            target.Add(new Terminal(Utils.SnapPointToGrid(pos), areOutputProviders));
        }
    }
}

