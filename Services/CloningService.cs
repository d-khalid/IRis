using Newtonsoft.Json;


namespace IRis.Services;


public static class CloningService
{
    public static T Clone<T>(T source)
    {
        JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        return JsonConvert.DeserializeObject<T>(
            JsonConvert.SerializeObject(source, settings), settings
        )!;
    }
}
