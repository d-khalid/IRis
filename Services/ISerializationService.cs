using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Services;


// Will be implemented by a JSON and XML serialization service
public interface ISerializationService
{
    public void SerializeComponents(Simulation simulation, string? filePath);
    public List<Component> DeserializeComponentsAsync(string content);
    async Task<List<Component>> DeserializeFromFileAsync(string filePath)
    {
        string data = await File.ReadAllTextAsync(filePath);
        return DeserializeComponentsAsync(data);
    }
    

}