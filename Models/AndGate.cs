using Avalonia;
using Avalonia.Media;

namespace  IRis.Models;


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