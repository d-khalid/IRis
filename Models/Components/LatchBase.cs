using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;

namespace IRis.Models.Components;

public abstract class LatchBase : CircuitComponent
{
    public LatchBase(double width, double height)
        : base(width, height)
    {
        Width = 5 * ComponentDefaults.TerminalSpacing;
        Height = 3 * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;

        Terminals = new Terminal[4];
        AddTerminalPoints();
        IsHitTestVisible = true;

        // Dictionary entry for state
        StoredStates["Q"] = LogicState.Low;
    }

    public override void Draw(DrawingContext ctx)
    {
        ctx.DrawRectangle(ComponentDefaults.GateFillBrush,
            ComponentDefaults.GatePen,
            new Rect(0, 0, Width, Height));

        DrawTerminalsAndLabels(ctx);
        base.Draw(ctx);
    }

    public override void DrawSelection(DrawingContext ctx)
    {
        double expandX = ComponentDefaults.TerminalWireLength + ComponentDefaults.TerminalRadius;
        double expandY = ComponentDefaults.TerminalRadius;

        ctx.DrawRectangle(
            ComponentDefaults.SelectionBrush,
            ComponentDefaults.SelectionPen,
            new Rect(
                -expandX,
                -expandY,
                Bounds.Width + 2 * expandX,
                Bounds.Height + 2 * expandY)
        );
    }

    internal abstract void DrawTerminalsAndLabels(DrawingContext ctx);
}