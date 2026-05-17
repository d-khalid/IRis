using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using IRis.ViewModels.Circuit.CircuitObjects.Core;
using System.Collections.ObjectModel;


namespace IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;


public partial class AndGateViewModel : GateViewModel
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];

    
    public void AddInput(TerminalViewModel input)
    {
        Inputs.Add(input);
        Width = Height = Inputs.Count * 20;
    }
}
