using CommunityToolkit.Mvvm.ComponentModel;
using IRis.ViewModels.Core;


namespace IRis.ViewModels.CircuitObjects.Components;


public partial class GateViewModel(TerminalViewModel output) : ComponentViewModel
{
    public readonly TerminalViewModel Output = output;
}

