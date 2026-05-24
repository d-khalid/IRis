// using System;
// using System.Collections.Generic;
// using System.Globalization;
// using System.Linq;
// using Avalonia;
// using Avalonia.Media;
// using IRis.Models.Core;
// using IRis.Models;
// using IRis.Services;

// namespace IRis.Models.Components;

// public class CustomComponent : Component, IOutputProvider
// {
//     protected string ComponentName;
//     public int InputCount;
//     protected int OutputCount;
//     protected List<CircuitFormulaConversionService.CircuitFormula> OutputFormulas;
    
//     public CustomComponent(string name, int inputCount = 2, int outputCount = 1, 
//         List<CircuitFormulaConversionService.CircuitFormula>? outputFormulas = null,
//         double width = ComponentDefaults.DefaultMuxWidth,
//         double height = ComponentDefaults.DefaultMuxHeight)
//         : base(width, height)
//     {
//         Console.WriteLine("Creating Custom Component");
//         ComponentName = name;
//         InputCount = inputCount;
//         OutputCount = outputCount;
//         OutputFormulas = outputFormulas ?? [];
        
//         // Calculate dimensions based on input/output count
//         int maxTerminals = Math.Max(InputCount, OutputCount);
//         // maxTerminals = (maxTerminals % 2 == 1) ? maxTerminals + 1 : maxTerminals;
//         Height = maxTerminals * ComponentDefaults.TerminalSpacing + ComponentDefaults.GridSpacing;
//         Height = (Height % 20 == 0 && OutputCount % 2 == 1) ? Height : Height + ComponentDefaults.GridSpacing;
//         Width = ComponentName.Length * 15;
//         Console.WriteLine("Maxterminals: " + maxTerminals);
//         Console.WriteLine("Height: " + Height + ", Width: " + Width + ", Name: " + ComponentName.Length);

//         // Total terminals = inputs + outputs
//         Terminals = new Terminal[InputCount + OutputCount];

//         AddTerminalPoints();

//         IsHitTestVisible = true;
//     }

//     public void ComputeOutput()
//     {
//         // Compute each output based on its formula
//         for (int outputIndex = 0; outputIndex < OutputCount; outputIndex++)
//         {
//             if (outputIndex < OutputFormulas.Count)
//             {
//                 // Get the formula for this output
//                 var formula = OutputFormulas[outputIndex];
                
//                 // Build input dictionary with actual values from terminals
//                 var inputs = GetInputValues();
                
//                 // Evaluate the formula using the service method
//                 bool result = CircuitFormulaConversionService.EvaluateFormula(formula.Formula, inputs);
                
//                 // Set the output terminal value
//                 int outputTerminalIndex = InputCount + outputIndex;
//                 if (Terminals![outputTerminalIndex].Wire != null)
//                 {
//                     Terminals[outputTerminalIndex].Wire!.Value = result ? LogicState.High : LogicState.Low;
//                 }
//             }
//             else
//             {
//                 // No formula available, default to Low
//                 int outputTerminalIndex = InputCount + outputIndex;
//                 if (Terminals![outputTerminalIndex].Wire != null)
//                 {
//                     Terminals[outputTerminalIndex].Wire!.Value = LogicState.Low;
//                 }
//             }
//         }
//     }

//     private Dictionary<string, bool> GetInputValues()
//     {
//         var inputs = new Dictionary<string, bool>();
        
//         // Get values from input terminals and map them to formula variables
//         for (int i = 0; i < InputCount; i++)
//         {
//             bool inputValue = false;
            
//             // Check if terminal has a wire and get its value
//             if (i < Terminals!.Length && Terminals[i].Wire != null)
//             {
//                 inputValue = Terminals[i].Wire!.Value == LogicState.High;
//             }
            
//             // Add both Input_X format (used by formula service) and A,B,C format
//             inputs[$"Input_{i + 1}"] = inputValue;
            
//             // Also support A, B, C... format for compatibility
//             char inputVar = (char)('A' + i);
//             inputs[inputVar.ToString()] = inputValue;
//         }
        
//         return inputs;
//     }

//     public override void Draw(DrawingContext ctx)
//     {
//         // Component Body
//         ctx.DrawRectangle(ComponentDefaults.GateFillBrush,
//             ComponentDefaults.GatePen, 
//             new Rect(0, 0, Width, Height));
        
//         DrawTerminalsAndLabels(ctx);
        
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
//         // Snap to multiples of Grid Spacing
//         Point SnapToGrid(Point pt)
//         {
//             double snapX = Math.Round(pt.X / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
//             double snapY = Math.Round(pt.Y / ComponentDefaults.GridSpacing) * ComponentDefaults.GridSpacing;
//             return new Point(snapX, snapY);
//         }

//         // Add input terminals on the left side
//         double inputSpacing = ComponentDefaults.TerminalSpacing; //Height / (InputCount + 1);
//         for (int i = 0; i < InputCount; i++)
//         {
//             Point pos = new Point(-ComponentDefaults.TerminalWireLength, inputSpacing * (i + 1));
//             Terminals![i] = new Terminal(SnapToGrid(pos), null!);
//         }

//         // Add output terminals on the right side
//         // double firstTerminalPos = OutputCount == 1 ?
//         //     Height / 2 : SnapSpaceToGrid(Height / 2) + ComponentDefaults.TerminalSpacing;
//         for (int i = 0; i < OutputCount; i++)
//         {
//             Point pos = new Point(Width + ComponentDefaults.TerminalWireLength, GetTerminalPointPosition(i, OutputCount, Height));
//             Terminals![InputCount + i] = new Terminal(SnapToGrid(pos), null!);
//         }
//     }

//     private double GetTerminalPointPosition(int indexNumber, int totalCount, double totalHeight)
//     {
//         // Compute the spacing between terminals
//         double spacing = totalHeight / (totalCount + 1);

//         // Position = spacing * (index + 1)
//         return spacing * (indexNumber + 1);
//     }


//     protected void DrawTerminalsAndLabels(DrawingContext ctx)
//     {
//         // Draw input terminals and labels
//         for (int i = 0; i < InputCount; i++)
//         {
//             // Draw connection line
//             ctx.DrawLine(ComponentDefaults.WirePen,
//                 Terminals![i].Position,
//                 new Point(0, Terminals[i].Position.Y));

//             // Draw terminal circle
//             ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
//                 Terminals[i].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

//             // Draw input label
//             char inputLabel = (char)('A' + i);
//             var text = new FormattedText(
//                 inputLabel.ToString(),
//                 CultureInfo.CurrentCulture,
//                 FlowDirection.LeftToRight,
//                 ComponentDefaults.LabelTypeface,
//                 ComponentDefaults.LabelSize,
//                 ComponentDefaults.LabelBrush
//             );
//             ctx.DrawText(text, new Point(4.5, Terminals[i].Position.Y - 6));
//         }

//         // Draw output terminals and labels
//         for (int i = 0; i < OutputCount; i++)
//         {
//             int terminalIndex = InputCount + i;

//             // Draw connection line
//             ctx.DrawLine(ComponentDefaults.WirePen,
//                 Terminals![terminalIndex].Position,
//                 new Point(Width, Terminals[terminalIndex].Position.Y));

//             // Draw terminal circle
//             ctx.DrawEllipse(ComponentDefaults.TerminalBrush, null,
//                 Terminals[terminalIndex].Position, ComponentDefaults.TerminalRadius, ComponentDefaults.TerminalRadius);

//             // Draw output label (Y for single output, Y0, Y1, etc. for multiple)
//             string outputLabel = OutputCount == 1 ? "Y" : $"Y{i}";
//             var text = new FormattedText(
//                 outputLabel,
//                 CultureInfo.CurrentCulture,
//                 FlowDirection.LeftToRight,
//                 ComponentDefaults.LabelTypeface,
//                 ComponentDefaults.LabelSize,
//                 ComponentDefaults.LabelBrush
//             );
//             ctx.DrawText(text, new Point(Width - 20, Terminals[terminalIndex].Position.Y - 6));
//         }

//         // Draw formula text in the center of the component (simplified for display)
//         if (OutputFormulas.Count > 0)
//         {
//             string displayText = ComponentName;

//             var formulaText = new FormattedText(
//                 displayText,
//                 CultureInfo.CurrentCulture,
//                 FlowDirection.LeftToRight,
//                 ComponentDefaults.LabelTypeface,
//                 ComponentDefaults.LabelSize,
//                 ComponentDefaults.LabelBrush
//             );

//             double textX = (Width - formulaText.Width) / 2;
//             double textY = (Height - formulaText.Height) / 2;
//             ctx.DrawText(formulaText, new Point(textX, textY));
//         }
//     }
// }