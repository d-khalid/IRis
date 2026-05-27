using CommunityToolkit.Mvvm.Input;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [RelayCommand]
    private static void AddAnd()
    {
        var sel = Selection.GetInstance();
        var prev = Preview.GetInstance();

        sel.Ditch();
        prev.Pick(new AndGateViewModel());
    }
}
