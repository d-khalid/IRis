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


    protected void DrawTerminals(DrawingContext ctx)
    {
        DrawInputTerminals(ctx);
        DrawOutputTerminal(ctx);
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

            _inputs[i].Terminal.Draw(ctx);
        }
    }


    private void DrawOutputTerminal(DrawingContext ctx)
    {
        ctx.DrawLine(
            pen: Constants.TerminalWirePen,
            p2: new Point(Output.Position.X - Constants.TerminalWireLength, Output.Position.Y),
            p1: Output.Position
        );

        Output.Terminal.Draw(ctx);
    }


    public abstract void ComputeOutput();
}
