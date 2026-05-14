using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace IRis.ViewModels;


public partial class SimulationViewModel : ViewModelBase
{
    public ObservableCollection<ComponentViewModel> Components { get; } = [];
    public ObservableCollection<WireViewModel> Wires { get; } = [];
    [ObservableProperty] private CircuitObjectViewModel? _preview = null;
}

