using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas;
using System.IO;
using System;
using Newtonsoft.Json;
using Avalonia.Styling;
using Avalonia.Threading;


namespace IRis.Services.Singleton;


public partial class AppState : SingletonBase<AppState>
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IRis", "AppState.json"
    );

    public static readonly string AutoSavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IRis", "autosave.iris"
    );

    public static bool FileNeedsSaving { get; set; } = false;
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    private bool _designTabActive = true;

    [ObservableProperty]
    private double _panSensistivity = 4;

    [ObservableProperty]
    private bool _terminalColorChangeAllowed = true;

    [ObservableProperty]
    private ThemeVariant _theme = ThemeVariant.Dark;

    [ObservableProperty]
    private string _currentFilePath = "(unsaved)";

    [ObservableProperty]
    [property: JsonIgnore]
    private bool _editingAllowed = true;

    [ObservableProperty]
    [property: JsonIgnore]
    private Point _mousePosition = new(0, 0);

    [ObservableProperty]
    [property: JsonIgnore]
    private string _lastCommand = "(no action yet)";


    public AppState()
    {
        Load();
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(MousePosition) or nameof(EditingAllowed))
            {
                return;
            }
            else if (e.PropertyName is nameof(LastCommand))
            {
                FileNeedsSaving = true;
                return;
            }

            Save();
        };

        _timer = new() { Interval = TimeSpan.FromMinutes(1) };
        _timer.Tick += (_, _) =>
        {
            AutoSaveCurrentSession();
        };
        _timer.Start();
    }


    partial void OnEditingAllowedChanged(bool value)
    {
        if (value is false && Simulation.Get().Running)
        {
            Selection.Get().UnHighlightAll();
            Preview.Get().Drop();
            WirePreview.Get().Nuke();
        }
    }


    private async void AutoSaveCurrentSession()
    {
        string path = CurrentFilePath;
        if (path == "(unsaved)")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(AutoSavePath)!);
            path = AutoSavePath;
        }

        try
        {
            var circuit = Simulation.Get().Objects;
            await File.WriteAllTextAsync(
                path, SerializationService.Serialize(circuit)
            );

            FileNeedsSaving = false;
        }
        catch (IOException)
        {
            Console.WriteLine("AutoSaveCurrentSession(): failed to write to file.");
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
