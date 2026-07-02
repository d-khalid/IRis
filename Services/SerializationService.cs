using System;
using System.Linq;
using System.Reflection;
using Avalonia.Collections;
using IRis.ViewModels.Main.Canvas;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace IRis.Services;

public class SerializationService
{
    public JsonSerializerSettings Settings() =>
        new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.Indented,
            SerializationBinder = new TypeNameBinder(),
            Converters = { new StringEnumConverter() },
        };

    public string Serialize<T>(T source)
    {
        if (source is null)
            return "";
        return JsonConvert.SerializeObject(source, Settings());
    }

    public AvaloniaList<CircuitObjectViewModel>? Deserialize(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<AvaloniaList<CircuitObjectViewModel>>(
                json,
                Settings()
            );
        }
        catch (JsonException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    private class TypeNameBinder : DefaultSerializationBinder
    {
        private static readonly Assembly _asm = typeof(TypeNameBinder).Assembly;

        public override void BindToName(
            Type serializedType,
            out string? assemblyName,
            out string? typeName
        )
        {
            if (serializedType.Assembly == _asm && !serializedType.IsGenericType)
            {
                assemblyName = null;
                typeName = serializedType.Name[..^9];
                return;
            }

            base.BindToName(serializedType, out assemblyName, out typeName);
        }

        public override Type BindToType(string? assemblyName, string typeName)
        {
            if (assemblyName is null)
            {
                var type = _asm.GetTypes().FirstOrDefault(t => t.Name == typeName + "ViewModel");
                if (type != null)
                    return type;
            }

            return base.BindToType(assemblyName, typeName);
        }
    }
}
