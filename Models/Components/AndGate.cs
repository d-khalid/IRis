using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System.Linq;


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

    // public override void UpdateOutputValue()
    // {
    //     var values = Inputs.Select(input => input.Wire.Value).ToArray();
    //
    //     // Logic applies for any no. of Inputs
    //     if (values.Any(v => v == null)) Output.Wire.Value = null;
    //     else Output.Wire.Value = values.All(v => v == true);
    // }
}