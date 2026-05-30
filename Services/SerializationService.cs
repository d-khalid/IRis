using Newtonsoft.Json;
using IRis.ViewModels.Main.Canvas;
using Avalonia.Collections;


namespace IRis.Services;


public static class SerializationService
{
    public static JsonSerializerSettings Settings() => new()
    {
        TypeNameHandling = TypeNameHandling.All,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        Formatting = Formatting.Indented
    };


    public static string Serialize<T>(T source)
    {
        if (source is null) return "";
        return JsonConvert.SerializeObject(source, Settings());
    }


    public static AvaloniaList<CircuitObjectViewModel>? Deserialize(string json)
    {
        try
        {
            return (AvaloniaList<CircuitObjectViewModel>?)
                JsonConvert.DeserializeObject(json, Settings());
        }

        catch (JsonException)
        {
            return null;
        }
    }
}
