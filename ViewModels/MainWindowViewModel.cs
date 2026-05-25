using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.ViewModels.Circuit;
using IRis.Models.Circuit.CircuitObjects.Core;
using Avalonia;
using System.Collections.ObjectModel;
using IRis.ViewModels.Circuit.CircuitObjects.Components.Gates;
using IRis.Models.Core;
using IRis.ViewModels.Circuit.Core;
using IRis.ViewModels.Circuit.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
    public Simulation Simulation { get; } = Simulation.GetInstance();
    public Preview Preview { get; } = Preview.GetInstance();
    public Selection Selection { get; } = Selection.GetInstance();
    public ClipboardManager Clipboard { get; } = ClipboardManager.GetInstance();
    
    [ObservableProperty] private Point _mousePosition = new(0, 0);


    [RelayCommand]
    private void DeleteKey()
    {
        for (int i = Selection.Objects.Count-1; i >= 0; i--)
        {
            CircuitObjectViewModel co = Selection.Objects[i];
            Simulation.Objects.Remove(co);
            Selection.Remove(co);
        }
    }


    [RelayCommand]
    private void EscapeKey()
    {
        Preview.Ditch();
    }


    [RelayCommand]
    private void CopyKey()
    {
        if (Preview.HasObjects())
        {
            Clipboard.Copy(Preview.Objects);
            Preview.Ditch();
        }
        else if (Selection.HasObjects())
        {
            Clipboard.Copy(Selection.Objects);
            Selection.Ditch();
        }
    }


    [RelayCommand]
    private void PasteKey()
    {
        Clipboard.Paste(Preview.Objects);
    }


    [RelayCommand]
    private void RotateKey()
    {
        ObservableCollection<CircuitObjectViewModel> collection;

        if (Preview.HasObjects()) collection = Preview.Objects;
        else if (Selection.HasObjects()) collection = Selection.Objects;
        else return;

        Point min = SimulationService.GetMinPointInCollection(collection);
        Point max = SimulationService.GetMaxPointInCollection(collection);
        Point center = SimulationService.Average(min, max);

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

        if (Preview.HasObjects()) collection = Preview.Objects;
        else if (Selection.HasObjects()) collection = Selection.Objects;
        else return;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is AndGateViewModel ag)
                ag.AddInput(new TerminalViewModel(TerminalType.Input, ag));
        }
    }


    [RelayCommand]
    private void RemoveInputKey()
    {
        ObservableCollection<CircuitObjectViewModel> collection;

        if (Preview.HasObjects()) collection = Preview.Objects;
        else if (Selection.HasObjects()) collection = Selection.Objects;
        else return;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is AndGateViewModel ag)
                ag.RemoveInput(ag.Inputs[^1]);
        }
    }
}
