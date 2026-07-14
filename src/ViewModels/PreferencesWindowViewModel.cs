using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using IRis.Services.Singleton;

namespace IRis.ViewModels;

public partial class PreferencesWindowViewModel(AppState appState) : ViewModelBase
{
    private readonly AppState _appState = appState;

    public string Theme
    {
        get
        {
            if (_appState.Theme == ThemeVariant.Dark)
                return "Dark";
            else if (_appState.Theme == ThemeVariant.Light)
                return "Light";
            else
                return "System";
        }
        set
        {
            _appState.Theme = value switch
            {
                "System" => ThemeVariant.Default,
                "Dark" => ThemeVariant.Dark,
                "Light" => ThemeVariant.Light,
                _ => ThemeVariant.Default,
            };
        }
    }
}
