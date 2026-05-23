using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using System.Collections.ObjectModel;
using IRis.ViewModels;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;


namespace IRis.ViewModels.Main;


public partial class MainCanvasViewModel : ViewModelBase
{
    [ObservableProperty] 
    private Simulation _simulation = (Simulation)Simulation.GetInstance();

    [ObservableProperty] 
    private PreviewManager _preview = PreviewManager.GetInstance();

    [ObservableProperty] 
    private SelectionManager _selection = SelectionManager.GetInstance();
}
