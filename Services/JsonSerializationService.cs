using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace IRis.Services;

public class JsonSerializationService : ISerializationService
{
    public void SerializeComponents(Simulation simulation, string? filePath)
    {
        if (filePath == null)
        {
            Console.WriteLine("No file selected!");
            return;
        }

        // Convert the circuit to DTOs
        CircuitDto circuit = new CircuitDto()
        {
            Components = simulation.Components
                .OfType<CircuitComponent>()
                .Select(ComponentDto.ToDto)
                .ToList(),

            Wires = simulation.Components
                .OfType<Wire>()
                .Select(WireDto.ToDto)
                .ToList()

        };

        // TODO: This file write might need to be async later on
        string json = JsonConvert.SerializeObject(circuit, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public List<Component> DeserializeComponentsAsync(string jsonContent)
    {
        try
        {
            CircuitDto? circuit = JsonConvert.DeserializeObject<CircuitDto>(jsonContent);

            if (circuit == null) throw new SerializationException();

            return CircuitDto.ToCircuit(circuit);

        }
        catch (Exception e)
        {
            Console.WriteLine("COULD NOT DESERIALIZE THE GIVEN JSON\n Also make this a popup window later");
        }

        return new List<Component>();
    }



}