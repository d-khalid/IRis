using Avalonia;
using IRis.ViewModels.Circuit.CircuitObjects;
using IRis.Services;


namespace IRis.Models.Core;


public partial class Preview : ManagerBase<Preview>
{
    public Point MouseOffset = new(0, 0);


    public void Update()
    {
        Simulation sim = (Simulation)Simulation.GetInstance();
        SimulationService.SnapCollectionToPosition(
            Objects,
            sim.CurrentMousePos, 
            MouseOffset
        );
    }


    public bool IsNewWire()
    {
        return Objects.Count == 1 && Objects[0] is WireViewModel;
    }


    public void CommitAll() 
    {
        Simulation sim = (Simulation)Simulation.GetInstance();

        if (IsNewWire())
        {
            WireViewModel w = (Objects[0] as WireViewModel)!;
            w.Opacity = 1.0;
            sim.Objects.Add(w);
            Ditch();
            return;
        }

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
