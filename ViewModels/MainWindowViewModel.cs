using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.ViewModels.Main.Canvas;
using IRis.Models.Main.Canvas.Core;
using Avalonia;
using System.Collections.ObjectModel;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;


namespace IRis.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private Simulation _simulation = Simulation.GetInstance();


    [RelayCommand]
    private void DeleteKey()
    {
        var sel = Selection.GetInstance();
        for (int i = sel.Objects.Count-1; i >= 0; i--)
        {
            CircuitObjectViewModel co = sel.Objects[i];
            Simulation.Objects.Remove(co);
            sel.Remove(co);
        }
    }


    [RelayCommand]
    private void EscapeKey()
    {
        var prev = Preview.GetInstance();
        prev.Ditch();
    }


    [RelayCommand]
    private void CopyKey()
    {
        var prev = Preview.GetInstance();
        var sel = Selection.GetInstance();
        var clip = ClipboardManager.GetInstance();

        if (prev.HasObjects())
        {
            clip.Copy(prev.Objects);
            prev.Ditch();
        }
        else if (sel.HasObjects())
        {
            clip.Copy(sel.Objects);
            sel.Ditch();
        }
    }


    [RelayCommand]
    private void PasteKey()
    {
        var clip = ClipboardManager.GetInstance();
        var prev = Preview.GetInstance();

        clip.Paste(prev.Objects);
    }


    [RelayCommand]
    private void RotateKey()
    {
        ObservableCollection<CircuitObjectViewModel> collection;
        var prev = Preview.GetInstance();
        var sel = Selection.GetInstance();

        if (prev.HasObjects()) collection = prev.Objects;
        else if (sel.HasObjects()) collection = sel.Objects;
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
        var prev = Preview.GetInstance();
        var sel = Selection.GetInstance();

        if (prev.HasObjects()) collection = prev.Objects;
        else if (sel.HasObjects()) collection = sel.Objects;
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
        var prev = Preview.GetInstance();
        var sel = Selection.GetInstance();

        if (prev.HasObjects()) collection = prev.Objects;
        else if (sel.HasObjects()) collection = sel.Objects;
        else return;

        foreach (CircuitObjectViewModel co in collection)
        {
            if (co is AndGateViewModel ag)
                ag.RemoveInput(ag.Inputs[^1]);
        }
    }
}
