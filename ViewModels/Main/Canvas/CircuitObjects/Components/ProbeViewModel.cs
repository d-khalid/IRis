using Avalonia.Media.Immutable;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects.Components;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.Core;
using Avalonia;
using IRis.Services;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public partial class ProbeViewModel : ComponentViewModel
{
    [ObservableProperty] private TerminalViewModel _input;
    [ObservableProperty] private ImmutableSolidColorBrush _background = new(Colors.DarkGray);
    [ObservableProperty] private string _label = "?";


    public ProbeViewModel() : this(new Probe()) {}
    private ProbeViewModel(Probe model) : base(model)
    {
        Input = new TerminalViewModel() { Type = TerminalType.Input };
        Width = Height = 20;

        model.Input.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Terminal.State)) 
                Update();
        };
    }


    protected override void UpdateTerminals()
    {
        double unrotatedX = X - 10;
        double unrotatedY = Y + 10;

        Point rotatedPos = SimulationService.RotateTerminalPosition(
            unrotatedX, unrotatedY, Rotation, Width, Height, X, Y
        );

        Input.X = rotatedPos.X;
        Input.Y = rotatedPos.Y;
    }


    public void Update()
    {
        var p = (Model as Probe)!;

        switch (p.Input.State)
        {
            case LogicState.High:
                Background = new(Colors.DarkGreen);
                Label = "1";
                break;
            case LogicState.Low:
                Background = new(Colors.DarkRed);
                Label = "0";
                break;
            case LogicState.Unknown:
                Background = new(Colors.DarkGray);
                Label = "?";
                break;
        }
    }
}
