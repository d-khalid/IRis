using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Core;


public partial class TerminalViewModel : ObservableObject
{
    [ObservableProperty] private double _x;
    [ObservableProperty] private double _y;
}

