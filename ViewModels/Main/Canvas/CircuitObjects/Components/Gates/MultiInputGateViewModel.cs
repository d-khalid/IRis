using System.Collections.ObjectModel;
using IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.Models.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.Core;
using Newtonsoft.Json;


namespace IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;


public abstract partial class MultiInputGateViewModel : GateViewModel
{
    [JsonIgnore] public ObservableCollection<TerminalViewModel> Inputs { get; } = [];


    public MultiInputGateViewModel(MultiInputGate model) : base(model)
    {
        foreach (Terminal i in model.Inputs)
            Inputs.Add(new TerminalViewModel(i, TerminalType.Input, false));

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
