using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using IRis.Models.Core;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.Views;

namespace IRis.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private AppState _appState;

    private readonly Simulation _simulation;
    private readonly Selection _selection;
    private readonly Preview _preview;
    private readonly WirePreview _wirePreview;
    private readonly ClipboardService _clipboardService;
    private readonly DragService _dragService;
    private readonly HoverEffectService _hoverEffectService;
    private readonly SerializationService _serialization;
    private readonly SimulationService _simulationService;

    public MainWindowViewModel(
        AppState appState,
        Simulation simulation,
        Selection selection,
        Preview preview,
        WirePreview wirePreview,
        ClipboardService clipboardService,
        DragService dragService,
        HoverEffectService hoverEffectService,
        SerializationService serialization,
        SimulationService simulationService
    )
    {
        AppState = appState;
        _simulation = simulation;
        _selection = selection;
        _preview = preview;
        _wirePreview = wirePreview;
        _clipboardService = clipboardService;
        _dragService = dragService;
        _hoverEffectService = hoverEffectService;
        _serialization = serialization;
        _simulationService = simulationService;
    }

    private async Task<bool> AskNukeChangesAsync()
    {
        var dialog = new ContentDialog
        {
            Title = "Unsaved Changes",
            Content = "You seem to have unsaved changes in this file. Should I save them?",
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Don't Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
        };

        var result = await dialog.ShowAsync();

        if (result is ContentDialogResult.Primary)
        {
            await SaveAsync();
            return true;
        }
        else if (result is ContentDialogResult.Secondary)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [RelayCommand]
    private async Task New()
    {
        if (!await AskNukeChangesAsync())
            return;
        AppState.CurrentFilePath = "(unsaved)";

        if (_simulation.Running)
            _simulation.Running = false;

        _selection.UnHighlightAll();
        _simulation.Nuke();
        _preview.Nuke();
        _wirePreview.Nuke();

        CommandService.Reset();
    }

    [RelayCommand]
    private async Task OpenAsync(string param)
    {
        if (param == "open" && !await AskNukeChangesAsync())
            return;
        if (_simulation.Running)
            _simulation.Running = false;

        var files = await App.ApplicationLifetime.MainWindow!.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Open Simulation",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("IRis Simulatable") { Patterns = ["*.iris"] },
                ],
            }
        );

        if (files.Count > 0)
        {
            string path = files[0].Path.LocalPath;
            var json = await File.ReadAllTextAsync(path);
            var collection = _serialization.Deserialize(json);

            if (collection is not null)
            {
                if (param != "merge")
                {
                    AppState.CurrentFilePath = path;
                    _simulation.Nuke();
                }

                _simulationService.RedrawEmptyWires(collection);
                _selection.Highlight(collection);
                _simulation.Add(collection);
            }
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_simulation.Running)
            _simulation.Running = false;
        if (AppState.CurrentFilePath == "(unsaved)")
        {
            await SaveAsAsync();
            return;
        }

        var json = _serialization.Serialize(_simulation.Objects);
        await File.WriteAllTextAsync(AppState.CurrentFilePath, json);

        AppState.FileNeedsSaving = false;
    }

    [RelayCommand]
    private async Task SaveAsAsync()
    {
        var file = await App.ApplicationLifetime.MainWindow!.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Save Simulation",
                DefaultExtension = "iris",
                FileTypeChoices =
                [
                    new FilePickerFileType("IRis Simulatable") { Patterns = ["*.iris"] },
                ],
            }
        );

        if (file is not null)
        {
            AppState.CurrentFilePath = file.Path.LocalPath;
            await SaveAsync();
        }
    }

    [RelayCommand]
    private static void Preferences() { }

    [RelayCommand]
    private static void Exit()
    {
        App.ApplicationLifetime.Shutdown();
    }

    [RelayCommand]
    private static void Undo() => CommandService.Undo();

    [RelayCommand]
    private static void Redo() => CommandService.Redo();

    [RelayCommand]
    private void Delete()
    {
        _simulation.Remove(_selection.Objects);
        _selection.UnHighlightAll();
    }

    [RelayCommand]
    private void Escape()
    {
        if (!_preview.IsEmpty())
            _preview.Nuke();

        if (!_wirePreview.IsEmpty())
            _wirePreview.Leave();
    }

    [RelayCommand]
    private static void UndoKey() => CommandService.Undo();

    [RelayCommand]
    private static void RedoKey() => CommandService.Redo();

    [RelayCommand]
    private void Copy()
    {
        if (!_preview.IsEmpty())
        {
            _clipboardService.Copy(_preview.Objects);
        }
        else if (!_selection.IsEmpty())
        {
            _clipboardService.Copy(_selection.Objects);
        }
        else if (_dragService.IsRunning())
        {
            _clipboardService.Copy(_dragService.Objects);
        }
    }

    [RelayCommand]
    private void Cut()
    {
        if (!_preview.IsEmpty())
        {
            _clipboardService.Copy(_preview.Objects);
            _preview.Nuke();
        }
        else if (!_selection.IsEmpty())
        {
            _clipboardService.Copy(_selection.Objects);
            _simulation.Remove(_selection.Objects);
            _selection.UnHighlightAll();
        }
        else if (_dragService.IsRunning())
        {
            _clipboardService.Copy(_dragService.Objects);
            _simulation.Remove(_dragService.Objects);
            _dragService.Stop();
        }
    }

    [RelayCommand]
    private async Task Paste()
    {
        _selection.UnHighlightAll();
        _preview.Nuke();
        _wirePreview.Nuke();
        _dragService.Stop();

        var pasted = await _clipboardService.Paste();
        _preview.Pick(pasted);
        _preview.UpdatePositionTo(AppState.MousePosition);
    }

    [RelayCommand]
    private void Rotate()
    {
        if (!_preview.IsEmpty())
            RotateCollection(_preview.Objects);
        else if (!_selection.IsEmpty())
            RotateCollection(_selection.Objects);
        else if (_dragService.IsRunning())
            RotateCollection(_dragService.Objects);
    }

    private void RotateCollection(AvaloniaList<CircuitObjectViewModel> collection)
    {
        Point min = _simulationService.GetMinPointInCollection(collection);
        Point max = _simulationService.GetMaxPointInCollection(collection);
        Point center = _simulationService.Average(min, max);

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                c.Rotation = (c.Rotation + 90) % 360;

                // this MATH bellow was done by Gemini 3.1 Pro, it works

                double objCenterX = c.X + (c.Width / 2.0);
                double objCenterY = c.Y + (c.Height / 2.0);

                double translatedX = objCenterX - center.X;
                double translatedY = objCenterY - center.Y;

                double newCenterX = -translatedY + center.X;
                double newCenterY = translatedX + center.Y;

                c.X = newCenterX - (c.Width / 2.0);
                c.Y = newCenterY - (c.Height / 2.0);
            }
        }
    }

    [RelayCommand]
    private void AddInput()
    {
        if (!_preview.IsEmpty())
            AddInput(_preview.Objects);
        else if (!_selection.IsEmpty())
            AddInput(_selection.Objects);
        else if (_dragService.IsRunning())
            AddInput(_dragService.Objects);

        static void AddInput(AvaloniaList<CircuitObjectViewModel> collection)
        {
            foreach (CircuitObjectViewModel co in collection)
            {
                if (co is MultiInputGateViewModel mig)
                    mig.AddInput();
                else if (co is MultiplexerViewModel mux)
                    mux.AddSelectLine();
                else if (co is DemultiplexerViewModel demux)
                    demux.AddSelectLine();
                else if (co is DecoderViewModel decoder)
                    decoder.AddSelectLine();
                else if (co is PriorityEncoderViewModel encoder)
                    encoder.AddSelectLine();
            }
        }
    }

    [RelayCommand]
    private void RemoveInput()
    {
        if (!_preview.IsEmpty())
            RemoveInput(_preview.Objects);
        else if (!_selection.IsEmpty())
            RemoveInput(_selection.Objects);
        else if (_dragService.IsRunning())
            RemoveInput(_dragService.Objects);

        static void RemoveInput(AvaloniaList<CircuitObjectViewModel> collection)
        {
            foreach (CircuitObjectViewModel co in collection)
            {
                if (co is MultiInputGateViewModel mig)
                {
                    mig.RemoveInput();
                }
                else if (co is MultiplexerViewModel mux)
                {
                    mux.RemoveSelectLine();
                }
                else if (co is DemultiplexerViewModel demux)
                {
                    demux.RemoveSelectLine();
                }
                else if (co is DecoderViewModel decoder)
                {
                    decoder.RemoveSelectLine();
                }
                else if (co is PriorityEncoderViewModel encoder)
                {
                    encoder.RemoveSelectLine();
                }
            }
        }
    }

    [RelayCommand]
    private void Toggle()
    {
        if (_hoverEffectService.HasToggle())
            (_hoverEffectService.GetObject() as ToggleViewModel)!.Toggle();
    }

    [RelayCommand]
    private static void SetTheme(string variant) { }

    [RelayCommand]
    private static void GenerateFromPrompt() { }

    [RelayCommand]
    private async Task GenerateFromImageAsync()
    {
        await new GenerateFromImageWindowView().ShowDialog(App.ApplicationLifetime.MainWindow!);
    }

    [RelayCommand]
    private void SelectAll()
    {
        _selection.Highlight(_simulation.Objects);
    }

    [RelayCommand]
    private void GrabAll()
    {
        _preview.Pick(_simulation.Objects);
        _simulation.Nuke();
    }

    [RelayCommand]
    private void GrabSelected()
    {
        _preview.Pick(_selection.Objects);
        _simulation.Remove(_selection.Objects);
        _selection.UnHighlightAll();
    }
}
