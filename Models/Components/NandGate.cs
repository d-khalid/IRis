using System.Linq;
using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;


namespace  IRis.Models.Components;


public class NandGate : Gate
{
    public NandGate(int numInputs) : base(numInputs, notMode: true)
    {

    }

    public override void Draw(DrawingContext ctx)
    {



        // 3. Draw terminals (lines + circles)
        DrawTerminals(ctx);

        this.DrawAnd(ctx);

        // 3. Draw the bubble at the end
        ctx.DrawEllipse(
            Brushes.White, // Fill (none)
            ComponentDefaults.GatePen, // Use same pen as gate
            new Point(Width + ComponentDefaults.BubbleRadius, Height / 2),
            ComponentDefaults.BubbleRadius,
            ComponentDefaults.BubbleRadius);

        base.Draw(ctx);
        
    }
    
    public override void ComputeOutput()
    {
        // If there's a missing wire, don't bother
        if (Terminals.Any(p => p.Wire == null)) return;

        // Funny LINQ expression
        if (Terminals.SkipLast(1).All(p => p.Wire.Value == LogicState.High))
        {
            Terminals[^1].Wire.Value = LogicState.Low;
        }
        else Terminals[^1].Wire.Value = LogicState.High;

    }
    
    

 
}