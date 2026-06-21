using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services;
using IRis.Services.Singleton;


namespace IRis.ViewModels;


public partial class AppViewModel : ViewModelBase
{
    [ObservableProperty] private AppState _appState = AppState.Get();


    public AppViewModel() => OnStartup();


    public static void OnStartup()
    {
        AppState.Get();
        LoadLastSessionFile();
        Simulation.Get().Running = true;
    }


    private static async void LoadLastSessionFile()
    {
        string lastOpenedFile = AppState.Get().CurrentFilePath;
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
            AppState.Get().CurrentFilePath = "(unsaved)";
            return;
        }

        var json = await File.ReadAllTextAsync(lastOpenedFile);
        var collection = SerializationService.Deserialize(json);

        if (collection is not null)
        {
            SimulationService.RedrawEmptyWires(collection);
            Simulation.Get().Add(collection);
        }
    }
}
