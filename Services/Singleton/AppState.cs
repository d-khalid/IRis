using System;
using System.IO;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace IRis.Services.Singleton;

public partial class AppState : ObservableObject
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IRis",
        "AppState.json"
    );

    public static readonly string AutoSavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IRis",
        "autosave.iris"
    );

    public static bool FileNeedsSaving { get; set; } = false;
    private readonly DispatcherTimer _timer;
    private readonly Simulation _simulation;
    private readonly Selection _selection;
    private readonly Preview _preview;
    private readonly WirePreview _wirePreview;

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

    public AppState(
        Simulation simulation,
        Selection selection,
        Preview preview,
        WirePreview wirePreview
    )
    {
        _simulation = simulation;
        _selection = selection;
        _preview = preview;
        _wirePreview = wirePreview;

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
        if (value is false && _simulation.Running)
        {
            _selection.UnHighlightAll();
            _preview.Drop();
            _wirePreview.Nuke();
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
            var circuit = _simulation.Objects;
            await File.WriteAllTextAsync(path, SerializationService.Serialize(circuit));

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

        File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    private void Load()
    {
        if (File.Exists(SettingsPath))
        {
            JsonConvert.PopulateObject(File.ReadAllText(SettingsPath), this);
        }
    }
}
