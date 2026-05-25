using System.Collections.ObjectModel;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;


public abstract partial class MultiInputGateViewModel : GateViewModel
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];

    
    public void AddInput(TerminalViewModel input)
    {
        Inputs.Add(input);
        Width = Height = Inputs.Count * 20;
    }

    
    public void RemoveInput(TerminalViewModel input)
    {
        if (Inputs.Count == 2) return;

        Inputs.Remove(input);
        Width = Height = Inputs.Count * 20;
    }


    protected override void UpdateInputTerminals()
    {
        double x = X - 10;
        double multiplier = 20;

        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].X = x;
            Inputs[i].Y = Y + (i * multiplier) + 10;
        }
    }
}
