using Avalonia;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.Services;
using IRis.ViewModels.Circuit.Core;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.Models.Base;


namespace IRis.Models.Core;


public partial class Preview : ManagerBase<Preview>
{
    public Point MouseOffset = new(0, 0);


    public void UpdatePosition(Point current)
    {
        SimulationService.SnapCollectionToPosition(Objects, current, MouseOffset);
    }


    public bool HasNewWire()
    {
        return Objects.Count == 1 && Objects[0] is WireViewModel;
    }


    public void EndWireAt(TerminalViewModel t)
    {
        if (Objects.Count == 1 && Objects[0] is WireViewModel) return;

        var w = (Objects[0] as WireViewModel)!;

        if (w.MainInput.IsOrphan) w.MainInput = t;
        else if (w.MainOutput.IsOrphan) w.MainOutput = t;
        else return;

        var sim = Simulation.GetInstance();
        w.Opacity = 1.0;
        sim.Objects.Add(w);
        Ditch();
    }


    public void StartWireAt(TerminalViewModel t)
    {
        TerminalViewModel input;
        TerminalViewModel output;

        if (t.FetchType() is TerminalType.Input)
        {
            input = t;
            output = new(TerminalType.Output, null);
        }
        else
        {
            input = new(TerminalType.Input, null);
            output = t;
        }

        WireViewModel wire = new(input, output);
        Add(wire);
    }


    public void CommitAll() 
    {
        Simulation sim = Simulation.GetInstance();

        foreach (var co in Objects)
        {
            if (co is ComponentViewModel c)
            {
                ComponentViewModel clone = CloningService.Clone(c);
                clone.Opacity = 1.0;
                sim.Objects.Add(clone);
            }

            else if (co is WireViewModel w)
            {
                WireViewModel clone = CloningService.Clone(w);
                clone.Opacity = 1.0;
                sim.Objects.Add(clone);
            }
        }
    }
}
