using System.Collections.ObjectModel;
using IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;


public abstract partial class MultiInputGateViewModel : GateViewModel
{
    public ObservableCollection<TerminalViewModel> Inputs { get; } = [];


    public MultiInputGateViewModel(MultiInputGate model, TerminalViewModel i1, TerminalViewModel i2, 
        TerminalViewModel output) : base(model, output)
    {
        if (i1 is not null) Inputs.Add(i1);
        if (i2 is not null) Inputs.Add(i2);

        Width = Height = Inputs.Count * 20;
    }


    public void AddInput(TerminalViewModel input)
    {
        if (Inputs.Count == 50) return;
        Inputs.Add(input);
        Width = Height = Inputs.Count * 20;

        (Model as MultiInputGate)!.AddInput(input.GetModel());
    }

    
    public void RemoveInput(TerminalViewModel input)
    {
        if (Inputs.Count == 2) return;
        Inputs.Remove(input);
        Width = Height = Inputs.Count * 20;

        (Model as MultiInputGate)!.RemoveInput(input.GetModel());
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
