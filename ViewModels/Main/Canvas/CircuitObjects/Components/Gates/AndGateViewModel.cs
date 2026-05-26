using IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;


public partial class AndGateViewModel(AndGate model, TerminalViewModel i1, TerminalViewModel i2,
    TerminalViewModel output) : MultiInputGateViewModel(model, i1, i2, output)
{
}
