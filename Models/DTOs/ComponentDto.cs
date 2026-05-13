// using Avalonia;
// using Avalonia.Controls;
// using IRis.Models.Components;
// using IRis.Models.Core;

// namespace IRis.Models.DTOs;

// public class ComponentDto
// {
//     public string Type { get; set; } = string.Empty;
//     public double X { get; set; }
//     public double Y { get; set; }
//     public bool IsSelected { get; set; }
//     public int Orientation { get; set; }
//     public int InputCount { get; set; }
//     public LogicState? State { get; set; }

//     public static ComponentDto ToDto(Component component)
//     {
//         return new ComponentDto
//         {
//             Type = CircuitComponentFactory.GetComponentTypeName(component),
//             X = Canvas.GetLeft(component),
//             Y = Canvas.GetTop(component),
//             IsSelected = component.IsSelected,
//             Orientation = (int)component.Orientation,
//             InputCount = CircuitComponentFactory.GetInputCount(component),
//             State = component switch
//             {
//                 LogicToggle toggle => toggle.State,
//                 LogicProbe probe => probe.State,
//                 _ => null,
//             },
//         };
//     }

//     public static Component? ToComponent(ComponentDto dto)
//     {
//         Component? component = CircuitComponentFactory.Create(dto.Type);
//         if (component == null)
//             return null;

//         component.Orientation = (ComponentOrientation)dto.Orientation;
//         component.IsSelected = dto.IsSelected;

//         if (component is LogicToggle toggle && dto.State.HasValue)
//             toggle.State = dto.State.Value;
//         else if (component is LogicProbe probe && dto.State.HasValue)
//             probe.State = dto.State.Value;

//         Canvas.SetLeft(component, dto.X);
//         Canvas.SetTop(component, dto.Y);
//         return component;
//     }
// }
