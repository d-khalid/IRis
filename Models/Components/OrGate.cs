using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;


namespace  IRis.Models.Components;


public class OrGate : Gate
{
    public OrGate(int numInputs) : base(numInputs)
    {
        
    }

    public override void Draw(DrawingContext ctx)
    {

        // 3. Draw terminals (input left, output right)
        DrawTerminals(ctx);
        
        this.DrawOr(ctx);
        
        base.Draw(ctx);


    }
}