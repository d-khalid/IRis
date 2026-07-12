using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public partial class Probe : Component
{
    public Terminal Input = null!;

    [ObservableProperty]
    private LogicState _state = LogicState.Unknown;

    public override void Simulate()
    {
        State = Input.State;
    }

    public override void Reset()
    {
        State = LogicState.Unknown;
    }
}
