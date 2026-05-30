using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia;


namespace IRis.Services.Singleton;


public partial class AppState : SingletonBase<AppState>
{
    [ObservableProperty] private Point _mousePosition = new(0, 0);
}
