using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.Services;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class FullAdderViewModel : ComponentViewModel
{
    [ObservableProperty]
    private TerminalViewModel _a = null!;

    partial void OnAChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as FullAdder)!.A = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _b = null!;

    partial void OnBChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as FullAdder)!.B = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _cin = null!;

    partial void OnCinChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as FullAdder)!.Cin = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _sum = null!;

    partial void OnSumChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as FullAdder)!.Sum = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _cout = null!;

    partial void OnCoutChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as FullAdder)!.Cout = value.GetModel();
    }

    public FullAdderViewModel()
        : this(new FullAdder()) { }

    private FullAdderViewModel(FullAdder model)
        : base(model)
    {
        Width = 40;
        Height = 60;
    }

    public override void UpdateTerminals()
    {
        if (A is null || B is null || Cin is null || Sum is null || Cout is null)
            return;

        PlaceTerminal(A, X - 10, Y + 10);
        PlaceTerminal(B, X - 10, Y + 30);
        PlaceTerminal(Cin, X - 10, Y + 50);
        PlaceTerminal(Sum, X + (Width + 10), Y + 15);
        PlaceTerminal(Cout, X + (Width + 10), Y + 45);
    }

    private void PlaceTerminal(TerminalViewModel terminal, double unrotatedX, double unrotatedY)
    {
        Point rotatedPos = _simulationService.RotateTerminalPosition(
            unrotatedX,
            unrotatedY,
            Rotation,
            Width,
            Height,
            X,
            Y
        );

        terminal.X = rotatedPos.X;
        terminal.Y = rotatedPos.Y;
    }
}
