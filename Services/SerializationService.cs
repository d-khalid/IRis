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
    public static string Serialize(ObservableCollection<CircuitObjectViewModel> collection)
    {
        if (collection is null) return "";

        return JsonConvert.SerializeObject(collection, Formatting.Indented);
    }


    public static ObservableCollection<CircuitObjectViewModel>? Deserialize(string json)
    {
        return (ObservableCollection<CircuitObjectViewModel>?)
            JsonConvert.DeserializeObject(json);
    }
}
