using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects.Components;
using IRis.Models.Main.Canvas.Core;
using IRis.Services;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public abstract partial class GateViewModel : ComponentViewModel
{
    [ObservableProperty] private TerminalViewModel _output;


    public GateViewModel(Gate model) : base(model)
    {
        Output = new TerminalViewModel(model.Output, TerminalType.Output, false);
    }


    protected override void UpdateTerminals()
    {
        UpdateOutputTerminal();
        UpdateInputTerminals();
    }


    protected void UpdateOutputTerminal()
    {
        double unrotatedX = X + (Width + 10);
        double unrotatedY = Y + (Height / 2);

        Point rotatedPos = SimulationService.RotateTerminalPosition(
            unrotatedX, unrotatedY, Rotation, Width, Height, X, Y
        );

        Output.X = rotatedPos.X;
        Output.Y = rotatedPos.Y;
    }


    protected abstract void UpdateInputTerminals();
}
