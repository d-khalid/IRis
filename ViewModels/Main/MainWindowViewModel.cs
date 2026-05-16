using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.ViewModels;
using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;
using Avalonia;
using System;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public SimulationViewModel Simulation { get; } = SimulationViewModel.GetInstance();


    [RelayCommand]
    private void DeleteKey()
    {
        for (int i = Simulation.SelectedObjects.Count-1; i >= 0; i--)
        {
            CircuitObjectViewModel co = Simulation.SelectedObjects[i];
            Simulation.CircuitObjects.Remove(co);
            Simulation.UnselectObject(co);
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
        for (int i = Simulation.SelectedObjects.Count-1; i >= 0; i--)
        {
            CircuitObjectViewModel co = Simulation.SelectedObjects[i];
            Simulation.UnselectObject(co);
            Simulation.CopiedObjects.Add(CloningService.Clone(co));
        }
    }


    [RelayCommand]
    private void PasteKey()
    {
        if (Simulation.CopiedObjects.Count == 0) return;

        UtilityService.SnapCollectionToPosition(
            Simulation.CopiedObjects, 
            Simulation.CurrentMousePos,
            Simulation.PreviewMouseOffset
        );

        Simulation.PreviewObjects.Clear();
        Simulation.PreviewMouseOffset = new Point(0, 0);
        Simulation.IsPreviewVisible = true;

        foreach (CircuitObjectViewModel co in Simulation.CopiedObjects)
        {
            CircuitObjectViewModel clone = CloningService.Clone(co);
            clone.Opacity = 0.5;
            Simulation.PreviewObjects.Add(clone);
        }
    }
}
