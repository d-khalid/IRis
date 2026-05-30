using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [RelayCommand]
    private static void AddAnd()
    {
        Selection.GetInstance().UnHighlightAll();

        AndGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.GetInstance().Pick(gate);
    }


    [RelayCommand]
    private static void AddToggle()
    {
        Selection.GetInstance().UnHighlightAll();

        ToggleViewModel toggle = new() { Output = new() };
        Preview.GetInstance().Pick(toggle);
    }


    [RelayCommand]
    private static void AddProbe()
    {
        Selection.GetInstance().UnHighlightAll();

        ProbeViewModel probe = new() { Input = new() };
        Preview.GetInstance().Pick(probe);
    }
}
