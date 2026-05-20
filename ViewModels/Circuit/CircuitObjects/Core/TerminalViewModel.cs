using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Circuit.CircuitObjects.Core;


namespace IRis.ViewModels.Circuit.CircuitObjects.Core;


public partial class TerminalViewModel(Terminal model, ComponentViewModel? parent) : ObservableObject
{
    private readonly Terminal _model = model;
    private readonly ComponentViewModel? _parent = parent;

    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;


    public TerminalType FetchType()
    {
        return _model.Type;
    }


    public bool IsOrphan()
    {
        return _parent == null;
    }
}
