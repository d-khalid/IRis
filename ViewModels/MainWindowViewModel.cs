using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using ICommand = System.Windows.Input.ICommand;

using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Services;
using IRis.Views;


namespace IRis.ViewModels;


public partial class MainWindowViewModel : ViewModelBase
{
    public readonly Simulation Simulation;


    public MainWindowViewModel(Simulation simulation)
    {
        Simulation = simulation;
        Simulation.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(Simulation.CurrentMousePos))
                OnPropertyChanged(nameof(CursorPosition));
        };
        

        KeyConfig = KeyGestureConfig.LoadKeyGestureConfig();
        KeyGestureConfig.SaveKeyGestureConfig(KeyConfig);


        // Initialize all commands
        NewCommand = new RelayCommand(New);
        OpenCommand = new AsyncRelayCommand(Open);
        SaveCommand = new AsyncRelayCommand(Save);
        SaveAsCommand = new AsyncRelayCommand(SaveAs);

        ExportComponentCommand = new AsyncRelayCommand(ExportComponent);
        ExportCircuitCommand = new RelayCommand(ExportCircuit);
        ExitCommand = new RelayCommand(Exit);

        UndoCommand = new RelayCommand(Undo);
        RedoCommand = new RelayCommand(Redo);
        CutCommand = new RelayCommand(Cut);
        CopyCommand = new RelayCommand(Copy);
        PasteCommand = new RelayCommand(Paste);
        DeleteCommand = new RelayCommand(Delete);

        AboutCommand = new RelayCommand(About);
        AiPromptCommand = new RelayCommand(AiGenerationFromPrompt);
        AiImageCommand = new RelayCommand(AiGenerationFromImage);

        AddComponentCommand = new RelayCommand<string>(AddComponent!);
        OtherComponentsCommand = new AsyncRelayCommand(OtherComponents);

        SimulationToggleCommand = new RelayCommand(OnSimulationToggleClick);
    }


    public string SimulationToggleText
    {
        get => Simulation.IsSimulating ? "Simulation: ON" : "Simulation: OFF";
    }


    public string SimulationToggleColor
    {
        get => Simulation.IsSimulating ? "Green" : "DarkRed";
    }


    public void OnSimulationToggleClick()
    {
        Simulation.IsSimulating = !Simulation.IsSimulating;

        OnPropertyChanged(nameof(SimulationToggleText));
        OnPropertyChanged(nameof(SimulationToggleColor));
        
        foreach (var component in Simulation.Components)
        {
            if (component is LogicProbe lp)
            {
                lp.Input.State = LogicState.Unknown;
                lp.InvalidateVisual();
            }
        }
    }






    private string? _openedFileName = null;
    private string _lastAction = " - ";
    public KeyGestureConfig KeyConfig { get; set; }


    public string? OpenedFileName
    {
        get => _openedFileName == null ? "(unsaved)" : _openedFileName;
        set => SetProperty(ref _openedFileName, value);
    }


    public string CursorPosition
    {
        get => $"({(int)Simulation.CurrentMousePos.X}, {(int)Simulation.CurrentMousePos.Y})";
    }


    public string LastAction
    {
        get => _lastAction;
        set => SetProperty(ref _lastAction, value);
    }


    public ICommand SimulationToggleCommand { get; }

    // File commands
    public ICommand NewCommand { get; }

    public ICommand AiPromptCommand { get; }

    // private AIGenerationWindowViewModel _currentPromptVm;

    private void AiGenerationFromPrompt()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot generate from prompt while simulating");
            return;
        }
        var window = new AIGenerationWindow();
        // _currentPromptVm = new AIGenerationWindowViewModel(window);

        var vm = window.DataContext as AIGenerationWindowViewModel;

        vm!.XmlGenerated += (xml) =>
        {
            Console.WriteLine("Event received");
            var components = JsonSerializationService.DeserializeComponentsAsync(xml);

            Simulation.DeleteAllComponents();
            Simulation.LoadComponents(components);
        };


        // Center it relative to main window
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        // Get reference to main window
        var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;

        window.ShowDialog(mainWindow!);
    }

    public ICommand AiImageCommand { get; }

    private void AiGenerationFromImage()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot generate from prompt while simulating");
            return;
        }
        var window = new ImageProcessingWindow();
        // _currentPromptVm = new AIGenerationWindowViewModel(window);

        var vm = window.DataContext as ImageProcessingWindowViewModel;

        vm!.XmlGenerated += (xml) =>
        {
            Console.WriteLine("Event received");
            var components = JsonSerializationService.DeserializeComponentsAsync(xml);

            Simulation.DeleteAllComponents();
            Simulation.LoadComponents(components);
        };


        // Center it relative to main window
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        // Get reference to main window
        var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;

        window.ShowDialog(mainWindow!);

    }

    private void New()
    {
    }

    public ICommand OpenCommand { get; }
    private async Task Open()
    {
        var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null) return;

        var files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Circuit JSON File",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            }
        });

        var file = files?.FirstOrDefault();
        if (file != null)
        {
            OpenedFileName = file.Path.LocalPath;
            // List<Component> loadedComponents = await JsonSerializationService.DeserializeFromFileAsync(OpenedFileName);
            // Simulation.LoadComponents(loadedComponents);
            Console.WriteLine("Path:" + OpenedFileName);
        }
    }

    public ICommand SaveCommand { get; }
    private async Task Save()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot save while simulating");
            return;
        }
        var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null) return;

        if (string.IsNullOrEmpty(_openedFileName))
        {
            var result = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Circuit JSON",
                SuggestedFileName = "circuit.json",
                DefaultExtension = "json",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } },
                    new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
                }
            });

            if (result != null)
            {
                _openedFileName = result.Path.LocalPath;
            }
        }

        if (!string.IsNullOrEmpty(_openedFileName))
        {
            // _serializer.SerializeComponents(Simulation, _openedFileName);
            Console.WriteLine("Saved to: " + _openedFileName);
        }
    }

    public ICommand SaveAsCommand { get; }
    private async Task SaveAs()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot save As while simulating");
            return;
        }
        var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow == null) return;

        var result = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Circuit JSON",
            SuggestedFileName = "circuit.json",
            DefaultExtension = "json",
            FileTypeChoices = new List<FilePickerFileType>
            {
                new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            }
        });

        if (result != null)
        {
            _openedFileName = result.Path.LocalPath;
        }

        if (!string.IsNullOrEmpty(_openedFileName))
        {
            // _serializer.SerializeComponents(Simulation, _openedFileName);
            Console.WriteLine("Saved to: " + _openedFileName);
        }
    }

    public ICommand ExitCommand { get; }
    private void Exit()
    {
        // Close the application
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
        else
        {
            // Fallback for other application lifetime types
            Environment.Exit(0);
        }
    }

    // Edit commands
    public ICommand UndoCommand { get; }

    private void Undo()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot undo while simulating");
            return;
        }
        Simulation.CommandManager.Undo();
        LastAction = "Undo";
    }

    public ICommand RedoCommand { get; }
    private void Redo()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot redo while simulating");
            return;
        }
        Simulation.CommandManager.Redo();
        LastAction = "Redo";
    }

    public ICommand CutCommand { get; }
    private void Cut()
    {
        Simulation.CutSelected();
        LastAction = "Cut to clipboard.";
    }

    public ICommand CopyCommand { get; }
    private void Copy()
    {
        // TODO: BE CAREFUL ABOUT THIS
        Simulation.CopySelected();
        LastAction = "Copied to clipboard.";
    }

    public ICommand PasteCommand { get; }
    private void Paste()
    {
        Simulation.PasteSelected();
        LastAction = "Pasted clipboard contents.";
    }

    public ICommand DeleteCommand { get; }
    private void Delete()
    {
        Simulation.DeleteSelectedComponents();
        LastAction = "Deleted selected components.";
    }

    // Help command
    public ICommand AboutCommand { get; }
    private void About()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot show about while simulating");
            return;
        }
        var aboutWindow = new AboutWindow();

        // Center it relative to main window
        aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        // Get reference to main window
        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
        {
            aboutWindow.ShowDialog(mainWindow);
        }
    }

    [RelayCommand]
    public void ShowProperties()
    {
        var propertiesWindow = new ComponentPropertiesWindow();
        propertiesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        // Get reference to main window
        var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow;

        propertiesWindow.ShowDialog(mainWindow!);
    }


    // Component command
    public ICommand AddComponentCommand { get; }
    private void AddComponent(string componentType)
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot add component while simulating");
            return;
        }
        Console.WriteLine($"Adding component: {componentType}");

        Simulation.PreviewCompType = componentType;
        LastAction = $"Selected Component [{componentType}]";
    }

    // Other components window
    public ICommand OtherComponentsCommand { get; }
    private async Task OtherComponents()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot add other components while simulating");
            return;
        }
        var otherComponentsWindow = new OtherComponentsWindow
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
        {
            CustomComponentData? result = await otherComponentsWindow.ShowDialog<CustomComponentData?>(mainWindow);

            if (result is not null)
            {
                string[] validNames = ["MUX", "DEMUX", "ENCODER", "DECODER", "SRL", "DL", "JKL", "TL"];
                // Add conditions for other complex components as well
                if (validNames.Contains(result.Name))
                {
                    Console.WriteLine($"Adding component: {result.Name}");
                    Simulation.PreviewCompType = result.Name;
                    LastAction = $"Selected Component [{result.Name}]";
                    return;
                }
                // Console.WriteLine($"Inputs: {result.InputCount}, Outputs: {result.OutputCount}");
                Simulation.CustomComponent = result;
                
                Console.WriteLine($"Adding component: {result.Name}");
                Simulation.PreviewCompType = "CUSTOM";
                LastAction = $"Selected Component [{result.Name}]";
            }
            else
            {
                Console.WriteLine("Dialog was canceled or no selection made.");
            }
        }
    }

    public ICommand ExportCircuitCommand { get; }
    private void ExportCircuit()
    {

    }
    
    public ICommand ExportComponentCommand { get; }
    private async Task ExportComponent()
    {
        if (Simulation.IsSimulating || !Simulation.GridEnabled)
        {
            Console.WriteLine("Cannot export while simulating");
            return;
        }
        var window = new ExportComponentWindow();
        // Center it relative to main window
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        // Get reference to main window (same pattern as OtherComponents method)
        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
        {
            var result = await window.ShowDialog<string?>(mainWindow);

            if (!string.IsNullOrEmpty(result)) // User clicked Export and entered a name
            {
                string componentName = result;
                Console.WriteLine($"Component name: {componentName}");
                // _serializer.SerializeComponents(Simulation, "RuntimeComponents/" + componentName + ".xml");
                Console.WriteLine("Saved to: " + _openedFileName);
            }
            else
            {
                Console.WriteLine("User clicked Cancel or closed the window.");
            }
            // If result is null, user clicked Cancel or closed the window
        }
    }
}

