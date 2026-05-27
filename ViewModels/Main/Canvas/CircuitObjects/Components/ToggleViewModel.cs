using System;
using Avalonia.Input;
using Avalonia.Media.Immutable;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects.Components;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public partial class ToggleViewModel : ComponentViewModel
{
    [ObservableProperty] private TerminalViewModel _output;
    [ObservableProperty] private ImmutableSolidColorBrush _background = new(Colors.DarkRed);
    [ObservableProperty] private string _label = "0";


    public ToggleViewModel() : this(new Toggle()) {}
    private ToggleViewModel(Toggle model) : base(model)
    {
        Output = new(model.Output, TerminalType.Output, false);
        Width = Height = 20;
    }


    protected override void UpdateTerminals()
    {
        Output.X = X + (Width + 10);
        Output.Y = Y + (Height / 2);
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
