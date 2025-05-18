using Avalonia;
using Avalonia.Media;

namespace  IRis.Models;


public class NandGate : Gate
{
    public NandGate(int numInputs) : base(numInputs, notMode:true)
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
}