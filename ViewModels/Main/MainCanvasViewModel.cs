using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using System.Collections.ObjectModel;
using IRis.ViewModels;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using IRis.ViewModels.Circuit.CircuitObjects.Core;


namespace IRis.ViewModels.Main;


public partial class MainCanvasViewModel(SimulationViewModel simulation) : ViewModelBase
{
    // public ObservableCollection<ComponentViewModel> CircuitObjects { get; } = [];
    private readonly SimulationViewModel _simulation = simulation;
}
