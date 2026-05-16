using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using IRis.Models;
using System.Collections.ObjectModel;
using IRis.ViewModels.Circuit;


namespace IRis.Services;


public static class SerializationService
{
    public static void SaveToFile(ObservableCollection<CircuitObjectViewModel> collection, string saveFilePath)
    {
        string json = JsonConvert.SerializeObject(collection, Formatting.Indented);
        File.WriteAllText(saveFilePath, json);
    }
}