using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;
using IRis.Services;
using IRis.Views;


namespace IRis.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly Simulation _simulation;

        private ISerializationService _serializer = new XmlSerializationService();

        private string? _openedFileName = null;

        private string _lastAction = " - ";

        public string? OpenedFileName
        {
            get => _openedFileName == null ? "(unsaved)" : _openedFileName;
            set => SetProperty(ref _openedFileName, value);
        }

        public string CursorPosition
        {
            get => $"({(int)_simulation.CurrentMousePos.X}, {(int)_simulation.CurrentMousePos.Y})";
        }

        public string LastAction
        {
            get => _lastAction;
            set => SetProperty(ref _lastAction, value);
        }

        private string _gridToggleText = "Grid: ON";
        public string GridToggleText
        {
            get => _gridToggleText;
            set => SetProperty(ref _gridToggleText, value);
        }

        public string SimulationButtonColor => _simulation.Simulating ? "Green" : "DarkRed";
        private string _simulationToggleText = "Simulation: OFF";
        public string SimulationToggleText
        {
            get => _simulationToggleText;
            set => SetProperty(ref _simulationToggleText, value);
        }

        public MainWindowViewModel(Simulation simulation)
        {
            // Use the CanvasService for adding/removing components
            _simulation = simulation;

            // Notify cursor pos about changes in LastMousePos
            _simulation.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Simulation.CurrentMousePos))
                {
                    // Notify that CursorPosition changed
                    OnPropertyChanged(nameof(CursorPosition));
                }
            };


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

            GridToggleCommand = new RelayCommand(GridToggle);
            SimulationToggleCommand = new RelayCommand(SimulationToggle);
        }

        // OPTIONS
        public ICommand GridToggleCommand { get; }

        public void GridToggle()
        {
            _simulation.SnapToGridEnabled = !_simulation.SnapToGridEnabled;
            _simulation.GridEnabled = !_simulation.GridEnabled;

            GridToggleText = _simulation.GridEnabled ? "Grid: ON" : "Grid: OFF";
        }

        public ICommand SimulationToggleCommand { get; }

        public void SimulationToggle()
        {
            _simulation.Simulating = !_simulation.Simulating;

            SimulationToggleText = _simulation.Simulating ? "Simulation: ON" : "Simulation: OFF";
            OnPropertyChanged(nameof(SimulationButtonColor));   // Changes the color of the button
            
            // Reset the probes to null
            foreach (var component in _simulation.Components)
            {
                if (component is LogicProbe lp)
                {
                    Console.WriteLine("LP value set to null");
                    lp.Terminals![0].Wire!.Value = null;
                    lp.InvalidateVisual();
                }
            }
        }

        // File commands
        public ICommand NewCommand { get; }

        public ICommand AiPromptCommand { get; }

        // private AIGenerationWindowViewModel _currentPromptVm;

        private void AiGenerationFromPrompt()
        {
            if (_simulation.Simulating || !_simulation.GridEnabled)
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
                var components = _serializer.DeserializeComponentsAsync(xml);

                _simulation.DeleteAllComponents();
                _simulation.LoadComponents(components);
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
            if (_simulation.Simulating || !_simulation.GridEnabled)
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
                var components = _serializer.DeserializeComponentsAsync(xml);

                _simulation.DeleteAllComponents();
                _simulation.LoadComponents(components);
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
                Title = "Select Circuit XML File",
                AllowMultiple = false,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType("XML Files") { Patterns = new[] { "*.xml" } },
                    new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
                }
            });

            var file = files?.FirstOrDefault();
            if (file != null)
            {
                OpenedFileName = file.Path.LocalPath;
                List<Component> loadedComponents = await _serializer.DeserializeFromFileAsync(OpenedFileName);
                _simulation.LoadComponents(loadedComponents);
                Console.WriteLine("Path:" + OpenedFileName);
            }
        }

        public ICommand SaveCommand { get; }
        private async Task Save()
        {
            if (_simulation.Simulating || !_simulation.GridEnabled)
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
                    Title = "Save Circuit XML",
                    SuggestedFileName = "circuit.xml",
                    DefaultExtension = "xml",
                    FileTypeChoices = new List<FilePickerFileType>
                    {
                        new FilePickerFileType("XML Files") { Patterns = new[] { "*.xml" } },
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
                _serializer.SerializeComponents(_simulation, _openedFileName);
                Console.WriteLine("Saved to: " + _openedFileName);
            }
        }

        public ICommand SaveAsCommand { get; }
        private async Task SaveAs()
        {
            if (_simulation.Simulating || !_simulation.GridEnabled)
            {
                Console.WriteLine("Cannot save As while simulating");
                return;
            }
            var mainWindow = (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow == null) return;

            var result = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Circuit XML",
                SuggestedFileName = "circuit.xml",
                DefaultExtension = "xml",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new FilePickerFileType("XML Files") { Patterns = new[] { "*.xml" } },
                    new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
                }
            });

            if (result != null)
            {
                _openedFileName = result.Path.LocalPath;
            }

            if (!string.IsNullOrEmpty(_openedFileName))
            {
                _serializer.SerializeComponents(_simulation, _openedFileName);
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
            if (_simulation.Simulating || !_simulation.GridEnabled)
            {
                Console.WriteLine("Cannot undo while simulating");
                return;
            }
            _simulation.Undo();
            LastAction = "Undo";
        }

        public ICommand RedoCommand { get; }
        private void Redo()
        {
            if (_simulation.Simulating || !_simulation.GridEnabled)
            {
                Console.WriteLine("Cannot redo while simulating");
                return;
            }
            _simulation.Redo();
            LastAction = "Redo";
        }

        public ICommand CutCommand { get; }
        private void Cut()
        {
            _simulation.CutSelected();
            LastAction = "Cut to clipboard.";
        }

        public ICommand CopyCommand { get; }
        private void Copy()
        {
            // TODO: BE CAREFUL ABOUT THIS
            _simulation.CopySelected();
            LastAction = "Copied to clipboard.";
        }

        public ICommand PasteCommand { get; }
        private void Paste()
        {
            _simulation.PasteSelected();
            LastAction = "Pasted clipboard contents.";
        }

        public ICommand DeleteCommand { get; }
        private void Delete()
        {
            _simulation.DeleteSelectedComponents();
            LastAction = "Deleted selected components.";
        }

        // Help command
        public ICommand AboutCommand { get; }
        private void About()
        {
            if (_simulation.Simulating || !_simulation.GridEnabled)
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
            if (_simulation.Simulating || !_simulation.GridEnabled)
            {
                Console.WriteLine("Cannot add component while simulating");
                return;
            }
            Console.WriteLine($"Adding component: {componentType}");

            _simulation.PreviewCompType = componentType;
            LastAction = $"Selected Component [{componentType}]";
        }

        // Other components window
        public ICommand OtherComponentsCommand { get; }
        private async Task OtherComponents()
        {
            if (_simulation.Simulating || !_simulation.GridEnabled)
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
                    string[] validNames = ["MUX", "DEMUX"];
                    // Add conditions for other complex components as well
                    if (validNames.Contains(result.Name))
                    {
                        Console.WriteLine($"Adding component: {result.Name}");
                        _simulation.PreviewCompType = result.Name;
                        LastAction = $"Selected Component [{result.Name}]";
                        return;
                    }
                    // Console.WriteLine($"Inputs: {result.InputCount}, Outputs: {result.OutputCount}");
                    _simulation.CustomComponent = result;
                    
                    Console.WriteLine($"Adding component: {result.Name}");
                    _simulation.PreviewCompType = "CUSTOM";
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
            if (_simulation.Simulating || !_simulation.GridEnabled)
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
                    _serializer.SerializeComponents(_simulation, "RuntimeComponents/" + componentName + ".xml");
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
}