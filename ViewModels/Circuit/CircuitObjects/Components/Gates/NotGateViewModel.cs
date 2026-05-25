using System.Collections.ObjectModel;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.ViewModels.Circuit.Core;


namespace IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;


public abstract partial class NotGateViewModel : GateViewModel
{
    public TerminalViewModel Input { get; }


    public NotGateViewModel(TerminalViewModel input)
    {
        Input = input;
        Width = Height = 40;
    }
}
