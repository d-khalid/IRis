using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class TristateBufferViewModel : ComponentViewModel
{
    [ObservableProperty]
    private TerminalViewModel _in = null!;

    partial void OnInChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as TristateBuffer)!.In = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _en = null!;

    partial void OnEnChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Input;
        (Model as TristateBuffer)!.En = value.GetModel();
    }

    [ObservableProperty]
    private TerminalViewModel _out = null!;

    partial void OnOutChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as TristateBuffer)!.Out = value.GetModel();
    }

    public TristateBufferViewModel()
        : this(new TristateBuffer()) { }

    private TristateBufferViewModel(TristateBuffer model)
        : base(model)
    {
        Width = Height = 40; // Same as NOT gate
    }

    public override void UpdateTerminals()
    {
        if (In is null || Out is null || En is null)
            return;

        PlaceTerminal(In, X - 10, Y + Height / 2);
        PlaceTerminal(En, X + Width / 2, (Y + Height + 10));
        PlaceTerminal(Out, X + Width + 10, (Y + Height / 2));
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
