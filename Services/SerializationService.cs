using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using IRis.Models;
using System.Collections.ObjectModel;
using IRis.ViewModels.Main.Canvas;


namespace IRis.Services;


public static class SerializationService
{
    public static JsonSerializerSettings Settings() => new()
    {
        TypeNameHandling = TypeNameHandling.All,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        Formatting = Formatting.Indented
    };

    public static string Serialize(ObservableCollection<CircuitObjectViewModel> collection)
    {
        if (collection is null) return "";

        return JsonConvert.SerializeObject(collection, Settings());
    }


    public static ObservableCollection<CircuitObjectViewModel>? Deserialize(string json)
    {
        return (ObservableCollection<CircuitObjectViewModel>?)
            JsonConvert.DeserializeObject(json, Settings());
    }
}
