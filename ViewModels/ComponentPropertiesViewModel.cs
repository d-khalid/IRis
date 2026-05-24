using IRis.Models;

namespace IRis.ViewModels;

public class ComponentPropertiesViewModel : ViewModelBase
{
    private readonly Simulation _simulation;
    
    public ComponentPropertiesViewModel(Simulation simulation)
    {
        _simulation = simulation;
    }
    

    // public string ComponentType
    // {
    //     get => _simulation.SelectedComponents.Count == 1 ? _simulation.SelectedComponents[0].ToString()
    // }
    public string ComponentType
    {
        get => "AndGate";
    }

    public double ComponentRotation
    {
        get => 0;
    }

    public int NumInputs
    {
        get => 3;
    }
}