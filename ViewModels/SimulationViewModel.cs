using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace IRis.ViewModels;


public partial class SimulationViewModel : ObservableObject
{
    public ObservableCollection<ComponentViewModel> Components = [];
    public ObservableCollection<WireViewModel> Wires = [];
    [ObservableProperty] private CircuitObjectViewModel? _preview = null;
}

