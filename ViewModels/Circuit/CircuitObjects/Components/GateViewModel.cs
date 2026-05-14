using CommunityToolkit.Mvvm.ComponentModel;
using IRis.ViewModels.Circuit.CircuitObjects.Core;


namespace IRis.ViewModels.Circuit.CircuitObjects.Components;


public partial class GateViewModel(TerminalViewModel output) : ComponentViewModel
{
    public readonly TerminalViewModel Output = output;
}

