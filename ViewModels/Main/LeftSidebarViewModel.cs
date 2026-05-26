using CommunityToolkit.Mvvm.Input;
using IRis.Models.Main.Canvas.Core;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;
using Avalonia;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [RelayCommand]
    private static void AddAnd()
    {
        var sel = Selection.GetInstance();
        var prev = Preview.GetInstance();

        sel.Ditch();
        prev.Ditch();

        AndGateViewModel gate = new() { Opacity = 0.5 };
        TerminalViewModel i1 = new(TerminalType.Input, gate);
        TerminalViewModel i2 = new(TerminalType.Input, gate);

        gate.AddInput(i1);
        gate.AddInput(i2);
        prev.Add(gate);

        prev.MouseOffset = new Point(gate.Width / 2, gate.Height / 2);
    }
}
