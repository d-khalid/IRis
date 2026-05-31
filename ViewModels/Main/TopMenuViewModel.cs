using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Services.Singleton;


namespace IRis.ViewModels.Main;


public partial class TopMenuViewModel : ViewModelBase
{
    [RelayCommand]
    private static void New()
    {
        if (Simulation.Get().Running) 
            Simulation.Get().Stop();

        Selection.Get().UnHighlightAll();
        Simulation.Get().Nuke();
        Preview.Get().Nuke();

        CommandService.Reset();
        AppState.Get().CurrentFilePath = "(unsaved)";
    }


    [RelayCommand]
    private static async Task OpenAsync()
    {
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
            AppState.Get().CurrentFilePath = files[0].Path.LocalPath;

            var json = await File.ReadAllTextAsync(AppState.Get().CurrentFilePath);
            var collection = SerializationService.Deserialize(json);

            if (collection is not null)
            {
                Simulation.Get().Nuke();
                SimulationService.RedrawWires(collection);
                Simulation.Get().Add(collection);
            }
        }
    }


    [RelayCommand]
    private static async Task SaveAsync()
    {
        if (AppState.Get().CurrentFilePath == "(unsaved)")
        {
            await SaveAsAsync();
            return;
        }

        var json = SerializationService.Serialize(Simulation.Get().Objects);
        await File.WriteAllTextAsync(AppState.Get().CurrentFilePath, json);
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
    private static void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
        {
            app.Shutdown();
        }
    }


    [RelayCommand] private static void Undo() => CommandService.Undo();
    [RelayCommand] private static void Redo() => CommandService.Redo();
}
