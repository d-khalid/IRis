using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;

public partial class ProbeViewModel : ComponentViewModel
{
    [ObservableProperty]
    private TerminalViewModel _input = null!;

    [ObservableProperty]
    [property: JsonIgnore]
    private IBrush _background;

    [ObservableProperty]
    [property: JsonIgnore]
    private string _label = "?";

    private readonly AppState _appState;

    public ProbeViewModel()
        : this(new Probe()) { }

    private ProbeViewModel(Probe model)
        : base(model)
    {
        _appState = App.Current.Services.GetRequiredService<AppState>();

        Model.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Probe.State))
                UpdateVisual((Model as Probe)!.State);
        };

        Width = Height = 20;

        App.Current.TryGetResource("UnknownStateBrush", _appState.Theme, out var res);

        if (res is IBrush brush)
            _background = brush;
        else
            _background = new SolidColorBrush(Colors.DarkGray);
    }

    partial void OnInputChanged(TerminalViewModel value) =>
        (Model as Probe)!.Input = value.GetModel();

    public override void UpdateTerminals()
    {
        if (Input is null)
            return;
        double unrotatedX = X - 10;
        double unrotatedY = Y + 10;

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

    public void UpdateVisual(LogicState state)
    {
        string resource;

        switch (state)
        {
            case LogicState.High:
                resource = "HighStateBrush";
                Label = "1";
                break;
            case LogicState.Low:
                resource = "LowStateBrush";
                Label = "0";
                break;
            case LogicState.Unknown:
                resource = "UnknownStateBrush";
                Label = "?";
                break;
            default:
                return;
        }

        App.Current.TryGetResource(resource, _appState.Theme, out var res);
        Background = (IBrush)res!;
    }
}
