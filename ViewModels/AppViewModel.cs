using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services;
using IRis.Services.Singleton;

namespace IRis.ViewModels;

public partial class AppViewModel : ViewModelBase
{
    [ObservableProperty]
    private AppState _appState;
    private readonly Simulation _simulation;
    private readonly SerializationService _serialization;
    private readonly SimulationService _simulationService;

    public AppViewModel(
        AppState appState,
        Simulation simulation,
        SerializationService serialization,
        SimulationService simulationService
    )
    {
        AppState = appState;
        _simulation = simulation;
        _serialization = serialization;
        _simulationService = simulationService;
        LoadLastSessionFile();
    }

    private void LoadLastSessionFile()
    {
        string lastOpenedFile = AppState.CurrentFilePath;
        if (lastOpenedFile == "(unsaved)")
        {
            if (!File.Exists(AppState.AutoSavePath))
            {
                return;
            }

            lastOpenedFile = AppState.AutoSavePath;
        }
        else if (!File.Exists(lastOpenedFile))
        {
            AppState.CurrentFilePath = "(unsaved)";
            return;
        }

        var json = File.ReadAllText(lastOpenedFile);
        var collection = _serialization.Deserialize(json);

        if (collection is not null)
        {
            _simulationService.RedrawEmptyWires(collection);
            _simulation.Add(collection);
        }
    }
}
