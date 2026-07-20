using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class TristateBuffer : Component
{
    public Terminal In = null!;
    public Terminal Out = null!;
    public Terminal En = null!;

    // TODO: Make a seperate LogicState.HighImpedance later
    /* There is a (somewhat) subtle difference between Hi-Z and Unknown
       Hi-Z resolves differently when multiple outputs are connected into one wire
       
       Example: (Hi-Z + Low) = Low
                (Unknown/High + Low) = Unknown
     
    */
    public override void Simulate()
    {
        if (En.State == LogicState.Low || En.State == LogicState.Unknown)
        {
            Out.State = LogicState.Unknown;
        }
        else
            Out.State = In.State;
    }

    public override void Reset()
    {
        In.State = LogicState.Unknown;
        Out.State = LogicState.Unknown;
        En.State = LogicState.Unknown;
    }
}
