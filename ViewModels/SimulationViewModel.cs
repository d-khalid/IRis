using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;


namespace IRis.ViewModels;


public partial class SimulationViewModel : ViewModelBase
{
    private static SimulationViewModel? _instance = null;

    public ObservableCollection<ComponentViewModel> Components { get; } = [];
    public ObservableCollection<WireViewModel> Wires { get; } = [];
    [ObservableProperty] private CircuitObjectViewModel? _preview = null;


    public SimulationViewModel()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public static SimulationViewModel GetInstance()
    {
        if (_instance == null)
            _instance = new SimulationViewModel();

        return _instance;
    }
}
