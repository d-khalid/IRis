using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.ViewModels;
using IRis.ViewModels.Circuit;
using IRis.ViewModels.Circuit.CircuitObjects;
using Avalonia;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using IRis.Models.Core;


namespace IRis.ViewModels.Main;


public partial class MainWindowViewModel : ViewModelBase
{
    public SimulationManager Simulation { get; } = SimulationManager.GetInstance();


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


    [RelayCommand]
    private void RotateKey()
    {
        ObservableCollection<CircuitObjectViewModel> collection;

        if (Simulation.PreviewObjects.Count > 0) collection = Simulation.PreviewObjects;
        else if (Simulation.SelectedObjects.Count > 0) collection = Simulation.SelectedObjects;
        else return;

        Point min = UtilityService.GetMinPointInCollection(collection);
        Point max = UtilityService.GetMaxPointInCollection(collection);
        Point center = UtilityService.Average(min, max);

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is ComponentViewModel c)
            {
                c.Rotation = (c.Rotation + 90) % 360;

                // this MATH bellow was done by Gemini 3.1 Pro, it works

                double objCenterX = c.X + (c.Width / 2.0);
                double objCenterY = c.Y + (c.Height / 2.0);

                double translatedX = objCenterX - center.X;
                double translatedY = objCenterY - center.Y;

                double newCenterX = -translatedY + center.X;
                double newCenterY = translatedX + center.Y;

                c.X = newCenterX - (c.Width / 2.0);
                c.Y = newCenterY - (c.Height / 2.0);
            }
        }
    }


    [RelayCommand]
    private void AddInputKey()
    {
        ObservableCollection<CircuitObjectViewModel> collection;

        if (Simulation.PreviewObjects.Count > 0) collection = Simulation.PreviewObjects;
        else if (Simulation.SelectedObjects.Count > 0) collection = Simulation.SelectedObjects;
        else return;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is AndGateViewModel ag)
                ag.AddInput(new(new Terminal(TerminalType.Input)));
        }
    }


    [RelayCommand]
    private void RemoveInputKey()
    {
        ObservableCollection<CircuitObjectViewModel> collection;

        if (Simulation.PreviewObjects.Count > 0) collection = Simulation.PreviewObjects;
        else if (Simulation.SelectedObjects.Count > 0) collection = Simulation.SelectedObjects;
        else return;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is AndGateViewModel ag)
                ag.RemoveInput(ag.Inputs[^1]);
        }
    }
}
