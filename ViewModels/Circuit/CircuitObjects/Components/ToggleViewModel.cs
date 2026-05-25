using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.ViewModels.Circuit.Core;


namespace IRis.ViewModels.Circuit.CircuitObjects.Components;


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