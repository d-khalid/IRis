using Avalonia.Collections;
using IRis.Models;
using IRis.ViewModels.Main.Canvas;
using Newtonsoft.Json;


namespace IRis.Services;


public static class CloningService
{
    public static T Clone<T>(T source)
    {
        return (T)JsonConvert.DeserializeObject(
            JsonConvert.SerializeObject(source, SerializationService.Settings()), 
            SerializationService.Settings()
        )!;
    }
}
