using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Services.Singleton;
using IRis.Views;

namespace IRis.ViewModels;

public partial class GenerateFromImageWindowViewModel : ViewModelBase
{
    private readonly GenerateFromImageWindowView _owner;

    private const string ExePath = @"sketchlogic.exe";
    private const string OutputIrisPath = @"temp.iris";
    private string? _selectedFilePath;

    private bool _hasImage;
    private bool _isGenerating;
    private bool CanGenerate => _hasImage && !_isGenerating;

    private readonly Simulation _simulation;
    private readonly Selection _selection;
    private readonly AppState _appState;
    private readonly SerializationService _serialization;
    private readonly SimulationService _simulationService;

    [ObservableProperty]
    private Bitmap? _previewImage;

    public GenerateFromImageWindowViewModel(
        GenerateFromImageWindowView owner,
        Simulation simulation,
        Selection selection,
        AppState appState,
        SerializationService serialization,
        SimulationService simulationService
    )
    {
        _owner = owner;
        _simulation = simulation;
        _selection = selection;
        _appState = appState;
        _serialization = serialization;
        _simulationService = simulationService;
    }

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
                    new FilePickerFileType("Images")
                    {
                        Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp", "*.webp"],
                    },
                ],
            }
        );

        if (files.Count == 0)
            return;

        _selectedFilePath = files[0].Path.LocalPath;
        PreviewImage = new Bitmap(_selectedFilePath);
        _hasImage = true;
        GenerateCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanGenerate))]
    private async Task Generate()
    {
        if (string.IsNullOrEmpty(_selectedFilePath))
            return;
        if (!File.Exists(ExePath))
            return;

        _isGenerating = true;
        GenerateCommand.NotifyCanExecuteChanged();

        try
        {
            using var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = ExePath,
                    Arguments = $"{_selectedFilePath} {OutputIrisPath}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            );

            if (process is null)
                return;
            await process.WaitForExitAsync();

            if (process.ExitCode != 0 || !File.Exists(OutputIrisPath))
                return;
            if (_simulation.Running)
                _simulation.Running = false;

            var json = await File.ReadAllTextAsync(OutputIrisPath);
            var collection = _serialization.Deserialize(json);
            if (collection is null)
                return;

            _appState.CurrentFilePath = OutputIrisPath;
            _simulationService.RedrawEmptyWires(collection);
            _selection.Highlight(collection);
            _simulation.Add(collection);

            _owner.Close();
        }
        finally
        {
            _isGenerating = false;
            GenerateCommand.NotifyCanExecuteChanged();
        }
    }
}
