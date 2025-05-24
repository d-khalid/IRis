using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;


namespace  IRis.Models.Components;


public class AndGate : Gate
{
    public AndGate(int numInputs) : base(numInputs)
    {
        
    }

    public override void Draw(DrawingContext ctx)
    {
        
       
        
        // 3. Draw terminals (lines + circles)
        DrawTerminals(ctx);
        
        this.DrawAnd(ctx);
        
        base.Draw(ctx);

        
       
    }
}