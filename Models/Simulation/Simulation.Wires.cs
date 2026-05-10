// // Models/Simulation.Wires.cs
// using System;
// using System.Collections.Generic;
// using Avalonia;
// using Avalonia.Controls;
// using CommunityToolkit.Mvvm.ComponentModel;
// using IRis.Models.Core;

// namespace IRis.Models;

// // Simplified wire helpers: after refactor wires/terminals are unsupported.
// public partial class Simulation : ObservableObject
// {
//     public object? FindClosestSnapTerminal(Point p) => null;

//     public Point GetAbsoluteTerminalPosition(object terminal) => new Point(-2, -2);

//     public bool IsInputTerminal(object terminal) => false;

//     public object? FindWireAtPosition(Point position) => null;

//     private bool IsPointInsideAnyComponent(Point point)
//     {
//         foreach (var component in _components)
//         {
//             var pos = new Point(Canvas.GetLeft(component), Canvas.GetTop(component));
//             var bounds = new Rect(pos, new Size(component.Size.Width, component.Size.Height));
//             if (bounds.Contains(point)) return true;
//         }
//         return false;
//     }

//     public bool IsWireInsideAnyComponent(List<Point> points) => false;

//     public bool DoesWireOverlapAnotherWire(List<Point> points) => false;

//     public bool IsWireSupersetOfAnotherWire(List<Point> wirePoints) => false;

//     public bool DoesWireSelfOverlap(List<Point> points) => false;

//     public bool DoesWireCrossTerminal(List<Point> points, object? exceptionCase = null) => false;

//     public static bool DoesWireHaveExtension(object wire) => false;
// }