using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public partial class ToggleViewModel : ComponentViewModel
{
    [ObservableProperty] private TerminalViewModel _output;


    public ToggleViewModel()
    {
        Output = new TerminalViewModel(TerminalType.Output, this);
    }


    protected override void UpdateTerminals()
    {
        Output.X = X + (Width + 10);
        Output.Y = Y + (Height / 2);
    }
}