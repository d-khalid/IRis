using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class DFlipFlopViewModel : ComponentViewModel
{
    [ObservableProperty]
    private TerminalViewModel _d = null!;

    partial void OnDChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as DFlipFlop)!.D = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _clk = null!;

    partial void OnClkChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as DFlipFlop)!.Clk = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _set = null!;

    partial void OnSetChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as DFlipFlop)!.Set = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _clr = null!;

    partial void OnClrChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as DFlipFlop)!.Clr = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _q = null!;

    partial void OnQChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as DFlipFlop)!.Q = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _qBar = null!;

    partial void OnQBarChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as DFlipFlop)!.QBar = value.GetModel();
    }

    public DFlipFlopViewModel()
        : this(new DFlipFlop()) { }

    private DFlipFlopViewModel(DFlipFlop model)
        : base(model)
    {
        Width = 40;
        Height = 60;
    }

    public override void UpdateTerminals()
    {
        if (D is null || Clk is null || Set is null || Clr is null || Q is null || QBar is null)
            return;

        PlaceTerminal(D, X - 10, Y + 20);
        PlaceTerminal(Clk, X - 10, Y + 40);
        PlaceTerminal(Set, X + (Width / 2), Y - 10);
        PlaceTerminal(Clr, X + (Width / 2), Y + Height + 10);
        PlaceTerminal(Q, X + (Width + 10), Y + 20);
        PlaceTerminal(QBar, X + (Width + 10), Y + 40);
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
