using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas;
using System.IO;
using System;
using Newtonsoft.Json;
using Avalonia.Styling;


namespace IRis.Services.Singleton;


public partial class AppState : SingletonBase<AppState>
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IRis", "settings.json"
    );

    [ObservableProperty]
    private bool _terminalColorChangeAllowed = true;

    [ObservableProperty]
    private ThemeVariant _theme = ThemeVariant.Dark;

    [ObservableProperty] [property: JsonIgnore] 
    private Point _mousePosition = new(0, 0);

    [ObservableProperty] [property: JsonIgnore]
    private string _currentFilePath = "(unsaved)";

    [ObservableProperty] [property: JsonIgnore]
    private string _lastCommand = "(no action yet)";


    public AppState()
    {
        Load();
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(MousePosition) or nameof(LastCommand)
                or nameof(CurrentFilePath))
                return;

            Save();
        };
    }


    partial void OnTerminalColorChangeAllowedChanged(bool value)
    {
        if (Simulation.Get().Running)
        {
            Simulation.Get().Stop();
            Simulation.Get().Start();
        }
    }


    private void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);

        File.WriteAllText(
            SettingsPath, JsonConvert.SerializeObject(this, Formatting.Indented)
        );
    }


    private void Load()
    {
        if (File.Exists(SettingsPath))
        {
            JsonConvert.PopulateObject(
                File.ReadAllText(SettingsPath), this
            );
        }
    }
}
