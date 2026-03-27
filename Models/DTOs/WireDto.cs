using Avalonia;
using IRis.Models.Components;
using IRis.Models.Core;
using System;
using System.Collections.Generic;

namespace IRis.Models.DTOs;

public class WireDto
{
    public Guid Id { get; set; }

    public LogicState? Value { get; set; }

    public List<Point> Points { get; set; }

    public static WireDto ToDto(Wire w)
    {
        return new WireDto()
        {
            Id = w.Id,

            Value = w.Value,
            Points = w.Points
        };
    }

    public static Wire ToWire(WireDto dto)
    {
        return new Wire()
        {
            Id = dto.Id,
            Value = dto.Value,
            Points = dto.Points,

            IsCommitted = true,
            IsBeingEdited = false,
            IsValid = true,
        };
    }
}