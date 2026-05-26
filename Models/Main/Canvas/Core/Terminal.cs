using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.Models.Main.Canvas.Core;


public partial class Terminal : ObservableObject
{
    [ObservableProperty] private LogicState _state = LogicState.Unknown;
}


public enum TerminalType
{
    Input,
    Output
}

public enum LogicState
{
    High,
    Low,
    Unknown
}
