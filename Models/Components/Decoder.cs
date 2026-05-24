// using System;
// using System.Globalization;
// using Avalonia;
// using Avalonia.Media;
// using IRis.Models.Core;

// namespace IRis.Models.Components;

// public class Decoder : Component, IOutputProvider
// {
  
//     public Decoder(int selectionLineCount,
//         double width = ComponentDefaults.DefaultMuxWidth,
//         double height = ComponentDefaults.DefaultMuxHeight)
//         : base(width, height)
//     {
//         SelectionLineCount = selectionLineCount;
//         OutputLineCount = (int)Math.Pow(2, SelectionLineCount);

//         Width  = (SelectionLineCount + 1) * ComponentDefaults.TerminalSpacing;
//         Height = (OutputLineCount + 1)    * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;

//         // Selection inputs + data outputs
//         Terminals = new Terminal[SelectionLineCount + OutputLineCount];

//         AddTerminalPoints();

//         IsHitTestVisible = true;
//     }

//     public void ComputeOutput()
//     {
//         int selectedIndex = 0;

//         // Decode binary selection inputs (MSB at S{n-1}, like your mux)
//         for (int i = 0; i < SelectionLineCount; i++)
//         {
//             if (Terminals![i].Wire!.Value == LogicState.High)
//             {
//                 selectedIndex += (int)Math.Pow(2, SelectionLineCount - i - 1);
//             }
//         }

//         // Default: all outputs Low
//         for (int k = 0; k < OutputLineCount; k++)
//         {
//             Terminals![SelectionLineCount + k].Wire!.Value = LogicState.Low;
//         }

//         // Set selected output High
//         Terminals![SelectionLineCount + selectedIndex].Wire!.Value = LogicState.High;
//     }

//     public override void Draw(DrawingContext ctx)
//     {
//         ctx.DrawRectangle(ComponentDefaults.GateFillBrush,
//             ComponentDefaults.GatePen,
//             new Rect(0, 0, Width, Height));

//         DrawTerminalsAndLabels(ctx, selectionLabel: 'S', outputLabel: 'Y');
//         base.Draw(ctx);
//     }

//     public override void DrawSelection(DrawingContext ctx)
//     {
//         double expandX = ComponentDefaults.TerminalWireLength + ComponentDefaults.TerminalRadius;
//         double expandY = ComponentDefaults.TerminalRadius;

//         ctx.DrawRectangle(
//             ComponentDefaults.SelectionBrush,
//             ComponentDefaults.SelectionPen,
//             new Rect(
//                 -expandX,
//                 -expandY,
//                 Bounds.Width + 2 * expandX,
//                 Bounds.Height + 2 * expandY)
//         );
//     }

//     public override void AddTerminalPoints(bool notMode = false)
//     {
//         Point SnapToGrid(Point pt)
//         {
//             double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
//             double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
//             return new Point(snapX, snapY);
//         }

//         // Selection inputs (left, vertically stacked)
//         for (int i = 0; i < SelectionLineCount; i++)
//         {
//             var pos = new Point(-ComponentDefaults.TerminalWireLength,
//                 ComponentDefaults.TerminalSpacing * (i + 1));
//             Terminals![i] = new Terminal(SnapToGrid(pos), null!);
//         }

//         // Outputs (right, vertically stacked)
//         for (int k = 0; k < OutputLineCount; k++)
//         {
//             var pos = new Point(Width + ComponentDefaults.TerminalWireLength - 5,
//                 ComponentDefaults.TerminalSpacing * (k + 1));
//             var snapped = SnapToGrid(pos);
//             Terminals![SelectionLineCount + k] = new Terminal(new Point(pos.X, snapped.Y), null!);
//         }
//     }

//     protected void DrawTerminalsAndLabels(DrawingContext ctx, char selectionLabel, char outputLabel)
//     {
//         // Selection inputs
//         for (int i = 0; i < SelectionLineCount; i++)
//         {
//             ctx.DrawLine(ComponentDefaults.WirePen, Terminals![i].Position,
//                 new Point(0, Terminals[i].Position.Y));
//             ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
//                 Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

//             var text = new FormattedText(
//                 $"{selectionLabel}{SelectionLineCount - i - 1}", // MSB first
//                 CultureInfo.CurrentCulture,
//                 FlowDirection.LeftToRight,
//                 ComponentDefaults.LabelTypeface,
//                 ComponentDefaults.LabelSize,
//                 ComponentDefaults.LabelBrush
//             );
//             ctx.DrawText(text, new Point(4.5, Terminals[i].Position.Y - 6));
//         }

//         // Outputs (right side)
//         for (int k = 0; k < OutputLineCount; k++)
//         {
//             int outIdx = SelectionLineCount + k;

//             ctx.DrawLine(ComponentDefaults.WirePen, Terminals![outIdx].Position,
//                 new Point(Terminals[outIdx].Position.X - ComponentDefaults.TerminalWireLength + 5, Terminals[outIdx].Position.Y));
//             ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
//                 Terminals[outIdx].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

//             var text = new FormattedText(
//                 $"{outputLabel}{k}",
//                 CultureInfo.CurrentCulture,
//                 FlowDirection.LeftToRight,
//                 ComponentDefaults.LabelTypeface,
//                 ComponentDefaults.LabelSize,
//                 ComponentDefaults.LabelBrush
//             );
//             ctx.DrawText(text, new Point(Width - 18, Terminals[outIdx].Position.Y - 6));
//         }
//     }
// }
