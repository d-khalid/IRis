// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Avalonia;
// using IRis.Models.Components;
// using IRis.Models.Core;

// namespace IRis.Models.DTOs;

// public class WireDto
// {
//     public Guid Id { get; set; }

//     public LogicState State { get; set; }

//     public List<Point> Points { get; set; } = new List<Point>();
    
//     public static WireDto ToDto(Wire w)
//     {
//         return new WireDto()
//         {
//             Id = w.Id,
//             State = w.State,
//             Points = w.Points.ToList()
//         };
//     }

//     public static Wire ToWire(WireDto dto)
//     {
//         Wire wire = new();

//         foreach (var point in dto.Points)
//             wire.AddPoint(point);

//         wire.State = dto.State;
//         wire.IsBeingEdited = false;
//         wire.IsValid = true;
//         return wire;
//     }
// }