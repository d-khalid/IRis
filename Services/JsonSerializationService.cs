// default libs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

// our modules
using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;


namespace IRis.Services;


public static class JsonSerializationService
{
    
    // TODO: These are basically direct calls to Newtonsoft.Json after XAML drawing
    public static void SaveToFile(Simulation simulation, string saveFilePath)
    {
        // Convert the circuit to DTOs
        // CircuitDto circuit = new()
        // {
        //     Components = simulation.Components
        //         .Select(p => ComponentDto.ToDto(p))
        //         .ToList(),
        //     
        //     // Wires = simulation.Components
        //     //     .Select(p => WireDto.ToDto(p))
        //     //     .ToList()
        //     
        // };
        //
        // // TODO: This file write might need to be async later on
        // string json = JsonConvert.SerializeObject(circuit, Formatting.Indented);
        // File.WriteAllText(saveFilePath, json);
    }

    public static List<Component> DeserializeComponentsAsync(string jsonContent)
    {
        // try
        // {
        //     CircuitDto? circuit = JsonConvert.DeserializeObject<CircuitDto>(jsonContent);
        //     
        //     if (circuit == null) throw new Exception();
        //
        //     return CircuitDto.ToCircuit(circuit);
        //
        // }
        // catch (Exception)
        // {
        //     Console.WriteLine("COULD NOT DESERIALIZE THE GIVEN JSON\n Also make this a popup window later");
        // }
        //
        // return [];
        return [];
    }

   
  
}