using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Services.Singleton;


namespace IRis.ViewModels;


public partial class GenerateFromImageWindowViewModel(Window owner) : ViewModelBase
{
    private const string ExePath = @"sketchlogic.exe";
    private const string OutputIrisPath = @"temp.iris";
    private string? _selectedFilePath;
    private readonly Window _owner = owner;
    private bool _hasImage;
    private bool _isGenerating;

    private bool CanGenerate => _hasImage && !_isGenerating;

    [ObservableProperty]
    private Bitmap? _previewImage;

    [RelayCommand]
    private async Task PickImageAsync()
    {
        var files = await _owner.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Select an Image to Generate From",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("Images") { Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp", "*.webp"] }
                ]
            }
        );

        if (files.Count == 0) return;

        _selectedFilePath = files[0].Path.LocalPath;
        PreviewImage = new Bitmap(_selectedFilePath);
        _hasImage = true;
        GenerateCommand.NotifyCanExecuteChanged();
    }


    [RelayCommand(CanExecute = nameof(CanGenerate))]
    private async Task Generate()
    {
        if (string.IsNullOrEmpty(_selectedFilePath)) return;
        if (!File.Exists(ExePath)) return;

        _isGenerating = true;
        GenerateCommand.NotifyCanExecuteChanged();
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = ExePath,
                Arguments = $"{_selectedFilePath} {OutputIrisPath}",
                UseShellExecute = false,
                CreateNoWindow = true,
            });

            if (process is null) return;
            await process.WaitForExitAsync();

            if (process.ExitCode != 0 || !File.Exists(OutputIrisPath)) return;
            if (Simulation.Get().Running)
                Simulation.Get().Running = false;

            var json = await File.ReadAllTextAsync(OutputIrisPath);
            var collection = SerializationService.Deserialize(json);
            if (collection is null) return;

            AppState.Get().CurrentFilePath = OutputIrisPath;
            SimulationService.RedrawEmptyWires(collection);
            Selection.Get().Highlight(collection);
            Simulation.Get().Add(collection);

            _owner.Close();
        }
        finally
        {
            _isGenerating = false;
            GenerateCommand.NotifyCanExecuteChanged();
        }
    }
}
