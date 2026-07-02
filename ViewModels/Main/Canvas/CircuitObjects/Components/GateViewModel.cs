using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.Services;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public abstract partial class GateViewModel(Gate model) : ComponentViewModel(model)
{
    [ObservableProperty]
    private TerminalViewModel _output = null!;

    partial void OnOutputChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as Gate)!.Output = value.GetModel();
    }

    public override void UpdateTerminals()
    {
        UpdateOutputTerminal();
        UpdateInputTerminals();
    }

    protected void UpdateOutputTerminal()
    {
        if (Output is null)
            return;
        double unrotatedX = X + (Width + 10);
        double unrotatedY = Y + (Height / 2);

        Point rotatedPos = _simulationService.RotateTerminalPosition(
            unrotatedX,
            unrotatedY,
            Rotation,
            Width,
            Height,
            X,
            Y
        );

        Output.X = rotatedPos.X;
        Output.Y = rotatedPos.Y;
    }

    protected abstract void UpdateInputTerminals();
}
