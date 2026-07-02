using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components.Gates;
using IRis.Models.Core;
using IRis.Services;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;

public partial class NotGateViewModel : GateViewModel
{
    [ObservableProperty]
    private TerminalViewModel _input = null!;

    partial void OnInputChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as NotGate)!.Input = value.GetModel();
    }

    public NotGateViewModel()
        : base(new NotGate())
    {
        Width = Height = 40;
    }

    protected override void UpdateInputTerminals()
    {
        if (Input is null)
            return;

        double unrotatedX = X - 10;
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

        Input.X = rotatedPos.X;
        Input.Y = rotatedPos.Y;
    }
}
