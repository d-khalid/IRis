using CommunityToolkit.Mvvm.ComponentModel;
using IRis.ViewModels.Core;


namespace IRis.ViewModels.CircuitObjects.Components.Gates;


public partial class AndGateViewModel : GateViewModel
{
    public readonly TerminalViewModel Inputs;


    public AndGateViewModel(TerminalViewModel inputs, TerminalViewModel output) : base(output)
    {
        Inputs = inputs;
    }
}

