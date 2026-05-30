using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.ViewModels.Main.Canvas;
using IRis.Models.Main.Canvas.Core;
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
    [ObservableProperty] private AppState _appState = AppState.GetInstance();


    [RelayCommand]
    private static void DeleteKey()
    {
        Simulation.GetInstance().Remove(Selection.GetInstance().Objects);
        Selection.GetInstance().UnHighlightAll();
    }


    [RelayCommand]
    private static void EscapeKey()
    {
        if (!Preview.GetInstance().IsEmpty())
            Preview.GetInstance().Nuke();
    }


    [RelayCommand]
    private static void CopyKey()
    {
        if (!Preview.GetInstance().IsEmpty())
        {
            ClipboardService.Copy(Preview.GetInstance().Objects);
            Preview.GetInstance().Nuke();
        }

        else if (!Selection.GetInstance().IsEmpty())
        {
            ClipboardService.Copy(Selection.GetInstance().Objects);
            Selection.GetInstance().UnHighlightAll();
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
        if (!Preview.GetInstance().IsEmpty())
            RotateCollection(Preview.GetInstance().Objects);

        else if (!Selection.GetInstance().IsEmpty())
            RotateCollection(Selection.GetInstance().Objects);

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
        if (!Preview.GetInstance().IsEmpty()) 
            AddInput(Preview.GetInstance().Objects);

        else if (!Selection.GetInstance().IsEmpty()) 
            AddInput(Selection.GetInstance().Objects);

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
        if (!Preview.GetInstance().IsEmpty()) 
            RemoveInput(Preview.GetInstance().Objects);

        else if (!Selection.GetInstance().IsEmpty()) 
            RemoveInput(Selection.GetInstance().Objects);

        else if (DragService.IsRunning()) 
            RemoveInput(DragService.Objects);


        static void RemoveInput(AvaloniaList<CircuitObjectViewModel> collection)
        {
            foreach (CircuitObjectViewModel co in collection)
            {
                if (co is MultiInputGateViewModel mig)
                    mig.Inputs.Remove(mig.Inputs[^1]);
            }
        }
    }
}
