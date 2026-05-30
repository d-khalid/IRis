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


    public static string Serialize(AvaloniaList<CircuitObjectViewModel> collection)
    {
        if (collection is null) return "";
        return JsonConvert.SerializeObject(collection, Settings());
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
