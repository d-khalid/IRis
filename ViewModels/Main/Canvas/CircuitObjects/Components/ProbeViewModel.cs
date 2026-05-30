using Avalonia.Media.Immutable;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects.Components;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.Core;
using Avalonia;
using Newtonsoft.Json;
using IRis.Services;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public partial class ProbeViewModel : ComponentViewModel
{
    [ObservableProperty] private TerminalViewModel _input = null!;
    partial void OnInputChanged(TerminalViewModel value)
    {
        (Model as Probe)!.Input = value.GetModel();
        (Model as Probe)!.Input.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Terminal.State)) UpdateVisual();
        };
    }


    [ObservableProperty] [property: JsonIgnore]
    private ImmutableSolidColorBrush _background = new(Colors.DarkGray);
    [ObservableProperty] [property: JsonIgnore] private string _label = "?";


    public ProbeViewModel() : this(new Probe()) { }
    private ProbeViewModel(Probe model) : base(model)
    {
        Width = Height = 20;
    }


    public override void UpdateTerminals()
    {
        if (Input is null) return;
        double unrotatedX = X - 10;
        double unrotatedY = Y + 10;

        Point rotatedPos = SimulationService.RotateTerminalPosition(
            unrotatedX, unrotatedY, Rotation, Width, Height, X, Y
        );

        Input.X = rotatedPos.X;
        Input.Y = rotatedPos.Y;
    }


    public void UpdateVisual()
    {
        switch ((Model as Probe)!.Input.State)
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
