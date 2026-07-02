using Newtonsoft.Json;

namespace IRis.Services;

public class CloningService
{
    public JsonSerializerSettings Settings() =>
        new()
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.Indented,
        };

    public T Clone<T>(T source)
    {
        return (T)
            JsonConvert.DeserializeObject(
                JsonConvert.SerializeObject(source, Settings()),
                Settings()
            )!;
    }
}
