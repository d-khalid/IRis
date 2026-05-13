using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using System.Collections.ObjectModel;
using IRis.ViewModels.CircuitObjects;
using IRis.ViewModels.CircuitObjects.Components.Gates;
using IRis.ViewModels.Core;
using Avalonia.Input;

using IRis.Models;
using IRis.Models.Core;
using IRis.Views;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<ComponentViewModel> CircuitObjects { get; } = [];


    [RelayCommand]
    private void AddAndGate()
    {
        var Input = new TerminalViewModel();

        var andGate = new AndGateViewModel(Input, Input)
        {
            X = 100, Y = 100, Width = 60, Height = 60
        };

        CircuitObjects.Add(andGate);
    }
}

