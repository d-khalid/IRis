using Avalonia;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.Services;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.Models.Main.Canvas.Core;
using IRis.Base;
using IRis.Models.Main.Canvas.CircuitObjects;


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


    public void Pick(ComponentViewModel c)
    {
        Ditch();
        Add(c);
        MouseOffset = new Point(c.Width / 2, c.Height / 2);
    }


    public void StartWireAt(TerminalViewModel t)
    {
        Terminal i = new();
        Terminal o = new();

        if (t.Type is TerminalType.Input) i = t.GetModel();
        else if (t.Type is TerminalType.Output) o = t.GetModel();
        else return;

        TerminalViewModel input = t;
        TerminalViewModel output = t;

        if (t.Type is TerminalType.Input) output = new(o, TerminalType.Output, true);
        else if (t.Type is TerminalType.Output) input = new(o, TerminalType.Input, true);
        else return;

        Wire model = new(i, o);
        WireViewModel wire = new(model, input, output);
        Add(wire);
    }


    public void EndWireAt(TerminalViewModel t)
    {
        if (!HasNewWire()) return;
        var w = (Objects[0] as WireViewModel)!;

        if (w.MainInput.IsOrphan) w.MainInput = t;
        else if (w.MainOutput.IsOrphan) w.MainOutput = t;
        else return;

        var sim = Simulation.GetInstance();
        w.Opacity = 1.0;
        sim.Objects.Add(w);
        Ditch();
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
