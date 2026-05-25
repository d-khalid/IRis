using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.ViewModels.Circuit.CircuitObjects;


namespace IRis.ViewModels.Circuit.Core;


public partial class TerminalViewModel(TerminalType type, ComponentViewModel? parent) : ObservableObject
{
    public Terminal Model { get; } = new(type);
    public bool IsOrphan { get; set; } = parent == null;

    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;


    public TerminalType FetchType()
    {
        return Model.Type;
    }
}
