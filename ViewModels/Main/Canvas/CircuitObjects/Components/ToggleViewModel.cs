using System;
using Avalonia.Input;
using Avalonia.Media.Immutable;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects.Components;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.Core;
using Avalonia;
using IRis.Services;
using Newtonsoft.Json;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public partial class ToggleViewModel : ComponentViewModel
{
    private TerminalViewModel _output = null!;
    public TerminalViewModel Output
    {
        get => _output;
        set
        {
            value.Type = TerminalType.Output;
            SetProperty(ref _output, value);
            (Model as Toggle)!.Output = value.GetModel();
        }
    }
    
    [ObservableProperty] [property: JsonIgnore] 
    private ImmutableSolidColorBrush _background = new(Colors.DarkRed);
    [ObservableProperty] [property: JsonIgnore] private string _label = "0";


    public ToggleViewModel() : this(new Toggle()) {}
    private ToggleViewModel(Toggle model) : base(model)
    {
        Width = Height = 20;
    }


    public LogicState State
    {
        get => (Model as Toggle)!.State;
        set 
        {
            (Model as Toggle)!.State = value;
            if (value is LogicState.High)
            {
                Background = new(Colors.DarkGreen);
                Label = "1";
            }
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
        var t = (Model as Toggle)!;

        if (t.State == LogicState.High)
        {
            t.State = LogicState.Low;
            Background = new(Colors.DarkRed);
            Label = "0";
        }
        
        else if (t.State == LogicState.Low)
        {
            t.State = LogicState.High;
            Background = new(Colors.DarkGreen);
            Label = "1";
        }
    }
}
