using CommunityToolkit.Mvvm.Input;
using IRis.Models.Main.Canvas.Core;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [RelayCommand]
    private static void AddAnd()
    {
        var sel = Selection.GetInstance();
        var prev = Preview.GetInstance();
        sel.Ditch();

        Terminal i1 = new();
        Terminal i2 = new();
        Terminal o = new();

        TerminalViewModel input1 = new(i1, TerminalType.Input, false);
        TerminalViewModel input2 = new(i2, TerminalType.Input, false);
        TerminalViewModel output = new(o, TerminalType.Output, false);

        AndGate model = new(i1, i2, o);
        AndGateViewModel gate = new(model, input1, input2, output);

        prev.Pick(gate);
    }
}
