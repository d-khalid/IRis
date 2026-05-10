using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

using IRis.Models;
using IRis.Services;
using IRis.Views;


namespace IRis.ViewModels;


public partial class MainWindowViewModel(Simulation simulation) : ViewModelBase
{
    private readonly Simulation _simulation = simulation;
    public Simulation Simulation => _simulation;



    // private string? _openedFileName = null;
    // private string _lastAction = " - ";


    // public string? OpenedFileName
    // {
    //     get => _openedFileName == null ? "(unsaved)" : _openedFileName;
    //     set => SetProperty(ref _openedFileName, value);
    // }


    // public string LastAction
    // {
    //     get => _lastAction;
    //     set => SetProperty(ref _lastAction, value);
    // }

    // // private AIGenerationWindowViewModel _currentPromptVm;

    // private void AiGenerationFromPrompt()
    // {
    //     if (_simulation.IsSimulating)
    //     {
    //         Console.WriteLine("Cannot generate from prompt while simulating");
    //         return;
    //     }
    //     var window = new AIGenerationWindow();
    //     // _currentPromptVm = new AIGenerationWindowViewModel(window);

    //     var vm = window.DataContext as AIGenerationWindowViewModel;

    //     vm!.XmlGenerated += (xml) =>
    //     {
    //         Console.WriteLine("Event received");
    //         var components = JsonSerializationService.DeserializeComponentsAsync(xml);
    //     };


    //     // Center it relative to main window
    //     window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

    //     // Get reference to main window
    //     var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
    //         ?.MainWindow;

    //     window.ShowDialog(mainWindow!);
    // }

    // private void AiGenerationFromImage()
    // {
    //     if (_simulation.IsSimulating)
    //     {
    //         Console.WriteLine("Cannot generate from prompt while simulating");
    //         return;
    //     }
    //     var window = new ImageProcessingWindow();
    //     // _currentPromptVm = new AIGenerationWindowViewModel(window);

    //     var vm = window.DataContext as ImageProcessingWindowViewModel;

    //     vm!.XmlGenerated += (xml) =>
    //     {
    //         Console.WriteLine("Event received");
    //         var components = JsonSerializationService.DeserializeComponentsAsync(xml);
    //     };


    //     // Center it relative to main window
    //     window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

    //     // Get reference to main window
    //     var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
    //         ?.MainWindow;

    //     window.ShowDialog(mainWindow!);

    // }

    // private void New()
    // {
    // }

    // private async Task Open()
    // {
    //     var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    //     if (mainWindow == null) return;

    //     var files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
    //     {
    //         Title = "Select Circuit JSON File",
    //         AllowMultiple = false,
    //         FileTypeFilter = new List<FilePickerFileType>
    //         {
    //             new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } },
    //             new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
    //         }
    //     });

    //     var file = files?.FirstOrDefault();
    //     if (file != null)
    //     {
    //         OpenedFileName = file.Path.LocalPath;
    //         // List<Component> loadedComponents = await JsonSerializationService.DeserializeFromFileAsync(OpenedFileName);
    //         // Simulation.LoadComponents(loadedComponents);
    //         Console.WriteLine("Path:" + OpenedFileName);
    //     }
    // }

    // private async Task Save()
    // {
    //     if (_simulation.IsSimulating)
    //     {
    //         Console.WriteLine("Cannot save while simulating");
    //         return;
    //     }
    //     var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    //     if (mainWindow == null) return;

    //     if (string.IsNullOrEmpty(_openedFileName))
    //     {
    //         var result = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
    //         {
    //             Title = "Save Circuit JSON",
    //             SuggestedFileName = "circuit.json",
    //             DefaultExtension = "json",
    //             FileTypeChoices = new List<FilePickerFileType>
    //             {
    //                 new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } },
    //                 new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
    //             }
    //         });

    //         if (result != null)
    //         {
    //             _openedFileName = result.Path.LocalPath;
    //         }
    //     }

    //     if (!string.IsNullOrEmpty(_openedFileName))
    //     {
    //         // _serializer.SerializeComponents(Simulation, _openedFileName);
    //         Console.WriteLine("Saved to: " + _openedFileName);
    //     }
    // }

    // private async Task SaveAs()
    // {
    //     if (_simulation.IsSimulating)
    //     {
    //         Console.WriteLine("Cannot save As while simulating");
    //         return;
    //     }
    //     var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
    //     if (mainWindow == null) return;

    //     var result = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
    //     {
    //         Title = "Save Circuit JSON",
    //         SuggestedFileName = "circuit.json",
    //         DefaultExtension = "json",
    //         FileTypeChoices = new List<FilePickerFileType>
    //         {
    //             new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } },
    //             new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
    //         }
    //     });

    //     if (result != null)
    //     {
    //         _openedFileName = result.Path.LocalPath;
    //     }

    //     if (!string.IsNullOrEmpty(_openedFileName))
    //     {
    //         // _serializer.SerializeComponents(Simulation, _openedFileName);
    //         Console.WriteLine("Saved to: " + _openedFileName);
    //     }
    // }

    // private void Exit()
    // {
    //     // Close the application
    //     if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    //     {
    //         desktop.Shutdown();
    //     }
    //     else
    //     {
    //         // Fallback for other application lifetime types
    //         Environment.Exit(0);
    //     }
    // }

    // private void Undo()
    // {
    //     if (_simulation.IsSimulating)
    //     {
    //         Console.WriteLine("Cannot undo while simulating");
    //         return;
    //     }
    //     // _simulation.CommandManager.Undo();
    //     LastAction = "Undo";
    // }

    // private void Redo()
    // {
    //     if (_simulation.IsSimulating)
    //     {
    //         Console.WriteLine("Cannot redo while simulating");
    //         return;
    //     }
    //     // _simulation.CommandManager.Redo();
    //     LastAction = "Redo";
    // }

    // private void Cut()
    // {
    //     // _simulation.CutSelected();
    //     LastAction = "Cut to clipboard.";
    // }

    // private void Copy()
    // {
    //     // TODO: BE CAREFUL ABOUT THIS
    //     // _simulation.CopySelected();
    //     LastAction = "Copied to clipboard.";
    // }

    // private void Paste()
    // {
    //     // _simulation.PasteSelected();
    //     LastAction = "Pasted clipboard contents.";
    // }

    // private void Delete()
    // {
    //     // _simulation.DeleteSelectedComponents();
    //     LastAction = "Deleted selected components.";
    // }

    // private void About()
    // {
    //     if (_simulation.IsSimulating)
    //     {
    //         Console.WriteLine("Cannot show about while simulating");
    //         return;
    //     }
    //     var aboutWindow = new AboutWindow();

    //     // Center it relative to main window
    //     aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    //     // Get reference to main window
    //     if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
    //     {
    //         aboutWindow.ShowDialog(mainWindow);
    //     }
    // }

    // public void ShowProperties()
    // {
    //     var propertiesWindow = new ComponentPropertiesWindow();
    //     propertiesWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

    //     // Get reference to main window
    //     var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
    //         ?.MainWindow;

    //     propertiesWindow.ShowDialog(mainWindow!);
    // }
    // private void ExportCircuit()
    // {

    // }

    // private async Task ExportComponent()
    // {
    //     if (_simulation.IsSimulating)
    //     {
    //         Console.WriteLine("Cannot export while simulating");
    //         return;
    //     }
    //     var window = new ExportComponentWindow();
    //     // Center it relative to main window
    //     window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

    //     // Get reference to main window (same pattern as OtherComponents method)
    //     if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
    //     {
    //         var result = await window.ShowDialog<string?>(mainWindow);

    //         if (!string.IsNullOrEmpty(result)) // User clicked Export and entered a name
    //         {
    //             string componentName = result;
    //             Console.WriteLine($"Component name: {componentName}");
    //             // _serializer.SerializeComponents(Simulation, "RuntimeComponents/" + componentName + ".xml");
    //             Console.WriteLine("Saved to: " + _openedFileName);
    //         }
    //         else
    //         {
    //             Console.WriteLine("User clicked Cancel or closed the window.");
    //         }
    //         // If result is null, user clicked Cancel or closed the window
    //     }
    // }
}

