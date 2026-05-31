using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;
using IRis.ViewModels.Main.Canvas;


namespace IRis.Services.Singleton;


public partial class AppState : SingletonBase<AppState>
{
    [ObservableProperty] private Point _mousePosition = new(0, 0);
    [ObservableProperty] private bool _terminalColorChangeAllowed = true;
    partial void OnTerminalColorChangeAllowedChanged(bool value)
    {
        if (Simulation.Get().Running)
        {
            Simulation.Get().Stop();
            Simulation.Get().Start();
        }
    }

    [ObservableProperty] private string _currentFilePath = "(unsaved)";
    [ObservableProperty] private string _lastCommand = "(none)";
}
