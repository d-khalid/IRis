// using System;
// using System.Collections.Generic;
// using Avalonia;
// using Avalonia.Controls;
// using Avalonia.Input;
// using Avalonia.Threading;
// using CommunityToolkit.Mvvm.ComponentModel;

// using IRis.Models.Components;
// using IRis.Models.Core;
// using IRis.Models.Commands;
// using IRis.Views;


// namespace IRis.Models;


// public partial class Simulation
// {
//     private DispatcherTimer GetDispatcherTimer()
//     {
//         DispatcherTimer timer = new()
//         { 
//             Interval = TimeSpan.FromMilliseconds(100)
//         };

//         timer.Tick += (s, e) => 
//         {
//             foreach (Component c in Components)
//                 if (c is IOutputProvider op)
//                     op.ComputeOutput();

//             foreach (Wire w in Wires)
//                 w.ComputeOutput();
//         };

//         return timer;
//     }


//     private void DrawGridOnCanvas(Canvas canvas)
//     {
//         for (double x = 0; x < canvas.MinWidth; x += Constants.GridSpacing)
//         {
//             Utils.DrawLineOnCanvas(
//                 canvas: _canvas, 
//                 p1: new Point(x, 0), 
//                 p2: new Point(x, canvas.MinHeight)
//             );
//         }

//         for (double y = 0; y < canvas.MinWidth; y += Constants.GridSpacing)
//         {
//             Utils.DrawLineOnCanvas(
//                 canvas: _canvas, 
//                 p1: new Point(0, y), 
//                 p2: new Point(canvas.MinWidth, y)
//             );
//         }
//     }
// }

