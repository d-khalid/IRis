using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System.Linq;


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
    
    

    public override void UpdateOutputValue()
    {
        var values = Inputs.Select(input => input.Value).ToArray();

        // Logic applies for any no. of Inputs
        if (values.Any(v => v == null)) Output.Value = null;
        else Output.Value = values.Any(v => v == true);
    }
}