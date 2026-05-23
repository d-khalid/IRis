using IRis.ViewModels.Circuit;
using IRis.Models.Circuit.CircuitObjects.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Avalonia;
using IRis.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;


namespace IRis.Models.Core;


public partial class ClipboardManager : ObservableObject
{
    private static ClipboardManager? _instance = null;
    private static readonly Simulation _simulation = (Simulation)Simulation.GetInstance();
    public ObservableCollection<CircuitObjectViewModel> Objects { get; } = [];


    public ClipboardManager()
    {
        if (_instance != null)
            throw new Exception("use GetInstance function instead pls.");
    }


    public static ClipboardManager GetInstance()
    {
        if (_instance == null)
            _instance = new ClipboardManager();

        return _instance;
    }


    public void Copy(ObservableCollection<CircuitObjectViewModel> collection)
    {
        if (collection is null) return;
        foreach (var co in collection)
            Objects.Add(CloningService.Clone(co));

        if (Application.Current?.ApplicationLifetime is not 
            IClassicDesktopStyleApplicationLifetime app || 
            app.MainWindow?.Clipboard is not IClipboard ic) return;

        string json = SerializationService.Serialize(collection);
        ic.SetTextAsync(json);
    }


    public async void Paste(ObservableCollection<CircuitObjectViewModel> collection)
    {
        if (Objects.Count > 0)   // if we have the objects in this instance
        {
            SimulationService.SnapCollectionToPosition(
                Objects, _simulation.CurrentMousePos, new Point(0, 0)
            );

            foreach (var co in Objects)
            {
                co.Opacity = 0.5;
                collection.Add(co);
            }

            Objects.Clear();
        }

        else                     // get from clipboard
        {
            if (Application.Current?.ApplicationLifetime is not 
                IClassicDesktopStyleApplicationLifetime app || 
                app.MainWindow?.Clipboard is not IClipboard ic) return;

            var json = await ic.GetTextAsync();
            if (json is null) return;

            var pasted = SerializationService.Deserialize(json);
            if (pasted is null) return;

            SimulationService.SnapCollectionToPosition(
                pasted, _simulation.CurrentMousePos, new Point(0, 0)
            );

            foreach (var co in pasted)
            {
                co.Opacity = 0.5;
                collection.Add(co);
            }
        }
    }
}
