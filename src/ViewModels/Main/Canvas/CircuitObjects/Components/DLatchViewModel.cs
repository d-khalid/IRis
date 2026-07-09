using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class DLatchViewModel : ComponentViewModel
{
    [ObservableProperty]
    private TerminalViewModel _d = null!;

    partial void OnDChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as DLatch)!.D = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _en = null!;

    partial void OnEnChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as DLatch)!.En = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _q = null!;

    partial void OnQChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as DLatch)!.Q = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _qBar = null!;

    partial void OnQBarChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as DLatch)!.QBar = value.GetModel();
    }

    public DLatchViewModel()
        : this(new DLatch()) { }

    private DLatchViewModel(DLatch model)
        : base(model)
    {
        Width = 40;
        Height = 40;
    }

    public override void UpdateTerminals()
    {
        if (D is null || En is null || Q is null || QBar is null)
            return;

        PlaceTerminal(D, X - 10, Y + 10);
        PlaceTerminal(En, X - 10, Y + 30);
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
