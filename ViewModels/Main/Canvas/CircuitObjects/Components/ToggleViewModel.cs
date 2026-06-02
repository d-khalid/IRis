using System;
using Avalonia.Input;
using Avalonia.Media.Immutable;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;
using Avalonia;
using IRis.Services;
using Newtonsoft.Json;
using Avalonia.Controls;
using IRis.Services.Singleton;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public partial class ToggleViewModel : ComponentViewModel
{
    [ObservableProperty] private TerminalViewModel _output = null!;
    partial void OnOutputChanged(TerminalViewModel value)
    {
        value.Type = TerminalType.Output;
        (Model as Toggle)!.Output = value.GetModel();
    }


    [ObservableProperty] [property: JsonIgnore]
    private IBrush _background;

    [ObservableProperty] [property: JsonIgnore] 
    private string _label = "0";


    public ToggleViewModel() : this(new Toggle()) { }
    private ToggleViewModel(Toggle model) : base(model)
    {
        Width = Height = 20;

        Application.Current!.TryGetResource("LowStateBrush", AppState.Get().Theme, out var res);

        if (res is IBrush brush) _background = brush;
        else _background = new SolidColorBrush(Colors.DarkGray);
    }


    public LogicState State
    {
        get => (Model as Toggle)!.State;
        set
        {
            (Model as Toggle)!.State = value;

            Label = value == LogicState.High ? "1" : "0";

            var resource = value == LogicState.High ? "HighStateBrush" : "LowStateBrush";
            Application.Current!.TryGetResource(resource, AppState.Get().Theme, out var res);
            Background = (IBrush)res!;
        }
    }


    public override void UpdateTerminals()
    {
        if (Output is null) return;
        double unrotatedX = X + (Width + 10);
        double unrotatedY = Y + (Height / 2);

        Point rotatedPos = SimulationService.RotateTerminalPosition(
            unrotatedX, unrotatedY, Rotation, Width, Height, X, Y
        );

        Output.X = rotatedPos.X;
        Output.Y = rotatedPos.Y;
    }


    public void Toggle()
    {
        if (State == LogicState.High)
            State = LogicState.Low;

        else if (State == LogicState.Low)
            State = LogicState.High;
    }
}
