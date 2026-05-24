using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models.DTOs;

public class CircuitDto
{
    public List<ComponentDto> Components { get; set; } = new List<ComponentDto>();

    public static List<Component> ToCircuit(CircuitDto circuit)
    {
        return circuit.Components
            .Select(ComponentDto.ToComponent)
            .Where(component => component != null)
            .Cast<Component>()
            .ToList();
    }
}