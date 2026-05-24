using IRis.Services;


namespace IRis.Models.Core;


public partial class Drag : ManagerBase<Drag>
{
    public Simulation Simulation = (Simulation)Simulation.GetInstance();
    public bool Used = false;


    public void Update()
    {
        var prev = (Preview)Preview.GetInstance();
        Used = true;

        SimulationService.SnapCollectionToPosition(
            Objects,
            Simulation.CurrentMousePos,
            prev.MouseOffset
        );
    }
}
