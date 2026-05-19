using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using System.Collections.ObjectModel;
using IRis.ViewModels;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using IRis.ViewModels.Circuit.CircuitObjects.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;


namespace IRis.ViewModels.Main;


public partial class MainCanvasViewModel : ViewModelBase
{
    [ObservableProperty] 
    private SimulationManager _simulation = SimulationManager.GetInstance();
}
