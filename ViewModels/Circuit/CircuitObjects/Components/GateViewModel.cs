using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.ViewModels.Circuit.CircuitObjects.Core;


namespace IRis.ViewModels.Circuit.CircuitObjects.Components;


public abstract partial class GateViewModel : ComponentViewModel
{
    public TerminalViewModel Output { get; }


    public GateViewModel()
    {
        Output = new TerminalViewModel(new Terminal(TerminalType.Output), this);
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
