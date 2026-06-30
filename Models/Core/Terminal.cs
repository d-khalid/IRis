using CommunityToolkit.Mvvm.ComponentModel;

namespace IRis.Models.Core;

public partial class Terminal : ObservableObject
{
    [ObservableProperty]
    private LogicState _state = LogicState.Unknown;
}

public enum TerminalType
{
    Input,
    Output,
}

public enum LogicState
{
    Low,
    High,
    Unknown,
}
