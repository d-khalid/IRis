using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System;

namespace IRis.Models.Components;

public class Encoder : CircuitComponent, IOutputProvider
{

    public Encoder(int selectionLineCount,
        double width = ComponentDefaults.DefaultMuxWidth,
        double height = ComponentDefaults.DefaultMuxHeight)
        : base(width, height)
    {
        SelectionLineCount = selectionLineCount;
        InputLineCount = (int)Math.Pow(2, SelectionLineCount);

        Width = (SelectionLineCount + 1) * ComponentDefaults.TerminalSpacing;
        Height = (InputLineCount + 1) * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;

        // Inputs + encoded outputs
        Terminals = new Terminal[InputLineCount + SelectionLineCount];

        AddTerminalPoints();

        IsHitTestVisible = true;
    }

    public void ComputeOutput()
    {
        int activeIndex = -1;

        // Find the first active input (priority encoder)
        for (int i = 0; i < InputLineCount; i++)
        {
            if (Terminals![i].Wire!.Value == LogicState.High)
            {
                activeIndex = i;
                break;
            }
        }

        // Default outputs Low
        for (int j = 0; j < SelectionLineCount; j++)
        {
            Terminals![InputLineCount + j].Wire!.Value = LogicState.Low;
        }

        if (activeIndex >= 0)
        {
            // Encode index to binary
            for (int j = 0; j < SelectionLineCount; j++)
            {
                bool bit = (activeIndex & (1 << (SelectionLineCount - j - 1))) != 0;
                Terminals![InputLineCount + j].Wire!.Value = bit ? LogicState.High : LogicState.Low;
            }
        }
    }

    public override void Draw(DrawingContext ctx)
    {
        ctx.DrawRectangle(ComponentDefaults.GateFillBrush,
            ComponentDefaults.GatePen,
            new Rect(0, 0, Width, Height));

        DrawTerminalsAndLabels(ctx, inputLabel: 'D', outputLabel: 'S');
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

    public override void AddTerminalPoints(bool notMode = false)
    {
        Point SnapToGrid(Point pt)
        {
            double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
            return new Point(snapX, snapY);
        }

        // Input lines (left)
        for (int i = 0; i < InputLineCount; i++)
        {
            var pos = new Point(-ComponentDefaults.TerminalWireLength,
                ComponentDefaults.TerminalSpacing * (i + 1));
            Terminals![i] = new Terminal(SnapToGrid(pos), null!);
        }

        // Output lines (right)
        for (int j = 0; j < SelectionLineCount; j++)
        {
            var pos = new Point(Width + ComponentDefaults.TerminalWireLength - 5,
                ComponentDefaults.TerminalSpacing * (j + 1));
            var snapped = SnapToGrid(pos);
            Terminals![InputLineCount + j] = new Terminal(new Point(pos.X, snapped.Y), null!);
        }
    }

    protected void DrawTerminalsAndLabels(DrawingContext ctx, char inputLabel, char outputLabel)
    {
        // Inputs (left side)
        for (int i = 0; i < InputLineCount; i++)
        {
            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![i].Position,
                new Point(0, Terminals[i].Position.Y));
            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = $"{inputLabel}{i}".CreateFormattedText();

            ctx.DrawText(text, new Point(4.5, Terminals[i].Position.Y - 6));
        }

        // Encoded outputs (right side)
        for (int j = 0; j < SelectionLineCount; j++)
        {
            int outIdx = InputLineCount + j;

            ctx.DrawLine(ComponentDefaults.WirePen, Terminals![outIdx].Position,
                new Point(Terminals[outIdx].Position.X - ComponentDefaults.TerminalWireLength + 5, Terminals[outIdx].Position.Y));

            ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
                Terminals[outIdx].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

            var text = $"{outputLabel}{SelectionLineCount - j - 1}".CreateFormattedText();

            ctx.DrawText(text, new Point(Width - 18, Terminals[outIdx].Position.Y - 6));
        }
    }
}
