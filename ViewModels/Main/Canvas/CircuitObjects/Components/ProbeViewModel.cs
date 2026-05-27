using Avalonia.Media.Immutable;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.CircuitObjects.Components;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public partial class ProbeViewModel : ComponentViewModel
{
    [ObservableProperty] private TerminalViewModel _input;
    [ObservableProperty] private ImmutableSolidColorBrush _background = new(Colors.DarkGray);
    [ObservableProperty] private string _label = "?";


    public ProbeViewModel() : this(new Probe()) {}
    private ProbeViewModel(Probe model) : base(model)
    {
        Input = new(model.Input, TerminalType.Input, false);
        Width = Height = 20;

        model.Input.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Terminal.State)) 
                Update();
        };
    }


    protected override void UpdateTerminals()
    {
        Input.X = X + (Width + 10);
        Input.Y = Y + (Height / 2);
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
