using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components;


public abstract partial class GateViewModel : ComponentViewModel
{
    [ObservableProperty] private TerminalViewModel _output;


    public GateViewModel()
    {
        Output = new TerminalViewModel(TerminalType.Output, this);
    }


    protected override void UpdateTerminals()
    {
        UpdateOutputTerminal();
        UpdateInputTerminals();
    }


    protected void UpdateOutputTerminal()
    {
        Output.X = X + (Width + 10);
        Output.Y = Y + (Height / 2);
    }


    protected abstract void UpdateInputTerminals();
}
