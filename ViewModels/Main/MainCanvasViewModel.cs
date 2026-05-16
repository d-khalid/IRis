using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using System.Collections.ObjectModel;
using IRis.ViewModels;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using IRis.ViewModels.Circuit.CircuitObjects.Core;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Main;


public partial class MainCanvasViewModel : ObservableObject
{
    [ObservableProperty] 
    private SimulationViewModel _simulation = SimulationViewModel.GetInstance();
}
