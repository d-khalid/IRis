using CommunityToolkit.Mvvm.Input;
using IRis.Models.Core;
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
        Selection.GetInstance().Ditch();

        AndGateViewModel gate = new() { Output = new() };
        gate.Inputs.Add(new());
        gate.Inputs.Add(new());

        Preview.GetInstance().Pick(gate);
    }


    [RelayCommand]
    private static void AddToggle()
    {
        var sel = Selection.GetInstance();
        var prev = Preview.GetInstance();

        sel.Ditch();
        prev.Pick(new ToggleViewModel() { Output = new() });
    }


    [RelayCommand]
    private static void AddProbe()
    {
        var sel = Selection.GetInstance();
        var prev = Preview.GetInstance();

        sel.Ditch();
        prev.Pick(new ProbeViewModel() { Input = new() });
    }
}
