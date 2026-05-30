using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.ViewModels.Main.Canvas;
using IRis.Models.Core;
using Avalonia;
using System.Collections.ObjectModel;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.Services.Singleton;
using IRis.ViewModels.Main.Canvas.Core;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Collections;


namespace IRis.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private AppState _appState = AppState.Get();


    [RelayCommand]
    private static void DeleteKey()
    {
        Simulation.Get().Remove(Selection.Get().Objects);
        Selection.Get().UnHighlightAll();
    }


    [RelayCommand]
    private static void EscapeKey()
    {
        if (!Preview.Get().IsEmpty())
            Preview.Get().Nuke();
    }


    [RelayCommand]
    private static void CopyKey()
    {
        if (!Preview.Get().IsEmpty())
        {
            ClipboardService.Copy(Preview.Get().Objects);
            Preview.Get().Nuke();
        }

        else if (!Selection.Get().IsEmpty())
        {
            ClipboardService.Copy(Selection.Get().Objects);
            Selection.Get().UnHighlightAll();
        }

        else if (DragService.IsRunning())
        {
            ClipboardService.Copy(DragService.Objects);
        }
    }


    [RelayCommand]
    private static void PasteKey()
    {
        ClipboardService.Paste();
    }


    [RelayCommand]
    private static void RotateKey()
    {
        if (!Preview.Get().IsEmpty())
            RotateCollection(Preview.Get().Objects);

        else if (!Selection.Get().IsEmpty())
            RotateCollection(Selection.Get().Objects);

        else if (DragService.IsRunning())
            RotateCollection(DragService.Objects);


        static void RotateCollection(AvaloniaList<CircuitObjectViewModel> collection)
        {
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
    }


    [RelayCommand]
    private static void AddInputKey()
    {
        if (!Preview.Get().IsEmpty()) 
            AddInput(Preview.Get().Objects);

        else if (!Selection.Get().IsEmpty()) 
            AddInput(Selection.Get().Objects);

        else if (DragService.IsRunning()) 
            AddInput(DragService.Objects);


        static void AddInput(AvaloniaList<CircuitObjectViewModel> collection)
        {
            foreach (CircuitObjectViewModel co in collection)
            {
                if (co is MultiInputGateViewModel mig)
                    mig.Inputs.Add(new TerminalViewModel());
            }
        }
    }


    [RelayCommand]
    private static void RemoveInputKey()
    {
        if (!Preview.Get().IsEmpty()) 
            RemoveInput(Preview.Get().Objects);

        else if (!Selection.Get().IsEmpty()) 
            RemoveInput(Selection.Get().Objects);

        else if (DragService.IsRunning()) 
            RemoveInput(DragService.Objects);


        static void RemoveInput(AvaloniaList<CircuitObjectViewModel> collection)
        {
            foreach (CircuitObjectViewModel co in collection)
            {
                if (co is MultiInputGateViewModel mig)
                {
                    mig.Inputs.Remove(mig.Inputs[^1]);
                }
            }
        }
    }
}
