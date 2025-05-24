using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;


namespace  IRis.Models.Components;


public class XnorGate : Gate
{
    public XnorGate(int numInputs) : base(numInputs, notMode:true)
    {
        
    }

    public override void Draw(DrawingContext ctx)
    {

        // 3. Draw terminals (input left, output right)
        DrawTerminals(ctx);
        
        this.DrawOr(ctx, true);
        
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