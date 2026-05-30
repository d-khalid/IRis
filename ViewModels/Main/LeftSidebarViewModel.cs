using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [RelayCommand]
    private static void AddAnd()
    {
        Selection.Get().UnHighlightAll();

        AndGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddNot()
    {
        Selection.Get().UnHighlightAll();

        NotGateViewModel gate = new() { Input = new(), Output = new() };
        Preview.Get().Pick(gate);
    }


    [RelayCommand]
    private static void AddToggle()
    {
        Selection.Get().UnHighlightAll();

        ToggleViewModel toggle = new() { Output = new() };
        Preview.Get().Pick(toggle);
    }


    [RelayCommand]
    private static void AddProbe()
    {
        Selection.Get().UnHighlightAll();

        ProbeViewModel probe = new() { Input = new() };
        Preview.Get().Pick(probe);
    }
}
