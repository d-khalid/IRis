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
        foreach (CircuitObjectViewModel co in Simulation.CircuitObjects)
        {
            if (co.IsSelected) 
            {
                co.IsSelected = false;
                Simulation.CopiedObjects.Add(UtilityService.Clone(co));
            }
        }
    }


    [RelayCommand]
    private void PasteKey()
    {
        if (Simulation.CopiedObjects.Count == 0) return;

        Point min = UtilityService.GetMinPointFromCollection(Simulation.CopiedObjects);
        double offsetX = min.X - Simulation.CurrentMousePos.X;
        double offsetY = min.Y - Simulation.CurrentMousePos.Y;

        Simulation.PreviewObjects.Clear();
        Simulation.IsPreviewVisible = true;
        foreach (CircuitObjectViewModel co in Simulation.CopiedObjects)
        {
            CircuitObjectViewModel clone = UtilityService.Clone(co);
            clone.Opacity = 0.5;

            if (clone is ComponentViewModel c)
            {
                Point target = UtilityService.SnapPointToGrid(
                    new Point(c.X - offsetX, c.Y - offsetY
                ));

                c.X = target.X;
                c.Y = target.Y;
            }

            Simulation.PreviewObjects.Add(clone);
        }
    }
}
