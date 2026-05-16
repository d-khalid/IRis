using CommunityToolkit.Mvvm.Input;
using IRis.ViewModels;
using IRis.ViewModels.Circuit.CircuitObjects;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public SimulationViewModel Simulation { get; } = SimulationViewModel.GetInstance();


    [RelayCommand]
    private void DeleteKey()
    {
        for (int i = Simulation.Components.Count-1; i >= 0; i--)
        {
            ComponentViewModel c = Simulation.Components[i];
            if (c.IsSelected)
                Simulation.Components.Remove(c);
        }

        for (int i = Simulation.Wires.Count-1; i >= 0; i--)
        {
            WireViewModel w = Simulation.Wires[i];
            if (w.IsSelected)
                Simulation.Wires.Remove(w);
        }
    }


    [RelayCommand]
    private void EscapeKey()
    {
        Simulation.Preview = null;
    }
}
