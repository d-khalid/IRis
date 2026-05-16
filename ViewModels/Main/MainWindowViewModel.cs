using CommunityToolkit.Mvvm.Input;
using IRis.ViewModels;
using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public SimulationViewModel Simulation { get; } = SimulationViewModel.GetInstance();


    [RelayCommand]
    private void DeleteKey()
    {
        for (int i = Simulation.CircuitObjects.Count-1; i >= 0; i--)
        {
            CircuitObjectViewModel co = Simulation.CircuitObjects[i];
            if (co.IsSelected)
                Simulation.CircuitObjects.Remove(co);
        }
    }


    [RelayCommand]
    private void EscapeKey()
    {
        Simulation.PreviewObjects.Clear();
    }
}
