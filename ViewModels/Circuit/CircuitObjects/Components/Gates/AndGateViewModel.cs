using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using IRis.ViewModels.Circuit.CircuitObjects.Core;


namespace IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;


public partial class AndGateViewModel(List<TerminalViewModel> inputs, TerminalViewModel output) : 
    GateViewModel(output)
{
    public readonly List<TerminalViewModel> Inputs = inputs;
}

