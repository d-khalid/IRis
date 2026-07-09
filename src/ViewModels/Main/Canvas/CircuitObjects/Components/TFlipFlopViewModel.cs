using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class TFlipFlopViewModel : ComponentViewModel
{
    [ObservableProperty]
    private TerminalViewModel _t = null!;

    partial void OnTChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as TFlipFlop)!.T = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _clk = null!;

    partial void OnClkChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as TFlipFlop)!.Clk = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _set = null!;

    partial void OnSetChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as TFlipFlop)!.Set = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _clr = null!;

    partial void OnClrChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as TFlipFlop)!.Clr = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _q = null!;

    partial void OnQChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as TFlipFlop)!.Q = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _qBar = null!;

    partial void OnQBarChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as TFlipFlop)!.QBar = value.GetModel();
    }

    public TFlipFlopViewModel()
        : this(new TFlipFlop()) { }

    private TFlipFlopViewModel(TFlipFlop model)
        : base(model)
    {
        Width = 40;
        Height = 60;
    }

    public override void UpdateTerminals()
    {
        if (T is null || Clk is null || Set is null || Clr is null || Q is null || QBar is null)
            return;

        PlaceTerminal(T, X - 10, Y + 10);
        PlaceTerminal(Clk, X - 10, Y + 30);
        PlaceTerminal(Set, X + (Width / 2), Y - 10);
        PlaceTerminal(Clr, X + (Width / 2), Y + Height + 10);
        PlaceTerminal(Q, X + (Width + 10), Y + 10);
        PlaceTerminal(QBar, X + (Width + 10), Y + 30);
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
