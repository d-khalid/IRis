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
        if (Simulation.GetInstance().Running)
        {
            Simulation.GetInstance().Stop();
            Simulation.GetInstance().Start();
        }
    }
}
