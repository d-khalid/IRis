using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using System.Collections.ObjectModel;
using IRis.ViewModels;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;


namespace IRis.ViewModels.Main;


public partial class MainCanvasViewModel : ViewModelBase
{
    [ObservableProperty] private Simulation _simulation = Simulation.GetInstance();
    [ObservableProperty] private Preview _preview = Preview.GetInstance();
    [ObservableProperty] private Selection _selection = Selection.GetInstance();
    public ClipboardManager Clipboard { get; } = ClipboardManager.GetInstance();


    [RelayCommand]
    private void CopyCommand()
    {
        if (Preview.HasObjects())
        {
            Clipboard.Copy(Preview.Objects);
            Preview.Ditch();
        }
        else if (Selection.HasObjects())
        {
            Clipboard.Copy(Selection.Objects);
            Selection.Ditch();
        }
    }
}
