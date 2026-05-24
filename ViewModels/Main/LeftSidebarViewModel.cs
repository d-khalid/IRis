using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.ViewModels;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using Tmds.DBus.Protocol;
using Avalonia;
using IRis.Models.Circuit.CircuitObjects.Core;
using IRis.Models.Core;
using IRis.ViewModels.Circuit.CircuitObjects.Core;
using System;


namespace IRis.ViewModels.Main;


public partial class LeftSidebarViewModel : ViewModelBase
{
    [ObservableProperty]
    private Simulation _simulation = (Simulation)Simulation.GetInstance();

    [ObservableProperty]
    private Preview _preview = (Preview)Preview.GetInstance();


    [RelayCommand]
    private void AddAnd()
    {
        AndGateViewModel gate = new() { Opacity = 0.5 };

        TerminalViewModel i1 = new(TerminalType.Input, gate);
        TerminalViewModel i2 = new(TerminalType.Input, gate);

        gate.AddInput(i1);
        gate.AddInput(i2);
        Preview.Add(gate);
        Preview.MouseOffset = new Point(25, 25);
    }
}
