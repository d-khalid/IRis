using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services;
using IRis.Services.Singleton;
using System;


namespace IRis.ViewModels;


public partial class AppViewModel : ViewModelBase
{
    [ObservableProperty]
    private AppState _appState;
    private readonly Simulation _simulation;


    public AppViewModel(AppState appState, Simulation simulation)
    {
        AppState = appState;
        _simulation = simulation;
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
        var collection = SerializationService.Deserialize(json);

        if (collection is not null)
        {
            SimulationService.RedrawEmptyWires(collection);
            _simulation.Add(collection);
        }
    }
}
