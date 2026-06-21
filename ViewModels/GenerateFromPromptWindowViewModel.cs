using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;


namespace IRis.ViewModels;


public partial class GenerateFromPromptWindowViewModel(Window owner) : ViewModelBase
{
    private readonly Window _owner = owner;
    
    [ObservableProperty]
    private string _prompt = string.Empty;

    [RelayCommand]
    private void Generate()
    {
        Prompt = "";
        _owner.Close();
    }
}
