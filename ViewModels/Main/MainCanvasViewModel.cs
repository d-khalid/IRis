using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using System.Collections.ObjectModel;
using IRis.ViewModels.CircuitObjects;
using IRis.ViewModels.CircuitObjects.Components.Gates;
using IRis.ViewModels.Core;


namespace IRis.ViewModels.Main;


public partial class MainCanvasViewModel : ViewModelBase
{
    public ObservableCollection<ComponentViewModel> CircuitObjects { get; } = [];
}
