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

    [ObservableProperty]
    private TerminalViewModel _k = null!;

    [ObservableProperty]
    private TerminalViewModel _clk = null!;

    [ObservableProperty]
    private TerminalViewModel _set = null!;

    [ObservableProperty]
    private TerminalViewModel _clr = null!;

    [ObservableProperty]
    private TerminalViewModel _q = null!;

    [ObservableProperty]
    private TerminalViewModel _qBar = null!;

    public JKFlipFlopViewModel()
        : this(new JKFlipFlop()) { }

    private JKFlipFlopViewModel(JKFlipFlop model)
        : base(model)
    {
        Width = 40;
        Height = 80;
    }

    partial void OnJChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.J = value.GetModel();
    }

    partial void OnKChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.K = value.GetModel();
    }

    partial void OnClkChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.Clk = value.GetModel();
    }

    partial void OnSetChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.Set = value.GetModel();
    }

    partial void OnClrChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as JKFlipFlop)!.Clr = value.GetModel();
    }

    partial void OnQChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as JKFlipFlop)!.Q = value.GetModel();
    }

    partial void OnQBarChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as JKFlipFlop)!.QBar = value.GetModel();
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

        PlaceTerminal(J, X - 10, Y + 20);
        PlaceTerminal(K, X - 10, Y + 40);
        PlaceTerminal(Clk, X - 10, Y + 60);
        PlaceTerminal(Set, X + (Width / 2), Y);
        PlaceTerminal(Clr, X + (Width / 2), Y + Height);
        PlaceTerminal(Q, X + (Width + 10), Y + 30);
        PlaceTerminal(QBar, X + (Width + 10), Y + 50);
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
