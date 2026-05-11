using Avalonia;
using Avalonia.Media;


namespace IRis.Models.Core;


public abstract class Gate(int numInputs, BoxSize size) : 
    Component(numInputs, numOutputs: 1, size), IOutputProvider
{
    public (Terminal Terminal, Point Position) Output
    {
        get => _outputs[0];
    }


    protected void DrawTerminals(DrawingContext ctx, bool notBubbleMode = false)
    {
        DrawInputTerminals(ctx);
        DrawOutputTerminal(ctx, notBubbleMode);
    }


    private void DrawInputTerminals(DrawingContext ctx)
    {
        for (int i = 0; i < _inputs.Count; i++)
        {
            ctx.DrawLine(
                pen: Constants.TerminalWirePen, 
                p1: _inputs[i].Position, 
                p2: new Point(Constants.TerminalWireLength, _inputs[i].Position.Y)
            );

            _inputs[i].Terminal.Draw(ctx, _inputs[i].Position);
        }
    }


    private void DrawOutputTerminal(DrawingContext ctx, bool notBubbleMode = false)
    {
        Point pt;
        if (notBubbleMode)
        {
            pt = new Point(
                Output.Position.X + Constants.NotBubbleRadius,
                Output.Position.Y
            );
        }
        else
        {
            pt = Output.Position;
        }

        ctx.DrawLine(
            pen: Constants.TerminalWirePen,
            p2: new Point(
                x: pt.X - Constants.TerminalWireLength, 
                y: pt.Y),
            p1: pt
        );

        Output.Terminal.Draw(ctx, pt);
    }


    public abstract void ComputeOutput();
}

