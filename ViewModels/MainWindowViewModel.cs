using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.ViewModels.Main.Canvas;
using IRis.Models.Core;
using Avalonia;
using System.Collections.ObjectModel;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.IO;
using System;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using FluentAvalonia.UI.Controls;


namespace IRis.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private AppState _appState = AppState.Get();


    private static async Task<bool> AskNukeChangesAsync()
    {
        var dialog = new ContentDialog
        {
            Title = "Unsaved Changes",
            Content = "You seem to have unsaved changes in this file. Should I save them?",
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Don't Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
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
    private static async Task New()
    {
        if (!await AskNukeChangesAsync()) return;
        AppState.Get().CurrentFilePath = "(unsaved)";

        if (Simulation.Get().Running) 
            Simulation.Get().Running = false;

        Selection.Get().UnHighlightAll();
        Simulation.Get().Nuke();
        Preview.Get().Nuke();
        WirePreview.Get().Nuke();

        CommandService.Reset();
    }


    [RelayCommand]
    private static async Task OpenAsync(string param)
    {
        if (param == "open" && !await AskNukeChangesAsync()) return;
        if (Simulation.Get().Running) Simulation.Get().Running = false;

        if (Application.Current?.ApplicationLifetime is not 
            IClassicDesktopStyleApplicationLifetime time || time.MainWindow is null)
            return;

        var files = await time.MainWindow.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Open Simulation",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("IRis Simulatable") { Patterns = ["*.iris"] }]
            }
        );

        if (files.Count > 0)
        {
            string path = files[0].Path.LocalPath;
            var json = await File.ReadAllTextAsync(path);
            var collection = SerializationService.Deserialize(json);

            if (collection is not null)
            {
                if (param != "merge")
                {
                    AppState.Get().CurrentFilePath = path;
                    Simulation.Get().Nuke();
                }

                SimulationService.RedrawEmptyWires(collection);
                Selection.Get().Highlight(collection);
                Simulation.Get().Add(collection);
            }
        }
    }


    [RelayCommand]
    private static async Task SaveAsync()
    {
        if (Simulation.Get().Running) Simulation.Get().Running = false;

        if (AppState.Get().CurrentFilePath == "(unsaved)")
        {
            await SaveAsAsync();
            return;
        }

        var json = SerializationService.Serialize(Simulation.Get().Objects);
        await File.WriteAllTextAsync(AppState.Get().CurrentFilePath, json);

        AppState.FileNeedsSaving = false;
    }


    [RelayCommand]
    private static async Task SaveAsAsync()
    {
        if (Application.Current?.ApplicationLifetime is not 
            IClassicDesktopStyleApplicationLifetime time || time.MainWindow is null)
            return;

        var file = await time.MainWindow.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Save Simulation",
                DefaultExtension = "iris",
                FileTypeChoices = [new FilePickerFileType("IRis Simulatable") { Patterns = ["*.iris"] }]
            }
        );

        if (file is not null)
        {
            AppState.Get().CurrentFilePath = file.Path.LocalPath;
            await SaveAsync();
        }
    }


    [RelayCommand]
    private static void Preferences()
    {
        
    }


    [RelayCommand]
    private static void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
        {
            app.Shutdown();
        }
    }


    [RelayCommand] private static void Undo() => CommandService.Undo();
    [RelayCommand] private static void Redo() => CommandService.Redo();


    [RelayCommand]
    private static void Delete()
    {
        Simulation.Get().Remove(Selection.Get().Objects);
        Selection.Get().UnHighlightAll();
    }


    [RelayCommand]
    private static void Escape()
    {
        if (!Preview.Get().IsEmpty())
            Preview.Get().Nuke();

        if (!WirePreview.Get().IsEmpty())
            WirePreview.Get().Leave();
    }


    [RelayCommand] private static void UndoKey() => CommandService.Undo();
    [RelayCommand] private static void RedoKey() => CommandService.Redo();


    [RelayCommand]
    private static void Copy()
    {
        if (!Preview.Get().IsEmpty())
        {
            ClipboardService.Copy(Preview.Get().Objects);
            Preview.Get().Nuke();
        }

        else if (!Selection.Get().IsEmpty())
        {
            ClipboardService.Copy(Selection.Get().Objects);
            Selection.Get().UnHighlightAll();
        }

        else if (DragService.IsRunning())
        {
            ClipboardService.Copy(DragService.Objects);
        }
    }


    [RelayCommand]
    private static void Paste()
    {
        ClipboardService.Paste();
    }


    [RelayCommand]
    private static void Rotate()
    {
        if (!Preview.Get().IsEmpty())
            RotateCollection(Preview.Get().Objects);

        else if (!Selection.Get().IsEmpty())
            RotateCollection(Selection.Get().Objects);

        else if (DragService.IsRunning())
            RotateCollection(DragService.Objects);


        static void RotateCollection(AvaloniaList<CircuitObjectViewModel> collection)
        {
            Point min = SimulationService.GetMinPointInCollection(collection);
            Point max = SimulationService.GetMaxPointInCollection(collection);
            Point center = SimulationService.Average(min, max);

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
    }


    [RelayCommand]
    private static void AddInput()
    {
        if (!Preview.Get().IsEmpty()) 
            AddInput(Preview.Get().Objects);

        else if (!Selection.Get().IsEmpty()) 
            AddInput(Selection.Get().Objects);

        else if (DragService.IsRunning()) 
            AddInput(DragService.Objects);


        static void AddInput(AvaloniaList<CircuitObjectViewModel> collection)
        {
            foreach (CircuitObjectViewModel co in collection)
            {
                if (co is MultiInputGateViewModel mig)
                    mig.Inputs.Add(new TerminalViewModel());
            }
        }
    }


    [RelayCommand]
    private static void RemoveInput()
    {
        if (!Preview.Get().IsEmpty()) 
            RemoveInput(Preview.Get().Objects);

        else if (!Selection.Get().IsEmpty()) 
            RemoveInput(Selection.Get().Objects);

        else if (DragService.IsRunning()) 
            RemoveInput(DragService.Objects);


        static void RemoveInput(AvaloniaList<CircuitObjectViewModel> collection)
        {
            foreach (CircuitObjectViewModel co in collection)
            {
                if (co is MultiInputGateViewModel mig)
                {
                    mig.Inputs.Remove(mig.Inputs[^1]);
                }
            }
        }
    }


    [RelayCommand]
    private static void Toggle()
    {
        if (HoverEffectService.HasToggle())
            (HoverEffectService.GetObject() as ToggleViewModel)!.Toggle();
    }


    [RelayCommand]
    private static void SetTheme(string variant)
    {
        
    }
}
