using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IRis.Models;
using IRis.Models.Core;

namespace IRis.Services;

public class XmlSerializationService : ISerializationService
{
    public void SerializeComponents(Simulation simulation, string? filePath)
    {
        if (filePath == null)
        {
            Console.WriteLine("No file selected!");
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(CircuitDto));
        List<ComponentDto> dtoList = simulation.Components.Select(p => p.ToDto()).ToList();
        CircuitDto circuit = new CircuitDto() { Components = dtoList };
        

        StreamWriter writer = new StreamWriter(filePath);
        serializer.Serialize(writer, circuit);

        writer.Close();
    }

    public async Task<List<Component>> DeserializeComponentsAsync(string filePath)
    {
        var serializer = new XmlSerializer(typeof(CircuitDto));

        // TODO: THIS IS ERROR PRONE, CLEANLY EXCEPTION HANDLE THIS PART
        await using (var stream = File.OpenRead(filePath))
        {
            CircuitDto dto = (CircuitDto)serializer.Deserialize(stream);
            return dto.Components.Select(p => ISerializationService.ConvertDtoToComponent(p)).ToList();
        }
    }
}