using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class JKFlipFlopViewModel : ComponentViewModel
{
    [ObservableProperty]
    private TerminalViewModel _j = null!;

    partial void OnJChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.J = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _k = null!;

    partial void OnKChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.K = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _clk = null!;

    partial void OnClkChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.Clk = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _set = null!;

    partial void OnSetChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.Set = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _clr = null!;

    partial void OnClrChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.Clr = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _q = null!;

    partial void OnQChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as JKFlipFlop)!.Q = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _qBar = null!;

    partial void OnQBarChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as JKFlipFlop)!.QBar = value.GetModel();
    }

    public JKFlipFlopViewModel()
        : this(new JKFlipFlop()) { }

    private JKFlipFlopViewModel(JKFlipFlop model)
        : base(model)
    {
        Width = 40;
        Height = 60;
    }

    public override void UpdateTerminals()
    {
        if (
            J is null
            || K is null
            || Clk is null
            || Set is null
            || Clr is null
            || Q is null
            || QBar is null
        )
            return;

        PlaceTerminal(J, X - 10, Y + 10);
        PlaceTerminal(K, X - 10, Y + 30);
        PlaceTerminal(Clk, X - 10, Y + 50);
        PlaceTerminal(Set, X + (Width / 2), Y - 10);
        PlaceTerminal(Clr, X + (Width / 2), Y + Height + 10);
        PlaceTerminal(Q, X + (Width + 10), Y + 15);
        PlaceTerminal(QBar, X + (Width + 10), Y + 45);
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
