using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.ViewModels;
using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;
using Avalonia;
using System;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public SimulationViewModel Simulation { get; } = SimulationViewModel.GetInstance();


    [RelayCommand]
    private void DeleteKey()
    {
        for (int i = Simulation.CircuitObjects.Count-1; i >= 0; i--)
        {
            CircuitObjectViewModel co = Simulation.CircuitObjects[i];
            if (co.IsSelected)
                Simulation.CircuitObjects.Remove(co);
        }
    }


    [RelayCommand]
    private void EscapeKey()
    {
        Simulation.PreviewObjects.Clear();
    }


    [RelayCommand]
    private void CopyKey()
    {
        Simulation.CopiedObjects.Clear();
        foreach (CircuitObjectViewModel co in Simulation.CircuitObjects)
        {
            if (co.IsSelected) 
            {
                co.IsSelected = false;
                Simulation.CopiedObjects.Add(CloningService.Clone(co));
            }
        }
    }


    [RelayCommand]
    private void PasteKey()
    {
        if (Simulation.CopiedObjects.Count == 0) return;

        UtilityService.SnapCollectionToPosition(
            Simulation.CopiedObjects, Simulation.CurrentMousePos
        );

        Simulation.PreviewObjects.Clear();
        Simulation.IsPreviewVisible = true;

        foreach (CircuitObjectViewModel co in Simulation.CopiedObjects)
        {
            CircuitObjectViewModel clone = CloningService.Clone(co);
            clone.Opacity = 0.5;
            Simulation.PreviewObjects.Add(clone);
        }
    }
}
