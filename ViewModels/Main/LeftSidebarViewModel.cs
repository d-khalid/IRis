using CommunityToolkit.Mvvm.Input;
using IRis.Models.Core;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [RelayCommand]
    private static void AddAnd()
    {
        Selection.GetInstance().Ditch();

        AndGateViewModel gate = new()
        {
            Output = new TerminalViewModel() { Type = TerminalType.Output }
        };
        gate.Inputs.Add(new TerminalViewModel());
        gate.Inputs.Add(new TerminalViewModel());

        Preview.GetInstance().Pick(gate);
    }
}
